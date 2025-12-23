using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ResultDisplay : MonoBehaviour
{
    // 既存の resultText は簡易表示用として残すか、削除してもOK
    public TextMeshProUGUI resultText;

    [System.Serializable]
    public class RankSlot
    {
        public TextMeshProUGUI rankText;  // "1st"
        public TextMeshProUGUI scoreText; // "Score: 50"
        public TextMeshProUGUI nameText;  // "Player 1"
    }

    [Header("InspectorでTextを割り当ててください")]
    public RankSlot[] rankSlots; // 1位～4位分の枠

    void Start()
    {
        ShowResults();
    }

    public void ShowResults()
    {
        // GameDataに保存されたスコアデータを取得
        // データがない場合のエラーハンドリング
        if (GameData.FinalScores == null || GameData.FinalScores.Count == 0)
        {
            if (resultText != null) resultText.text = "No Score Data Found.";
            return;
        }

        // スコアが高い順（降順）に並び替え
        var sortedScores = GameData.FinalScores.OrderByDescending(x => x.Value).ToList();

        // 勝者表示（簡易テキスト用）
        if (resultText != null && sortedScores.Count > 0)
        {
            int winnerTag = sortedScores[0].Key;
            resultText.text = $"CONGRATULATIONS!\nWINNER: PLAYER {winnerTag}";
        }

        // 詳細ランキング表示
        if (rankSlots != null)
        {
            for (int i = 0; i < rankSlots.Length; i++)
            {
                if (i < sortedScores.Count)
                {
                    // データがある場合
                    int playerTag = sortedScores[i].Key;
                    int score = sortedScores[i].Value;

                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = $"No.{i + 1}";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = $"{score}p";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = $"Player {playerTag}";
                }
                else
                {
                    // データがない枠は空にする
                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = "-";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = "";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = "";
                }
            }
        }
    }
}