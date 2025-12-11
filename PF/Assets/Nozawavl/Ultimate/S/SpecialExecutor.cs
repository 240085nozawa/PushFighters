using UnityEngine;

public class SpecialExecutor : MonoBehaviour
{
    private string selectedSkillName;
    private MonoBehaviour activeSpecial;
    private PlayerController playerController;
    private string specialButtonName;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[SpecialExecutor] PlayerController が見つかりません。");
            return;
        }

        // ---- ボタン名設定（P1 or P2）----
        switch (playerController.PlayerTag)
        {
            case 1: specialButtonName = "P1_Special"; break;
            case 2: specialButtonName = "P2_Special"; break;
            case 3: specialButtonName = "P3_Special"; break;
            case 4: specialButtonName = "P4_Special"; break;
            default: specialButtonName = "P1_Special"; break;
        }

        // ---- SpecialManager から選択スキル読込 ----
        int id = (playerController.PlayerTag == 1)
            ? SpecialManager.Instance.SelectedSkill_P1
            : SpecialManager.Instance.SelectedSkill_P2;

        selectedSkillName = SpecialManager.Instance.GetSkillName(id);

        if (string.IsNullOrEmpty(selectedSkillName))
        {
            Debug.LogWarning($"[Player{playerController.PlayerTag}] スキルが選ばれていません！");
            return;
        }

        Debug.Log($"[Player{playerController.PlayerTag}] 必殺スキル：{selectedSkillName}");

        DisableAll();
        EnableSelected();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(selectedSkillName) &&
            Input.GetButtonDown(specialButtonName))
        {
            ExecuteSpecial();
        }
    }

    // ---- すべての必殺技を無効化 ----
    private void DisableAll()
    {
        var beam = GetComponent<SpecialBeam>();
        if (beam) beam.enabled = false;

        var world = GetComponent<TheWorld>();
        if (world) world.enabled = false;

        var bomb = GetComponent<BombAttack>();
        if (bomb) bomb.enabled = false;

        var counter = GetComponent<AllCounters>();
        if (counter) counter.enabled = false;
    }

    // ---- 選んだスキルだけ有効化 ----
    private void EnableSelected()
    {
        switch (selectedSkillName)
        {
            case "SpecialBeam":
                var beam = GetComponent<SpecialBeam>();
                if (beam) { beam.enabled = true; activeSpecial = beam; }
                break;

            case "TheWorld":
                var world = GetComponent<TheWorld>();
                if (world) { world.enabled = true; activeSpecial = world; }
                break;

            case "BombAttack":
                var bomb = GetComponent<BombAttack>();
                if (bomb) { bomb.enabled = true; activeSpecial = bomb; }
                break;

            case "AllCounters":
                var counter = GetComponent<AllCounters>();
                if (counter) { counter.enabled = true; activeSpecial = counter; }
                break;

            default:
                Debug.LogWarning($"未定義スキル: {selectedSkillName}");
                break;
        }
    }

    // ---- Activate() を実行して発動 ----
    public void ExecuteSpecial()
    {
        if (activeSpecial == null)
        {
            Debug.LogWarning($"[Player{playerController.PlayerTag}] 有効なスキルがありません！");
            return;
        }

        var method = activeSpecial.GetType().GetMethod("Activate");
        if (method != null)
        {
            method.Invoke(activeSpecial, null);
            Debug.Log($"[Player{playerController.PlayerTag}] {selectedSkillName} 発動！");
        }
        else
        {
            Debug.LogWarning($"{selectedSkillName} に Activate() がありません！");
        }
    }
}
