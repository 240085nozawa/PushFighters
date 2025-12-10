using UnityEngine;
using System.Collections; // IEnumeratorのために必要
using System.Linq;
using System.Globalization;

public class PlayerController : MonoBehaviour
{
    public int PlayerTag;


    [Header("ULT")]
    public bool canMove = true;     //虚式紫で使ってる
    [HideInInspector] public bool isTimeStopped = false;    //TheWorldで使ってる
    [HideInInspector] public bool isStunned = false; // BombATK用のスタン状態フラグ
    [HideInInspector] public bool hasBeenStunnedByBomb = false; // BombATK用：一度だけスタン
    public bool canTakeDamage = true; // ← 追加
    private SpecialBase currentSpecial;
    public  int specialGaugeValue = 0; // UIが参照する値
    private const int MAX_GAUGE = 100;
    // ★★★ 新規追加: 増加タイマー制御 ★★★
    private Coroutine gaugeTickerCoroutine;
    private bool isGaugeTicking = false;
    public float increaseultamount = 0.5f;
    public bool canKnockback = true; // ノックバックを受けるかどうか



    [Header("Mass")]
    public float[] massStages = { 3.0f, 2.0f, 1.0f }; // ★ Massの3段階を設定
    public int currentMassStage = 0;
    public float recoveryInterval = 2f; // 2秒ごとに回復
    public float recoveryTimer;
    public bool isRecovering = false;
    public Color[] massColors = { Color.green, Color.yellow, Color.red }; // ★ 3段階の色を設定

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
    public string horizontalAxis = "Horizontal"; // WASD用は "Horizontal"
    public string verticalAxis = "Vertical";   // WASD用は "Vertical"
    public string punchButton = "Punch";      // WASD用は "Punch"
    public string dashButton = "Dash";

    //public string specialButtun = "Special";
    private SpecialExecutor specialExecutor;

    // --- 必殺技クラスへの参照 ---
    private SpecialBeam specialBeam;
    private BombAttack bombAttack;
    private TheWorld theWorld;
    private AllCounters allCounters;

    public int scoreTimer = 0; // ★ 1秒ごとに増加させるint型変数
    private bool isTimerActive = false; // タイマーが起動中かどうかのフラグ
    void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody>();

        playerRenderer = GetComponentInChildren<Renderer>();

        // 🔽 ここで「どのスクリプトがついているか」を調べて記録
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
        //specialExecutor = GetComponent<SpecialExecutor>();
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    // ★★★ 修正箇所: 名前検索をやめ、コンポーネントから確実に特定する ★★★
    //    // 自分（このプレハブ）の中にあるカメラを、ON/OFF問わず全て取得
    //    Camera myCam = GetComponentInChildren<Camera>(true);

    //    if (myCam != null)
    //    {
    //        // このプレイヤーが「自分（操作主）」の場合
    //        if (IsOwner)
    //        {
    //            // 自分のカメラのGameObjectをONにする
    //            myCam.gameObject.SetActive(true);
    //            myCam.tag = "MainCamera"; // タグをMainCameraにする

    //            // 音声リスナーもON
    //            AudioListener listener = myCam.GetComponent<AudioListener>();
    //            if (listener != null) listener.enabled = true;

    //            Debug.Log($"[Player {PlayerTag}] 自分のカメラを有効化しました。(Owner)");

    //            // ★重要★ シーンにもともと置いてあった「ロビー用カメラ」などを消す
    //            // これをやらないと、自分のカメラと重複して描画がおかしくなることがあります
    //            Camera[] otherCameras = FindObjectsOfType<Camera>();
    //            foreach (Camera c in otherCameras)
    //            {
    //                // 自分以外の MainCamera タグがついたカメラをOFFにする
    //                if (c != myCam && c.CompareTag("MainCamera"))
    //                {
    //                    c.gameObject.SetActive(false);
    //                }
    //            }
    //        }
    //        // このプレイヤーが「他人（通信相手）」の場合
    //        else
    //        {
    //            // ★重要★ コンポーネントではなく、GameObjectごとOFFにする！
    //            // これにより、カメラについている追尾スクリプトやAudioListenerも一括で止まります
    //            myCam.gameObject.SetActive(false);
    //            myCam.tag = "Untagged"; // 念のためタグも外す

    //            Debug.Log($"[Player {PlayerTag}] 他人のカメラを無効化しました。(Remote)");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError($"[Player {PlayerTag}] カメラが見つかりません！プレハブにCameraコンポーネントがあるか確認してください。");
    //    }
    //    // ★★★ ここまで ★★★


