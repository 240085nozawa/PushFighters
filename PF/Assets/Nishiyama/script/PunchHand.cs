using UnityEngine;

public class PunchHand : MonoBehaviour
{
    public float lifetime = 0.3f;
    public float knockbackForce = 10f;

    [Header("Effects")]
    public GameObject kokusenPrefab; // インスペクターで黒閃のプレハブを割り当て

    private GameObject spawnedKokusen; // 生成したパーティクルを保持

    void Start()
    {
        // 指定時間後にパンチオブジェクト自体を削除
        Destroy(gameObject, lifetime);
    }

    void OnDestroy()
    {
        // パンチ本体が消える際、エフェクトが残っていれば削除
        if (spawnedKokusen != null)
        {
            Destroy(spawnedKokusen);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // パンチハンドの所有者 (ゲージが増加するプレイヤー) を取得
        PlayerController owner = GetComponentInParent<PlayerController>();
        PlayerController opponent = other.GetComponent<PlayerController>();

        // 相手がプレイヤーであり、自分自身ではない場合
        if (opponent != null && opponent != owner)
        {
            // 黒閃（Kokusen）エフェクトの生成
            SpawnKokusenEffect();

            owner.IncreaseSpecialGauge(5);
            Debug.Log($"PunchHand: 相手プレイヤー ({other.name}) にヒット。ゲージ増加 (+5)。");

            // カウンター中などでノックバック無効な場合は処理を抜ける
            if (!opponent.canKnockback) return;

            Rigidbody opponentRb = other.GetComponent<Rigidbody>();
            if (opponentRb != null)
            {
                Vector3 knockbackDirection = transform.forward;
                opponentRb.velocity = Vector3.zero;
                opponentRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
            opponent.TakeDamage();
            return;
        }

        // 一般的な敵（Playerタグを持つもの）への処理
        if (other.CompareTag("Player"))
        {
            owner.IncreaseSpecialGauge(1);
            Debug.Log("PunchHand: 一般的な敵にヒット。ゲージ増加 (+1)。");

            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 knockbackDirection = transform.forward;
                enemyRb.velocity = Vector3.zero;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    // パーティクル生成用のメソッド
    void SpawnKokusenEffect()
    {
        if (kokusenPrefab != null && spawnedKokusen == null) // 二重生成防止
        {
            // パンチの位置と回転でエフェクトを生成
            spawnedKokusen = Instantiate(kokusenPrefab, transform.position, transform.rotation);
            // 親子関係にしたい場合は以下を有効化（パンチに追従させたい場合）
            // spawnedKokusen.transform.SetParent(this.transform);
        }
    }
}