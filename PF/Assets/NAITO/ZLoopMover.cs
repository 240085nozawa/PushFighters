using UnityEngine;

public class ZLoopMover : MonoBehaviour
{
    [SerializeField] float startZ = 12f;   // 始点Z
    [SerializeField] float endZ = -10f;  // 終点Z
    [SerializeField] float duration = 7f;  // かける時間（秒）

    void Start()
    {
        // Zをスタート位置にそろえる
        var p = transform.position;
        p.z = startZ;
        transform.position = p;

        StartCoroutine(MoveLoop());
    }

    System.Collections.IEnumerator MoveLoop()
    {
        while (true)
        {
            // 12 → -10 を duration秒かけて移動
            float t = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(startPos.x, startPos.y, endZ);

            while (t < duration)
            {
                t += Time.deltaTime;
                float rate = Mathf.Clamp01(t / duration);
                transform.position = Vector3.Lerp(startPos, endPos, rate); // 線形補間で移動[web:9]
                yield return null; // 1フレーム待つ[web:7]
            }

            // -10に到達したら一瞬でZ=12に戻す
            Vector3 resetPos = transform.position;
            resetPos.z = startZ;
            transform.position = resetPos;

            // すぐ次のループでまた 12 → -10 を7秒かけて移動
        }
    }
}
