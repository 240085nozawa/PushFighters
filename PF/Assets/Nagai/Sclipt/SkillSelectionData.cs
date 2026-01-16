public static class SkillSelectionData
{
    // プレイヤー人数（2 or 4）
    public static int playerCount = 2;

    // =====================
    // キャラID
    // =====================
    public static int p1Character = -1;
    public static int p2Character = -1;
    public static int p3Character = -1;
    public static int p4Character = -1;

    // =====================
    // スキルID
    // =====================
    // 0=ビーム, 1=自爆, 2=カウンター, 3=停止
    public static int p1Skill = -1;
    public static int p2Skill = -1;
    public static int p3Skill = -1;
    public static int p4Skill = -1;

    public static void Clear()
    {
        p1Character = p2Character = p3Character = p4Character = -1;
        p1Skill = p2Skill = p3Skill = p4Skill = -1;
    }
}
