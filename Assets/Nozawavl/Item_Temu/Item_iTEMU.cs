using UnityEngine;

public class Item_iTEMU : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("スポーンさせたいTemuプレハブ")]
    public GameObject temuPrefab;

    [Header("スポーン位置のオフセット（例: 右に1）")]
    public Vector3 spawnOffset = new Vector3(1, 0, 0);

    // プレイヤーと接触した時に呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // 接触した相手がプレイヤーかどうかを判定
        if (other.CompareTag("Player"))
        {
            // このアイテムを消す
            Destroy(gameObject);

            // 接触したプレイヤーを記録
            GameObject touchedPlayer = other.gameObject;

            // シーン上の全プレイヤーを取得
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                // 触れた本人にはスポーンしない
                if (player != touchedPlayer)
                {
                    // プレイヤーの近くにTemuをスポーン
                    Vector3 spawnPos = player.transform.position + spawnOffset;
                    Instantiate(temuPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}
