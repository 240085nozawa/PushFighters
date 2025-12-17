using UnityEngine;
using UnityEngine.InputSystem;

public class StartDrawUI : MonoBehaviour
{
    [SerializeField] private CharacterLottery characterLottery;
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
                characterLottery.StartLottery();
            else
                Debug.LogError("CharacterLottery ñ¢ê›íË");
        }
    }
}
