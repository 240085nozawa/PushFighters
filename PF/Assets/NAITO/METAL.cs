using UnityEngine;

public class METAL : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーのスクリプト取得
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                // 0.8倍速度にして10秒間維持
                playerScript.SetSpeedMultiplier(0.5f, 10f);
            }

            // 自分自身を消滅
            Destroy(gameObject);
        }
    }
    // ※このスクリプトはアイテムキューブに付与、ColliderはIs TriggerをON
}
