using UnityEngine;
using DG.Tweening;	//DOTweenを使うときはこのusingを入れる
public class Ready : MonoBehaviour　//READYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ローカル座標の(0,0,0)へ0.5秒で移動する
        this.transform.DOLocalMove(new Vector3(50f, 0f, 0f), 0.2f).SetDelay(0.3f);
        this.transform.DOLocalMove(new Vector3(-1353f, 0f, 0f), 0.2f).SetDelay(1f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}