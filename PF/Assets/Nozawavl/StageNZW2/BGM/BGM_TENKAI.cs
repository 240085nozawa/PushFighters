using UnityEngine;

public class BGM_TENKAI : MonoBehaviour
{
    public void ChangeBGM(AudioClip newClip)
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio.clip == newClip) return;

        audio.clip = newClip;
        audio.Play();
    }
}
