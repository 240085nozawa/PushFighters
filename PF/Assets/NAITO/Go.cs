using UnityEngine;

using DG.Tweening;

using TMPro; // TextMeshPro用

public class FadeTMP : MonoBehaviour  //GOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

{
    [SerializeField]
    private TextMeshProUGUI _target; // フェードさせるTextMeshProUGUIをインスペクターでアサイ
    void Start()
    {
        // 2.8秒後にローカル座標(0,0,0)へ0.2秒かけて移動

        transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetDelay(3.3f);

        // 3.5秒後に0.2秒かけて拡大

        transform.DOScale(new Vector3(5f, 5f, 5f), 0.2f).SetDelay(3.8f);

        // 3.8秒後に1.5秒かけて透明度を0.2に
        if (_target != null)
        {

            _target.DOFade(0f, 0.2f).SetDelay(3.8f);

        }
    }
}
