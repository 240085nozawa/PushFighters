using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartDrawUI2P : MonoBehaviour
{
    [Header("開始UI（まとめて消す）")]
    [SerializeField] private GameObject startUIRoot;

    [Header("抽選スクリプト（2P用）")]
    [SerializeField] private CharacterLottery2P characterLottery2P;

    private bool started = false;

    void Start()
    {
        if (startUIRoot != null)
            startUIRoot.SetActive(true);
    }

    void Update()
    {
        if (started) return;

        bool startInput =
            Keyboard.current != null &&
            (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame
            );

        if (startInput)
        {
            StartDraw();
        }
    }

    void StartDraw()
    {
        started = true;

        if (startUIRoot != null)
            startUIRoot.SetActive(false);

        if (characterLottery2P != null)
            characterLottery2P.StartLottery();
        else
            Debug.LogError("❌ CharacterLottery2P が設定されていません");
    }
}
