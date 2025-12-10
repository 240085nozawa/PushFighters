using UnityEngine;

public class SpecialManager : MonoBehaviour
{
    public static SpecialManager Instance;

    // --- 選択結果 ---
    public int SelectedSkill_P1 = -1;
    public int SelectedSkill_P2 = -1;

    // --- スキル名 ---
    private string[] skillNames = new string[]
    {
        "Beam",
        "TimeStop",
        "SelfDestruct",
        "Counter"
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // プレイヤーのスキルを保存
    public void SelectSkill(int playerNumber, int skillIndex)
    {
        if (playerNumber == 1)
            SelectedSkill_P1 = skillIndex;
        else if (playerNumber == 2)
            SelectedSkill_P2 = skillIndex;

        Debug.Log($"[SpecialManager] Player {playerNumber} selected {skillIndex}");
    }

    // スキル番号を返す
    public int GetSelectedSkill(int playerNumber)
    {
        return (playerNumber == 1) ? SelectedSkill_P1 : SelectedSkill_P2;
    }

    // スキル名を返す
    public string GetSkillName(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skillNames.Length)
            return "None";

        return skillNames[skillIndex];
    }
}
