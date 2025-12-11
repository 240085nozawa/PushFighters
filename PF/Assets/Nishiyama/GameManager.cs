using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Netcode; // ★★★ 追加: Netcodeのイベントのために必要 ★★★
using Unity.VisualScripting;
// using DG.Tweening.Core.Easing; // 不要なusingディレクティブは削除しました

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string resultSceneName = "ResultScene";
    public List<(int playerNumber, float finishTime)> finishOrder = new List<(int, float)>();
    public int totalPlayersInScene = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

  
   
    public void PlayerFinished(int playerNumber)
    {
        // 1. 敗者リストに追加 (この処理はそのまま維持)
        finishOrder.Add((playerNumber, Time.time));

        Debug.Log($"Player {playerNumber} がゲームオーバーになりました。");

        // 2. 2人対戦の勝利判定を実行
        if (finishOrder.Count == totalPlayersInScene - 1) // 1人ゲームオーバーになった
        {
            Debug.Log("最後の生存者が確定しました。リザルトシーンへ移行します。");
            LoadResultScene();
        }
    }

    // GameManager.cs の GetFinalRanking() 関数 (シンプル化)
    public List<int> GetFinalRanking()
    {
        // 敗者リストを取得 (例: [P1])
        List<int> defeatedPlayers = finishOrder.Select(item => item.playerNumber).ToList();

        List<int> finalRank = new List<int>();

        // プレイヤーの総数から、敗者のリストに載っていないプレイヤーを勝者として特定
        for (int i = 1; i <= totalPlayersInScene; i++)
        {
            // 敗者リストに含まれていないプレイヤーが勝者（1位）
            if (!defeatedPlayers.Contains(i))
            {
                finalRank.Add(i); // 1位プレイヤーをリストの先頭に追加
                break;
            }
        }

        finalRank.AddRange(defeatedPlayers);

        return finalRank;
    }

    private void LoadResultScene()
    {
        // 多重遷移を防ぐためにチェック
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != resultSceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(resultSceneName);
        }
    }

}