using System.Collections;
using UnityEngine;

public class GameStartSound : MonoBehaviour
{
    [Header("効果音設定")]
    public AudioClip seClip; // 効果音クリップを直接指定
    [Range(0f, 1f)]
    public float seVolume = 1f;

    [Header("BGM設定")]
    public AudioSource bgmAudioSource; // BGM用AudioSourceのみ
    public AudioClip bgmClip;

    void Start()
    {
        // シーン開始時1回のみ実行
        StartCoroutine(PlaySEThenBGM());
    }

    private IEnumerator PlaySEThenBGM()
    {
        // 効果音を一時再生（PlayOneShotでクリップ直接指定）
        bgmAudioSource.PlayOneShot(seClip, seVolume);

        // 効果音長さ分待機（安全マージン0.1秒追加）
        float waitTime = seClip.length + 0.1f;
        yield return new WaitForSeconds(waitTime);

        // BGM再生開始
        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }
}
