using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // ★シーン遷移に必要
using System.Collections.Generic;  // ★Dictionaryに必要

public class GameTimer : MonoBehaviour
{
    [Tooltip("タイマーの初期設定時間 (秒)")]
    public float startTime = 120f;

    [Tooltip("時間を表示するTextMeshProコンポーネント")]
    public TextMeshProUGUI timerText;

    [Tooltip("遷移先のリザルトシーン名")]
    public string resultSceneName = "Result"; // ★追加: リザルトシーンの名前

    private float currentTime;
    private bool isTimerRunning = false;

    [HideInInspector] public bool isStopped = false;
    private Coroutine timerCoroutine;

    void Start()
    {
        currentTime = startTime;

        if (timerText == null)
        {
            Debug.LogError("GameTimerにtimerTextが設定されていません！");
            return;
        }

        isTimerRunning = true;
        timerCoroutine = StartCoroutine(Countdown());
    }

    void Update()
    {
        UpdateTimerDisplay(currentTime);
    }

    IEnumerator Countdown()
    {
        while (isTimerRunning && currentTime > 0)
        {
            while (isStopped)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            if (!isStopped)
            {
                currentTime -= 1f;
            }
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;

            // ★追加: タイムアップ時の処理
            FinishGame();
        }
    }

    // ★追加: ゲーム終了処理
    void FinishGame()
    {
        Debug.Log("タイムアップ！スコアを集計してリザルトへ移行します。");

        // 1. シーン内の全プレイヤーを探す
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        Dictionary<int, int> scores = new Dictionary<int, int>();

        // 2. スコアを辞書に保存
        foreach (var p in players)
        {
            // PlayerTagをキー、currentScoreを値として保存
            if (!scores.ContainsKey(p.PlayerTag))
            {
                scores.Add(p.PlayerTag, p.currentScore);
            }
        }

        // 3. Static変数にデータを渡す
        GameData.FinalScores = scores;

        // 4. シーン遷移
        SceneManager.LoadScene(resultSceneName);
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
}