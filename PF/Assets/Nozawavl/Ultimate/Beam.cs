using UnityEngine;

public class Beam : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// SpecialBeamで生成されたビーム本体の挙動を管理する。
    /// - 発動者(PlayerTag)を記録
    /// - 他プレイヤー(PlayerTagが異なる)に当たるとmassStageを3段階目に変更
    /// - 当たった相手をビームの進行方向に押し出す
    /// [Tooltip("このビームを発射したプレイヤーのタグ番号（1?4）")]
    [Tooltip("このビームを発射したプレイヤーのタグ番号（1?4）")]
    public int ownerTag;

    [Tooltip("押し出す力の大きさ")]
    public float pushForce = 20f;

    [Tooltip("ビームの進行方向（SpecialBeamから渡す）")]
    public Vector3 beamDirection;

    private void OnTriggerEnter(Collider other)
    {
        // --- プレイヤー以外に当たった場合は無視 ---
        if (!other.CompareTag("Player")) return;

        // --- プレイヤー情報を取得 ---
        PlayerController target = other.GetComponent<PlayerController>();
        if (target == null) return;

        // --- 自分自身（発動者）は無視 ---
        if (target.PlayerTag == ownerTag)
        {
            Debug.Log($"[Beam] 発動者 Player{ownerTag} 自身には無効。");
            return;
        }

        // --- Rigidbodyを取得 ---
        Rigidbody targetRb = other.GetComponent<Rigidbody>();
        if (targetRb == null)
        {
            Debug.LogWarning($"[Beam] Player{target.PlayerTag} に Rigidbody がありません。");
            return;
        }

        if (!target.canKnockback) return; // ← AllCounters中なら吹っ飛ばさない

        // --- ① massStageを3段階目に設定 ---
        int lastIndex = target.massStages.Length - 1;
        targetRb.mass = target.massStages[lastIndex];
        target.currentMassStage = lastIndex; // 内部ステージを正しく更新

        // 見た目も変更（massColorsを利用）
        if (target.massColors != null && target.massColors.Length > lastIndex)
        {
            Renderer r = target.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                r.material.color = target.massColors[lastIndex];
            }
        }

        Debug.Log($"[Beam] Player{ownerTag} のビームが Player{target.PlayerTag} に命中！ → massStage 3");

        // --- ② recoveryIntervalを発動（TakeDamage呼び出し） ---
        target.TakeDamage(); // PlayerController内部でリカバリータイマーが作動

        // --- ③ 吹き飛ばし処理（AddForce） ---
        Vector3 knockbackDir = beamDirection.normalized;
        targetRb.velocity = Vector3.zero; // 現在の速度をリセット
        targetRb.AddForce(knockbackDir * pushForce, ForceMode.Impulse);

        Debug.Log($"[Beam] Player{target.PlayerTag} を {knockbackDir} 方向に吹き飛ばしました。");
    }
}
