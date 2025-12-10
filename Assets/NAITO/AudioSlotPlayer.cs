using UnityEngine;

public class AudioSlotPlayer : MonoBehaviour
{
    public AudioSource audioSource; // シーン上のAudio Source（サウンド再生本体）
    public AudioClip[] audioClips = new AudioClip[3]; // 再生したい音源（曲・SE）の配列
    [Range(0f, 3f)]
    public float secondClipVolumeMultiplier = 1.5f; // 効果音の音量倍率（Inspectorから調整可能）

    private int state = 0;    // 再生状態管理用（0:1つ目再生中, 1:2つ目, 2:3つ目）
    private float timer = 0f; // 再生経過時間（秒）

    void Start()
    {
        PlayFirstClip(); // ゲーム開始時に1つ目を再生
    }

    void Update()
    {
        // 1つ目の曲が再生中
        if (state == 0)
        {
            timer += Time.deltaTime; // 経過時間をカウント
            if (timer > 60f)         // ←ここを60秒で切り替える！
            {
                PlaySecondClip();
            }
        }
        // 2つ目の効果音が再生中
        else if (state == 1)
        {
            if (!audioSource.isPlaying) // 2つ目のSEが鳴り終わったら
            {
                PlayThirdClip();        // 3つ目に切り替え
            }
        }
    }

    // 1つ目を再生
    void PlayFirstClip()
    {
        timer = 0f;           // 経過時間リセット
        state = 0;            // 状態を1つ目に
        audioSource.pitch = 1.0f;        // 標準速度
        audioSource.volume = 1.0f;       // 標準音量
        audioSource.clip = audioClips[0];// 1つ目の音源セット
        audioSource.Play();               // 再生
    }

    // 2つ目を再生
    void PlaySecondClip()
    {
        if (state != 1) // 二度呼ばれても1度しか切り替えないように
        {
            state = 1; // 状態を2つ目へ
            audioSource.pitch = 1.0f;        // 標準速度
            audioSource.volume = secondClipVolumeMultiplier; // Inspectorで指定した倍率で音量アップ
            audioSource.clip = audioClips[1];// 2つ目の音源セット
            audioSource.Play();              // 再生
        }
    }

    // 3つ目を再生
    void PlayThirdClip()
    {
        if (state != 2)
        {
            state = 2; // 状態を3つ目へ
            audioSource.pitch = 1.25f;        // 1.25倍速（音程も上がる）
            audioSource.volume = 1.0f;        // 標準音量
            audioSource.clip = audioClips[2]; // 3つ目の音源セット
            audioSource.Play();               // 再生
        }
    }
}
