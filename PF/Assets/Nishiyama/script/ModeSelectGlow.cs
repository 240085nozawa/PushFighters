using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Input Systemを使用

public class ModeSelectGlow : MonoBehaviour
{
    [System.Serializable]
    public class ModeButton
    {
        [Header("ボタン設定")]
        public GameObject buttonObject; // 光らせる対象のオブジェクト
        public Outline outline;         // Outlineコンポーネント

        [Header("モード設定")]
        public int playerCount;         // 人数 (2 or 4)
        public string sceneName;        // ★ 移動先のシーン名 ("BattleScene_2P" など)
    }

    public ModeButton[] modeButtons; // インスペクターで設定するリスト
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

        // ⌨️ 【キーボード操作: 矢印キー または WASD】
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            {
                horizontal = 1f;
            }
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
                // 右へ移動（最大値で止まる）
                currentIndex = Mathf.Min(currentIndex + 1, modeButtons.Length - 1);
                UpdateSelection();
                inputTimer = 0f;
            }
            else if (horizontal < -0.5f)
            {
                // 左へ移動（0で止まる）
                currentIndex = Mathf.Max(currentIndex - 1, 0);
                UpdateSelection();
                inputTimer = 0f;
            }
        }

        // 🎯 決定（Space または Enter）
        bool keySubmit = false;
        if (Keyboard.current != null)
        {
            keySubmit = Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame;
        }

        if (keySubmit)
        {
            SelectMode();
        }
    }

    // 選択状態の見た目を更新（Outlineを光らせる）
    void UpdateSelection()
    {
        for (int i = 0; i < modeButtons.Length; i++)
        {
            bool selected = (i == currentIndex);

            if (modeButtons[i].outline != null)
            {
                modeButtons[i].outline.enabled = selected;

                if (selected)
                {
                    // 選択中は黄色く光らせる
                    modeButtons[i].outline.effectColor = new Color(1f, 1f, 0.3f, 1f);
                    modeButtons[i].outline.effectDistance = new Vector2(8f, 8f);
                }
            }
        }
    }

    // 決定時の処理
    void SelectMode()
    {
        canInput = false;
        var selected = modeButtons[currentIndex];

        Debug.Log($"決定: {selected.buttonObject.name}, 人数: {selected.playerCount}, 移動先: {selected.sceneName}");

        // ▼ データを保存（必要に応じてGameDataなどに変更してください）
        SkillSelectionData.playerCount = selected.playerCount;

        // ★★★ ここで設定されたシーンへ移動します ★★★
        if (!string.IsNullOrEmpty(selected.sceneName))
        {
            SceneManager.LoadScene(selected.sceneName);
        }
        else
        {
            Debug.LogError("移動先のシーン名が設定されていません！インスペクターを確認してください。");
        }
    }
}