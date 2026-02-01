using UnityEngine;
using UnityEngine.UI;
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
        public Image characterIcon; // 画像用
    }

    [Header("★2Pモード用の設定")]
    [Tooltip("2人プレイ時に表示する親オブジェクト（パネルなど）")]
    public GameObject panel2P;
    [Tooltip("2人プレイ用のスロット一覧")]
    public RankSlot[] rankSlots2P;

    [Header("★4Pモード用の設定")]
    [Tooltip("4人プレイ時に表示する親オブジェクト（パネルなど）")]
    public GameObject panel4P;
    [Tooltip("4人プレイ用のスロット一覧")]
    public RankSlot[] rankSlots4P;

    [Header("シーン設定")]
    public string nextSceneName = "Title";

    void Start()
    {
        ShowResults();
    }

    void Update()
    {
        // 🎮 コントローラーのAボタン
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

        // 1. どちらのモードを使うか判定
        // GameData.PlayerCount が 2以下なら2Pモード、それ以外なら4Pモード
        bool is2PMode = (GameData.PlayerCount <= 2);

        // 2. モードに応じてUIの表示/非表示を切り替え
        if (panel2P != null) panel2P.SetActive(is2PMode);
        if (panel4P != null) panel4P.SetActive(!is2PMode);

        // 3. 使用するスロット配列を決定
        RankSlot[] currentSlots = is2PMode ? rankSlots2P : rankSlots4P;

        // --- 以下、データの流し込み処理 ---

        // スコアの高い順に並べ替え
        var sortedScores = GameData.FinalScores
            .OrderByDescending(x => x.Value)
            .ToList();

        // 勝者表示
        if (resultText != null && sortedScores.Count > 0)
        {
            int winnerTag = sortedScores[0].Key;
            resultText.text = $"CONGRATULATIONS!\nWINNER: PLAYER {winnerTag}";
        }

        if (currentSlots != null)
        {
            int currentRank = 1;

            for (int i = 0; i < currentSlots.Length; i++)
            {
                if (i < sortedScores.Count)
                {
                    int playerTag = sortedScores[i].Key;
                    int score = sortedScores[i].Value;

                    // 同率順位の計算
                    if (i > 0)
                    {
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
                    if (currentSlots[i].rankText) currentSlots[i].rankText.text = $"No.{currentRank}";
                    if (currentSlots[i].scoreText) currentSlots[i].scoreText.text = $"{score}p";
                    if (currentSlots[i].nameText) currentSlots[i].nameText.text = $"Player {playerTag}";

                    // 画像UI反映
                    if (currentSlots[i].characterIcon != null)
                    {
                        if (GameData.PlayerIcons.ContainsKey(playerTag) && GameData.PlayerIcons[playerTag] != null)
                        {
                            currentSlots[i].characterIcon.sprite = GameData.PlayerIcons[playerTag];
                            currentSlots[i].characterIcon.enabled = true;
                            currentSlots[i].characterIcon.preserveAspect = true;
                        }
                        else
                        {
                            currentSlots[i].characterIcon.enabled = false;
                        }
                    }
                }
                else
                {
                    // データがないスロットは空欄に
                    if (currentSlots[i].rankText) currentSlots[i].rankText.text = "-";
                    if (currentSlots[i].scoreText) currentSlots[i].scoreText.text = "";
                    if (currentSlots[i].nameText) currentSlots[i].nameText.text = "";
                    if (currentSlots[i].characterIcon) currentSlots[i].characterIcon.enabled = false;
                }
            }
        }
    }
}