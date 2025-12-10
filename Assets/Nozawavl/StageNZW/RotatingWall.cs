using UnityEngine;

public class RotatingWall : MonoBehaviour
{
    [Header("回転にかける時間")]
    public float rotateTime = 3f; // 0→360 までの時間、360→0 の時間

    private float timer = 0f;
    private bool rotatingForward = true; // true = 0→360, false = 360→0

    void Update()
    {
        timer += Time.deltaTime;

        // 進行度（0?1）
        float t = timer / rotateTime;

        if (rotatingForward)
        {
            // 0° → 360°
            float yRot = Mathf.Lerp(0f, 360f, t);
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            // 終了判定
            if (t >= 1f)
            {
                rotatingForward = false;
                timer = 0f;
            }
        }
        else
        {
            // 360° → 0°
            float yRot = Mathf.Lerp(360f, 0f, t);
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            // 終了判定
            if (t >= 1f)
            {
                rotatingForward = true;
                timer = 0f;
            }
        }
    }
}
