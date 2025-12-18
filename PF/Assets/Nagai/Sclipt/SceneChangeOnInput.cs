using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections;

public class SceneChangeByInput : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "ModeSentaku";
    [SerializeField] private float delayTime = 1f;

    [Header("点滅させる Image")]
    [SerializeField] private Image messageImage;

    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;   // BGM（止めない）
    [SerializeField] private AudioSource seSource;    // SE（押したとき）

    private bool isChanging = false;

    void Start()
    {
        if (bgmSource != null)
            bgmSource.Play();
    }

    void Update()
    {
        if (isChanging) return;

        // キーボード入力
        bool anyKey = Keyboard.current?.anyKey.wasPressedThisFrame == true;

        // ゲームパッド入力
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

    private IEnumerator BlinkAndChangeScene()
    {
        isChanging = true;

        // SE再生（BGMは止めない）
        if (seSource != null)
            seSource.Play();

        StartCoroutine(BlinkImage());
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator BlinkImage()
    {
        if (messageImage == null) yield break;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < delayTime)
        {
            visible = !visible;
            messageImage.enabled = visible;

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        messageImage.enabled = true;
    }
}
