using UnityEngine;

public class SeaCreatureMover : MonoBehaviour
{
    private SeaCreatureSpawner spawner;
    private int side;          // -1 = 左から右へ, +1 = 右から左へ
    private float speed;       // 移動速度

    public void Init(SeaCreatureSpawner spawner, int side, float speed)
    {
        this.spawner = spawner;
        this.side = side;
        this.speed = speed;

        // 進行方向を向かせる（X軸正方向を右と想定）
        Vector3 dir = (side < 0) ? Vector3.right : Vector3.left;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up); // 3D で進行方向を向く。[web:44][web:47]
    }

    void Update()
    {
        if (spawner == null) return;

        // X方向にまっすぐ移動
        Vector3 move = (side < 0 ? Vector3.right : Vector3.left) * speed * Time.deltaTime;
        transform.position += move;

        // BOXの端まで行ったら削除
        float halfW = spawner.GetHalfWidth();
        Vector3 localPos = transform.position - spawner.transform.position;

        if (Mathf.Abs(localPos.x) > halfW + 1f) // 少し余裕を持たせて判定
        {
            Destroy(gameObject);
        }
    }
}
