using UnityEngine;
using System.Collections;

public class BombATK : MonoBehaviour
{
    [Header("発動者（Player4）")]
    public GameObject owner;

    [Header("ノックバック強度")]
    public float knockbackPower = 5f;

    [Header("スタン時間")]
    public float stunDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        // Player以外無視
        if (!other.CompareTag("Player")) return;

        // 発動者無視（rootで判定）
        if (owner != null && other.transform.root.gameObject == owner) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // AllCounter中は無効
        if (!pc.canKnockback) return;

        // すでにスタン中なら無視
        if (pc.isStunned) return;

        // =========================
        // 横方向のみノックバック
        // =========================
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        dir = dir.normalized;

        Vector3 targetPos = pc.transform.position + dir * knockbackPower;
        targetPos.y = pc.transform.position.y;

        pc.StartCoroutine(KnockbackRoutine(pc, targetPos));
        pc.StartCoroutine(StunRoutine(pc));
    }

    private IEnumerator KnockbackRoutine(PlayerController pc, Vector3 targetPos)
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startPos = pc.transform.position;

        while (elapsed < duration)
        {
            Vector3 pos = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            pos.y = startPos.y;
            pc.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        pc.transform.position = targetPos;
    }

    private IEnumerator StunRoutine(PlayerController pc)
    {
        pc.isStunned = true;

        float originalSpeed = pc.moveSpeed;
        bool originalCanMove = pc.canMove;

        pc.moveSpeed = 0f;
        pc.canMove = false;

        yield return new WaitForSeconds(stunDuration);

        pc.moveSpeed = originalSpeed;
        pc.canMove = originalCanMove;
        pc.isStunned = false;
    }
}