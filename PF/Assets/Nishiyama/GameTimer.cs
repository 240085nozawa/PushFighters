using UnityEngine;
using TMPro; // ★ TextMeshProを使用する場合、これを追加
using System.Collections; // コルーチンのために必要

public class GameTimer : MonoBehaviour
{
    // Inspectorで設定: 初期時間（秒）
    [Tooltip("タイマーの初期設定時間 (秒)")]
    public float startTime = 120f; // 2分 = 120秒

    // Inspectorで設定: 時間を表示するTextコンポーネント
    [Tooltip("時間を表示するTextMeshProコンポーネント")]
    public TextMeshProUGUI timerText;

    private float currentTime;
    private bool isTimerRunning = false;

    [HideInInspector] public bool isStopped = false;　//TheWorld用
    private Coroutine timerCoroutine;

    void Start()
    {
        currentTime = startTime; // 初期時間を設定

        // TextMeshProコンポーネントが設定されているか確認
        if (timerText == null)
        {
            Debug.LogError("GameTimerにtimerTextが設定されていません！");
            return;
        }

        isTimerRunning = true;
        // 時間計測を開始
        timerCoroutine = StartCoroutine(Countdown());
    }

    void Update()
    {
        // 時止め中は更新しない
        //if (isStopped) return;

        // 毎フレーム、時間の表示を更新
        UpdateTimerDisplay(currentTime);
    }

    // ★★★ コルーチン: 1秒ごとの時間計測と処理を実行 ★★★
    IEnumerator Countdown()
    {
        while (isTimerRunning && currentTime > 0)
        {
            // ?? 時止め中は待機
            while (isStopped)
            {
                yield return null; // 1フレーム待つ（停止状態が解除されるまで）
            }

            // 1秒経過を待つ
            yield return new WaitForSeconds(1f);

            // 時止め中なら減らさない
            if (!isStopped)
            {
                currentTime -= 1f;
            }
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            Debug.Log("タイムアップ！ゲーム終了処理を実行。");
        }

        //↓TheWolrd前
        //while (isTimerRunning && currentTime > 0)
        //{
        //    // 1秒間待機
        //    yield return new WaitForSeconds(1f);

        //    // 時間を1秒減らす
        //    currentTime -= 1f;
        //}

        //// 時間が0になった後の処理
        //if (currentTime <= 0)
        //{
        //    currentTime = 0; // 確実に0で停止
        //    isTimerRunning = false;
        //    Debug.Log("タイムアップ！ゲーム終了処理を実行。");

        //    // 例: Time.timeScale = 0f; (ゲームを停止)
        //}
    }

    // ★★★ UI表示のフォーマット関数 (分:秒 形式) ★★★
    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        // 秒を分と秒に分割
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // UIに表示 (例: 02:00)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 他のスクリプトからタイマーを開始したい場合などに使用できます
    public void StopTimer()
    {
        isTimerRunning = false;
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
}