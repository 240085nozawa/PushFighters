using UnityEngine;
using UnityEngine.InputSystem; // Input System必須

public class StartDrawUI : MonoBehaviour
{
    [Header("抽選スクリプト（4P用）")]
    [SerializeField] private CharacterLottery characterLottery;

    [Header("効果音設定")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startPressSE;

    private bool started = false;

    void Update()
    {
        if (started) return;

        // ---------------------------------------------------------
        // 入力チェック (キーボード & ゲームパッド)
        // ---------------------------------------------------------
        bool input = false;

        // 🎮 ゲームパッド (Aボタン / 南ボタン)
        // ★ buttonEast から buttonSouth に変更しました
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            input = true;
        }

        // ⌨️ キーボード (Enter / Space)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                input = true;
            }
        }

        // ---------------------------------------------------------
        // 実行処理
        // ---------------------------------------------------------
        if (input)
        {
            Debug.Log("Start pressed");

            started = true;
            gameObject.SetActive(false); // 自分を消す

            if (characterLottery != null)
                characterLottery.StartLotteryCoroutine();  // コルーチン開始
            else
                Debug.LogError("CharacterLottery 未設定");
        }
    }
}