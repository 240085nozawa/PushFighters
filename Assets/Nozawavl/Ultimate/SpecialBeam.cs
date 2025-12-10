using UnityEngine;
using System.Collections;

/// <summary>
/// 特殊技：ビーム攻撃（呪術廻戦の虚式・紫のような挙動）
/// ・1秒チャージ後に発射
/// ・発射方向は押した瞬間のプレイヤーの向き
/// ・自分以外のプレイヤーにヒット
/// ・発動中は無敵＆移動不可
/// </summary>
public class SpecialBeam : MonoBehaviour
{
    [Header("=== Beam Settings ===")]

    private Transform shootPoint;        // 自動検出（プレイヤー子オブジェクト）
    private GameObject beamPrefab;       // Resourcesからロード
    private PlayerController playerController;

    [Tooltip("ビームの速度（m/s）")]
    public float beamSpeed = 30f;

    [Tooltip("ビームの寿命（秒）")]
    public float beamLifetime = 2f;

    [Tooltip("Raycastの最大距離（ビームが貫通する範囲）")]
    public float maxRayDistance = 100f;

    [Tooltip("ヒットを検出するレイヤー（例: Enemy, Wallなど）")]
    public LayerMask hitMask;
    private bool isFiring = false;
    private bool isInvincible = false;

    // ★ このプレイヤーのタグ番号を保持（PlayerControllerから取得）
    [HideInInspector]
    public int playerTagNumber;

    private void Start()
    {
        // --- PlayerControllerを取得 ---
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[SpecialBeam] PlayerController が見つかりません。");
            return;
        }

        // --- PlayerTagを取得して記録 ---
        playerTagNumber = playerController.PlayerTag;
        Debug.Log($"[SpecialBeam] PlayerTag = {playerTagNumber}");

        // --- ShootPointを探す ---
        shootPoint = transform.Find("ShootPoint");
        if (shootPoint == null)
        {
            Debug.LogWarning("[SpecialBeam] ShootPoint が見つかりません。プレイヤーの子に 'ShootPoint' を置いてください。");
        }

        // --- BeamPrefabをResourcesからロード ---
        beamPrefab = Resources.Load<GameObject>("BeamPrefab");
        if (beamPrefab == null)
        {
            Debug.LogError("[SpecialBeam] Resources/BeamPrefab が見つかりません。");
        }
    }

    private void Update()

    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFiring)
        {
            StartCoroutine(FireBeamRoutine());
        }
    }

    public void Activate()
    {
        if(!isFiring)
        {
            StartCoroutine(FireBeamRoutine());
        }
    }

    /// <summary>

    /// ビーム発射ルーチン（チャージ→発射）

    /// </summary>

    private IEnumerator FireBeamRoutine()
    {
        isFiring = true;

        // 1秒間は移動不可＆無敵化
        playerController.canMove = false;
        isInvincible = true;

        // ① ビーム生成
        if (beamPrefab == null || shootPoint == null)
        {
            Debug.LogError("[SpecialBeam] BeamPrefab または ShootPoint が未設定です。");
            yield break;
        }

        GameObject beamObj = Instantiate(beamPrefab, shootPoint.position, shootPoint.rotation);

        // --- Beamに情報を渡す ---

        Beam beamScript = beamObj.GetComponent<Beam>();
        if (beamScript != null)
        {
            beamScript.ownerTag = playerTagNumber;       // ← ここでSpecialBeam側のPlayerTagを渡す
            beamScript.beamDirection = shootPoint.forward;
        }

        Debug.Log($"[SpecialBeam] Player{playerTagNumber} がビーム発動！");

        // ② チャージ時間（1秒）
        yield return new WaitForSeconds(1f);

        // ③ ビーム発射
        Rigidbody rb = beamObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = beamObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        rb.velocity = shootPoint.forward * beamSpeed;

        // ④ 一定時間後に削除
        Destroy(beamObj, beamLifetime);

        // ⑤ 無敵解除＆移動再開
        yield return new WaitForSeconds(beamLifetime);

        playerController.canMove = true;

        isInvincible = false;

        isFiring = false;

    }

    /// <summary>

    /// 他スクリプト（パンチなど）から呼び出せる無敵状態確認

    /// </summary>

    public bool IsInvincible()

    {

        return isInvincible;

    }

}
 

    /// <summary>
    /// 必殺技の発動入力をチェック（Xキー または Xboxトリガー）
    /// </summary>
    //private bool CheckSpecialMoveInput()
    //{
    //    // 1. キーボードのXキーチェック
    //    bool isXKeyDown = Input.GetKeyDown(KeyCode.X);

    //    // 2. Xboxコントローラーのトリガーボタンチェック
    //    // Input.GetKeyDown()を使うことで、「押された瞬間」だけを検出します。
    //    bool isTriggerButtonDown = Input.GetKeyDown(TRIGGER_BUTTON_CODE1) || Input.GetKeyDown(TRIGGER_BUTTON_CODE2);

    //    // Xキーの瞬間入力 OR トリガーボタンの瞬間入力
    //    return isXKeyDown || isTriggerButtonDown;
    //}


//private void Update()
//{
//    // Update() は毎フレーム呼ばれる
//    if (Input.GetKeyDown(KeyCode.Space))
//    {
//        Debug.Log("Special Beam 発動！");
//        // 実際の攻撃処理などを書く
//    }
//}

