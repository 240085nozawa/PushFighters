using UnityEngine;
using System.Collections;
using System.Linq;
using System.Globalization;

public class PlayerController : MonoBehaviour
{
    public int PlayerTag;

    [Header("Score")]
    public int currentScore = 0; // ★ 追加: 現在のスコア

    [Header("ULT")]
    public bool canMove = true;
    [HideInInspector] public bool isTimeStopped = false;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public bool hasBeenStunnedByBomb = false;
    public bool canTakeDamage = true;
    private SpecialBase currentSpecial;
    public int specialGaugeValue = 0;
    private const int MAX_GAUGE = 100;
    private Coroutine gaugeTickerCoroutine;
    private bool isGaugeTicking = false;
    public float increaseultamount = 0.5f;
    public bool canKnockback = true;

    [Header("Mass")]
    public float[] massStages = { 3.0f, 2.0f, 1.0f };
    public int currentMassStage = 0;
    public float recoveryInterval = 2f;
    public float recoveryTimer;
    public bool isRecovering = false;
    public Color[] massColors = { Color.green, Color.yellow, Color.red };

    [Header("Move")]
    private Rigidbody rb;
    private Renderer playerRenderer;
    public float moveSpeed = 5f;
    public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;
    public float rotationSpeed = 10f;
    public GameObject punchHandPrefab;
    public Transform punchPoint;
    public float punchCooldown = 0f;
    private float nextPunchTime = 0f;

    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.1f;
    private float nextDashTime = 0f;
    private bool isDashing = false;

    [Header("controller")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string punchButton = "Punch";
    public string dashButton = "Dash";

    private SpecialExecutor specialExecutor;
    private SpecialBeam specialBeam;
    private BombAttack bombAttack;
    private TheWorld theWorld;
    private AllCounters allCounters;

    public int scoreTimer = 0;
    private bool isTimerActive = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 20f;     // 吹き飛ぶ強さ
    public float knockbackDuration = 0.5f; // 操作不能になる時間

    void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponentInChildren<Renderer>();

        specialBeam = GetComponent<SpecialBeam>();
        bombAttack = GetComponent<BombAttack>();
        theWorld = GetComponent<TheWorld>();
        allCounters = GetComponent<AllCounters>();

        if (rb != null)
        {
            rb.mass = massStages[0];
        }

        if (playerRenderer != null && massColors.Length > 0)
        {
            playerRenderer.material.color = massColors[0];
        }
        isTimerActive = true;

        StartGaugeTicker();
    }

