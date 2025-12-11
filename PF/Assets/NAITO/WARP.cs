using UnityEngine;

public class EffectVisibilityController : MonoBehaviour
{
    public GameObject effectSet1;  // シーンに置いたセット1のエフェクト
    public GameObject effectSet2;  // シーンに置いたセット2のエフェクト

    private float timer;

    void Start()
    {
        timer = 0f;

        // 開始時はセット1を表示、セット2は非表示
        if (effectSet1 != null) effectSet1.SetActive(true);
        if (effectSet2 != null) effectSet2.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 60秒経過でセット1を非表示にする
        if (timer >= 60f && effectSet1 != null && effectSet1.activeSelf)
        {
            effectSet1.SetActive(false);
        }

        // 62.5秒経過でセット2を表示する
        if (timer >= 62.5f && effectSet2 != null && !effectSet2.activeSelf)
        {
            effectSet2.SetActive(true);
        }
    }
}
