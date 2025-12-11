using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ModeSelectGlow : MonoBehaviour
{
    [System.Serializable]
    public class ModeButton
    {
        public GameObject buttonObject;
        public Outline outline;
        public int playerCount;  // 2 or 4 を割り当てる
    }

    public ModeButton[] modeButtons;
    private int currentIndex = 0;

    private bool canInput = true;
    private float inputDelay = 0.25f;
    private float inputTimer = 0f;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        if (!canInput) return;
        inputTimer += Time.deltaTime;

        float horizontal = 0f;

        // 🎮 【ゲームパッド】
        if (Gamepad.current != null)
        {
            horizontal = Gamepad.current.leftStick.x.ReadValue();
            if (Gamepad.current.dpad.right.isPressed) horizontal = 1f;
            else if (Gamepad.current.dpad.left.isPressed) horizontal = -1f;
        }

        // ⌨️ 【キーボード】
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.isPressed) horizontal = 1f;
            else if (Keyboard.current.leftArrowKey.isPressed) horizontal = -1f;
        }

        // ⏱ 入力遅延
        if (inputTimer >= inputDelay)
        {
            if (horizontal > 0.5f)
            {
                currentIndex = Mathf.Min(currentIndex + 1, modeButtons.Length - 1);
                UpdateSelection();
                inputTimer = 0f;
            }
            else if (horizontal < -0.5f)
            {
                currentIndex = Mathf.Max(currentIndex - 1, 0);
                UpdateSelection();
                inputTimer = 0f;
            }
        }

        // 🎯 決定（Bボタン or Enter）
        bool padSubmit = (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);
        bool keySubmit = (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame);

        if (padSubmit || keySubmit)
        {
            SelectMode();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < modeButtons.Length; i++)
        {
            bool selected = (i == currentIndex);
            modeButtons[i].outline.enabled = selected;

            if (selected)
            {
                modeButtons[i].outline.effectColor = new Color(1f, 1f, 0.3f, 1f);
                modeButtons[i].outline.effectDistance = new Vector2(8f, 8f);
            }
        }
    }

    void SelectMode()
    {
        canInput = false;
        var selected = modeButtons[currentIndex];

        Debug.Log($"選択されたモード: {selected.buttonObject.name}, 人数: {selected.playerCount}");

        SkillSelectionData.playerCount = selected.playerCount;

        SceneManager.LoadScene("SpecialSelectScene");
    }
}
