using UnityEngine;
using System.Collections;

public class YUKAMUVE : MonoBehaviour
{
    // 4つの回転候補（0度からのターゲット回転）
    private readonly Quaternion[] rotations = new Quaternion[]
    {
        Quaternion.Euler(3f, 0f, 0f),   // X: +3度
        Quaternion.Euler(-3f, 0f, 0f),  // X: -3度
        Quaternion.Euler(0f, 0f, 3f),   // Z: +3度
        Quaternion.Euler(0f, 0f, -3f),  // Z: -3度
    };

    private int lastIndex = -1; // 前回選ばれたインデックス

    [Tooltip("4種類の回転それぞれに割り当てるエフェクトをドラッグ＆ドロップしてください")]
    public GameObject[] effectPrefabs = new GameObject[4]; // Inspectorで設定できる配列

    private GameObject currentEffectInstance;

    void Start()
    {
        StartCoroutine(RotationLoop());
    }

    IEnumerator RotationLoop()
    {
        // 最初の10秒待つ
        yield return new WaitForSeconds(10f);

        while (true)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, rotations.Length);
            } while (newIndex == lastIndex);

            lastIndex = newIndex;

            Quaternion targetRotation = rotations[newIndex];

            // 選ばれた回転に対応するエフェクトを生成
            SpawnEffect(newIndex);

            // 5秒かけて回転
            yield return RotateOverTime(targetRotation, 5f);

            // 5秒間そのまま待機
            yield return new WaitForSeconds(5f);

            // 5秒かけて初期回転に戻る
            yield return RotateOverTime(Quaternion.identity, 5f);

            // エフェクト削除
            DestroyCurrentEffect();

            // 15秒待ってから次のループ
            yield return new WaitForSeconds(15f);
        }
    }

    // エフェクト生成
    void SpawnEffect(int index)
    {
        DestroyCurrentEffect();

        if (effectPrefabs != null && index >= 0 && index < effectPrefabs.Length && effectPrefabs[index] != null)
        {
            currentEffectInstance = Instantiate(effectPrefabs[index], transform.position, Quaternion.identity, transform);
        }
    }

    // エフェクト削除
    void DestroyCurrentEffect()
    {
        if (currentEffectInstance != null)
        {
            Destroy(currentEffectInstance);
            currentEffectInstance = null;
        }
    }

    // 回転補間 Coroutine
    IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
