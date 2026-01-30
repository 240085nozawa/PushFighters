using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class ResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    [System.Serializable]
    public class RankSlot
    {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI nameText;
    }

    public RankSlot[] rankSlots;

    [Header("Bボタンで戻るシーン名")]
    public string nextSceneName = "Title";

    void Start()
    {
        ShowResults();
    }

    void Update()
    {
        // 🎮 コントローラーのAボタン（Xbox基準 = Button 0）
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void ShowResults()
    {
        if (GameData.FinalScores == null || GameData.FinalScores.Count == 0)
        {
            if (resultText != null) resultText.text = "No Score Data Found.";
            return;
        }

        // スコアの高い順に並べ替え
        var sortedScores = GameData.FinalScores
            .OrderByDescending(x => x.Value)
            .ToList();

        // 1位の表示（同点1位が複数いても、とりあえずリスト先頭の人を表示）
        if (resultText != null && sortedScores.Count > 0)
        {
            int winnerTag = sortedScores[0].Key;
            resultText.text = $"CONGRATULATIONS!\nWINNER: PLAYER {winnerTag}";
        }

        if (rankSlots != null)
        {
            int currentRank = 1;

            for (int i = 0; i < rankSlots.Length; i++)
            {
                if (i < sortedScores.Count)
                {
                    int playerTag = sortedScores[i].Key;
                    int score = sortedScores[i].Value;

                    // ★追加: 同率順位の計算ロジック
                    if (i > 0)
                    {
                        // 前の人とスコアが同じなら、順位(currentRank)はそのまま
                        // スコアが低ければ、本来の順位(i + 1)に更新
                        // 例: 100点(1位), 100点(1位), 80点(3位)
                        if (score < sortedScores[i - 1].Value)
                        {
                            currentRank = i + 1;
                        }
                    }
                    else
                    {
                        currentRank = 1;
                    }

                    // UI反映
                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = $"No.{currentRank}";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = $"{score}p";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = $"Player {playerTag}";
                }
                else
                {
                    // データがないスロットは空欄に
                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = "-";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = "";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = "";
                }
            }
        }
    }
}