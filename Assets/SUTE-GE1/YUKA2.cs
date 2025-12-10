using System.Collections;
using UnityEngine;

public class YUKA2 : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        StartCoroutine(Sequence());
    }

    // コルーチンの流れ管理
    private IEnumerator Sequence()
    {
        // 90秒まで通常色
        yield return new WaitForSeconds(90f);

        float flashDuration = 5f;
        float elapsed = 0f;
        bool isRed = false;

        // 5秒間、赤→元の色交互変化
        while (elapsed < flashDuration)
        {
            rend.material.color = isRed ? originalColor : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        // 色を元に戻す
        rend.material.color = originalColor;

        // 移動開始 (95秒時点)
        yield return StartCoroutine(YMoveAndDestroy());
    }

    // オブジェクトのY座標を下げて消す処理
    private IEnumerator YMoveAndDestroy()
    {
        float duration = 10f;
        float elapsed = 0f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, -20f, startPosition.z);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        Destroy(gameObject);
    }

    private void Update() { }
}
