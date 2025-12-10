using UnityEngine;
using System.Collections;

public class BombATK : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// - 中にいる間は軽く押し出される（OnTriggerStay）
    /// - 出た瞬間にノックバックレベルで強く吹き飛ばされる（OnTriggerExit）
    /// - スタンは一度だけ発生
    /// 
    [Header("攻撃発動者（影響を受けない対象）")]
    public GameObject owner;

    [Header("ノックバックの基本強度")]
    public float baseKnockbackPower = 5f;

    [Header("スタン継続時間（秒）")]
    public float stunDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        // 自分は無視
        if (other.gameObject == owner) return;

        // Player 以外は無視
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // --- ?? AllCounters発動中なら無効化 ---
        if (!pc.canKnockback)
        {
            Debug.Log($"[BombATK] Player{pc.PlayerTag} は AllCounters中のため、爆風効果を無効化。");
            return;
        }

        // すでにスタン中なら何もしない
        if (pc.isStunned) return;

        // --- 外方向ベクトル ---
        Vector3 dir = (other.transform.position - transform.position).normalized;

        // --- massStagesからノックバック強度を決定 ---
        float knockbackPower = baseKnockbackPower;
        if (pc.massStages != null && pc.massStages.Length > 0)
        {
            float mass = pc.massStages[pc.currentMassStage];
            knockbackPower = Mathf.Max(baseKnockbackPower / Mathf.Max(mass, 0.1f), 1f);
        }

        // --- transformベースで吹っ飛ばす（Translate移動対応） ---
        Vector3 targetPos = pc.transform.position + dir * knockbackPower;
        pc.StartCoroutine(KnockbackRoutine(pc, targetPos));

        // --- スタン開始 ---
        pc.StartCoroutine(StunRoutine(pc));

        Debug.Log($"{pc.name} が爆風に当たってノックバック＋スタン！");
    }

    private IEnumerator KnockbackRoutine(PlayerController pc, Vector3 targetPos)
    {
        // 0.15秒かけて滑らかに押し出す
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startPos = pc.transform.position;

        while (elapsed < duration)
        {
            pc.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pc.transform.position = targetPos;
    }

    private IEnumerator StunRoutine(PlayerController pc)
    {
        pc.isStunned = true;

        // 元の状態を記録
        float originalSpeed = pc.moveSpeed;
        bool originalCanMove = pc.canMove;

        // スタン効果
        pc.moveSpeed = 0f;
        pc.canMove = false;
        Debug.Log($"{pc.name} はスタン中...");

        yield return new WaitForSeconds(stunDuration);

        // スタン解除
        pc.moveSpeed = originalSpeed;
        pc.canMove = originalCanMove;
        pc.isStunned = false;

        Debug.Log($"{pc.name} はスタン解除！");
    }
}
