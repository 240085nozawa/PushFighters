using UnityEngine;
using System.Collections;

public class BombAttack : MonoBehaviour
{
    [Header("必殺技フラグ")]
    public bool isActive = false;

    [Header("溜め演出Prefab")]
    public GameObject chargeEffectPrefab;
    public float chargeDuration = 2f;

    [Header("爆破演出Prefab")]
    public GameObject explosionEffectPrefab;
    public float explosionDuration = 1.5f;

    [Header("爆破判定Prefab（BombATK付き）")]
    public GameObject explosionATKPrefab;

    private PlayerController pc;

    private bool isRunning = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        // 発動リクエストが来ていて、まだ実行中でない場合だけ実行
        if (isActive && !isRunning)
        {
            isActive = false;
            StartCoroutine(BombAttackRoutine());
        }

        // Spaceキーで発動
        if (Input.GetKeyDown(KeyCode.X))
        {
            Activate();
        }
    }

    private IEnumerator BombAttackRoutine()
    {
        isRunning = true;

        // =========================
        // プレイヤー4移動停止
        // =========================
        //if (pc != null)
        //{
        //    pc.canMove = false;
        //}

        // =========================
        // 溜め演出
        // =========================s
        if (chargeEffectPrefab != null)
        {
            GameObject charge = Instantiate(
                chargeEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(charge, chargeDuration);
        }

        yield return new WaitForSeconds(chargeDuration);

        // =========================
        // 爆破演出
        // =========================
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(explosion, explosionDuration);
        }

        // =========================
        // 爆破判定生成
        // =========================
        if (explosionATKPrefab != null)
        {
            GameObject atk = Instantiate(
                explosionATKPrefab,
                transform.position,
                Quaternion.identity
            );

            BombATK bombATK = atk.GetComponent<BombATK>();
            if (bombATK != null)
            {
                bombATK.owner = this.gameObject; // Player4を渡す
            }

            Destroy(atk, explosionDuration);
        }

        yield return new WaitForSeconds(explosionDuration);

        // =========================
        // 移動再開
        // =========================
        if (pc != null)
        {
            pc.canMove = true;
        }

        isRunning = false;
    }
}
