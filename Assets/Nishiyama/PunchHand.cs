using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PunchHand : MonoBehaviour
{
    public float lifetime = 0.3f;
    public float knockbackForce = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. パンチハンドの所有者 (ゲージが増加するプレイヤー) を取得
        PlayerController owner = GetComponentInParent<PlayerController>();
        PlayerController opponent = other.GetComponent<PlayerController>();

        if (opponent != null&& opponent != owner)
        {
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
}
