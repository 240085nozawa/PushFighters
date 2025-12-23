using UnityEngine;
using System.Collections.Generic;

public class SeaCreatureSpawner : MonoBehaviour
{
    [Header("生成する生き物プレハブ")]
    public GameObject[] creaturePrefabs;

    [Header("範囲（中心＝このオブジェクト）")]
    public float width = 50f;      // ローカルX方向サイズ
    public float height = 20f;     // ローカルZ方向サイズ
    public float yPosition = 0f;   // 高さ

    [Header("流れる向き設定")]
    [Tooltip("0=+X/-X(横)、90=+Z/-Z(縦) など。Y軸回転角度。")]
    public float flowAngleY = 0f;  // 進行方向の基準角度（度）

    [Header("生成タイミング")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 4f;

    [Header("同時に存在できる最大数")]
    public int maxCreatures = 4;

    [Header("移動速度")]
    public float moveSpeedMin = 2f;
    public float moveSpeedMax = 5f;

    private float timer;
    private float nextInterval;
    private readonly List<GameObject> aliveCreatures = new List<GameObject>();

    private void Start()
    {
        SetNextInterval();
    }

    private void Update()
    {
        aliveCreatures.RemoveAll(c => c == null);

        if (aliveCreatures.Count >= maxCreatures)
            return;

        timer += Time.deltaTime;
        if (timer >= nextInterval)
        {
            SpawnCreature();
            timer = 0f;
            SetNextInterval();
        }
    }

    private void SetNextInterval()
    {
        nextInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnCreature()
    {
        if (creaturePrefabs == null || creaturePrefabs.Length == 0)
            return;

        GameObject prefab = creaturePrefabs[Random.Range(0, creaturePrefabs.Length)];

        // ■ 流れる基準方向（forward）をY回転から作る
        Quaternion flowRot = Quaternion.Euler(0f, flowAngleY, 0f);
        Vector3 forward = flowRot * Vector3.right;   // 基準進行方向
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized; // 進行方向に対して右
        Vector3 up = Vector3.up;

        // side = -1 → 片側の端、+1 → 反対側の端から出す
        int side = Random.value < 0.5f ? -1 : 1;

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        // ■ 端ジャスト：中心 ± forward * halfW
        Vector3 center = transform.position;
        Vector3 edgePos = center + forward * (side < 0 ? -halfW : halfW);

        // ■ 幅方向(=right)にランダムオフセットを加える
        float offset = Random.Range(-halfH, halfH);
        Vector3 spawnPos = edgePos + right * offset + up * yPosition;

        GameObject creature = Instantiate(prefab, spawnPos, Quaternion.identity);
        aliveCreatures.Add(creature);

        SeaCreatureMover mover = creature.GetComponent<SeaCreatureMover>();
        if (mover == null)
            mover = creature.AddComponent<SeaCreatureMover>();

        float speed = Random.Range(moveSpeedMin, moveSpeedMax);

        // 進行方向は「flowAngleY」「side」から渡す
        mover.InitDirection(this, forward, side, speed);
    }

    // Mover が端判定に使う半径（進行方向方向の半分の長さ）
    public float GetHalfLength()
    {
        return width * 0.5f;
    }

    public float GetFlowAngleY()
    {
        return flowAngleY;
    }
}
