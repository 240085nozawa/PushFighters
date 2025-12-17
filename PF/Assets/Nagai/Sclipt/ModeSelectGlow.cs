using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // ★ Input Systemパッケージが必要です

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

        // ⌨️ 【キーボード操作のみ】
        if (Keyboard.current != null)
        {
            // 右移動: 右矢印 または Dキー
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            {
                horizontal = 1f;
            }
            // 左移動: 左矢印 または Aキー
            else if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            {
                horizontal = -1f;
            }
        }

        // ⏱ 入力遅延処理（カーソル移動）
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

        // 🎯 決定（Enter または Space）
        bool keySubmit = false;
        if (Keyboard.current != null)
        {
            keySubmit = Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        if (keySubmit)
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
                // 選択中は黄色く光らせる
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

        // ゲームデータに人数を保存
        // ※ GameDataクラスが存在しない場合はここをコメントアウトするか修正してください
        if (typeof(GameData) != null)
        {
            // SkillSelectionDataではなくGameDataを使う場合はこちら
            // GameData.PlayerCount = selected.playerCount; 
        }

        // 元のコードにあったデータ保存クラス
        SkillSelectionData.playerCount = selected.playerCount;

        SceneManager.LoadScene("SpecialSelectScene");
    }
}