using UnityEngine;
using UnityEngine.UI;

public class KnockbackImageUI : MonoBehaviour
{
    [Header("自動取得設定")]
    [Tooltip("プレイヤー何番を表示するか")]
    public int targetPlayerNumber = 1;

    // 自動取得
    public PlayerController targetPlayer;

    [Header("画像をセットするUI")]
    public Image targetImage;

    [Header("画像のリスト (Lv.1, Lv.2, Lv.3)")]
    public Sprite[] levelSprites;

    void Update()
    {
        // ★プレイヤーがいなければ探す
        if (targetPlayer == null)
        {
            FindTargetPlayer();
            return;
        }

        if (targetImage == null || levelSprites.Length == 0) return;

        int index = targetPlayer.currentMassStage;

        if (index >= 0 && index < levelSprites.Length)
        {
            if (targetImage.sprite != levelSprites[index])
            {
                targetImage.sprite = levelSprites[index];
            }
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