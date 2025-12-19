using UnityEngine;

public class VerticalLoopMovement : MonoBehaviour
{
    [Header("上下移動設定")]
    public float startY;
    public float amplitude = 0.5f;
    public float speed = 1f;

    private Rigidbody rb;
    private Vector3 initialPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbodyが必要です！");
            enabled = false;
            return;
        }

        initialPos = transform.position;
        startY = initialPos.y;

        // 物理用設定
        rb.isKinematic = true; // 物理干渉なしでスムーズ移動
        rb.interpolation = RigidbodyInterpolation.Interpolate; // 滑らかさUP
    }

    void FixedUpdate() // Update → FixedUpdate に変更
    {
        float newY = startY + Mathf.Sin(Time.time * speed) * -amplitude;
        Vector3 targetPos = new Vector3(initialPos.x, newY, initialPos.z);

        // transform.position ではなく Rigidbody で移動
        rb.MovePosition(targetPos);
    }
}
