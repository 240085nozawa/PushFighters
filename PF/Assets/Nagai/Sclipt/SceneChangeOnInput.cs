using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Input Systemを使うために必要
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

        bool isPressed = false;

        // 1. キーボードのエンターキーなど（デバッグ用などに残す場合はここを調整、不要なら削除可）
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            isPressed = true;
        }

        // 2. ゲームパッド入力のチェック
        // 接続されているすべてのゲームパッドを確認する
        foreach (var pad in Gamepad.all)
        {
            if (pad == null) continue;

            // ★修正箇所: buttonSouth (Xbox:A, PS:×, Switch:B) が押されたかチェック
            if (pad.buttonSouth.wasPressedThisFrame)
            {
                isPressed = true;
                break;
            }
        }

        // 入力があったらシーン遷移処理を開始
        if (isPressed)
        {
            StartCoroutine(BlinkAndChangeScene());
        }
    }

    private IEnumerator BlinkAndChangeScene()
    {
        isChanging = true;

        // SE再生
        if (seSource != null)
            seSource.Play();

        // 点滅開始
        StartCoroutine(BlinkImage());

        // 指定時間待つ
        yield return new WaitForSeconds(delayTime);

        // シーンロード
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