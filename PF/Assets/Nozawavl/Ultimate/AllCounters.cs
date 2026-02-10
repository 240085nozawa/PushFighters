using UnityEngine;
using System.Collections;

public class AllCounters : MonoBehaviour
{
    /// Xキーで10秒間、反射用バリア（Barrier）をPlayerの子として生成する。
    /// 10秒経過後に自動的に削除する。

    [Header("設定")]
    public KeyCode activateKey = KeyCode.X;  // 発動キー
    public float activeDuration = 10f;       // バリア持続時間（秒）

    [Header("参照")]
    public PlayerController playerController; // プレイヤー情報

    // ★追加：ボイス再生に使う AudioSource を外からドラッグ
    [Header("必殺ボイス設定")]
    [Tooltip("ボイスを再生する AudioSource（プレイヤー側のをドラッグ推奨）")]
    public AudioSource voiceSource;          // ここに AudioSource をドラッグ
    [Tooltip("必殺発動時に再生するボイス")]
    public AudioClip specialVoiceClip;       // ここにボイスをドラッグ
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;

    private GameObject activeBarrier;         // 現在生成されているバリア
    private bool isActive = false;            // 発動中フラグ

    private void Start()
    {
        // PlayerControllerを自動取得
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        // voiceSource が未設定なら、同じオブジェクトの AudioSource を自動取得してみる（保険）
        if (voiceSource == null)
        {
            voiceSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
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

    private IEnumerator ActivateBarrier()
    {
        isActive = true;

        // ★ 発動と同時にボイス再生（AudioSource は外からドラッグ）
        if (specialVoiceClip != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(specialVoiceClip, voiceVolume);
            Debug.Log("[AllCounters] 必殺ボイス再生");
        }
        else
        {
            Debug.LogWarning("[AllCounters] voiceSource または specialVoiceClip が設定されていません。");
        }

        // --- 【1】発動時にMassを初期化 ---
        if (playerController != null)
        {
            playerController.currentMassStage = 0;
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null && playerController.massStages.Length > 0)
                rb.mass = playerController.massStages[0];

            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のMassを初期化（Stage0）に戻しました。");
        }

        // --- TakeDamageを一時的に無効化 ---
        if (playerController != null)
        {
            playerController.canTakeDamage = false;
            playerController.canKnockback = false;
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
        spawnPos.y = 1f;

        // --- Barrier生成 ---
        activeBarrier = Instantiate(barrierPrefab, spawnPos, Quaternion.identity);

        // Playerの子として設定
        activeBarrier.transform.SetParent(transform);

        // 子にした後でも位置がずれないように、ローカル位置を再調整
        Vector3 localPos = activeBarrier.transform.localPosition;
        localPos.y = 1f - transform.position.y;
        activeBarrier.transform.localPosition = localPos;

        // --- Reflectionスクリプトに所有者情報を渡す ---
        Reflection reflection = activeBarrier.GetComponent<Reflection>();
        if (reflection != null)
        {
            reflection.ownerTag = playerController.PlayerTag;
            reflection.ownerObject = this.gameObject;
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
            playerController.canTakeDamage = true;
            playerController.canKnockback = true;
            Debug.Log($"[AllCounters] Player{playerController.PlayerTag} のダメージ無効化OFF");
        }

        isActive = false;
    }
}
