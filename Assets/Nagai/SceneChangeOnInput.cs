using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SceneChangeByInput : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "ModeSentaku";
    [SerializeField] private float delayTime = 1f;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private AudioSource bgmSource;   // BGM（止めない）
    [SerializeField] private AudioSource seSource;    // SE（押したとき）

    private bool isChanging = false;

    void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.Play();
        }
    }

    void Update()
    {
        if (isChanging) return;

        // キーボードのどれかが押された
        bool anyKey = Keyboard.current?.anyKey.wasPressedThisFrame == true;

        // ゲームパッドのどれかが押された
        bool anyGamepad = false;
        foreach (var pad in Gamepad.all)
        {
            if (pad == null) continue;

            foreach (var control in pad.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                {
                    anyGamepad = true;
                    break;
                }
            }

            if (anyGamepad) break;
        }

        if (anyKey || anyGamepad)
        {
            StartCoroutine(BlinkAndChangeScene());
        }
    }

    private System.Collections.IEnumerator BlinkAndChangeScene()
    {
        isChanging = true;

        // SE再生（BGMは止めない）
        if (seSource != null)
            seSource.Play();

        StartCoroutine(BlinkText());
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(nextSceneName);
    }

    private System.Collections.IEnumerator BlinkText()
    {
        if (messageText == null) yield break;

        float blinkDuration = delayTime;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < blinkDuration)
        {
            visible = !visible;
            messageText.enabled = visible;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        messageText.enabled = true;
    }
}