    // ★ 追加: スコア加算用関数
    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"Player {PlayerTag} Score: {currentScore} (+{amount})");
    }

    void Update()
    {
        if (isTimeStopped) return;

        move();

        if (specialGaugeValue == 100)
        {
            // Debug.Log("ULT Rdy");
        }

        if (Input.GetButtonDown(punchButton))
        {
            Punch();
        }

        if (isRecovering)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0f)
            {
                RecoverMass();
                recoveryTimer = recoveryInterval;
            }
        }

        if (!isDashing && Input.GetButtonDown(dashButton) && Time.time >= nextDashTime)
        {
            Dash();
        }

        CheckAndActivateSpecial();
    }

    private void CheckAndActivateSpecial()
    {
        string specialButton = $"P{PlayerTag}_Special";

        if (Input.GetAxis(specialButton) > 0 && specialGaugeValue >= 100)
        {
            if (specialBeam != null) specialBeam.Activate();
            else if (bombAttack != null) bombAttack.Activate();
            else if (theWorld != null) theWorld.Activate();
            else if (allCounters != null) allCounters.Activate();

            DecreaseSpecialGauge(MAX_GAUGE);
        }
    }

    void move()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveZ = Input.GetAxisRaw(verticalAxis);

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (isDashing)
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                LastMoveDirection = moveDirection;
                RotatePlayer(LastMoveDirection);
            }
            return;
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            LastMoveDirection = moveDirection;
            RotatePlayer(LastMoveDirection);
        }
    }

    void RotatePlayer(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void Punch()
    {
        if (Time.time < nextPunchTime) return;
        if (punchHandPrefab == null || punchPoint == null) return;

        nextPunchTime = Time.time + punchCooldown;

        GameObject punchGO = Instantiate(punchHandPrefab, punchPoint.position, punchPoint.rotation);
        punchGO.transform.SetParent(transform);

        AttackSource src = punchGO.GetComponent<AttackSource>();
        if (src == null) src = punchGO.AddComponent<AttackSource>();

        src.ownerTag = PlayerTag;
    }

    public void TakeDamage()
    {
        if (!canTakeDamage) return;

        if (currentMassStage < massStages.Length - 1)
        {
            currentMassStage++;
            if (rb != null) rb.mass = massStages[currentMassStage];
            if (playerRenderer != null && currentMassStage < massColors.Length)
                playerRenderer.material.color = massColors[currentMassStage];
        }

        if (currentMassStage > 0)
        {
            isRecovering = true;
            recoveryTimer = recoveryInterval;
        }
    }

    public void RecoverMass()
    {
        if (currentMassStage > 0)
        {
            currentMassStage--;
            if (rb != null) rb.mass = massStages[currentMassStage];
            if (playerRenderer != null && currentMassStage < massColors.Length)
                playerRenderer.material.color = massColors[currentMassStage];
        }

        if (currentMassStage == 0)
        {
            isRecovering = false;
        }
    }

    void Dash()
    {
        if (Time.time < nextDashTime || isDashing) return;

        nextDashTime = Time.time + dashCooldown;
        isDashing = true;

        Vector3 dashDirection = LastMoveDirection.normalized;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        }
        StartCoroutine(StopDashingAfterTime(dashDuration));
    }

    IEnumerator StopDashingAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDashing = false;
        if (rb != null) rb.velocity = Vector3.zero;
    }

    public void ApplyKnockback(Vector3 direction)
    {
        if (!canKnockback) return; // ノックバック無効フラグがあれば中断

        StartCoroutine(KnockbackRoutine(direction));
    }

    // ノックバックの実際の処理
    IEnumerator KnockbackRoutine(Vector3 direction)
    {
        // 1. 操作不能にする
        canMove = false;
        isDashing = false; // ダッシュもキャンセル

        // 2. 物理挙動で吹き飛ばす
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // 一旦停止
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        Debug.Log($"[Player {PlayerTag}] Knockback!");

        // 3. 硬直時間待つ
        yield return new WaitForSeconds(knockbackDuration);

        // 4. 復帰
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // 滑りを止める
        }
        canMove = true;
    }

    public void IncreaseSpecialGauge(int amount)
    {
        specialGaugeValue = Mathf.Min(specialGaugeValue + amount, MAX_GAUGE);

        if (specialGaugeValue == MAX_GAUGE)
        {
            StopGaugeTicker();
        }
        else if (!isGaugeTicking && specialGaugeValue < MAX_GAUGE)
        {
            StartGaugeTicker();
        }
    }

    public void DecreaseSpecialGauge(int amount)
    {
        specialGaugeValue = Mathf.Max(specialGaugeValue - amount, 0);
        if (!isGaugeTicking && specialGaugeValue < MAX_GAUGE)
        {
            StartGaugeTicker();
        }
    }

    private void StartGaugeTicker()
    {
        if (gaugeTickerCoroutine != null) return;
        if (specialGaugeValue >= MAX_GAUGE) return;

        isGaugeTicking = true;
        gaugeTickerCoroutine = StartCoroutine(GaugeTicker());
    }

    private void StopGaugeTicker()
    {
        if (gaugeTickerCoroutine != null)
        {
            StopCoroutine(gaugeTickerCoroutine);
            gaugeTickerCoroutine = null;
        }
        isGaugeTicking = false;
    }

    IEnumerator GaugeTicker()
    {
        while (specialGaugeValue < MAX_GAUGE)
        {
            yield return new WaitForSeconds(2f);
            IncreaseSpecialGauge(1);
        }
    }

    public void SetSpecial(SpecialBase special)
    {
        currentSpecial = special;
    }

    public void OnGameOver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerFinished(PlayerTag);
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }

    public float GetPunchCooldownRatio()
    {
        if (punchCooldown <= 0f || Time.time >= nextPunchTime) return 1f;
        return 1f - ((nextPunchTime - Time.time) / punchCooldown);
    }

    public float GetDashCooldownRatio()
    {
        if (dashCooldown <= 0f || Time.time >= nextDashTime) return 1f;
        return 1f - ((nextDashTime - Time.time) / dashCooldown);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameOverTag"))
        {
            OnGameOver();
        }

        if (other.CompareTag("wave"))
        {
            // 相手(other)から自分(transform)への方向 = 吹き飛ぶ方向
            Vector3 knockbackDir = (transform.position - other.transform.position).normalized;

            // Y軸（高さ）成分を消して水平に飛ばす
            knockbackDir.y = 0;
            knockbackDir = knockbackDir.normalized;

            ApplyKnockback(knockbackDir);
        }
    }
}