// Gimmick_Stun.cs
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Stun : MonoBehaviour, IGimmick
{
    [Header("Stun プレハブ")]
    public GameObject stunPrefab;

    [Header("ギミック存続時間（色確定中の 2秒）")]
    public float duration = 2f;

    private List<GameObject> spawned = new List<GameObject>();

    public string GetName() => "Stun";

    public void Activate(List<int> tileNumbers)
    {
        spawned.Clear();

        foreach (int num in tileNumbers)
        {
            GameObject tile = GameObject.Find(num.ToString());
            if (tile == null) continue;

            Transform f = tile.transform.Find(num + "f");
            if (f == null) continue;

            GameObject hit = Instantiate(stunPrefab, f.position, Quaternion.identity);
            spawned.Add(hit);

            Destroy(hit, duration);
        }
    }

    public void Deactivate()
    {
        // 特に何もしない（Destroy(hit,duration) が自動で処理）
    }
}