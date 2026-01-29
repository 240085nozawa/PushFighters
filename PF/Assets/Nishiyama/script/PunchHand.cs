using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PunchHand : MonoBehaviour
{
    public float lifetime = 0.3f;
    public float knockbackForce = 10f;

    // ★追加項目: パーティクルのプレハブ
    [Header("Effects")]
    public GameObject kokusenPrefab;

    // ★追加項目: 生成したパーティクルを保持する変数
    private GameObject spawnedKokusen;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // ★追加項目: PunchHand本体が消える時に呼ばれる
    void OnDestroy()
    {
        if (spawnedKokusen != null)
        {
            Destroy(spawnedKokusen);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. パンチハンドの所有者 (ゲージが増加するプレイヤー) を取得
        PlayerController owner = GetComponentInParent<PlayerController>();
        PlayerController opponent = other.GetComponent<PlayerController>();

        if (opponent != null&& opponent != owner)
        {
            // ★追加項目: 黒閃（Kokusen）の生成
            SpawnKokusenEffect();

            owner.IncreaseSpecialGauge(5);
            Debug.Log($"PunchHand: 相手プレイヤー ({other.name}) にヒット。ゲージ増加 (+5)。");

            if (!opponent.canKnockback) return; // ← AllCounters中なら吹っ飛ばさない


            Rigidbody opponentRb = other.GetComponent<Rigidbody>();

         if (opponentRb != null)
         {
             Vector3 knockbackDirection = transform.forward;
            opponentRb.velocity = Vector3.zero;
            opponentRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
         }
            opponent.TakeDamage();

            return;
            //untimorimori
        }

        
        if (other.CompareTag("Player"))
        {
            owner.IncreaseSpecialGauge(1);
            Debug.Log("PunchHand: 一般的な敵にヒット。ゲージ増加 (+5)。");

            // 敵にノックバックを適用
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 knockbackDirection = transform.forward;
                enemyRb.velocity = Vector3.zero;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }
    // ★追加項目: パーティクル生成用のメソッド
    void SpawnKokusenEffect()
    {
        if (kokusenPrefab != null && spawnedKokusen == null) // 二重生成防止
        {
            // パンチの位置と回転で生成
            spawnedKokusen = Instantiate(kokusenPrefab, transform.position, transform.rotation);
        }
    }
}
