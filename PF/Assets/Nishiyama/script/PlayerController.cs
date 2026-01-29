using UnityEngine;
using System.Collections;
using System.Linq;
using System.Globalization;

public class PlayerController : MonoBehaviour
{
    public int PlayerTag;

    [Header("Score")]
    public int currentScore = 0; // ★ 追加: 現在のスコア

    [Header("レベル管理")]
    public int knockbackLevel = 1; // これをUIで表示します (初期値1)

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
 

    [Header("Move")]
    private Rigidbody rb;
    private Renderer playerRenderer;
    public float moveSpeed = 5f;

    [Header("Punch Timing")]
    public float windUpTime = 0.5f;   // ★追加: パンチが出るまでのタメ時間
    public float recoveryTime = 1.0f; // ★追加: パンチ後の硬直時間
    private bool isAttacking = false; // ★追加: 現在攻撃中かどうかのフラグ
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

    [Header("Spawn Info")]
    // 🔥CHANGE
    public int spawnBoxNumber = 0; // 0 = 未設定

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

    private AnimationController animController;

    void Start()
    {
        Application.targetFrameRate = 60;

        animController = GetComponentInChildren<AnimationController>();

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

        AutoSetupFromNearestSpawnPoint();

        isTimerActive = true;

        StartGaugeTicker();

       
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"Player {PlayerTag} Score: {currentScore} (+{amount})");
    }

    void AutoSetupFromNearestSpawnPoint()
    {
        // 1. シーン内のすべての SpawnPointInfo を探す
        SpawnPointInfo[] allPoints = FindObjectsOfType<SpawnPointInfo>();

        SpawnPointInfo nearestPoint = null;
        float minDistance = 0.5f; // 0.5m以内なら「自分のスポーン地点」とみなす

        foreach (var point in allPoints)
        {
            // 自分とスポーン地点の距離を測る
            float dist = Vector3.Distance(transform.position, point.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                nearestPoint = point;
            }
        }

        // 2. 近くに見つかったら、そこから設定をコピーする
        if (nearestPoint != null)
        {
            horizontalAxis = nearestPoint.horizontalAxis;
            verticalAxis = nearestPoint.verticalAxis;
            punchButton = nearestPoint.punchButton;
            dashButton = nearestPoint.dashButton;

            PlayerTag = nearestPoint.playerNumber;
            spawnBoxNumber = nearestPoint.playerNumber;

            Debug.Log($"[自動設定完了] 私は {nearestPoint.name} にスポーンしました。操作: {horizontalAxis}");
        }
        else
        {
            Debug.LogError("足元に SpawnPointInfo が見つかりません！スポーン位置とキャラの位置がズレているか、スクリプトがありません。");
        }
    }


    public void SetupFromSpawnPoint(SpawnPointInfo info)
    {
        // 1. 番号を受け取る
        PlayerTag = info.playerNumber;
        spawnBoxNumber = info.playerNumber;

        // 2. 文字列のチェックと強制修正
        // データが空っぽ(nullまたは"")なら、自動で「P○_Horizontal」を作る
        if (string.IsNullOrEmpty(info.horizontalAxis))
        {
            Debug.LogWarning($"[自動修正] P{PlayerTag} の入力設定が空でした。自動生成します。");
            horizontalAxis = $"P{PlayerTag}_Horizontal";
            verticalAxis = $"P{PlayerTag}_Vertical";
            punchButton = $"P{PlayerTag}_Punch";
            dashButton = $"P{PlayerTag}_Dash";
        }
        else
        {
            // データが入っていればそれを使う
            horizontalAxis = info.horizontalAxis;
            verticalAxis = info.verticalAxis;
            punchButton = info.punchButton;
            dashButton = info.dashButton;
        }

        Debug.Log($"[設定完了] P{PlayerTag} は '{horizontalAxis}' で動きます");
    }

    void Update()
    {
        // ★追加: まだ番号をもらっていない(0番の)ときは、何もしないで待つ
        if (PlayerTag == 0) return;

        if (isTimeStopped) return;
        // ★デバッグ用：現在どの入力を読み取ろうとしているか確認
        // (確認できたらコメントアウトしてください。ログが大量に出ます)
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log($"[PlayerTag: {PlayerTag}] 確認中: {horizontalAxis} / 入力値: {Input.GetAxisRaw(horizontalAxis)}");
        }



       
        // ★修正: 攻撃中は移動処理などをスキップして、その場で停止させる
        if (isAttacking)
        {
            if (rb != null) rb.velocity = Vector3.zero; // 滑り防止
            return;
        }

        move();

        if (specialGaugeValue == 100)
        {
            // Debug.Log("ULT Rdy");
        }

        // ★修正: ボタンを押したら「Punch()」を直接呼ばず、コルーチンを開始する
        if (Input.GetButtonDown(punchButton))
        {
            StartCoroutine(PunchSequence());
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

        // ボタン入力があり、かつゲージがMAXなら発動
        if (Input.GetAxis(specialButton) > 0 && specialGaugeValue >= 100)
        {
            // =================================================
            // ★ここです！ ゲージを減らすのと同時にアニメ再生命令！
            // =================================================
            if (animController != null)
            {
                animController.PlayUltAnimation(); // 「再生しろ！」と命令
            }

            // 技の効果を発動
            if (specialBeam != null) specialBeam.Activate();
            else if (bombAttack != null) bombAttack.Activate();
            else if (theWorld != null) theWorld.Activate();
            else if (allCounters != null) allCounters.Activate();

            // ゲージを消費 (-100)
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

    IEnumerator PunchSequence()
    {
        isAttacking = true;

        // 1. 予備動作（タメ）
        // ここで「構えモーション」などを入れると良いです
        yield return new WaitForSeconds(windUpTime);

        // 2. パンチ実行
        // 既存のプレハブ生成処理を呼び出します
        Punch();

        // 3. 硬直（パンチした後も少し動けない）
        yield return new WaitForSeconds(recoveryTime);

        isAttacking = false;
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
}