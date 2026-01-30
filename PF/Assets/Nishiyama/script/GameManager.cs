using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("順位ポイント設定")]
    public int[] rankPoints = { 50, 30, 15, 10 };

    [Header("現在の生存プレイヤー数")]
    public int activePlayerCount = 0; // 最初は0にしておく

    [Header("リザルトシーンの名前")]
    public string resultSceneName = "ResultScene";

    private List<int> deadPlayerTags = new List<int>();

    // ゲーム開始フラグ（プレイヤーが出揃うまで判定しない用）
    private bool isGameStarted = false;

    void Awake()
    {
        // シーン内にGameManagerが2つあるとバグるので、重複チェック
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        GameData.FinalScores.Clear();
        // ★ここで数えるのをやめました（タイミング問題の元凶なので）

        // 少し待ってからゲーム開始フラグを立てる（スポーン待ち）
        Invoke("EnableGameCheck", 0.5f);
    }

    void EnableGameCheck()
    {
        isGameStarted = true;
        Debug.Log($"ゲーム開始判定ON。現在の参加者: {activePlayerCount}人");
    }

    // ★追加: プレイヤーが自分で登録しに来る場所
    public void RegisterPlayer(PlayerController player)
    {
        activePlayerCount++;
        Debug.Log($"プレイヤー参加登録: P{player.PlayerTag} (現在 {activePlayerCount}人)");
    }

    public void PlayerFinished(int playerTag)
    {
        // ゲーム開始前なら無視
        if (!isGameStarted) return;

        if (deadPlayerTags.Contains(playerTag)) return;

        deadPlayerTags.Add(playerTag);

        int rank = activePlayerCount;
        int points = GetPointsForRank(rank);

        GivePointsToPlayer(playerTag, points);
        SavePlayerScore(playerTag);

        Debug.Log($"P{playerTag} 脱落。残り {activePlayerCount - 1}人");

        activePlayerCount--;

        // 残り1人になったら終了
        if (activePlayerCount <= 1)
        {
            HandleWinner();
        }
    }

    void HandleWinner()
    {
        // まだ生存している人を探す
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (PlayerController pc in allPlayers)
        {
            if (!deadPlayerTags.Contains(pc.PlayerTag) && pc.gameObject.activeInHierarchy)
            {
                int points = GetPointsForRank(1);
                pc.AddScore(points);
                GameData.FinalScores[pc.PlayerTag] = pc.currentScore;
                Debug.Log($"優勝！ P{pc.PlayerTag}");
                break;
            }
        }

        Debug.Log("リザルトへ移動します...");
        Invoke("GoToResultScene", 3.0f);
    }

    void SavePlayerScore(int playerTag)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var pc in players)
        {
            if (pc.PlayerTag == playerTag)
            {
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
        if (index >= 0 && index < rankPoints.Length) return rankPoints[index];
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

    // デバッグ表示（生存人数が見えるように）
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.red;
        GUI.Label(new Rect(10, 10, 500, 100), $"生存: {activePlayerCount}人", style);
    }
}