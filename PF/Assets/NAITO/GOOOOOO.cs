using UnityEngine;

public class SlowAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip startAudioClip;
    [SerializeField, Range(0.1f, 2.0f)] private float playbackSpeed = 0.8f; // 0.8=80%速度

    private AudioSource audioSource;

    private void Awake()
    {
        enabled = true;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.clip = startAudioClip;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        enabled = true;
        audioSource.pitch = playbackSpeed; // 再生速度設定（0.5=半速、1.0=通常）
        audioSource.Play();
        Debug.Log($"音声再生速度: {playbackSpeed}x");
    }
}
