using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ResultDisplay : MonoBehaviour
{
    [System.Serializable]
    public class RankSlot
    {
        public Image rankImage;   // No.1 ～ No.4
        public Image nameImage;   // キャラ画像
    }

    [System.Serializable]
    public class PlayerCharacter
    {
        public int playerTag;          // Player番号（1～4）
        public Sprite characterSprite; // キャラ画像
    }

    public RankSlot[] rankSlots;        // 0=No.1, 1=No.2, 2=No.3, 3=No.4
    public Sprite[] rankSprites;        // 同上
    public PlayerCharacter[] playerCharacters;

    public string nextSceneName = "Title";

    void Start()
    {
        ShowResults();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowResults()
    {
        // 初期化
        foreach (var slot in rankSlots)
        {
            slot.rankImage.enabled = false;
            slot.nameImage.enabled = false;
        }

        if (GameData.FinalScores == null || GameData.FinalScores.Count == 0)
            return;

        // スコア順に並べる
        var sorted = GameData.FinalScores
            .OrderByDescending(x => x.Value)
            .ToList();

        // 各プレイヤーの順位を確定
        Dictionary<int, int> playerRanks = new Dictionary<int, int>();

        int currentRank = 1;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (i > 0 && sorted[i].Value < sorted[i - 1].Value)
            {
                currentRank = i + 1;
            }
            playerRanks[sorted[i].Key] = currentRank;
        }

        // 順位に応じて「直接」スロットへ配置
        foreach (var pair in playerRanks)
        {
            int playerTag = pair.Key;
            int rank = pair.Value;

            int slotIndex = rank - 1;
            if (slotIndex >= rankSlots.Length) continue;

            // 順位画像
            rankSlots[slotIndex].rankImage.sprite = rankSprites[slotIndex];
            rankSlots[slotIndex].rankImage.enabled = true;

            // キャラ画像
            var pc = playerCharacters.First(p => p.playerTag == playerTag);
            rankSlots[slotIndex].nameImage.sprite = pc.characterSprite;
            rankSlots[slotIndex].nameImage.enabled = true;
        }
    }
}