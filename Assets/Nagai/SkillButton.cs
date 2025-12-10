using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Image buttonImage;
    public Outline outline;

    public Color normalColor = Color.white;
    public Color selectedColorP1 = Color.yellow;
    public Color selectedColorP2 = new Color(0.3f, 0.6f, 1f); // 水色
    public Color selectedBoth = Color.green;

    // ★ 必須メソッド！（エラー原因）
    public void SetSelectedState(bool p1, bool p2)
    {
        // アウトラインをつける
        outline.enabled = p1 || p2;

        // 色切り替え
        if (p1 && p2)
            buttonImage.color = selectedBoth;
        else if (p1)
            buttonImage.color = selectedColorP1;
        else if (p2)
            buttonImage.color = selectedColorP2;
        else
            buttonImage.color = normalColor;
    }
}
