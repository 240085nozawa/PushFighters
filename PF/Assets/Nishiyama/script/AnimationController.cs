using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Input Settings")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string punchButton = "Punch";

    [Header("Ult Settings")]
    public string ultAxisName = "P1_Special";

    // ★追加: 必殺技のアニメーション再生時間（秒）
    // アニメーションの長さに合わせてInspectorで調整してください（例: 2.0秒）
    public float ultDuration = 2.0f;

    [Header("Punch Timing")]
    public float windUpTime = 0.5f;
    public float recoveryTime = 1.0f;

    // 攻撃中（パンチ or ULT）かどうかを管理
    private bool isAttacking = false;

    void Update()
    {
        // --- 1. 移動アニメーション ---
        float x = Input.GetAxisRaw(horizontalAxis);
        float z = Input.GetAxisRaw(verticalAxis);
        bool isMoving = new Vector2(x, z).sqrMagnitude > 0;
        animator.SetBool("isDash", isMoving);

        // --- 2. パンチアニメーション ---
        // 攻撃中でなければ入力を受け付ける
        if (!isAttacking && Input.GetButtonDown(punchButton))
        {
            StartCoroutine(AnimatePunchSequence());
        }

        // --- 3. ULTアニメーション ---
        // 攻撃中でなければ入力を受け付ける
        if (!isAttacking)
        {
            // A. キーボード: GetKeyDown (押した瞬間だけ検知)
            bool isSpaceDown = Input.GetKeyDown(KeyCode.Space);

            // B. パッド: トリガーが深く押されているか
            float triggerValue = Input.GetAxis(ultAxisName);
            bool isTriggerPressed = triggerValue > 0.5f;

            // どちらかが入力されたらコルーチン開始
            if (isSpaceDown || isTriggerPressed)
            {
                StartCoroutine(AnimateUltSequence());
            }
        }
    }

    IEnumerator AnimatePunchSequence()
    {
        isAttacking = true;
        animator.SetBool("isPunch", true);

        // タメ + 硬直
        yield return new WaitForSeconds(windUpTime + recoveryTime);

        animator.SetBool("isPunch", false);
        isAttacking = false;
    }

    // ★追加: ULT用コルーチン
    IEnumerator AnimateUltSequence()
    {
        isAttacking = true; // 他の行動をブロック
        animator.SetBool("isUltimet", true); // アニメーションON

        // 必殺技の動作時間ぶん待機
        // これにより、ボタンをすぐ離してもアニメーションは維持されます
        yield return new WaitForSeconds(ultDuration);

        animator.SetBool("isUltimet", false); // アニメーションOFF
        isAttacking = false; // 行動可能に戻す
    }
}