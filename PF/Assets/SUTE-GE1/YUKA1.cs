using System.Collections;
using UnityEngine;

public class YUKA1 : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    private Coroutine colorCoroutine;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        colorCoroutine = StartCoroutine(FlashColorBeforeMove());
    }

    private IEnumerator FlashColorBeforeMove()
    {
        // 動き出すまでの待機時間（全体で90秒なので、5秒前=85秒まで通常色）
        yield return new WaitForSeconds(85f);

        float flashDuration = 5f;
        float elapsed = 0f;
        bool isRed = false;

        // 赤→元の色と交互に色変更（5秒間）
        while (elapsed < flashDuration)
        {
            rend.material.color = isRed ? originalColor : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.5f); // 0.5秒ごとに色変更
            elapsed += 0.5f;
        }

        // 元の色に戻す
        rend.material.color = originalColor;

        // 落下開始のコルーチンへ
        StartCoroutine(YMoveAndDestroy());
    }

    private IEnumerator YMoveAndDestroy()
    {
        float duration = 10f; // 下げるのにかける時間
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
