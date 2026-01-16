using UnityEngine;

public class KnockbackBall : MonoBehaviour
{
    [Header("ヒット時に弾を消滅させるか")]
    public bool destroyOnHit = true;

    // 何かに触れたときの処理
    void OnTriggerEnter(Collider other)
    {
        // 1. ぶつかった相手から PlayerController を探す
        PlayerController player = other.GetComponent<PlayerController>();

        // もし PlayerController がついていれば（＝相手はプレイヤー）
        if (player != null)
        {
            // 2. 弾き飛ばす方向を計算
            // 「弾の位置」から「プレイヤーの位置」へのベクトル ＝ 突き飛ばす向き
            Vector3 direction = (other.transform.position - transform.position).normalized;

            // 高さは無視して水平に飛ばす
            direction.y = 0;
            direction = direction.normalized;

            // 3. プレイヤー側のノックバック処理を実行！
            // (前回 PlayerController に作った関数を呼び出します)
            player.ApplyKnockback(direction);

            Debug.Log("弾がプレイヤーに命中！ノックバック発生");

            // 4. 当たった弾を消す
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
