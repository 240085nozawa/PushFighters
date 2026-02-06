using UnityEngine;

// プレイヤーが自分の AudioSource とボイスを持っておくコンポーネント
public class PlayerFallVoice : MonoBehaviour
{
    [Header("このプレイヤー専用の AudioSource")]
    public AudioSource myAudioSource;  // ★プレイヤーの AudioSource をドラッグ

    [Header("このプレイヤー専用の落下ボイス")]
    public AudioClip fallVoiceClip;    // ★ここに落下ボイスをドラッグ

    [Header("音量倍率")]
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;   // デフォルトでドデカ音量
}
