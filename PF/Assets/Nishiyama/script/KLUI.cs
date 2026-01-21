using UnityEngine;
using TMPro;

public class MassLevelNumber : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("ここにPlayerControllerがついているオブジェクトを入れる")]
    public PlayerController targetPlayer;

    [Tooltip("数字を表示するテキスト")]
    public TextMeshProUGUI levelText;

    void Update()
    {
        // 参照がなければ何もしない
        if (targetPlayer == null || levelText == null) return;

        // 内部データ(0, 1, 2) に 1 を足して、(1, 2, 3) にする
        int displayLevel = targetPlayer.currentMassStage + 1;

        // テキストを更新
        levelText.text = displayLevel.ToString();

        // 【おまけ】もし文字色も連動させたいなら（変数がpublicなら可能）
        // levelText.color = targetPlayer.massColors[targetPlayer.currentMassStage];
    }
}