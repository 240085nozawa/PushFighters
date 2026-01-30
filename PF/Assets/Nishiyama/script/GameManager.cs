using UnityEngine;
using System.Collections; // コルーチン用
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

// ★モード選択用の定義
public enum GameMode
{
    TwoPlayers,  // 2人モード
    FourPlayers  // 4人モード
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("★ モード設定 (シーンに合わせて変更してください)")]
    public GameMode currentGameMode = GameMode.FourPlayers;

    [Header("順位ポイント設定 (2P用: 1位, 2位)")]
    public int[] rankPoints2P = { 50, 10 };

    [Header("順位ポイント設定 (4P用: 1位, 2位, 3位, 4位)")]
    public int[] rankPoints4P = { 50, 30, 15, 10 };

    [Header("現在の生存プレイヤー数")]
    public int activePlayerCount = 0;

    [Header("リザルトシーンの名前")]
    public string resultSceneName = "ResultScene";

    private List<int> deadPlayerTags = new List<int>();

    // ★重複防止用のリスト
    private List<PlayerController> registeredPlayers = new List<PlayerController>();

    private bool isGameStarted = false;
    private bool isGameEnded = false;

    void Awake()
    {
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

        // ★ GameDataの設定もモードに合わせて更新
        GameData.PlayerCount = (currentGameMode == GameMode.TwoPlayers) ? 2 : 4;

        // リスト初期化
        registeredPlayers.Clear();
        activePlayerCount = 0;

        // ゲーム開始待ち
        Invoke("EnableGameCheck", 0.5f);
    }

    void EnableGameCheck()
    {
        isGameStarted = true;
        Debug.Log($"ゲーム開始判定ON。モード: {currentGameMode}, 現在の参加者: {activePlayerCount}人");

        // ★設定ミス警告
        if (currentGameMode == GameMode.TwoPlayers && activePlayerCount > 2)
        {
            Debug.LogError("⚠ 設定ミス: 2人モードですが、3人以上のプレイヤーが検出されています！");
        }
    }

    // ★修正: 重複チェック付きの登録処理
    public void RegisterPlayer(PlayerController player)
    {
        // 1. まったく同じオブジェクトがすでに登録されていたら無視
        if (registeredPlayers.Contains(player))
        {
            return;
        }

        // 2. 違うオブジェクトだけど「同じ番号(P1など)」が来たら警告（重複バグの原因）
        foreach (var p in registeredPlayers)
        {
            if (p.PlayerTag == player.PlayerTag)
            {
                Debug.LogWarning($"⚠ 重複警告: P{player.PlayerTag} が複数体います。余分なプレハブがシーンに残っていませんか？");
                // ここで return すれば重複登録を完全に防げますが、
                // 意図的にやっている可能性もあるので警告に留めて登録は許可します
                // もし「8人」になるのを防ぎたいなら、ここの return のコメントアウトを外してください
                // return; 
            }
        }

        registeredPlayers.Add(player);
        activePlayerCount++;

        Debug.Log($"プレイヤー参加登録: P{player.PlayerTag} (現在 {activePlayerCount}人)");
    }

    public void PlayerFinished(int playerTag)
    {
        if (!isGameStarted) return;
        if (deadPlayerTags.Contains(playerTag)) return;

        deadPlayerTags.Add(playerTag);

        // 現在の生存数 = 順位
        int rank = activePlayerCount;

        // モードに応じたポイントを取得
        int points = GetPointsForMode(rank);

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
        if (isGameEnded) return;
        isGameEnded = true;

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (PlayerController pc in allPlayers)
        {
            if (!deadPlayerTags.Contains(pc.PlayerTag) && pc.gameObject.activeInHierarchy)
            {
                // 1位のポイントを与える
                int points = GetPointsForMode(1);
                pc.AddScore(points);

                GameData.FinalScores[pc.PlayerTag] = pc.currentScore;
                Debug.Log($"優勝！ P{pc.PlayerTag}");
                break;
            }
        }

        // 時間停止していても動くようにコルーチンで遷移
        StartCoroutine(TransitionToResult());
    }

    IEnumerator TransitionToResult()
    {
        Debug.Log("3秒後にリザルト画面へ移動します...");
        // ゲーム内時間が止まっていても待機できる
        yield return new WaitForSecondsRealtime(3.0f);
        SceneManager.LoadScene(resultSceneName);
    }

    // ★モードに応じてポイントを変える
    int GetPointsForMode(int rank)
    {
        int index = rank - 1;
        int[] currentPointsArray;

        if (currentGameMode == GameMode.TwoPlayers)
        {
            currentPointsArray = rankPoints2P;
        }
        else
        {
            currentPointsArray = rankPoints4P; // デフォルトは4人設定
        }

        if (index >= 0 && index < currentPointsArray.Length)
        {
            return currentPointsArray[index];
        }

        return 0;
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

    //void OnGUI()
    //{
    //    // 確認用（不要なら削除してください）
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 40;
    //    style.normal.textColor = Color.red;
    //    GUI.Label(new Rect(10, 10, 500, 100), $"[{currentGameMode}] 生存: {activePlayerCount}人", style);
    //}
}