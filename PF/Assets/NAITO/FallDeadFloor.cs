using UnityEngine;

public class FallDeadFloorTrigger : MonoBehaviour
{
    [Header("落下時に出すエフェクト")]
    [SerializeField] private GameObject fallEffectPrefab;

    [Header("落下時に出す音声")]
    [SerializeField] private AudioClip fallSoundClip;
    [SerializeField] private float soundVolume = 1f;
    [SerializeField] private bool playSound3D = true; // 3D空間で聞こえるか

    [Header("プレイヤー判定用タグ")]
    [SerializeField] private string playerTag = "Player";

    [Header("エフェクト自動削除秒数(0以下なら消さない)")]
    [SerializeField] private float effectLifetime = 3f;

    private Collider _triggerCollider;
    private AudioSource _audioSource;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
        if (_triggerCollider == null)
        {
            Debug.LogError("[FallDeadFloorTrigger] Collider がありません。");
        }
        else if (!_triggerCollider.isTrigger)
        {
            Debug.LogWarning("[FallDeadFloorTrigger] isTrigger が OFF なので ON にしてください。");
        }

        // AudioSource を自動生成（なければ）
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tag でプレイヤーを判定
        if (!other.CompareTag(playerTag)) return;

        // プレイヤーの位置から、このトリガーコライダー上の最近点を取得
        Vector3 hitPos = _triggerCollider.ClosestPoint(other.transform.position);

        // エフェクト生成
        if (fallEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                fallEffectPrefab,
                hitPos,
                Quaternion.identity
            );

            // 一定時間で自動削除
            if (effectLifetime > 0f)
            {
                Destroy(effect, effectLifetime);
            }
        }

        // 落下音再生
        PlayFallSound(hitPos);

        // プレイヤー死亡・リスポーン処理など
        KillPlayer(other.gameObject);
    }

    private void PlayFallSound(Vector3 position)
    {
        if (fallSoundClip == null) return;

        _audioSource.volume = soundVolume;
        _audioSource.clip = fallSoundClip;
        _audioSource.spatialBlend = playSound3D ? 1f : 0f; // 3D:1, 2D:0
        _audioSource.Play();

        // ループしない、1回だけ鳴らす
        if (_audioSource.isPlaying)
        {
            Invoke(nameof(StopSound), fallSoundClip.length);
        }
    }

    private void StopSound()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log("[FallDeadFloorTrigger] Player 落下死: " + player.name);
        // Destroy(player); // 必要に応じて有効化
    }
}
