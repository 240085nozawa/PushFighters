using UnityEngine;

public class VerticalLoopMovement : MonoBehaviour
{
    public float startY;        // 初期Y位置（インスペクターで設定）
    public float amplitude = 0.5f;  // 移動幅（0から-0.5）
    public float speed = 1f;    // 移動速度（小さくするとゆっくり）

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        float newY = startY + Mathf.Sin(Time.time * speed) * -amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
