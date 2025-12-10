using System.Collections;
using UnityEngine;

public class YUKA4 : MonoBehaviour
{
    private Renderer rend;
    private Color[] originalColors;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Renderer not found.");
            return;
        }

        // 各マテリアルの元の色を保持
        originalColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            originalColors[i] = rend.materials[i].color;
        }

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        yield return new WaitForSeconds(100f);

        float flashDuration = 5f;
        float elapsed = 0f;
        bool isRed = false;

        while (elapsed < flashDuration)
        {
            for (int i = 0; i < rend.materials.Length; i++)
            {
                // マテリアル名でlambert1だけ色変化させる
                if (rend.materials[i].name.StartsWith("lambert1"))
                    rend.materials[i].color = isRed ? originalColors[i] : Color.red;
                else
                    rend.materials[i].color = originalColors[i];
            }
            isRed = !isRed;
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        // 全マテリアルを元の色に戻す
        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].color = originalColors[i];
        }

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
}
