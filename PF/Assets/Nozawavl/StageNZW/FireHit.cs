using UnityEngine;

public class FireHit : MonoBehaviour
{
    [Header("Fireが当たったとき増加するノックバック段階")]
    public int increaseStage = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // player の currentMassStage を増やす（上限あり）
        pc.currentMassStage = Mathf.Clamp(
            pc.currentMassStage + increaseStage,
            0,
            pc.massStages.Length - 1
        );

        // massStages[currentMassStage] に合わせて質量変更
        pc.GetComponent<Rigidbody>().mass = pc.massStages[pc.currentMassStage];

        // 色変更処理
        if (pc.currentMassStage < pc.massColors.Length)
        {
            Renderer r = pc.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                r.material.color = pc.massColors[pc.currentMassStage];
            }
        }

        // 自動回復開始
        pc.isRecovering = true;
        pc.recoveryTimer = pc.recoveryInterval;

        Debug.Log(
            $"Fire hit! PlayerTag:{pc.PlayerTag} → currentMassStage:{pc.currentMassStage}"
        );
    }
}
