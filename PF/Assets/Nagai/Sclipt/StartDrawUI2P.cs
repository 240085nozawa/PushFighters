using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Input System必須

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

        // ---------------------------------------------------------
        // 処理実行
        // ---------------------------------------------------------
        if (!press) return;

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

    System.Collections.IEnumerator StartDrawCoroutine()
    {
        // 開始音
        if (audioSource != null && startPressSE != null)
        {
            audioSource.PlayOneShot(startPressSE);
        }

        // 2秒待機
        yield return new WaitForSeconds(2f);

        // ループ音開始
        if (audioSource != null && loopSE != null)
        {
            audioSource.clip = loopSE;
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

        // 音を止める
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        // ★★★ 追加：ルーレット本体に停止命令を送る ★★★
        if (characterLottery2P != null)
        {
            // CharacterLottery2P に "StopLottery" という関数があると仮定して呼び出します
            // もしエラーが出る場合は、CharacterLottery2Pのコードを見せてください
            characterLottery2P.StopLottery();
        }
    }
}