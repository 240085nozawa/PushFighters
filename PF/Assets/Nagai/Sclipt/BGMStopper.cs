using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMStopper : MonoBehaviour
{
    private AudioSource bgmSource;

    [System.Obsolete]
    void Awake()
    {
        // BGM—p‚ÌAudioSource‚ðŽæ“¾
        bgmSource = FindObjectOfType<AudioSource>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
    void Start()
    {
        Debug.Log("BGM Start");
        GetComponent<AudioSource>().Play();
    }


}
