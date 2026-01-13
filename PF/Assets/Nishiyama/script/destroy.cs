using UnityEngine;
using System.Collections;

public class FadeAutoDestroy3D : MonoBehaviour
{
    [Header("全体の表示時間（秒）")]
    public float lifeTime = 3.0f;

    [Header("フェードインにかかる時間")]
    public float fadeInTime = 0.5f;

    [Header("フェードアウトにかかる時間")]
    public float fadeOutTime = 0.5f;

    private Renderer myRenderer;
    private Color originalColor;

    void Awake()
    {
        // 自分のレンダラー（描画コンポーネント）を取得
        myRenderer = GetComponent<Renderer>();

        if (myRenderer == null)
        {
            // もし自分になければ、子オブジェクトから探す（モデルの構造によるため）
            myRenderer = GetComponentInChildren<Renderer>();
        }

        if (myRenderer != null)
        {
            // 元の色を保存しておく
            originalColor = myRenderer.material.color;

            // 最初は透明(Alpha = 0)にする
            Color startColor = originalColor;
            startColor.a = 0f;
            myRenderer.material.color = startColor;
        }
        else
        {
            Debug.LogError("Rendererが見つかりません！MeshRendererがあるオブジェクトにつけてください。");
        }
    }

    void Start()
    {
        if (myRenderer != null)
        {
            StartCoroutine(FadeSequence());
        }
        else
        {
            // レンダラーがないなら即消す（エラー回避）
            Destroy(gameObject);
        }
    }

    IEnumerator FadeSequence()
    {
        // --- 1. フェードイン (0 -> 1) ---
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalColor.a, timer / fadeInTime); // 元の不透明度まで戻す
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(originalColor.a); // 念のためセット

        // --- 2. 待機 ---
        float waitTime = lifeTime - fadeInTime - fadeOutTime;
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // --- 3. フェードアウト (1 -> 0) ---
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeOutTime);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);

        // --- 4. 消滅 ---
        Destroy(gameObject);
    }

    // 透明度を適用するヘルパー関数
    void SetAlpha(float alpha)
    {
        if (myRenderer != null)
        {
            Color c = myRenderer.material.color;
            c.a = alpha;
            myRenderer.material.color = c;
        }
    }
}