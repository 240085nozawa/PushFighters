using UnityEngine;
using TMPro;

public class SpecialGaugeNumber : MonoBehaviour
{
    [Header("自動取得設定")]
    [Tooltip("プレイヤー何番を表示するか")]
    public int targetPlayerNumber = 1;

    // 自動取得
    public PlayerController targetPlayer;

    [Header("数字を表示するテキスト")]
    public TextMeshProUGUI valueText;

    void Update()
    {
        // ★プレイヤーがいなければ探す
        if (targetPlayer == null)
        {
            FindTargetPlayer();
            return;
        }

        if (valueText != null)
        {
            valueText.text = targetPlayer.specialGaugeValue.ToString();
        }
    }

    void FindTargetPlayer()
    {
        var allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var p in allPlayers)
        {
            if (p.PlayerTag == targetPlayerNumber)
            {
                targetPlayer = p;
                break;
            }
        }
    }
}