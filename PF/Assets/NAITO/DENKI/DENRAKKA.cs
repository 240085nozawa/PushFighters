using UnityEngine;

public class FallWithWarning : MonoBehaviour
{
    [Header("Timing")]
    public float waitTime = 30f;        // 落下開始までの待機時間
    public float warningDuration = 5f;  // 揺れ＆点滅する時間
    public float fallDuration = 5f;     // 落下にかける時間

    [Header("Warning visual")]
    public Color warningColor = Color.red;
    public float colorToggleInterval = 0.5f;
    public float shakeMagnitude = 0.05f;  // 揺れの大きさ

    float elapsed;
    bool isWarning;
    bool isFalling;

    Color originalColor;
    Material mat;
    Vector3 basePos;
    float colorTimer;
    bool useWarningColor;

    float fallTimer;
    float startY;
    float targetY = -10f;

    void Start()
    {
        basePos = transform.position;
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // マテリアルインスタンスを取得（共有マテリアルを書き換えないよう注意）
            mat = renderer.material;
            originalColor = mat.color;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // 25〜30秒：警告状態
        if (!isWarning && elapsed >= waitTime - warningDuration && elapsed < waitTime)
        {
            isWarning = true;
            basePos = transform.position; // 現在位置を基準に揺らす
        }

        // 30秒〜：落下状態
        if (!isFalling && elapsed >= waitTime)
        {
            isFalling = true;
            isWarning = false;
            // 落下開始時の設定
            fallTimer = 0f;
            startY = transform.position.y;
            // 色を元に戻しておく
            if (mat != null)
            {
                mat.color = originalColor;
            }
        }

        if (isWarning)
        {
            UpdateWarning();
        }

        if (isFalling)
        {
            UpdateFall();
        }
    }

    void UpdateWarning()
    {
        // 小刻みに揺らす（XZは固定でYだけでもOK）
        Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
        shakeOffset.z = 0f; // 2DならZは固定など
        transform.position = basePos + shakeOffset;

        // 0.5秒ごとに色を切り替え
        if (mat != null)
        {
            colorTimer += Time.deltaTime;
            if (colorTimer >= colorToggleInterval)
            {
                colorTimer -= colorToggleInterval;
                useWarningColor = !useWarningColor;
                mat.color = useWarningColor ? warningColor : originalColor;
            }
        }
    }

    void UpdateFall()
    {
        fallTimer += Time.deltaTime;
        float t = Mathf.Clamp01(fallTimer / fallDuration);
        float newY = Mathf.Lerp(startY, targetY, t);
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // もしランタイム生成の material を使っている場合、必要ならここで破棄
        // if (mat != null) Destroy(mat);
    }
}
