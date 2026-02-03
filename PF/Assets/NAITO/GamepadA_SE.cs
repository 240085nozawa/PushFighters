using UnityEngine;

public class GamepadA_SE : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;   // インスペクターで設定
    [SerializeField] AudioClip seClip;         // インスペクターで設定
    [SerializeField] float cooldown = 0.2f;    // 連打防止用クールタイム(秒)

    float lastPlayTime = -999f;

    void Update()
    {
        // Aボタン（Gamepadの「joystick button 0」を Input Manager で "Jump" に割り当てている想定）
        if (Input.GetButtonDown("Jump"))
        {
            // 一定時間内は鳴らさない
            if (Time.time - lastPlayTime >= cooldown)
            {
                PlaySE();
                lastPlayTime = Time.time;
            }
        }
    }

    void PlaySE()
    {
        if (audioSource == null || seClip == null) return;

        audioSource.PlayOneShot(seClip);
    }
}
