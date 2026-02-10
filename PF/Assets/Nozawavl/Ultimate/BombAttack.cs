using UnityEngine;
using System.Collections;

public class BombAttack : MonoBehaviour
{
    [Header("必殺技フラグ")]
    public bool isActive = false;

    [Header("溜め演出Prefab")]
    public GameObject chargeEffectPrefab;
    public float chargeDuration = 2f;

    [Header("爆破演出Prefab")]
    public GameObject explosionEffectPrefab;
    public float explosionDuration = 1.5f;

    [Header("爆破判定Prefab（BombATK付き）")]
    public GameObject explosionATKPrefab;

    [Header("必殺ボイス設定")]
    [Tooltip("ボイスを再生する AudioSource（プレイヤー側のをドラッグ推奨）")]
    public AudioSource voiceSource;
    [Tooltip("2.5秒後に再生する必殺ボイス")]
    public AudioClip bombVoiceClip;
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;
    [Tooltip("フェードアウト時間（秒）")]
    [Range(0.1f, 3.0f)]
    public float fadeOutDuration = 1.0f;

    private PlayerController pc;
    private bool isRunning = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        if (isActive && !isRunning)
        {
            isActive = false;
            StartCoroutine(BombAttackRoutine());
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Activate();
        }
    }

    private IEnumerator BombAttackRoutine()
    {
        isRunning = true;

        // 溜め演出
        if (chargeEffectPrefab != null)
        {
            GameObject charge = Instantiate(
                chargeEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(charge, chargeDuration);
        }

        // ★★★ 2.5秒後にボイス再生＋フェードアウト開始 ★★★
        yield return new WaitForSeconds(chargeDuration);

        if (bombVoiceClip != null && voiceSource != null)
        {
            // ★修正：再生前に音量を必ず元に戻す
            voiceSource.volume = 1f;
            voiceSource.PlayOneShot(bombVoiceClip, voiceVolume);
            Debug.Log("[BombAttack] 必殺ボイス再生開始");

            // フェードアウト開始（非同期）
            StartCoroutine(FadeOutVoice(fadeOutDuration));
        }
        else
        {
            Debug.LogWarning("[BombAttack] voiceSource または bombVoiceClip が設定されていません。");
        }

        // 爆破演出
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(explosion, explosionDuration);
        }

        // 爆破判定生成
        if (explosionATKPrefab != null)
        {
            GameObject atk = Instantiate(
                explosionATKPrefab,
                transform.position,
                Quaternion.identity
            );

            BombATK bombATK = atk.GetComponent<BombATK>();
            if (bombATK != null)
            {
                bombATK.owner = this.gameObject;
            }

            Destroy(atk, explosionDuration);
        }

        yield return new WaitForSeconds(explosionDuration);

        if (pc != null)
        {
            pc.canMove = true;
        }

        isRunning = false;
    }

    /// <summary>
    /// ボイスのフェードアウト処理（1秒前から開始）
    /// </summary>
    private IEnumerator FadeOutVoice(float duration)
    {
        float fadeStartDelay = bombVoiceClip.length - 1f;
        yield return new WaitForSeconds(fadeStartDelay);

        float startVolume = voiceSource.volume;
        float elapsed = 0f;

        Debug.Log($"[BombAttack] ボイスフェード開始（{fadeStartDelay}秒後）");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            voiceSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        // ★修正：0にしすぎない（0.01残す）
        voiceSource.volume = 0.01f;
        Debug.Log("[BombAttack] ボイスフェードアウト完了");
    }
}
