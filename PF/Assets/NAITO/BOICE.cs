using UnityEngine;

public class BOICE : MonoBehaviour
{
    [Header("落下ボイス（PlayerTag 1〜4に対応）")]
    [Tooltip("Element 0 = PlayerTag 1, Element 1 = PlayerTag 2, ...")]
    public AudioClip[] fallVoices = new AudioClip[4];

    [Header("音量設定")]
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;  // ★ 1.0=通常, 2.0=2倍音量

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;  // AudioSource自体の音量も最大に
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        int voiceIndex = player.PlayerTag - 1;
        Debug.Log($"Player {player.PlayerTag} fell! Voice index: {voiceIndex}");

        PlayFallVoice(voiceIndex);
    }

    void PlayFallVoice(int index)
    {
        if (index < 0 || index >= fallVoices.Length) return;

        AudioClip clip = fallVoices[index];
        if (clip == null) return;

        Debug.Log($"Play fall voice: PlayerTag {index + 1}");

        // ★ 音量倍率指定で超ドデカ音量！
        audioSource.PlayOneShot(clip, voiceVolume);
    }
}
