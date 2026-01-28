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
        // 🎮 コントローラーのBボタン（Xbox基準）
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
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

        var sortedScores = GameData.FinalScores
            .OrderByDescending(x => x.Value)
            .ToList();

        if (resultText != null && sortedScores.Count > 0)
        {
            int winnerTag = sortedScores[0].Key;
            resultText.text = $"CONGRATULATIONS!\nWINNER: PLAYER {winnerTag}";
        }

        if (rankSlots != null)
        {
            for (int i = 0; i < rankSlots.Length; i++)
            {
                if (i < sortedScores.Count)
                {
                    int playerTag = sortedScores[i].Key;
                    int score = sortedScores[i].Value;

                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = $"No.{i + 1}";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = $"{score}p";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = $"Player {playerTag}";
                }
                else
                {
                    if (rankSlots[i].rankText) rankSlots[i].rankText.text = "-";
                    if (rankSlots[i].scoreText) rankSlots[i].scoreText.text = "";
                    if (rankSlots[i].nameText) rankSlots[i].nameText.text = "";
                }
            }
        }
    }
}
