using UnityEngine;
using TMPro; // TextMeshProを使用
using System.Collections.Generic;
using System.Linq; // Reverse() のために必要

public class ResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Start()
    {
        // 1. TextMeshProUGUIコンポーネントの初期化チェック
        if (resultText == null)
        {
            resultText = GetComponent<TextMeshProUGUI>();
            if (resultText == null)
            {
                Debug.LogError("ResultDisplay: TextMeshProUGUIコンポーネントが見つかりません。");
                return;
            }
        }

        // 2. GameManagerの存在チェック
        if (GameManager.Instance == null)
        {
            resultText.text = "Error: GameManagerが見つかりません。";
            return;
        }

        List<int> ranking = GameManager.Instance.GetFinalRanking();

        // 勝者 (1位) はリストの最初の要素
        if (ranking.Count > 0)
        {
            int winnerTag = ranking[0];
            resultText.text = $"CONGRATULATIONS!\n\nWINNER:\n\nPLAYER {winnerTag} ";
        }
        else
        {
            resultText.text = "Error: 順位データがありません。";
        }

        
    }
}