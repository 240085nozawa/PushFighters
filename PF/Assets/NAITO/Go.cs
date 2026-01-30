using UnityEngine;
using DG.Tweening;
using TMPro;             // TextMeshPro 用
using UnityEngine.UI;    // Image 用

public class FadeTMP : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _targetText;  // フェードさせる TextMeshProUGUI

    [SerializeField]
    private Image _targetImage;           // フェードさせる Image（画像）

    void Start()
    {
        // 1.3秒後にローカル座標(0,0,0)へ0.2秒かけて移動
        transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f)
                 .SetDelay(1.4f);

        // 1.6秒後に0.2秒かけて拡大
        transform.DOScale(new Vector3(5f, 5f, 5f), 0.2f)
                 .SetDelay(1.8f);

        // 1.6秒後に0.2秒かけてテキストをフェードアウト
        if (_targetText != null)
        {
            _targetText.DOFade(0f, 0.2f)
                       .SetDelay(1.8f);
        }

        // 1.6秒後に0.2秒かけて画像をフェードアウト
        if (_targetImage != null)
        {
            _targetImage.DOFade(0f, 0.2f)
                        .SetDelay(1.8f);
        }
    }
}
