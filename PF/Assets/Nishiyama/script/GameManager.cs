using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // ★ シーン遷移に必要
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("順位ポイント設定 (1位, 2位, 3位, 4位...)")]
    public int[] rankPoints = { 50, 30, 15, 10 };

    [Header("現在の生存プレイヤー数")]
    public int activePlayerCount;

    [Header("リザルトシーンの名前")]
    public string resultSceneName = "ResultScene"; // ★ 設定が必要

    // 脱落したプレイヤーのリスト（順位計算用）
    private List<int> deadPlayerTags = new List<int>();

    // リザルト画面で使うために順位リストを返す関数（今は使わないが互換性のため残す）
    public List<int> GetFinalRanking()
    {
        return deadPlayerTags;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // ゲーム開始時の人数を保存
        activePlayerCount = FindObjectsOfType<PlayerController>().Length;
        Debug.Log($"ゲーム開始: 参加人数 {activePlayerCount}人");
    }

    // プレイヤーがゲームオーバーになったら呼ばれる関数
    public void PlayerFinished(int playerTag)
    {
        // 既に登録済みなら無視
        if (deadPlayerTags.Contains(playerTag)) return;

        // 脱落リストに追加
        deadPlayerTags.Add(playerTag);

        // --- 順位決定とスコア加算 ---

        // 今回の順位 = 現在の生存数 (例: 4人中1人脱落したら、その人は4位)
        int rank = activePlayerCount;

        // ポイント付与 (配列は0始まりなので rank-1)
        int points = GetPointsForRank(rank);

        // プレイヤーにスコア加算
        GivePointsToPlayer(playerTag, points);

        Debug.Log($"Player {playerTag} 脱落。順位: {rank}位, ポイント: {points}p");

        // 生存数を減らす
        activePlayerCount--;

        // --- 最後の1人になったかチェック ---
        if (activePlayerCount == 1)
        {
            HandleWinner();
        }
    }

    // 最後の生存者（勝者）の処理
    void HandleWinner()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (PlayerController pc in allPlayers)
        {
            if (!deadPlayerTags.Contains(pc.PlayerTag))
            {
                // この人が1位！
                int points = GetPointsForRank(1); // 1位のポイント
                pc.AddScore(points);
                Debug.Log($"優勝！ Player {pc.PlayerTag}。順位: 1位, ポイント: {points}p");
                break;
            }
        }

        // ★ 全員のスコアをGameDataに保存してシーン移動
        SaveAllScores();
        Debug.Log("3秒後にリザルト画面へ移動します...");
        Invoke("GoToResultScene", 3.0f);
    }

    void SaveAllScores()
    {
        GameData.FinalScores.Clear();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController pc in players)
        {
            // 各プレイヤーのタグとスコアを保存
            GameData.FinalScores[pc.PlayerTag] = pc.currentScore;
        }
    }

    void GoToResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    // 順位に応じたポイントを安全に取得する関数
    int GetPointsForRank(int rank)
    {
        int index = rank - 1;
        if (index >= 0 && index < rankPoints.Length)
        {
            return rankPoints[index];
        }
        return 0;
    }

    // 指定したタグのプレイヤーにポイントを加算
    void GivePointsToPlayer(int tag, int score)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController pc in players)
        {
            if (pc.PlayerTag == tag)
            {
                pc.AddScore(score);
                break;
            }
        }
    }
}