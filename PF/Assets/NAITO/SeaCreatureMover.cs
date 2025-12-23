using UnityEngine;

public class SeaCreatureMover : MonoBehaviour
{
    private SeaCreatureSpawner spawner;
    private Vector3 baseForward;   // 基準進行方向（flowAngleYで決まる）
    private int side;              // -1 or +1
    private float speed;

    // Spawner から呼ぶ初期化
    public void InitDirection(SeaCreatureSpawner spawner, Vector3 baseForward, int side, float speed)
    {
        this.spawner = spawner;
        this.baseForward = baseForward.normalized;
        this.side = side;
        this.speed = speed;

        // 実際の進行方向（片側→反対側）
        Vector3 moveDir = (side < 0) ? baseForward : -baseForward;

        // モデルの「前」が +Z なら LookRotation(moveDir)
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up); // 進行方向を向く。[web:44]
    }

    private void Update()
    {
        if (spawner == null) return;

        Vector3 moveDir = (side < 0) ? baseForward : -baseForward;
        transform.position += moveDir * speed * Time.deltaTime;

        // 端を越えたら消す（中心から進行方向成分だけ見る）
        float halfLen = spawner.GetHalfLength();
        Vector3 centerToMe = transform.position - spawner.transform.position;

        // 進行方向成分 = Dot(centerToMe, baseForward)
        float d = Vector3.Dot(centerToMe, baseForward);

        if (Mathf.Abs(d) > halfLen + 1f)
        {
            Destroy(gameObject);
        }
    }
}
