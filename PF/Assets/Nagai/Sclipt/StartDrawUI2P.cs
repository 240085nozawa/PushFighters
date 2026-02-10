using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Input System必須
using System.Collections;

public class StartDrawUI2P : MonoBehaviour
{
    [Header("開始UI（まとめて消す）")]
    [SerializeField] private GameObject startUIRoot;

    [Header("抽選スクリプト（2P用）")]
    [SerializeField] private CharacterLottery2P characterLottery2P;

    [Header("効果音設定")]
    [SerializeField] private AudioSource audioSource;      // SE用の AudioSource
    [SerializeField] private AudioClip startPressSE;       // 開始時のSE
    [SerializeField] private AudioClip loopSE;             // ループSE

    [Header("SE音量設定")]
    [Range(0f, 3f)]
    [SerializeField] private float seVolume = 1.5f;          // ★インスペクター操作用★

    private bool started = false;      // スタートしたか
    private bool stopping = false;     // ストップ済みか

    void Start()
    {
        if (startUIRoot != null)
            startUIRoot.SetActive(true);
    }

    void Update()
    {
        // ---------------------------------------------------------
        // 入力チェック (キーボード & ゲームパッド)
        // ---------------------------------------------------------
        bool press = false;

        // ⌨️ キーボード (Space / Enter)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                press = true;
            }
        }

        // 🎮 ゲームパッド (Aボタン / 南ボタン)
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                press = true;
            }
        }

        if (!press) return;

        // ---------------------------------------------------------
        // 処理実行
        // ---------------------------------------------------------
        if (!started)
        {
            StartDraw();
        }
        else if (!stopping)
        {
            StopDraw();
        }
    }

    void StartDraw()
    {
        started = true;

        if (startUIRoot != null)
            startUIRoot.SetActive(false);

        StartCoroutine(StartDrawCoroutine());
    }

    IEnumerator StartDrawCoroutine()
    {
        // 開始SE
        if (audioSource != null && startPressSE != null)
        {
            audioSource.PlayOneShot(startPressSE, seVolume);
        }

        // 2秒待機
        yield return new WaitForSeconds(2f);

        // ループSE開始
        if (audioSource != null && loopSE != null)
        {
            audioSource.clip = loopSE;
            audioSource.volume = seVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        // ルーレット開始
        if (characterLottery2P != null)
        {
            characterLottery2P.StartLottery();
        }
        else
        {
            Debug.LogError("❌ CharacterLottery2P が設定されていません");
        }
    }

    void StopDraw()
    {
        stopping = true;

        // 音停止
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        // ルーレット停止
        if (characterLottery2P != null)
        {
            characterLottery2P.StopLottery();
        }
    }
}
