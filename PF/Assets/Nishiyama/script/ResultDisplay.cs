using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class ResultDisplay : MonoBehaviour
{
    [System.Serializable]
    public class RankSlot
    {
        public Image rankImage;   // 順位Image（No.1など）
        public Image nameImage;   // 名前Image（Player / キャラ）
    }

    [Header("順位スロット")]
    public RankSlot[] rankSlots;

    [Header("順位画像（0 = No.1, 1 = No.2 ...）")]
    public Sprite[] rankSprites;

    [Header("名前画像（0 = Player1, 1 = Player2 ...）")]
    public Sprite[] nameSprites;

    [Header("戻るシーン名")]
    public string nextSceneName = "Title";

    void Start()
    {
        ShowResults();
    }

    void Update()
    {
        // コントローラー Aボタン（Xbox: Button0）
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowResults()
    {
        if (GameData.FinalScores == null || GameData.FinalScores.Count == 0)
        {
            // データなし → 全非表示
            foreach (var slot in rankSlots)
            {
                slot.rankImage.enabled = false;
                slot.nameImage.enabled = false;
            }
            return;
        }

        // スコアの高い順に並び替え
        var sortedScores = GameData.FinalScores
            .OrderByDescending(x => x.Value)
            .ToList();

        int currentRank = 1;

        for (int i = 0; i < rankSlots.Length; i++)
        {
            if (i < sortedScores.Count)
            {
                int playerTag = sortedScores[i].Key;
                int score = sortedScores[i].Value;

                // 同点順位処理（1位・1位・3位）
                if (i > 0 && score < sortedScores[i - 1].Value)
                {
                    currentRank = i + 1;
                }

                // ===== 順位Image =====
                int rankIndex = currentRank - 1;
                if (rankIndex >= 0 && rankIndex < rankSprites.Length)
                {
                    rankSlots[i].rankImage.sprite = rankSprites[rankIndex];
                    rankSlots[i].rankImage.enabled = true;
                }
                else
                {
                    rankSlots[i].rankImage.enabled = false;
                }

                // ===== 名前Image =====
                int nameIndex = playerTag - 1;
                if (nameIndex >= 0 && nameIndex < nameSprites.Length)
                {
                    rankSlots[i].nameImage.sprite = nameSprites[nameIndex];
                    rankSlots[i].nameImage.enabled = true;
                }
                else
                {
                    rankSlots[i].nameImage.enabled = false;
                }
            }
            else
            {
                // 余ったスロットは非表示
                rankSlots[i].rankImage.enabled = false;
                rankSlots[i].nameImage.enabled = false;
            }
        }
    }
}