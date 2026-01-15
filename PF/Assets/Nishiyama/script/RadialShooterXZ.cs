using UnityEngine;
using System.Collections;

public class RadialShooterRandom : MonoBehaviour
{
    // ==========================================
    // ▼ 表示用オブジェクトの設定
    // ==========================================
    [Header("時間になったら表示させるプレハブ")]
    public GameObject targetPrefab;
    public Vector3 targetSpawnPos = new Vector3(0, 0, 0);

    [Header("表示から発射までの待機時間（秒）")]
    public float delayTime = 1.0f;

    // ==========================================
    // ▼ 弾の発射設定
    // ==========================================
    [Header("発射設定")]
    public GameObject ballPrefab;
    public int bulletCount = 12;
    public float speed = 10f;
    public float spawnHeight = 1.0f;
    public float spawnRadius = 1.0f;

    [Header("ランダムタイマー設定")]
    public float timeLimit = 120f;
    public int shootCount = 2;

    [Header("連続発射防止")] // ★追加
    public float coolTime = 5.0f; // ★追加: これより短い間隔では発動させない

    // --- 内部変数 ---
    private float timer = 0f;
    private float[] targetTimes;
    private bool[] hasFired;
    private bool isFinished = false;

    void Start()
    {
        targetTimes = new float[shootCount];
        hasFired = new bool[shootCount];

        string logMessage = "今回のイベント予定時刻: ";

        // ★★★ 修正: 時間決定ロジック ★★★
        for (int i = 0; i < shootCount; i++)
        {
            float candidateTime = 0f;
            bool isTimeValid = false;
            int attempts = 0; // 無限ループ防止用のカウント

            // 有効な時間が決まるまで再抽選（最大100回試行）
            while (!isTimeValid && attempts < 100)
            {
                attempts++;

                // ランダムな時間を候補として生成
                float safeMaxTime = Mathf.Max(1.0f, timeLimit - delayTime - 2.0f);
                candidateTime = Random.Range(1.0f, safeMaxTime);

                // 「過去に決めた時間」と近すぎないかチェック
                isTimeValid = true; // とりあえずOKと仮定
                for (int j = 0; j < i; j++)
                {
                    // 差の絶対値がクールタイム未満ならNG
                    if (Mathf.Abs(candidateTime - targetTimes[j]) < coolTime)
                    {
                        isTimeValid = false;
                        break;
                    }
                }
            }

            // 決定した時間を保存
            targetTimes[i] = candidateTime;
            hasFired[i] = false;
            logMessage += $"[{targetTimes[i]:F1}秒] ";
        }

        Debug.Log(logMessage);
    }

    void Update()
    {
        if (isFinished) return;

        timer += Time.deltaTime;

        if (timer >= timeLimit)
        {
            isFinished = true;
            Debug.Log("終了。");
            return;
        }

        // 予定時刻チェック
        for (int i = 0; i < shootCount; i++)
        {
            if (!hasFired[i] && timer >= targetTimes[i])
            {
                StartCoroutine(SpawnAndShootSequence(i + 1));
                hasFired[i] = true;
            }
        }

        // 手動テスト
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnAndShootSequence(0));
        }
    }

    IEnumerator SpawnAndShootSequence(int countIndex)
    {
        SpawnObjectAt(targetSpawnPos);

        yield return new WaitForSeconds(delayTime);

        Shoot();
    }

    public void SpawnObjectAt(Vector3 position)
    {
        if (targetPrefab != null)
        {
            Instantiate(targetPrefab, position, Quaternion.identity);
        }
    }

    public void Shoot()
    {
        if (ballPrefab == null) return;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

            Vector3 centerPos = new Vector3(transform.position.x, spawnHeight, transform.position.z);
            Vector3 spawnPos = centerPos + (direction * spawnRadius);

            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                rb.velocity = direction * speed;
            }
        }
    }
}