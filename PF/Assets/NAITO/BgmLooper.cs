using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgmLooper : MonoBehaviour
{
    [SerializeField] AudioClip bgmClip;  // ここに曲をドラッグ&ドロップ[web:31][web:33][web:37]
    AudioSource bgm;

    void Awake()
    {
        bgm = GetComponent<AudioSource>();
        bgm.clip = bgmClip;    // スクリプトでアタッチした曲を設定[web:31][web:33]
        bgm.loop = true;       // 自動ループON[web:21]
    }

    void Start()
    {
        if (bgmClip != null)
            bgm.Play();        // 再生開始[web:28]
    }
}
