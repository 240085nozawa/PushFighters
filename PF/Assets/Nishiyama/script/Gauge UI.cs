using UnityEngine;
using UnityEngine.UI;
using System.Linq; // 検索用

public class GaugeUI : MonoBehaviour
{
    public enum GaugeType
    {
        SpecialGauge,
        PunchCooldown,
        DashCooldown
    }

    [Header("自動取得設定")]
    [Tooltip("このUIはプレイヤー何番の情報を表示しますか？")]
    public int targetPlayerNumber = 1; // 1, 2, 3, 4

    [Header("表示設定")]
    public GaugeType gaugeType = GaugeType.SpecialGauge;

    // publicですがInspectorで設定する必要はありません（自動で入ります）
    public PlayerController player;

    private Image gaugeImage;
    private const float MAX_GAUGE = 100f;

    void Start()
    {
        gaugeImage = GetComponent<Image>();
        if (gaugeImage != null && gaugeImage.type != Image.Type.Filled)
        {
            Debug.LogWarning($"{gameObject.name}: Image Typeを 'Filled' にしてください！");
        }
    }

    void Update()
    {
        // ★プレイヤーがまだ見つかっていない場合、シーン内から探す
        if (player == null)
        {
            FindTargetPlayer();
            return; // 見つかるまでは更新しない
        }

        // ゲージの計算処理
        float fillAmount = 0f;
        switch (gaugeType)
        {
            case GaugeType.SpecialGauge:
                fillAmount = (float)player.specialGaugeValue / MAX_GAUGE;
                break;
            case GaugeType.PunchCooldown:
                fillAmount = player.GetPunchCooldownRatio();
                break;
            case GaugeType.DashCooldown:
                fillAmount = player.GetDashCooldownRatio();
                break;
        }

        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = fillAmount;
        }
    }

    // 指定された番号(PlayerTag)を持つプレイヤーを探す関数
    void FindTargetPlayer()
    {
        // シーン内の全プレイヤーを取得
        var allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var p in allPlayers)
        {
            if (p.PlayerTag == targetPlayerNumber)
            {
                player = p;
                // Debug.Log($"[GaugeUI] P{targetPlayerNumber} を発見しリンクしました");
                break;
            }
        }
    }
}