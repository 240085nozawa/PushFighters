// Bomb.cs
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("爆発までの秒数")]
    public float explodeTime = 2f;

    [Header("爆風エフェクト")]
    public GameObject fireEffect;

    [Header("爆風が消えるまでの秒数")]
    public float fireLifetime = 1.5f;

    [Header("Bombの落下速度（重力倍率）")]
    public float fallSpeed = 9.8f;

    private float timer = 0f;
    private bool exploded = false;

    void Update()
    {
        // --- 爆弾を落下させる ---
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // --- 爆発カウント ---
        timer += Time.deltaTime;

        if (!exploded && timer >= explodeTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        // --- 爆風 Fire エフェクトを生成 ---
        if (fireEffect != null)
        {
            GameObject fire = Instantiate(fireEffect, transform.position, Quaternion.identity);

            // Fire を fireLifetime 秒後に削除
            Destroy(fire, fireLifetime);
        }

        // --- Bomb本体を削除 ---
        Destroy(gameObject);
    }
}