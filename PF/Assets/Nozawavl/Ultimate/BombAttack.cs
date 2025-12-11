using UnityEngine;
using System.Collections;

public class BombAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// 爆破攻撃の処理を行うスクリプト
    /// - 所有者（owner）は影響を受けない
    /// - 爆風範囲内のPlayerを外側にノックバック
    /// - massStageに応じてノックバック強度を調整
    /// - 3秒後に自動削除

    [Header("ResourcesからロードするPrefab名（Resources/BombAttackPrefab など）")]
    public string bombPrefabName = "BombAttack"; // Resources/BombAttack.prefab に置く

    [Header("生成位置（指定がない場合は自分の位置）")]
    public Transform spawnPoint;

    [Header("生成した爆破を削除するまでの時間（秒）")]
    public float destroyDelay = 3f;

    private GameObject spawnedBomb;

    private bool isFiring = false;

    private void Update()
    {
        // 仮で Xキー押下時に発動
        if (Input.GetKeyDown(KeyCode.Space) && !isFiring)
        {
            StartCoroutine(FireBombRoutine());
        }
    }
    public void Activate()
    {
        if (!isFiring)
        {
            StartCoroutine(FireBombRoutine());
        }
    }

    /// 爆破発動処理をコルーチンで管理。
    /// - 発動者を一時的に移動停止
    /// - 爆破生成
    /// - 一定時間後に削除して移動復帰
    private IEnumerator FireBombRoutine()
    {
        isFiring = true;

        // --- ?? プレイヤー移動停止 ---
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.canMove = false;
            Debug.Log($"[BombAttack] {pc.name} は爆破準備中（移動停止）");
        }

        // --- ?? 爆破プレハブをロード＆生成 ---
        GameObject bombPrefab = Resources.Load<GameObject>(bombPrefabName);
        if (bombPrefab == null)
        {
            Debug.LogError($"Resources/{bombPrefabName}.prefab が見つかりません！");
            yield break;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        spawnedBomb = Instantiate(bombPrefab, pos, Quaternion.identity);

        // 発動者情報を渡す（BombATKで除外するため）
        BombATK atk = spawnedBomb.GetComponent<BombATK>();
        if (atk != null)
        {
            atk.owner = this.gameObject;
        }

        Debug.Log($"{gameObject.name} が爆破必殺技を発動！");

        // --- ?? 爆破持続時間中は待機 ---
        yield return new WaitForSeconds(destroyDelay);

        // --- ?? 爆破削除 ---
        if (spawnedBomb != null)
        {
            Destroy(spawnedBomb);
            Debug.Log("[BombAttack] 爆破終了。");
        }

        // --- ? プレイヤー移動再開 ---
        if (pc != null)
        {
            pc.canMove = true;
            Debug.Log($"[BombAttack] {pc.name} の移動を再開。");
        }

        isFiring = false;
    }

}
