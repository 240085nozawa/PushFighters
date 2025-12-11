using UnityEngine;
using System.Collections;

public class SkillSeedTrap : MonoBehaviour
{
    [Header("エフェクト＆オブジェクト")]
    [Tooltip("種エフェクトのプレハブ")]
    public GameObject seedEffectPrefab; // 種のエフェクトプレハブをInspectorから設定する

    [Tooltip("木のプレハブ")]
    public GameObject treePrefab; // 木のプレハブをInspectorから設定する

    [Header("コート中心")]
    [Tooltip("相手コートの中心（木の生成基準位置）")]
    public Transform opponentCourtCenter; // 木を生成する基準位置のTransformを設定する

    [Header("サウンド")]
    [Tooltip("スキル発動時SE")]
    public AudioClip skillSound; // スキル発動時の効果音を設定する

    [Tooltip("木が生える直前SE")]
    public AudioClip secondSkillSound; // 木が生える直前の効果音を設定する

    [Tooltip("音再生用AudioSource（未設定なら自動追加）")]
    public AudioSource audioSource; // 効果音の再生に使うAudioSource（なければ自動追加）

    [Header("木の生成範囲")]
    [Tooltip("コート中心から木を置く範囲X")]
    public float rangeX = 3f; // 木を生成する範囲のX方向の最大幅

    [Tooltip("コート中心から木を置く範囲Z")]
    public float rangeZ = 2f; // 木を生成する範囲のZ方向の最大幅

    private bool hasSpawnedTrees = false; // 木を生成済みかどうかのフラグ

    void Start()
    {
        // AudioSourceが設定されていなければ現在のオブジェクトから取得、なければ新規作成
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        // AudioSourceの初期設定
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D音に設定
        audioSource.volume = 1f;
        audioSource.loop = false;
        audioSource.mute = false;

        // 5秒後に種エフェクトの生成を開始する（Invokeの秒数を調整）
        Invoke(nameof(ActivateSeedEffect), 5f);
    }

    // 種エフェクトの生成＆その後木の生成コルーチンを開始
    private void ActivateSeedEffect()
    {
        StartCoroutine(SeedAndTreesRoutine());
    }

    private IEnumerator SeedAndTreesRoutine()
    {
        // 木を生やす基準位置（opponentCourtCenterがあればそちら、なければこのオブジェクトの位置）
        Vector3 seedPos = opponentCourtCenter != null ? opponentCourtCenter.position : transform.position;

        // スキル発動効果音を再生
        if (skillSound != null) audioSource.PlayOneShot(skillSound);

        // 種のエフェクト生成（コート中心の真上に生成）
        if (seedEffectPrefab != null)
        {
            GameObject seed = Instantiate(seedEffectPrefab, seedPos + Vector3.up * 5f, Quaternion.identity);
            Destroy(seed, 3f); // 3秒後自動削除
        }

        // 2秒待機（つまり、開始から7秒後に木を生成）
        yield return new WaitForSeconds(2f);

        // 木が生える直前効果音を再生
        if (secondSkillSound != null) audioSource.PlayOneShot(secondSkillSound);

        // まだ木を生成していなければ生成処理を行う
        if (!hasSpawnedTrees && treePrefab != null && opponentCourtCenter != null)
        {
            for (int i = 0; i < 4; i++)
            {
                // コート中心からX,Z方向にランダムにオフセットを作成
                float randX = Random.Range(-rangeX, rangeX);
                float randZ = Random.Range(-rangeZ, rangeZ);
                Vector3 treePos = opponentCourtCenter.position + new Vector3(randX, 0f, randZ);

                // 木のプレハブを位置treePosで生成
                GameObject tree = Instantiate(treePrefab, treePos, Quaternion.identity, null);

                // 大きさをX,Y,Zすべて15倍にする（均等に拡大）
                tree.transform.localScale = new Vector3(13f, 2f, 10f);

                Debug.Log($"Tree spawned at {treePos} with scale {tree.transform.localScale}");
            }
            hasSpawnedTrees = true; // 1回だけ生成
        }

        Debug.Log("🌱 SkillSeedTrap：発動完了");
    }
}
