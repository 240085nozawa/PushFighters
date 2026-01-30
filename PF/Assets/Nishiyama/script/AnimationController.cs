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

    [Header("Ultimet Settings")]
    public float ultDuration = 2.0f;
    public GameObject ultEffectPrefab;
    public AudioClip ultSound;

    [Header("Punch Settings")]
    public float windUpTime = 0.1f;   // 発生までの時間（これだけは残さないとエフェクトとズレます）
    // public float recoveryTime = 0.5f; // ←削除しました
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
        float x = Input.GetAxisRaw(horizontalAxis);
        float z = Input.GetAxisRaw(verticalAxis);
        bool hasMoveInput = new Vector2(x, z).sqrMagnitude > 0;

        // 攻撃中でも移動入力をAnimatorに送り続けるように変更
        // これにより、攻撃が終わった瞬間にAnimatorが即座に反応できます
        if (hasMoveInput)
        {
            animator.SetBool("isDash", true);
        }
        else
        {
            animator.SetBool("isDash", false);
        }

        // 攻撃中でなければ新しいパンチを受け付ける
        if (!isAttacking)
        {
            if (Input.GetButtonDown(punchButton))
            {
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

        // 瞬間的に移動アニメーションを止める（滑り防止）
        // ただしUpdateですぐに上書きされるため、実質一瞬だけ止まる効果
        animator.SetBool("isDash", false);
        animator.SetBool("isPunch", true);

        // 1. 発生までのタメ（0.1秒など）
        yield return new WaitForSeconds(windUpTime);

        // 2. ヒット処理（エフェクト・音）
        PlayEffect(punchEffectPrefab);
        PlaySound(punchSound);

        // 3. アニメーターのトリガーを戻す
        animator.SetBool("isPunch", false);

        // 【削除】硬直待機ループを完全に削除しました
        // float timer = 0f;
        // while (timer < recoveryTime) ...

        // 4. 即座に攻撃終了
        // これで次のフレームからすぐに移動や次の攻撃が可能になります
        isAttacking = false;
    }

    IEnumerator AnimateUltSequence()
    {
        isAttacking = true;
        // 必殺技中は移動キー入力を無視したいので、Updateの制御とは別に強制オフし続ける工夫が必要ですが
        // 簡易的にここではDashをオフにします
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