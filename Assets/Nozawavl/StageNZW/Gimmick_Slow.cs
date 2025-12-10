// Gimmick_Slow.cs
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Slow : MonoBehaviour, IGimmick
{
    [Header("Slow プレハブ")]
    public GameObject slowPrefab;

    [Header("ギミック存続時間（色確定中の 2秒）")]
    public float duration = 2f;

    private List<GameObject> spawned = new List<GameObject>();

    public string GetName() => "Slow";

    public void Activate(List<int> tileNumbers)
    {
        spawned.Clear();

        foreach (int num in tileNumbers)
        {
            GameObject tile = GameObject.Find(num.ToString());
            if (tile == null) continue;

            Transform f = tile.transform.Find(num + "f");
            if (f == null) continue;

            GameObject hit = Instantiate(slowPrefab, f.position, Quaternion.identity);
            spawned.Add(hit);

            Destroy(hit, duration);
        }
    }

    public void Deactivate()
    {
        // Destroy(hit, duration) に任せる
    }
}