using UnityEngine;
using System.Collections;

public class Temu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // プレイヤーに触れたらこのアイテムを消す
    [Header("速度低下デバフの設定")]
    [Tooltip("速度を何倍に下げるか（例: 0.5 で半分の速度になる）")]
    public float slowMultiplier = 0.5f;

    [Tooltip("速度低下デバフが続く時間（秒）")]
    public float slowDuration = 2f;

    [Header("スタンデバフの設定")]
    [Tooltip("スタンで動けなくなる時間（秒）")]
    public float stunDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ランダムでデバフを付与
            StartCoroutine(ApplyRandomDebuff(other.gameObject));

            // 見た目を消す（MeshRenderer）
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }

            // 当たり判定も消す（Collider）
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }
    }

    /// <summary>
    /// ランダムでデバフを選択して付与
    /// </summary>
    private IEnumerator ApplyRandomDebuff(GameObject player)
    {
        int random = Random.Range(0, 2); // 0 または 1

        switch (random)
        {
            case 0:
                yield return StartCoroutine(SpeedDownDebuff(player));
                break;
            case 1:
                yield return StartCoroutine(StunDebuff(player));
                break;
        }

        // デバフ処理が終わってからアイテムを完全に破壊
        Destroy(gameObject);
    }

    /// <summary>
    /// デバフ① 移動速度低下
    /// </summary>
    private IEnumerator SpeedDownDebuff(GameObject player)
    {
        Debug.Log($"{player.name} に速度低下デバフ！");

        var controller = player.GetComponent<PlayerController>() ?? player.GetComponentInChildren<PlayerController>();

        if (controller != null)
        {
            float originalSpeed = controller.moveSpeed;

            // 移動速度を低下させる
            controller.moveSpeed *= slowMultiplier;

            yield return new WaitForSeconds(slowDuration);

            // 元に戻す
            controller.moveSpeed = originalSpeed;
            Debug.Log($"{player.name} の速度が元に戻った");
        }
    }

    /// <summary>
    /// デバフ② 行動不能（スタン）
    /// </summary>
    private IEnumerator StunDebuff(GameObject player)
    {
        Debug.Log($"{player.name} がスタン！");

        var controller = player.GetComponent<PlayerController>() ?? player.GetComponentInChildren<PlayerController>();

        if (controller != null)
        {
            float originalSpeed = controller.moveSpeed;

            // 完全に動けなくする
            controller.moveSpeed = 0f;

            // stunDuration 秒間そのまま
            yield return new WaitForSeconds(stunDuration);

            // 元の速度に戻す
            controller.moveSpeed = originalSpeed;
            Debug.Log($"{player.name} が動けるようになった");
        }
    }
}
