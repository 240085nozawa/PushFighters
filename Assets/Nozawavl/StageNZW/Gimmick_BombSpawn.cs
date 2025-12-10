// Gimmick_BombSpawn.cs
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_BombSpawn : MonoBehaviour, IGimmick
{
    [Header("爆弾プレハブ")]
    public GameObject bombPrefab;

    public string gimmickName = "BombSpawn";

    private List<GameObject> spawnedBombs = new List<GameObject>();

    public void Activate(List<int> tileNumbers)
    {
        Debug.Log("Gimmick_BombSpawn Activate");

        if (bombPrefab == null)
        {
            Debug.LogError("Gimmick_BombSpawn: bombPrefab not set");
            return;
        }

        foreach (int num in tileNumbers)
        {
            GameObject tileObj = GameObject.Find(num.ToString());
            if (tileObj == null) continue;

            Transform spawnPoint = tileObj.transform.Find(num + "f");
            if (spawnPoint == null)
            {
                Debug.LogWarning($"Gimmick_BombSpawn: {num}f not found");
                continue;
            }

            GameObject b = Instantiate(bombPrefab, spawnPoint.position, Quaternion.identity);
            spawnedBombs.Add(b);
        }
    }

    public void Deactivate()
    {
        // 未爆発ボムを消す（必要ならここを変更）
        foreach (var b in spawnedBombs) if (b != null) Destroy(b);
        spawnedBombs.Clear();
        Debug.Log("Gimmick_BombSpawn Deactivated");
    }

    public string GetName() => gimmickName;
}
