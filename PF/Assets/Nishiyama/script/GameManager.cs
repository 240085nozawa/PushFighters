using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("順位ポイント設定 (1位, 2位, 3位, 4位...)")]
    public int[] rankPoints = { 50, 30, 15, 10 };

    [Header("現在の生存プレイヤー数")]
    public int activePlayerCount;

    [Header("リザルトシーンの名前")]
    public string resultSceneName = "ResultScene";

    // 脱落したプレイヤーのリスト
    private List<int> deadPlayerTags = new List<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // ゲーム開始時にスコアデータをリセット
        GameData.FinalScores.Clear();

        // ★修正: FindObjectsOfTypeは非表示のオブジェクトも拾うことがあるため、
        // 「実際にActiveなやつ」だけをフィルタリングして数える
        var allPlayers = FindObjectsOfType<PlayerController>();
        int count = 0;
        foreach (var p in allPlayers)
        {
            if (p.gameObject.activeInHierarchy) // シーンに出てきているかチェック
            {
                count++;
            }
        }
        activePlayerCount = count;

        Debug.Log($"============== ゲーム開始 ==============");
        Debug.Log($"現在認識されているプレイヤー数: {activePlayerCount} 人");
        // もしここで「3」とか「4」と出たら、シーンのどこかに余計なキャラが隠れています！
    }

    // プレイヤーがゲームオーバーになったら呼ばれる
    public void PlayerFinished(int playerTag)
    {
        if (deadPlayerTags.Contains(playerTag)) return;

        deadPlayerTags.Add(playerTag);

        // 順位とポイント計算
        int rank = activePlayerCount;
        int points = GetPointsForRank(rank);

        // ポイント加算
        GivePointsToPlayer(playerTag, points);

        // ★修正点1: 脱落した瞬間に、その人の最終スコアをGameDataに保存する
        // (後でDestroyされても大丈夫なようにする)
        SavePlayerScore(playerTag);

        Debug.Log($"Player {playerTag} 脱落。順位: {rank}位, ポイント: {points}p");

        activePlayerCount--;

        if (activePlayerCount == 1)
        {
            HandleWinner();
        }
    }

    void HandleWinner()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (PlayerController pc in allPlayers)
        {
            if (!deadPlayerTags.Contains(pc.PlayerTag))
            {
                // 1位の処理
                int points = GetPointsForRank(1);
                pc.AddScore(points);

                // ★修正点2: 勝者のスコアも保存
                GameData.FinalScores[pc.PlayerTag] = pc.currentScore;

                Debug.Log($"優勝！ Player {pc.PlayerTag}。順位: 1位, ポイント: {points}p");
                break;
            }
        }

        // シーン遷移
        Debug.Log("リザルト画面へ移動します...");
        Invoke("GoToResultScene", 3.0f);
    }

    // ★修正点3: 指定したプレイヤーの現在のスコアをGameDataに書き込む
    void SavePlayerScore(int playerTag)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var pc in players)
        {
            if (pc.PlayerTag == playerTag)
            {
                // 辞書に登録（上書き）
                GameData.FinalScores[playerTag] = pc.currentScore;
                break;
            }
        }
    }

    void GoToResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    int GetPointsForRank(int rank)
    {
        int index = rank - 1;
        if (index >= 0 && index < rankPoints.Length)
        {
            return rankPoints[index];
        }
        return 0;
    }

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