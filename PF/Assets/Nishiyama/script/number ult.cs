using UnityEngine;
using TMPro;

public class SpecialGaugeNumber : MonoBehaviour
{
    [Header("ここに参照したいプレイヤーをドラッグ")]
    public PlayerController targetPlayer; // ★ここをpublicにして直接アタッチできるように変更

    [Header("数字を表示するテキスト")]
    public TextMeshProUGUI valueText;

    void Update()
    {
        // プレイヤーがセットされていない場合は何もしない（エラー防止）
        if (targetPlayer == null) return;

        // テキストの更新処理
        if (valueText != null)
        {
            // プレイヤーのゲージ数値をそのまま表示
            valueText.text = targetPlayer.specialGaugeValue.ToString();
        }
    }
}
