using UnityEngine;
using UnityEngine.InputSystem;

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

        bool input =
            (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) ||
            (Keyboard.current != null &&
             (
                 Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                 Keyboard.current.spaceKey.wasPressedThisFrame
             ));

        if (input)
        {
            Debug.Log("Start pressed");

            started = true;
            gameObject.SetActive(false);

            if (characterLottery != null)
                characterLottery.StartLotteryCoroutine();  // これでコルーチン開始
            else
                Debug.LogError("CharacterLottery 未設定");
        }
    }
}
