using UnityEngine;
using System.Collections;

public class TheWorld : MonoBehaviour
{
    [Header("発動者のタグ（例：Player1）")]
    public string ownerTag;

    [Header("停止時間（秒）")]
    public float stopDuration = 5f;

    [Header("The World 演出エフェクト")]
    public GameObject theWorldEffectPrefab;
    public float effectDuration = 2f;

    // ★効果音設定
    [Header("効果音")]
    public AudioClip activateSound;        // 発動時（0秒）
    public AudioClip stopSound;            // 1.3秒後（時間停止と同時）
    [Range(0.5f, 3.0f)]
    public float soundVolume = 2.0f;

    private bool isActive = false;
    private GameObject owner;
    private GameTimer gameTimer;
    private AudioSource audioSource;

    void Start()
    {
        owner = this.gameObject;
        gameTimer = FindObjectOfType<GameTimer>();

        // AudioSource自動生成
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;  // 2D音
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isActive) return;

        Debug.Log("【The World】発動準備…");
        isActive = true;

        // ★効果音1：発動時即再生（0秒）
        if (activateSound != null)
        {
            audioSource.PlayOneShot(activateSound, soundVolume);
            Debug.Log("【The World】効果音1再生（0秒）");
        }

        // ★1.3秒後に全て実行
        StartCoroutine(ExecuteTheWorld(1.3f));
    }

    // ★1.3秒後に時間停止・効果音2・エフェクトをまとめて実行
    private IEnumerator ExecuteTheWorld(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ★効果音2：時間停止と同時（1.3秒後）
        if (stopSound != null)
        {
            audioSource.PlayOneShot(stopSound, soundVolume);
            Debug.Log("【The World】効果音2再生（1.3秒後）");
        }

        // ★エフェクト生成
        if (theWorldEffectPrefab != null)
        {
            GameObject effect = Instantiate(theWorldEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
            Debug.Log("【The World】エフェクト生成");
        }

        // ★時間停止実行
        Debug.Log("【The World】時よ止まれ…！！");
        StartCoroutine(StopTimeForOthers());
    }

    private IEnumerator StopTimeForOthers()
    {
        // タイマー停止
        if (gameTimer != null)
            gameTimer.isStopped = true;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player == owner) continue;
            if (!string.IsNullOrEmpty(ownerTag) && player.CompareTag(ownerTag)) continue;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isTimeStopped = true;
            }
        }

        yield return new WaitForSeconds(stopDuration);

        // タイマー再開
        if (gameTimer != null)
            gameTimer.isStopped = false;

        // 復帰処理
        foreach (GameObject player in players)
        {
            if (player == owner) continue;
            if (!string.IsNullOrEmpty(ownerTag) && player.CompareTag(ownerTag)) continue;

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isTimeStopped = false;
            }
        }

        Debug.Log("【The World】時は動き出す…");
        isActive = false;
    }
}
