using UnityEngine;

public class BOICE : MonoBehaviour
{
    [Header("落下ボイス（PlayerTag 1〜4に対応）")]
    [Tooltip("Element 0 = PlayerTag 1, Element 1 = PlayerTag 2, ...")]
    public AudioClip[] fallVoices = new AudioClip[4];  // サイズ4固定

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // 2Dサウンド
    }

    private void OnTriggerEnter(Collider other)
    {
        // PlayerController コンポーネントを取得
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // PlayerTag（1〜4）を 0〜3 に変換
        int voiceIndex = player.PlayerTag - 1;  // 1→0, 2→1, 3→2, 4→3

        Debug.Log($"Player {player.PlayerTag} fell! Voice index: {voiceIndex}");

        PlayFallVoice(voiceIndex);
    }

    void PlayFallVoice(int index)
    {
        // 範囲チェック
        if (index < 0 || index >= fallVoices.Length)
        {
            Debug.LogWarning($"Invalid voice index: {index}");
            return;
        }

        AudioClip clip = fallVoices[index];
        if (clip == null)
        {
            Debug.LogWarning($"No voice clip at index {index}");
            return;
        }

        Debug.Log($"Play fall voice: PlayerTag {index + 1}");
        audioSource.PlayOneShot(clip);
    }
}
