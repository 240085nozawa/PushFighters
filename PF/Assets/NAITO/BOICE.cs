using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BOICE : MonoBehaviour
{
    [Header("音量設定")]
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;  // 1.0=通常, 2.0=2倍音量

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;   // 2D音
        audioSource.volume = 1f;         // AudioSourceは最大
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 当たった相手が PlayerFallVoice を持っているかチェック
        PlayerFallVoice fallVoice = other.GetComponent<PlayerFallVoice>();
        if (fallVoice == null) return;

        AudioClip clip = fallVoice.fallVoiceClip;
        if (clip == null) return;

        Debug.Log($"Fall voice from: {other.name}");

        // プレイヤーが持っているボイスを鳴らす
        audioSource.PlayOneShot(clip, voiceVolume);
    }
}
