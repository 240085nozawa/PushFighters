using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartDrawUI2P : MonoBehaviour
{
    [Header("開始UI（まとめて消す）")]
    [SerializeField] private GameObject startUIRoot;

    [Header("抽選スクリプト（2P用）")]
    [SerializeField] private CharacterLottery2P characterLottery2P;

    [Header("効果音設定")]
    [SerializeField] private AudioSource audioSource;      // SE用の AudioSource をドラッグ
    [SerializeField] private AudioClip startPressSE;       // スペース押した瞬間のSE
    [SerializeField] private AudioClip loopSE;             // 2秒後からループさせるSE（WAV推奨）

    private bool started = false;      // 1回目のスタート済み
    private bool stopping = false;     // 2回目入力で停止フラグ

    void Start()
    {
        if (startUIRoot != null)
            startUIRoot.SetActive(true);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool press =
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame;

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
        // スペース押した瞬間のSE
        if (audioSource != null && startPressSE != null)
        {
            audioSource.PlayOneShot(startPressSE);
        }

        // 2秒待つ
        yield return new WaitForSeconds(2f);

        // 2秒後のループSE開始
        if (audioSource != null && loopSE != null)
        {
            audioSource.clip = loopSE;
            audioSource.loop = true;      // ループON[web:14]
            audioSource.Play();
        }

        // ルーレット開始
        if (characterLottery2P != null)
            characterLottery2P.StartLottery();
        else
            Debug.LogError("❌ CharacterLottery2P が設定されていません");
    }

    void StopDraw()
    {
        stopping = true;

        // ループSEを止める（ルーレットを止めるタイミング）
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        // ここでルーレット停止をしたいなら、CharacterLottery2P 内で完結させる形にする
        // （このクラスからは何も呼ばない、という要望なので何もしない）
    }
}
