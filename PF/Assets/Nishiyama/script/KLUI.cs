using UnityEngine;
using TMPro;

public class MassLevelNumber : MonoBehaviour
{
    [Header("自動取得設定")]
    [Tooltip("プレイヤー何番を表示するか")]
    public int targetPlayerNumber = 1;

    // 自動で入るのでドラッグ不要
    public PlayerController targetPlayer;

    [Tooltip("数字を表示するテキスト")]
    public TextMeshProUGUI levelText;

    void Update()
    {
        // ★プレイヤーがいなければ探す
        if (targetPlayer == null)
        {
            FindTargetPlayer();
            return;
        }

        if (levelText == null) return;

        // 表示更新 (0,1,2 -> 1,2,3)
        int displayLevel = targetPlayer.currentMassStage + 1;
        levelText.text = displayLevel.ToString();
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