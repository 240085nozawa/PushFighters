using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SpecialSelectController : MonoBehaviour
{
    [Header("アウトラインUI")]
    public SkillButton[] skillButtons;   // ← Outlineつきボタン
                                         // 0=ビーム 1=時間停止 2=自爆 3=カウンター

    private int index1P = 0;
    private int index2P = 0;

    private bool decided1P = false;
    private bool decided2P = false;

    public string nextScene = "MainScene";

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        Handle1P();
        Handle2P();

        // 両方決定済みならシーン移動
        if (decided1P && decided2P)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    // -----------------------------
    // 1P 操作 （← → で移動、Enterで決定）
    // -----------------------------
    void Handle1P()
    {
        if (!decided1P)
        {
            // ←
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                index1P = Mathf.Max(0, index1P - 1);
                UpdateUI();
            }

            // →
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                index1P = Mathf.Min(skillButtons.Length - 1, index1P + 1);
                UpdateUI();
            }

            // 決定（Enter）
            if (Input.GetKeyDown(KeyCode.Return))
            {
                decided1P = true;
                SpecialManager.Instance.SelectSkill(1, index1P);
                UpdateUI();
            }
        }
    }

    // -----------------------------
    // 2P 操作 （A D で移動、右Shiftで決定）
    // -----------------------------
    void Handle2P()
    {
        if (!decided2P)
        {
            // A
            if (Input.GetKeyDown(KeyCode.A))
            {
                index2P = Mathf.Max(0, index2P - 1);
                UpdateUI();
            }

            // D
            if (Input.GetKeyDown(KeyCode.D))
            {
                index2P = Mathf.Min(skillButtons.Length - 1, index2P + 1);
                UpdateUI();
            }

            // 決定（右Shift）
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                decided2P = true;
                SpecialManager.Instance.SelectSkill(2, index2P);
                UpdateUI();
            }
        }
    }

    // -----------------------------
    // UI更新（両者のアウトライン色変更）
    // -----------------------------
    void UpdateUI()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            bool p1 = (i == index1P);
            bool p2 = (i == index2P);

            skillButtons[i].SetSelectedState(p1, p2);
        }
    }
}
