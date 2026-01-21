using UnityEngine;
using UnityEngine.UI; // Imageを扱うために必要

public class KnockbackImageUI : MonoBehaviour
{
    [Header("対象のプレイヤー")]
    public PlayerController targetPlayer;

    [Header("画像をセットするUI")]
    public Image targetImage; // ここにUIのImageコンポーネントを入れます

    [Header("画像のリスト (上から Lv.1, Lv.2, Lv.3 の順)")]
    public Sprite[] levelSprites; // 用意した画像をここに登録します

    void Update()
    {
        // 設定が足りない場合は何もしない
        if (targetPlayer == null || targetImage == null || levelSprites.Length == 0) return;

        // プレイヤーの現在の段階 (0, 1, 2) を取得
        int index = targetPlayer.currentMassStage;

        // 配列の範囲内かチェックしてから適用
        if (index >= 0 && index < levelSprites.Length)
        {
            // 現在の画像と違う場合のみ差し替える（負荷軽減）
            if (targetImage.sprite != levelSprites[index])
            {
                targetImage.sprite = levelSprites[index];
            }
        }
    }
}