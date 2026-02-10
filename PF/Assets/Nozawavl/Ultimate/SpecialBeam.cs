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
    [Tooltip("ビームの速度（m/s）")]
    public float beamSpeed = 30f;

    [Tooltip("ビームの寿命（秒）")]
    public float beamLifetime = 2f;

    [Tooltip("Raycastの最大距離（ビームが貫通する範囲）")]
    public float maxRayDistance = 100f;

    [Tooltip("ヒットを検出するレイヤー（例: Enemy, Wallなど）")]
    public LayerMask hitMask;

    [Header("=== 必殺技ボイス ===")]
    public AudioClip beamVoiceClip;
    [Range(0.5f, 3.0f)]
    public float voiceVolume = 2.0f;

    private Transform shootPoint;
    private GameObject beamPrefab;
    private PlayerController playerController;
    private AudioSource audioSource;
    private bool isFiring = false;
    private bool isInvincible = false;

    [HideInInspector]
    public int playerTagNumber;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[SpecialBeam] PlayerController が見つかりません。");
            return;
        }

        playerTagNumber = playerController.PlayerTag;
        Debug.Log($"[SpecialBeam] PlayerTag = {playerTagNumber}");

        shootPoint = transform.Find("ShootPoint");
        if (shootPoint == null)
        {
            Debug.LogWarning("[SpecialBeam] ShootPoint が見つかりません。");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;

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
        if (!isFiring)
        {
            StartCoroutine(FireBeamRoutine());
        }
    }

    private IEnumerator FireBeamRoutine()
    {
        isFiring = true;

        // 必殺発動ボイス即再生！
        if (beamVoiceClip != null)
        {
            audioSource.volume = 1f;  // 音量リセット
            audioSource.PlayOneShot(beamVoiceClip, voiceVolume);
            Debug.Log($"[SpecialBeam] Player{playerTagNumber} ボイス再生！");
        }

        // 移動不可＆無敵化
        playerController.canMove = false;
        isInvincible = true;

        // ビーム生成
        if (beamPrefab == null || shootPoint == null)
        {
            Debug.LogError("[SpecialBeam] BeamPrefab または ShootPoint が未設定です。");
            yield break;
        }

        GameObject beamObj = Instantiate(beamPrefab, shootPoint.position, shootPoint.rotation);
        Beam beamScript = beamObj.GetComponent<Beam>();
        if (beamScript != null)
        {
            beamScript.ownerTag = playerTagNumber;
            // ★吹っ飛ばし方向を渡す！
            beamScript.beamDirection = shootPoint.forward;
        }

        Debug.Log($"[SpecialBeam] Player{playerTagNumber} がビーム発動！");

        // チャージ時間（1秒）
        yield return new WaitForSeconds(1f);

        // ビーム発射
        Rigidbody rb = beamObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = beamObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        rb.velocity = shootPoint.forward * beamSpeed;

        // 一定時間後に削除
        Destroy(beamObj, beamLifetime);

        // 無敵解除＆移動再開
        yield return new WaitForSeconds(beamLifetime);
        playerController.canMove = true;
        isInvincible = false;
        isFiring = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