    //    // --- 初期位置の設定 ---
    //    if (OwnerClientId == 0)
    //    {
    //        transform.position = new Vector3(-3, 1, 0);
    //    }
    //    else if (OwnerClientId == 1)
    //    {
    //        transform.position = new Vector3(0, 1, 0);
    //    }
    //    else if (OwnerClientId == 2)
    //    {
    //        transform.position = new Vector3(3, 1, 0);
    //    }
    //}

    void Update()
    {

        //if (!IsOwner)
        //{
        //   return;
        //}

        if (isTimeStopped) return;

      
        move();

        if (specialGaugeValue == 100)
        {
            Debug.Log("ULT Rdy");
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
                recoveryTimer = recoveryInterval; // タイマーをリセット
            }
        }

        // ダッシュ入力のチェックを isDashing でロック
        if (!isDashing && Input.GetButtonDown(dashButton) && Time.time >= nextDashTime)
        {
            Dash();
        }
        // --- 必殺技発動チェック ---
        CheckAndActivateSpecial();
      
    }
    private void CheckAndActivateSpecial()
    {
        // PlayerTagに対応する入力ボタン名を動的に決定
        string specialButton = $"P{PlayerTag}_Special";

        if (Input.GetAxis(specialButton) > 0 && specialGaugeValue >= 100)
        {
            // どの必殺技スクリプトがついているかを確認
            if (specialBeam != null)
            {
                Debug.Log($"[Player{PlayerTag}] SpecialBeam発動！");
                specialBeam.Activate();
            }
            else if (bombAttack != null)
            {
                Debug.Log($"[Player{PlayerTag}] BombAttack発動！");
                bombAttack.Activate();
            }
            else if (theWorld != null)
            {
                Debug.Log($"[Player{PlayerTag}] TheWorld発動！");
                theWorld.Activate();
            }
            else if (allCounters != null)
            {
                Debug.Log($"[Player{PlayerTag}] AllCounters発動！");
                allCounters.Activate();
            }
            else
            {
                Debug.LogWarning($"[Player{PlayerTag}] 必殺技スクリプトがアタッチされていません！");
            }

            DecreaseSpecialGauge(MAX_GAUGE);
        }

    }

    void move()
    {
        //// ★ スタン中は一切動けない
        //if (isStunned)
        //{
        //    return;
        //}

        if (!canMove) return;


        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveZ = Input.GetAxisRaw(verticalAxis);

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // ダッシュ中は通常の移動入力を無視
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
            RotatePlayer(LastMoveDirection); // 回転処理を呼び出し
        }
    }

    void RotatePlayer(Vector3 direction)
    {

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void Punch()
    {
        if (Time.time < nextPunchTime)
        {
            return;
        }

        if (punchHandPrefab == null || punchPoint == null)
        {
            Debug.LogError("PunchHand Prefab または Punch Point が設定されていません！");
            return;
        }

        nextPunchTime = Time.time + punchCooldown;

        GameObject punchGO = Instantiate(punchHandPrefab, punchPoint.position, punchPoint.rotation);

        punchGO.transform.SetParent(transform);

        Debug.Log("パンチハンドを生成し、親子関係を設定しました。");

        AttackSource src = punchGO.GetComponent<AttackSource>();
        if (src == null)
        {
            src = punchGO.AddComponent<AttackSource>();
        }

        src.ownerTag = PlayerTag; // ★この1行が必須！
        Debug.Log($"[Punch] Player{PlayerTag} の PunchHand に AttackSource を設定しました");

    }

    public void TakeDamage()
    {
        if (!canTakeDamage)
        {
            Debug.Log($"[PlayerController] Player{PlayerTag} は現在ダメージ無効中（AllCounters中）");
            return;
        }

        if (currentMassStage < massStages.Length - 1)
        {
            currentMassStage++;

            if (rb != null)
            {
                rb.mass = massStages[currentMassStage];
                Debug.Log($"Massが減少しました。現在のMass: {rb.mass} (Stage {currentMassStage + 1})");
            }

            if (playerRenderer != null && currentMassStage < massColors.Length)
            {
                playerRenderer.material.color = massColors[currentMassStage];
                Debug.Log($"プレイヤーの色がMass Stage {currentMassStage + 1} に対応する色に変化しました。");
            }
        }
        else
        {
            Debug.Log("Massは既に最小値です。");
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

            if (rb != null)
            {
                rb.mass = massStages[currentMassStage];
                Debug.Log($"Massが回復しました。現在のMass: {rb.mass} (Stage {currentMassStage + 1})");
            }

            if (playerRenderer != null && currentMassStage < massColors.Length)
            {
                playerRenderer.material.color = massColors[currentMassStage];
                Debug.Log($"プレイヤーの色がMass Stage {currentMassStage + 1} に対応する色に回復しました。");
            }
        }

        if (currentMassStage == 0)
        {
            isRecovering = false;
            Debug.Log("Mass回復完了。");
        }
    }

    void Dash()
    {
        if (Time.time < nextDashTime || isDashing)
        {
            return;
        }

        nextDashTime = Time.time + dashCooldown;
        isDashing = true;

        Vector3 dashDirection = LastMoveDirection.normalized;

         if (rb != null)
        {
            rb.velocity = Vector3.zero;

            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        }

        Debug.Log($"ダッシュ発動! 次の発動可能時刻: {nextDashTime:F2}");

        StartCoroutine(StopDashingAfterTime(dashDuration));
    }

    IEnumerator StopDashingAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        isDashing = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero; 
        }
    }

    public void IncreaseSpecialGauge(int amount)
    {
        // 現在の値に増加量を加え、最大値 (100) を超えないように制限
        specialGaugeValue = Mathf.Min(specialGaugeValue + amount, MAX_GAUGE);

        if (specialGaugeValue == MAX_GAUGE)
        {
            StopGaugeTicker(); // MAXになったらタイマーを停止
            Debug.Log("必殺技ゲージがMAXになりました！自動増加停止。");
        }
        else if (!isGaugeTicking && specialGaugeValue < MAX_GAUGE) // MAX未満で、停止中の場合
        {
            StartGaugeTicker(); // タイマーを再開
        }
    }

    public void DecreaseSpecialGauge(int amount)
    {
        specialGaugeValue = Mathf.Max(specialGaugeValue - amount, 0); // 最小値は0

        // ゲージがMAX未満になったら自動増加を再開
        if (!isGaugeTicking && specialGaugeValue < MAX_GAUGE)
        {
            StartGaugeTicker();
            Debug.Log("ゲージ減少に伴い、自動増加を再開しました。");
        }
    }

    private void StartGaugeTicker()
    {
        if (gaugeTickerCoroutine != null) return; // 既に実行中なら何もしない
        if (specialGaugeValue >= MAX_GAUGE) return; // MAXなら起動しない

        isGaugeTicking = true;
        gaugeTickerCoroutine = StartCoroutine(GaugeTicker());
        Debug.Log("ゲージ自動増加タイマー開始。");
    }

    private void StopGaugeTicker()
    {
        if (gaugeTickerCoroutine != null)
        {
            StopCoroutine(gaugeTickerCoroutine);
            gaugeTickerCoroutine = null;
        }
        isGaugeTicking = false;
        Debug.Log("ゲージ自動増加タイマー停止。");
    }

    IEnumerator GaugeTicker()
    {
        // 100未満であることを確認しながら、継続的にループ
        while (specialGaugeValue < MAX_GAUGE)
        {
            yield return new WaitForSeconds(2f); // 1秒待機

            // 1秒経ったら1増加させる
            // IncreaseSpecialGaugeを呼び出すことで、増加と同時にMAXチェックが行われる
            IncreaseSpecialGauge(1);
        }
        // ループ終了（MAXに達した）場合、コルーチンは自動で終了する
    }
    public void SetSpecial(SpecialBase special)
    {
        currentSpecial = special;
    }

    IEnumerator SecondTicker()
    {
        // プレイヤーが生きている間、またはタイマーが有効な間、ループ
        while (isTimerActive)
        {
            // 1秒間待機
            yield return new WaitForSeconds(1f);

            // 1秒経ったら変数を1増加させる
            specialGaugeValue++;

            Debug.Log($"タイマー: {specialGaugeValue}秒経過");
        }
    }

    public void OnGameOver()
    {
     
        if (GameManager.Instance != null)
        {
            // ★★★ 修正箇所: PlayerTag (int) を渡す ★★★
            GameManager.Instance.PlayerFinished(PlayerTag);
        }

        // 2. プレイヤーを操作不能/非表示にする
        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// パンチのクールダウン率を取得 (0.0 = 使用直後, 1.0 = 使用可能)
    /// </summary>
  // パンチのクールダウン率を 0.0～1.0 で返す
    public float GetPunchCooldownRatio()
    {
        if (punchCooldown <= 0f || Time.time >= nextPunchTime) return 1f;
        return 1f - ((nextPunchTime - Time.time) / punchCooldown);
    }

    // ダッシュのクールダウン率を 0.0～1.0 で返す
    public float GetDashCooldownRatio()
    {
        if (dashCooldown <= 0f || Time.time >= nextDashTime) return 1f;
        return 1f - ((nextDashTime - Time.time) / dashCooldown);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. 触れたオブジェクトがゲームオーバータグを持つかチェック
        if (other.CompareTag("GameOverTag"))
        {
            Debug.Log($"{gameObject.name} がトラップに接触し、ゲームオーバー！");

            // 2. ゲームオーバー処理の呼び出しと順位確定
            OnGameOver();
        }
    }

}
