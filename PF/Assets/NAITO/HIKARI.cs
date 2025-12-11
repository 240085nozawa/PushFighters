using UnityEngine;

public class RandomMoverWithinStage : MonoBehaviour
{
    public GameObject stage;  // ドラッグでステージオブジェクトをアタッチ
    private Vector3 stageMin; // ステージの境界 最小位置
    private Vector3 stageMax; // ステージの境界 最大位置

    private Vector3 moveDirection;  // 現在の移動方向（常に1方向に動く）
    private float speed = 6f;        // 移動速度（初期は6）

    private Light attachedLight;     // ライトコンポーネント

    private float elapsedTime = 0f;      // ゲーム開始からの経過時間
    private bool isSpeedBoosted = false; // 速度アップ済みかどうかのフラグ

    private float lightOffTimer = 0f;    // ライト消灯中の経過時間
    private bool isLightOff = false;     // ライトが消えているかどうか

    void Start()
    {
        if (stage == null)
        {
            Debug.LogError("ステージオブジェクトをアタッチしてください");
            enabled = false;
            return;
        }

        Collider stageCollider = stage.GetComponent<Collider>();
        if (stageCollider == null)
        {
            Debug.LogError("ステージにはColliderが必要です");
            enabled = false;
            return;
        }

        stageMin = stageCollider.bounds.min;
        stageMax = stageCollider.bounds.max;

        // スポットライトの初期位置をステージ内のランダムな位置に設定
        float randomX = Random.Range(stageMin.x, stageMax.x);
        float randomZ = Random.Range(stageMin.z, stageMax.z);
        transform.position = new Vector3(randomX, transform.position.y, randomZ);

        // 最初の移動方向はランダムな単位ベクトル
        SetRandomDirection();

        attachedLight = GetComponent<Light>();
        if (attachedLight == null)
        {
            Debug.LogWarning("ライトコンポーネントが見つかりません");
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 60秒経過後、まだ速度アップしていなければライト消灯開始
        if (elapsedTime >= 60f && !isSpeedBoosted)
        {
            if (!isLightOff)
            {
                // ライトを消す
                if (attachedLight != null)
                {
                    attachedLight.enabled = false;
                }
                isLightOff = true;
                lightOffTimer = 0f;
            }

            if (isLightOff)
            {
                lightOffTimer += Time.deltaTime;

                // 2.5秒間ライト消灯を維持
                if (lightOffTimer >= 2.5f)
                {
                    // ライト再点灯、速度を倍に変更
                    if (attachedLight != null)
                    {
                        attachedLight.enabled = true;
                    }
                    speed *= 2f;
                    isSpeedBoosted = true;
                    isLightOff = false;
                }
                else
                {
                    // ライト消灯中は動かない
                    return;
                }
            }
        }

        // ライト消灯中でなければ通常移動処理
        Vector3 nextPos = transform.position + moveDirection * speed * Time.deltaTime;

        // ステージの端に到達したら新しい方向に変更（ランダムな方向転換）
        if (nextPos.x < stageMin.x || nextPos.x > stageMax.x || nextPos.z < stageMin.z || nextPos.z > stageMax.z)
        {
            SetRandomDirection();
            nextPos = transform.position + moveDirection * speed * Time.deltaTime;
            nextPos.x = Mathf.Clamp(nextPos.x, stageMin.x, stageMax.x);
            nextPos.z = Mathf.Clamp(nextPos.z, stageMin.z, stageMax.z);
        }

        transform.position = nextPos;
    }

    void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));
        moveDirection.Normalize();
    }
}
