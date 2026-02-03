using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ModeSelectGlow : MonoBehaviour
{
    [System.Serializable]
    public class ModeButton
    {
        [Header("ボタン設定")]
        public GameObject buttonObject;
        public Outline outline;

        [Header("モード設定")]
        public int playerCount;
        public string sceneName;
    }

    [Header("モードボタン設定")]
    public ModeButton[] modeButtons;

    [Header("決定SE設定")]
    [SerializeField] AudioSource confirmAudioSource; // 決定音
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

        // --- 移動入力 ---
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

        if (Gamepad.current != null)
        {
            Vector2 dpad = Gamepad.current.dpad.ReadValue();
            Vector2 stick = Gamepad.current.leftStick.ReadValue();

            if (dpad.x > 0.5f || stick.x > 0.5f)
            {
                horizontal = 1f;
            }
            else if (dpad.x < -0.5f || stick.x < -0.5f)
            {
                horizontal = -1f;
            }
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

        // --- 決定入力 ---
        bool isSubmit = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                isSubmit = true;
            }
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                isSubmit = true;
            }
        }

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

            if (modeButtons[i].outline != null)
            {
                modeButtons[i].outline.enabled = selected;

                if (selected)
                {
                    modeButtons[i].outline.effectColor = new Color(1f, 1f, 0.3f, 1f);
                    modeButtons[i].outline.effectDistance = new Vector2(8f, 8f);
                }
            }
        }
    }

    void SelectMode()
    {
        canInput = false;
        var selected = modeButtons[currentIndex];

        Debug.Log($"決定: {selected.buttonObject.name}, 人数: {selected.playerCount}, 移動先: {selected.sceneName}");

        SkillSelectionData.playerCount = selected.playerCount;

        if (string.IsNullOrEmpty(selected.sceneName))
        {
            Debug.LogError("移動先のシーン名が設定されていません！インスペクターを確認してください。");
            canInput = true;
        }
    }

    System.Collections.IEnumerator SelectModeAfterDelay(float delay)
    {
        SelectMode();

        var selected = modeButtons[currentIndex];

        if (string.IsNullOrEmpty(selected.sceneName))
        {
            yield break;
        }

        // ★★★ 爆音決定SE（PlayOneShotで音量2倍突破！）★★★
        if (confirmAudioSource != null && confirmClip != null)
        {
            confirmAudioSource.volume = 1.0f;

            // 複数重複再生で超大音量！
            confirmAudioSource.PlayOneShot(confirmClip, 2.0f);  // 2倍音量
            confirmAudioSource.PlayOneShot(confirmClip, 1.5f);  // 重ねてさらに大音量
        }

        // 1秒待機
        yield return new WaitForSeconds(delay);

        // シーン移動
        SceneManager.LoadScene(selected.sceneName);
    }
}
