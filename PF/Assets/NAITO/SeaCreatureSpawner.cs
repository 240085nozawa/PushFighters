using UnityEngine;
using System.Collections.Generic;

public class SeaCreatureSpawner : MonoBehaviour
{
    [Header("生成する生き物プレハブ")]
    public GameObject[] creaturePrefabs;   // サメ・イルカなどのプレハブ

    [Header("海の範囲（中心＝このオブジェクト）")]
    public float width = 50f;              // X方向サイズ
    public float height = 20f;             // Z方向サイズ
    public float yPosition = 0f;           // 泳ぐ高さ（Y）

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
        // 死んだ個体をリストから除外
        aliveCreatures.RemoveAll(c => c == null);

        // すでに最大数いたら生成しない
        if (aliveCreatures.Count >= maxCreatures)
        {
            return;
        }

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
        {
            return;
        }

        // ランダムな種類を選ぶ
        GameObject prefab = creaturePrefabs[Random.Range(0, creaturePrefabs.Length)];

        // -1: 左端スタート → 右に泳ぐ,  +1: 右端スタート → 左に泳ぐ
        int side = Random.value < 0.5f ? -1 : 1;

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        // ★ 端ジャストのX座標（BOXの本当の端）
        float x = (side < 0) ? -halfW : halfW;

        // ZはBOX内ランダム
        float z = Random.Range(-halfH, halfH);

        Vector3 center = transform.position;
        Vector3 spawnPos = new Vector3(center.x + x,
                                       center.y + yPosition,
                                       center.z + z);

        // 生成
        GameObject creature = Instantiate(prefab, spawnPos, Quaternion.identity);
        aliveCreatures.Add(creature);

        // Mover を取得 or 追加して初期化
        SeaCreatureMover mover = creature.GetComponent<SeaCreatureMover>();
        if (mover == null)
        {
            mover = creature.AddComponent<SeaCreatureMover>();
        }

        float speed = Random.Range(moveSpeedMin, moveSpeedMax);
        mover.Init(this, side, speed);
    }

    // Mover が端判定に使う幅
    public float GetHalfWidth()
    {
        return width * 0.5f;
    }
}
