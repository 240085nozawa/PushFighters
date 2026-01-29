using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Input Settings")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string punchButton = "Punch";

    // ULTの入力チェック設定はもう不要です（PlayerControllerがやるので）
    public float ultDuration = 2.0f;

    [Header("Punch Timing")]
    public float windUpTime = 0.5f;
    public float recoveryTime = 1.0f;

    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        // 移動とパンチ用に入力設定だけは親からコピーしておく
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
            // 親がいない場合のバックアップ（足元サーチ）
            AutoSetupFromNearestSpawnPoint();
        }
    }

    void AutoSetupFromNearestSpawnPoint()
    {
        // 足元のスポナーを探すバックアップ処理
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
        // 1. 移動アニメーション
        float x = Input.GetAxisRaw(horizontalAxis);
        float z = Input.GetAxisRaw(verticalAxis);
        bool isMoving = new Vector2(x, z).sqrMagnitude > 0;
        animator.SetBool("isDash", isMoving);

        // 2. パンチアニメーション
        if (!isAttacking && Input.GetButtonDown(punchButton))
        {
            StartCoroutine(AnimatePunchSequence());
        }

        // ★ ULTの入力監視（Input.GetAxis...）は削除しました！
        // PlayerControllerから直接 PlayUltAnimation() を呼んでもらいます。
    }

    // ★ PlayerControllerから「-100された瞬間」に呼ばれる関数
    public void PlayUltAnimation()
    {
        // すでに再生中なら重複させない
        if (isAttacking) return;

        StartCoroutine(AnimateUltSequence());
    }

    IEnumerator AnimatePunchSequence()
    {
        isAttacking = true;
        animator.SetBool("isPunch", true);
        yield return new WaitForSeconds(windUpTime + recoveryTime);
        animator.SetBool("isPunch", false);
        isAttacking = false;
    }

    IEnumerator AnimateUltSequence()
    {
        isAttacking = true;
        animator.SetBool("isUltimet", true);

        Debug.Log(">> ULTアニメーション作動！ <<");

        yield return new WaitForSeconds(ultDuration);
        animator.SetBool("isUltimet", false);
        isAttacking = false;
    }
}