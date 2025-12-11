using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    // ★追加: どのゲージを表示するか選ぶための設定
    public enum GaugeType
    {
        SpecialGauge,   // 必殺技ゲージ
        PunchCooldown,  // パンチ用
        DashCooldown    // ダッシュ用
    }

    [Header("設定")]
    [Tooltip("このUIで何を表示するか選んでください")]
    public GaugeType gaugeType = GaugeType.SpecialGauge; // デフォルトは必殺技

    [Tooltip("参照するプレイヤーコントローラー")]
    public PlayerController player;

    private Image gaugeImage;
    private const float MAX_GAUGE = 100f; // 必殺技用の最大値

    void Start()
    {
        gaugeImage = GetComponent<Image>();

        if (player == null || gaugeImage == null)
        {
            Debug.LogError("GaugeUIの設定が不完全です。");
            enabled = false;
            return;
        }

        if (gaugeImage.type != Image.Type.Filled)
        {
            Debug.LogWarning("ゲージのImage TypeをInspectorで 'Filled' に設定してください！");
        }
    }

    void Update()
    {
        if (player == null) return;

        float fillAmount = 0f;

        // ★選ばれたタイプによって計算方法を変える
        switch (gaugeType)
        {
            case GaugeType.SpecialGauge:
                // 必殺技ゲージの計算
                fillAmount = (float)player.specialGaugeValue / MAX_GAUGE;
                break;

            case GaugeType.PunchCooldown:
                // PlayerControllerに追加したパンチ用関数を使う
                fillAmount = player.GetPunchCooldownRatio();
                break;

            case GaugeType.DashCooldown:
                // PlayerControllerに追加したダッシュ用関数を使う
                fillAmount = player.GetDashCooldownRatio();
                break;
        }

        // 計算した値を適用
        gaugeImage.fillAmount = fillAmount;
    }
}