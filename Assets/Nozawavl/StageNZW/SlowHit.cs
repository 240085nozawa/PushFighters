using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowHit : MonoBehaviour
{
    [Header("遅くする倍率（0.5 = 半分）")]
    public float slowMultiplier = 0.5f;

    [Header("Slow が続く秒数")]
    public float slowDuration = 3f;

    // プレイヤーごとに Slow 状態を記録（複数 Slow の同時適用を防ぐ）
    private static Dictionary<PlayerController, Coroutine> activeSlows
        = new Dictionary<PlayerController, Coroutine>();

    // プレイヤーごとの元スピード保持
    private static Dictionary<PlayerController, float> originalSpeeds
        = new Dictionary<PlayerController, float>();

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // 元の速度をまだ保持していなければ記録
        if (!originalSpeeds.ContainsKey(pc))
        {
            originalSpeeds[pc] = pc.moveSpeed;
        }

        // すでに Slow 中 → 時間延長
        if (activeSlows.ContainsKey(pc))
        {
            // 一度停止して、再度 Slow 開始
            pc.StopCoroutine(activeSlows[pc]);
            activeSlows[pc] = pc.StartCoroutine(ApplySlow(pc));
            return;
        }

        // Slow 未発動 → 新しく開始
        activeSlows[pc] = pc.StartCoroutine(ApplySlow(pc));
    }

    private IEnumerator ApplySlow(PlayerController pc)
    {
        // 元の速度
        float baseSpeed = originalSpeeds[pc];

        // Slow 適用
        pc.moveSpeed = baseSpeed * slowMultiplier;

        // Slow 持続
        yield return new WaitForSeconds(slowDuration);

        // Slow 解除（nullチェック）
        if (pc != null)
        {
            pc.moveSpeed = baseSpeed;
        }

        // 状態クリア
        activeSlows.Remove(pc);
        originalSpeeds.Remove(pc);
    }
}