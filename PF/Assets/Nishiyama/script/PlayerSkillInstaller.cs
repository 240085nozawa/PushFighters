using UnityEngine;

public class PlayerSkillInstaller : MonoBehaviour
{
    [Header("1P = 1, 2P = 2（インスペクタで設定）")]
    public int playerID = 1;

    //void Start()
    //{
    //    // 選択スキルを取得
    //    int skillIndex = (playerID == 1) ? SelectedSkills.p1Skill : SelectedSkills.p2Skill;

    //    if (skillIndex < 0) return;

    //    // まず全部停止（すでに付いている場合対策）
    //    DisableAllSkills();

    //    // 選択スキルのみ有効
    //    AddSkill(skillIndex);
    //}

    void DisableAllSkills()
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

    void AddSkill(int index)
    {
        switch (index)
        {
            case 0:
                gameObject.AddComponent<SpecialBeam>();
                break;
            case 1:
                gameObject.AddComponent<TheWorld>();
                break;
            case 2:
                gameObject.AddComponent<BombAttack>();
                break;
            case 3:
                gameObject.AddComponent<AllCounters>();
                break;
        }
    }
}