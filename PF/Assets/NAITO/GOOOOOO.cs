using UnityEngine;

using System.Collections;

public class SlowAudioPlayer : MonoBehaviour

{

    [SerializeField] private AudioClip startAudioClip;

    [SerializeField, Range(0.1f, 2.0f)] private float playbackSpeed = 0.8f; // 0.8=80%速度

    [SerializeField, Range(0f, 5f)] private float delaySeconds = 0.1f; // 再生遅延時間

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

        // 0.2秒後に再生開始

        StartCoroutine(PlayWithDelay(delaySeconds));

        Debug.Log($"音声再生速度: {playbackSpeed}x, 遅延: {delaySeconds}秒");

    }

    private IEnumerator PlayWithDelay(float delay)

    {

        yield return new WaitForSeconds(delay);

        audioSource.Play();

    }

}

