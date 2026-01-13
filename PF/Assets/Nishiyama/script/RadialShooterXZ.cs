using UnityEngine;

public class RadialShooterXZ : MonoBehaviour
{
    [Header("発射する弾のプレハブ")]
    public GameObject ballPrefab;

    [Header("弾の数（360度を何分割するか）")]
    public int bulletCount = 12;

    [Header("弾の速さ")]
    public float speed = 10f;

    void Update()
    {
        // テスト用: スペースキーで発射
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // 弾数分だけループ
        for (int i = 0; i < bulletCount; i++)
        {
            // 1. 角度を計算 (360度 ÷ 個数)
            float angle = i * (360f / bulletCount) * Mathf.Deg2Rad;

            // 2. 進行方向のベクトルを作成 (XZ平面)
            // X = Cos(角度), Z = Sin(角度), Y = 0
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

            // 3. 弾を生成（自分の位置から）
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

            // 4. 速度を与える
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 地面と水平に飛ばすため重力を切る（必要に応じてtrueにしてください）
                rb.useGravity = false;
                rb.velocity = direction * speed;
            }
        }
    }
}
