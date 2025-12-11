using UnityEngine;
using System.Collections;

public class AllCounters : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// Xキーで10秒間、反射用バリア（Barrier）をPlayerの子として生成する。
    /// 10秒経過後に自動的に削除する。
    /// 
    [Header("設定")]
    public KeyCode activateKey = KeyCode.X;  // 発動キー
    public float activeDuration = 10f;       // バリア持続時間（秒）

    [Header("参照")]
    public PlayerController playerController; // プレイヤー情報

    private GameObject activeBarrier;         // 現在生成されているバリア
    private bool isActive = false;            // 発動中フラグ

    private void Start()
    {
        // PlayerControllerを自動取得
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Xキーで発動（既に発動中なら無視）
        if (Input.GetKeyDown(activateKey) && !isActive)
        {
            StartCoroutine(ActivateBarrier());
        }
    }
    public void Activate()
    {
        if (!isActive)
        {
            StartCoroutine(ActivateBarrier());
        }
    }

    /// <summary>
    /// Barrier生成と削除を管理するコルーチン
    /// </summary>
    private IEnumerator ActivateBarrier()
    {
        isActive = true;

        // --- 【1】発動時にMassを初期化 ---
        if (playerController != null)
        {
            playerController.currentMassStage = 0; // 最初のレベルへ戻す
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null && playerController.massStages.Length > 0)
                rb.mass = playerController.massStages[0];

            // 見た目（色）も初期化
            Renderer r = playerController.GetComponentInChildren<Renderer>();
            if (r != null && playerController.massColors.Length > 0)
                r.material.color = playerController.massColors[0];

            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のMassを初期化（Stage0）に戻しました。");
        }

        // --- TakeDamageを一時的に無効化 ---
        if (playerController != null)
        {
            playerController.canTakeDamage = false; // ★ 追加（この10秒間ダメージ無効）
            playerController.canKnockback = false; // ★ ノックバック無効化
            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のダメージ無効化ON");
        }

        // --- Barrierロード ---
        GameObject barrierPrefab = Resources.Load<GameObject>("Barrier");
        if (barrierPrefab == null)
        {
            Debug.LogError("[AllCounters] Resources/Barrier が見つかりません。");
            isActive = false;
            yield break;
        }

        // --- Barrier生成位置を決定 ---
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f; // Y座標を1に固定

        // --- Barrier生成 ---
        activeBarrier = Instantiate(barrierPrefab, spawnPos, Quaternion.identity);

        // Playerの子として設定
        activeBarrier.transform.SetParent(transform);

        // 子にした後でも位置がずれないように、ローカル位置を再調整
        Vector3 localPos = activeBarrier.transform.localPosition;
        localPos.y = 1f - transform.position.y; // 親の高さ分を考慮して調整
        activeBarrier.transform.localPosition = localPos;

        // --- Reflectionスクリプトに所有者情報を渡す ---
        Reflection reflection = activeBarrier.GetComponent<Reflection>();
        if (reflection != null)
        {
            reflection.ownerTag = playerController.PlayerTag;  // このプレイヤー番号
            reflection.ownerObject = this.gameObject;           // このプレイヤー自身
        }

        Debug.Log($"[AllCounters] Player{playerController.PlayerTag} がBarrierを展開！（Y=1固定）");

        // --- 持続時間を待つ ---
        yield return new WaitForSeconds(activeDuration);

        // --- Barrier削除 ---
        if (activeBarrier != null)
        {
            Destroy(activeBarrier);
            activeBarrier = null;
            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のBarrierが終了しました。");
        }

        // --- TakeDamageを再び有効化 ---
        if (playerController != null)
        {
            playerController.canTakeDamage = true; // ★ 追加（元に戻す）
            playerController.canKnockback = true; // ★ ノックバック有効化を戻す
            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のダメージ無効化OFF");
        }

        isActive = false;
    }

    //public void Activate()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Special allC 発動！");
    //        // 実際の攻撃処理などを書く
    //    }

    //}
}
