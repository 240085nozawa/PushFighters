using UnityEngine;
using System.Collections;

public class DEN : MonoBehaviour
{
    public float stunDuration = 3f;
    public GameObject stunEffectPrefab;
    public float effectHeightOffset = 1.0f; // エフェクトを生成する高さのオフセット

    // スタン中の音用
    public AudioSource stunAudioSource;   // Inspector で割り当て
    public AudioClip stunClip;           // スタン用SE（任意で指定）

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(StunPlayer(playerController));
            }
        }
    }

    private IEnumerator StunPlayer(PlayerController player)
    {
        // エフェクト生成
        Vector3 effectPosition = player.transform.position + new Vector3(0, effectHeightOffset, 0);
        GameObject effectInstance = null;
        if (stunEffectPrefab != null)
        {
            effectInstance = Instantiate(stunEffectPrefab, effectPosition, Quaternion.identity, player.transform);
        }

        // スタン音 再生開始
        if (stunAudioSource != null)
        {
            if (stunClip != null)
            {
                stunAudioSource.clip = stunClip;
            }
            stunAudioSource.loop = true;   // スタン中ループ
            stunAudioSource.Play();
        }

        // プレイヤー操作不能
        player.enabled = false;

        // スタン時間待機
        yield return new WaitForSeconds(stunDuration);

        // プレイヤー操作復帰
        player.enabled = true;

        // エフェクト削除
        if (effectInstance != null)
        {
            Destroy(effectInstance);
        }

        // スタン音 停止
        if (stunAudioSource != null)
        {
            stunAudioSource.loop = false;
            stunAudioSource.Stop();
        }
    }
}
