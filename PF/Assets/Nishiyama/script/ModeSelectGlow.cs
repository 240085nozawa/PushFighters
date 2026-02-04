using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ModeSelectGlow : MonoBehaviour
{
    [System.Serializable]
    public class ModeButton
    {
        [Header("ボタン本体")]
        public GameObject buttonObject;

        // ★変更: Outlineではなく、枠用のImageを指定するように変更
        [Header("選択枠の画像 (子のImageなど)")]
        public Image selectionFrame;

        [Header("モード設定")]
        public int playerCount;
        public string sceneName;
    }

    [Header("モードボタン設定")]
    public ModeButton[] modeButtons;

    [Header("選択枠の見た目設定")]
    [Tooltip("選択されたときの色 (アルファ値に注意！)")]
    public Color selectColor = new Color(1f, 1f, 0.3f, 1f); // デフォルトは黄色

    [Header("決定SE設定")]
    [SerializeField] AudioSource confirmAudioSource;
    [SerializeField] AudioClip confirmClip;

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

        // --- 入力検知 ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) horizontal = 1f;
            else if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) horizontal = -1f;
        }

        if (Gamepad.current != null)
        {
            Vector2 dpad = Gamepad.current.dpad.ReadValue();
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (dpad.x > 0.5f || stick.x > 0.5f) horizontal = 1f;
            else if (dpad.x < -0.5f || stick.x < -0.5f) horizontal = -1f;
        }

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

        // --- 決定 ---
        bool isSubmit = false;
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) isSubmit = true;
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) isSubmit = true;

        if (isSubmit)
        {
            StartCoroutine(SelectModeAfterDelay(1f));
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < modeButtons.Length; i++)
        {
            bool selected = (i == currentIndex);
            Image frame = modeButtons[i].selectionFrame;

            if (frame != null)
            {
                // 選択されている画像だけを表示(Enabled)にする
                frame.enabled = selected;

                if (selected)
                {
                    // 色を適用 (元のUIの色に左右されず、このImageの色が変わります)
                    frame.color = selectColor;
                }
            }
        }
    }

    void SelectMode()
    {
        canInput = false;
        var selected = modeButtons[currentIndex];
        SkillSelectionData.playerCount = selected.playerCount;
    }

    System.Collections.IEnumerator SelectModeAfterDelay(float delay)
    {
        SelectMode();
        var selected = modeButtons[currentIndex];

        if (string.IsNullOrEmpty(selected.sceneName)) yield break;

        if (confirmAudioSource != null && confirmClip != null)
        {
            confirmAudioSource.volume = 1.0f;
            confirmAudioSource.PlayOneShot(confirmClip, 2.0f);
            confirmAudioSource.PlayOneShot(confirmClip, 1.5f);
        }

        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(selected.sceneName);
    }
}