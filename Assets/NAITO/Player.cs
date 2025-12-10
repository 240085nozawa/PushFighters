using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float baseSpeed = 5f;   // 基本の移動速度（Inspectorで調整）
    float speedMultiplier = 1f;              // 速度倍率

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        transform.Translate(dir * baseSpeed * speedMultiplier * Time.deltaTime);
    }

    // 外部から効果を与えるためのメソッド
    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        // コルーチンで一定時間後に元に戻す
        StartCoroutine(SpeedEffectCoroutine(multiplier, duration));
    }

    System.Collections.IEnumerator SpeedEffectCoroutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f; // 通常速度へ
    }
}
