using System.Collections;
using UnityEngine;

public class YUKA3 : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        StartCoroutine(Sequence());  // ここをYMoveAndDestroyではなくSequenceにする
    }

    private IEnumerator Sequence()
    {
        // 95秒待機（通常色）
        yield return new WaitForSeconds(95f);

        float flashDuration = 5f;
        float elapsed = 0f;
        bool isRed = false;

        // 5秒間赤⇔元色交互点滅
        while (elapsed < flashDuration)
        {
            rend.material.color = isRed ? originalColor : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        rend.material.color = originalColor;

        // 100秒時点で移動開始
        yield return StartCoroutine(YMoveAndDestroy());
    }

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

    private void Update()
    {
        // 空欄でOK
    }
}
