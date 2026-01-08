using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("消えるまでの時間（秒）")]
    public float lifeTime = 3.0f;

    void Start()
    {
        // 生成されてから lifeTime 秒後に、自分自身(gameObject)を破壊する
        Destroy(gameObject, lifeTime);
    }
}