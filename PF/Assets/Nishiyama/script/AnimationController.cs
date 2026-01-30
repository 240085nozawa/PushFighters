using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private AudioSource audioSource;

    [Header("Input Settings")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string punchButton = "Punch";

    [Header("Punch Cooldown")] // ★追加: パンチ専用のクールタイム設定
    public float punchCooldown = 0.5f; // 連打できる間隔（秒）
    private float nextPunchTime = 0f;  // 次にパンチできる時間

    [Header("Ultimet Settings")]
    public float ultDuration = 2.0f;
    public GameObject ultEffectPrefab;
    public AudioClip ultSound;

    [Header("Punch Settings")]
    public float windUpTime = 0.1f;
    public GameObject punchEffectPrefab;
    public AudioClip punchSound;

    // 攻撃中かどうか
    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        SetupFromParentController();
    }

    void SetupFromParentController()
    {
        PlayerController pc = GetComponentInParent<PlayerController>();
        if (pc != null)
        {
            horizontalAxis = pc.horizontalAxis;
            verticalAxis = pc.verticalAxis;
            punchButton = pc.punchButton;
        }
        else
        {
            AutoSetupFromNearestSpawnPoint();
        }
    }

    void AutoSetupFromNearestSpawnPoint()
    {
        SpawnPointInfo[] allPoints = FindObjectsOfType<SpawnPointInfo>();
        SpawnPointInfo nearestPoint = null;
        float minDistance = 2.0f;
        Vector3 checkPos = transform.root.position;

        foreach (var point in allPoints)
        {
            float dist = Vector3.Distance(checkPos, point.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestPoint = point;
            }
        }
        if (nearestPoint != null)
        {
            horizontalAxis = nearestPoint.horizontalAxis;
            verticalAxis = nearestPoint.verticalAxis;
            punchButton = nearestPoint.punchButton;
        }
    }

    void Update()
    {
        // 1. 移動アニメーション（歩き・走り）
        float x = Input.GetAxisRaw(horizontalAxis);
        float z = Input.GetAxisRaw(verticalAxis);
        bool hasMoveInput = new Vector2(x, z).sqrMagnitude > 0;

        if (hasMoveInput)
        {
            animator.SetBool("isDash", true);
        }
        else
        {
            animator.SetBool("isDash", false);
        }

        // 2. パンチ入力監視（クールタイム判定付き）
        if (!isAttacking)
        {
            // ★修正: 現在時刻が「次にパンチできる時間」を過ぎているかチェック
            if (Input.GetButtonDown(punchButton) && Time.time >= nextPunchTime)
            {
                // 次のパンチが可能になる時間をセット（現在時刻 + クールタイム）
                nextPunchTime = Time.time + punchCooldown;

                StartCoroutine(AnimatePunchSequence());
            }
        }
    }

    public void PlayUltAnimation()
    {
        if (isAttacking) return;
        StartCoroutine(AnimateUltSequence());
    }

    IEnumerator AnimatePunchSequence()
    {
        isAttacking = true;

        animator.SetBool("isDash", false);
        animator.SetBool("isPunch", true);

        // タメ時間
        yield return new WaitForSeconds(windUpTime);

        PlayEffect(punchEffectPrefab);
        PlaySound(punchSound);

        animator.SetBool("isPunch", false);

        // 攻撃終了
        isAttacking = false;
    }

    IEnumerator AnimateUltSequence()
    {
        isAttacking = true;
        animator.SetBool("isDash", false);
        animator.SetBool("isUltimet", true);

        PlayEffect(ultEffectPrefab);
        PlaySound(ultSound);

        yield return new WaitForSeconds(ultDuration);

        animator.SetBool("isUltimet", false);
        isAttacking = false;
    }

    void PlayEffect(GameObject prefab)
    {
        if (prefab != null)
        {
            Vector3 spawnPos = transform.position + transform.forward * 1.0f + Vector3.up * 1.0f;
            Instantiate(prefab, spawnPos, transform.rotation);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}