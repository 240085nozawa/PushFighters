using UnityEngine;

public class DualAnimatorSwitcher : MonoBehaviour
{
    [Header("=== Animator をドラッグ ===")]
    [Tooltip("60秒まではこれが再生される（Walk用）")]
    public Animator walkAnimator;

    [Tooltip("60秒後にこれが再生される（Idle用）")]
    public Animator idleAnimator;

    [Header("=== タイミング設定 ===")]
    [Tooltip("何秒後に切り替えるか（デフォルト60秒）")]
    [Range(1f, 300f)]
    public float switchTime = 60f;

    [Header("=== デバッグ表示 ===")]
    [SerializeField] private float currentTimer;
    [SerializeField] private bool hasSwitched;

    // 内部用
    private bool switched;

    void Start()
    {
        InitializeAnimators();
    }

    void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// 初期化：Walkのみ有効、Idleは無効
    /// </summary>
    void InitializeAnimators()
    {
        SetAnimatorActive(walkAnimator, true);
        SetAnimatorActive(idleAnimator, false);

        Debug.Log($"{gameObject.name}: Walkアニメーション開始。{switchTime}秒後にIdleへ切り替え", this);
    }

    /// <summary>
    /// タイマー更新と切り替え判定
    /// </summary>
    void UpdateTimer()
    {
        if (switched) return;

        currentTimer += Time.deltaTime;

        if (currentTimer >= switchTime)
        {
            SwitchToIdle();
        }
    }

    /// <summary>
    /// Idleアニメーションへ切り替え
    /// </summary>
    void SwitchToIdle()
    {
        // Walkを停止
        SetAnimatorActive(walkAnimator, false);

        // Idleを開始
        SetAnimatorActive(idleAnimator, true);

        switched = true;
        hasSwitched = true;

        Debug.Log($"{gameObject.name}: {switchTime}秒経過！ Idleアニメーション開始", this);
    }

    /// <summary>
    /// Animatorの有効/無効切り替え（GameObjectごと制御）
    /// </summary>
    void SetAnimatorActive(Animator animator, bool isActive)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animatorがアサインされていません", this);
            return;
        }

        animator.gameObject.SetActive(isActive);

        if (isActive)
        {
            Debug.Log($"Animator[{animator.name}] 有効化", animator.gameObject);
        }
    }

    // === Inspectorからリセット用 ===
    [ContextMenu("タイマーリセット")]
    void ResetTimer()
    {
        currentTimer = 0f;
        switched = false;
        hasSwitched = false;
        InitializeAnimators();
        Debug.Log("タイマーリセット完了", this);
    }

    [ContextMenu("今すぐIdleに切り替え")]
    void ForceSwitchToIdle()
    {
        currentTimer = switchTime;
        UpdateTimer();
    }

#if UNITY_EDITOR
    // === エディター表示拡張 ===
    private void OnValidate()
    {
        // インスペクターで変更されたら即反映
        if (walkAnimator != null) walkAnimator.gameObject.name = "Animator_Walk";
        if (idleAnimator != null) idleAnimator.gameObject.name = "Animator_Idle";
    }
#endif
}
