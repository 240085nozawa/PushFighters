using UnityEngine;
using System.Collections;

public class Reflection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// - Barrier にアタッチして使用する反射スクリプト。
    /// - PunchHand / Beam / BombATK の攻撃を検知し、攻撃元に同じ効果を返す。

    [Header("反射主情報")]
    public int ownerTag;            // バリアを出したプレイヤー番号（1?4）
    public GameObject ownerObject;  // バリアの親（Player）

    private bool isReflecting = false; // 無限反射防止フラグ

    private readonly string[] reflectableTags = { "PunchHand", "Beam", "BombATK" };

    private void OnTriggerEnter(Collider other)
    {
        if (isReflecting) return;

        foreach (string tag in reflectableTags)
        {
            if (other.CompareTag(tag))
            {
                int attackerTag = 0;

                // Beamか、それ以外かで分ける
                if (tag == "Beam")
                {
                    // BeamならBeamスクリプトを優先
                    Beam beam = other.GetComponentInParent<Beam>();
                    if (beam != null)
                    {
                        attackerTag = beam.ownerTag;
                        Debug.Log($"[Reflection] Beam から attackerTag={attackerTag} を取得");
                    }
                    else
                    {
                        Debug.LogWarning($"[Reflection] Beamスクリプトが見つかりません: {other.name}");
                        return;
                    }
                }
                else
                {
                    // それ以外の攻撃（PunchHand / BombATK）はAttackSource経由
                    AttackSource src = other.GetComponentInParent<AttackSource>();
                    if (src != null)
                    {
                        attackerTag = src.ownerTag;
                        Debug.Log($"[Reflection] AttackSource から attackerTag={attackerTag} を取得");
                    }
                    else
                    {
                        Debug.LogWarning($"[Reflection] 攻撃オブジェクトにAttackSourceがありません: {other.name}");
                        return;
                    }
                }

                if (attackerTag == 0)
                {
                    Debug.LogWarning($"[Reflection] 攻撃オブジェクトに ownerTag が設定されていません: {other.name}");
                    return;
                }

                if (attackerTag == ownerTag) return;

                Debug.Log($"[Reflection] Player{ownerTag} のバリアが {tag} を反射！ → 攻撃元: Player{attackerTag}");

                switch (tag)
                {
                    case "PunchHand":
                        StartCoroutine(ReflectPunchHand(attackerTag));
                        break;
                    case "Beam":
                        StartCoroutine(ReflectBeam(attackerTag));
                        break;
                    case "BombATK":
                        StartCoroutine(ReflectBombATK(attackerTag));
                        break;
                }

                break;
            }
        }
    }

    // ================================
    // 各攻撃タイプ別の反射メソッド
    // ================================

    /// <summary>
    /// PunchHand 反射処理：相手に TakeDamage() を適用
    /// </summary>
    private IEnumerator ReflectPunchHand(int attackerTag)
    {
        isReflecting = true;

        yield return new WaitForSeconds(0.05f);

        // --- 攻撃者を PlayerTag から探す（安全版） ---
        GameObject attacker = null;
        PlayerController targetPC = null;

        foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
        {
            if (pc.PlayerTag == attackerTag)
            {
                attacker = pc.gameObject;
                targetPC = pc;
                break;
            }
        }

        if (attacker == null || targetPC == null)
        {
            Debug.LogWarning($"[Reflection] Player{attackerTag} が見つかりません (PunchHand反射)");
            isReflecting = false;
            yield break;
        }

        // --- ダメージを適用 ---
        targetPC.TakeDamage();
        Debug.Log($"[Reflection] Player{attackerTag} に PunchHand の反射ダメージ！");

        isReflecting = false;
    }

    /// <summary>
    /// Beam 反射処理：相手を吹っ飛ばし + massStageを最終段階に設定
    /// </summary>
    private IEnumerator ReflectBeam(int attackerTag)
    {
        isReflecting = true;

        yield return new WaitForSeconds(0.05f);

        GameObject attacker = null;
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
        {
            if (pc.PlayerTag == attackerTag)
            {
                attacker = pc.gameObject;
                break;
            }
        }

        PlayerController targetPC = attacker.GetComponent<PlayerController>();
        Rigidbody rb = attacker.GetComponent<Rigidbody>();
        if (targetPC != null && rb != null)
        {
            int lastIndex = targetPC.massStages.Length - 1;

            // massStageを最終段階に変更
            rb.mass = targetPC.massStages[lastIndex];
            targetPC.currentMassStage = lastIndex;

            // 見た目更新
            Renderer r = attacker.GetComponentInChildren<Renderer>();
            if (r != null && targetPC.massColors.Length > lastIndex)
                r.material.color = targetPC.massColors[lastIndex];

            // 反射方向（バリア→攻撃者）
            Vector3 dir = (attacker.transform.position - ownerObject.transform.position).normalized;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * 20f, ForceMode.Impulse);

            // TakeDamage() でリカバリー発動
            targetPC.TakeDamage();

            Debug.Log($"[Reflection] Player{attackerTag} に Beam反射！吹っ飛ばし＋massStage3");
        }

        isReflecting = false;
    }

    /// <summary>
    /// BombATK 反射処理：相手をスタン（2秒間動けなくする）
    /// </summary>
    private IEnumerator ReflectBombATK(int attackerTag)
    {
        isReflecting = true;

        yield return new WaitForSeconds(0.05f);

        GameObject attacker = null;
        foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
        {
            if (pc.PlayerTag == attackerTag)
            {
                attacker = pc.gameObject;
                break;
            }
        }

        PlayerController targetPC = attacker.GetComponent<PlayerController>();
        if (targetPC != null && !targetPC.isStunned)
        {
            targetPC.StartCoroutine(ApplyStun(targetPC, 2f));
            Debug.Log($"[Reflection] Player{attackerTag} に BombATK のスタン反射（2秒）！");
        }

        isReflecting = false;
    }

    /// <summary>
    /// スタン処理（共通）
    /// </summary>
    private IEnumerator ApplyStun(PlayerController pc, float duration)
    {
        pc.canMove = false;
        pc.isStunned = true;
        yield return new WaitForSeconds(duration);
        pc.canMove = true;
        pc.isStunned = false;
        Debug.Log($"[Reflection] Player{pc.PlayerTag} のスタン解除");
    }
}
