using UnityEngine;
using System.Collections;

public class CompleteGameStartController : MonoBehaviour
{
    [Header("音声設定")]
    [SerializeField] private AudioClip readyGoClip;
    [SerializeField, Range(0.1f, 2.0f)] private float playbackSpeed = 0.8f;

    [Header("遅延設定")]
    [SerializeField] private float startDelay = 1.5f; // ゲーム開始からReadyGoまでの待ち時間

    private AudioSource audioSource;
    private bool gameStarted = false;

    private void Awake()
    {
        // AudioSource自動設定
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = readyGoClip;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start()
    {
        // ゲーム開始：即座に全体停止
        Time.timeScale = 0f;

        // startDelay秒後にReadyGo開始
        StartCoroutine(StartReadyGoSequence());
    }

    private IEnumerator StartReadyGoSequence()
    {
        // 1. 開始遅延（例：1.5秒無音）
        yield return new WaitForSecondsRealtime(startDelay);

        // 2. ReadyGo再生開始（速度調整）
        audioSource.pitch = playbackSpeed;
        audioSource.Play();
        Debug.Log($"ReadyGo開始 (速度: {playbackSpeed}x)");

        // 3. 音声終了まで待機
        yield return new WaitForSecondsRealtime(readyGoClip.length / playbackSpeed);

        // 4. ゲーム本番開始！
        Time.timeScale = 1f;
        gameStarted = true;
        Debug.Log("🎮 GAME START!");
    }

    // いつでもゲーム状態確認可能
    public bool IsGameActive() => gameStarted;
}
