using UnityEngine;

public class PunchHand : MonoBehaviour
{
    public float lifetime = 0.3f;
    public float knockbackForce = 10f;

    //[Header("Effects")]
    //public GameObject kokusenPrefab; // インスペクターで黒閃のプレハブを割り当て

    //private GameObject spawnedKokusen; // 生成したパーティクルを保持
    //// ★追加項目: エフェクトを発生させる場所（Playerの子オブジェクトなどをアサイン）
    //public Transform kokusenSpawner;

    [Header("Effects")]
    public GameObject kokusenPrefab;

    // ★内部で保持する変数
    private Transform kokusenSpawner;
    private GameObject spawnedKokusen;

    void Start()
    {
        // ★修正：親（Player）の子供の中から "KokusenSP" という名前のオブジェクトを探す
        // GetComponentInParentで親を取得してから探すので、シーン全体のFindより高速で安全です
        Transform parentTransform = GetComponentInParent<PlayerController>()?.transform;
        if (parentTransform != null)
        {
            kokusenSpawner = parentTransform.Find("KokusenSP");
        }

        
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
        if (kokusenPrefab != null && spawnedKokusen == null)
        {
            // kokusenSpawnerが見つかっていて、まだエフェクトが生成されていない場合
            if (kokusenPrefab != null && spawnedKokusen == null && kokusenSpawner != null)
            {
                // KokusenSPの位置と回転で生成
                spawnedKokusen = Instantiate(kokusenPrefab, kokusenSpawner.position, kokusenSpawner.rotation);

                // スポナーの子供にして追従させる（パンチが消えてもエフェクトは維持したい場合はここをコメントアウト）
                spawnedKokusen.transform.SetParent(kokusenSpawner);
            }
        }
    }
}