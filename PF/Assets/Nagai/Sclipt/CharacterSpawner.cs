using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("設定")]
    public GameObject[] characterPrefabs;
    public Transform[] spawnPoints; // SpawnPointInfoがついた場所

    void Start()
    {
        // 安全装置
        if (SkillSelectionData.playerCount == 0) SkillSelectionData.playerCount = 4;
        SpawnCharacters();
    }

    void SpawnCharacters()
    {
        int count = SkillSelectionData.playerCount;

        for (int i = 0; i < count; i++)
        {
            int charId = GetCharacterId(i);
            if (charId >= characterPrefabs.Length) charId = 0;

            // ★以前のコードにあった pc.xxxxx = ... は全て削除！
            // ただ生成するだけ。あとはプレイヤーが勝手に設定を読み込みます。
            if (spawnPoints[i] != null)
            {
                Instantiate(characterPrefabs[charId], spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
    }

    int GetCharacterId(int index)
    {
        return index switch
        {
            0 => SkillSelectionData.p1Character,
            1 => SkillSelectionData.p2Character,
            2 => SkillSelectionData.p3Character,
            3 => SkillSelectionData.p4Character,
            _ => 0
        };
    }
}