using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgmLooper : MonoBehaviour
{
    [SerializeField] AudioClip bgmClip;  // ここに曲をドラッグ&ドロップ
    AudioSource bgm;

    void Awake()
    {
        bgm = GetComponent<AudioSource>();
        bgm.clip = bgmClip;    // スクリプトでアタッチした曲を設定
        bgm.loop = true;       // 自動ループON
    }

    void Start()
    {
        if (bgmClip != null)
        {
            // 1.8秒後に PlayBgm を呼ぶ
            Invoke(nameof(PlayBgm), 1.8f);
        }
    }

    void PlayBgm()
    {
        bgm.Play();            // 再生開始
    }
}
