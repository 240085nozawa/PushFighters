using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("キャラPrefab（ID順）")]
    [SerializeField] private GameObject[] characterPrefabs;

    [Header("スポーン位置（1P〜4P）")]
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        SpawnCharacters();
    }

    void SpawnCharacters()
    {
        int playerCount = SkillSelectionData.playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            int characterId = GetCharacterId(i);

            if (characterId < 0 || characterId >= characterPrefabs.Length)
            {
                Debug.LogError($"❌ P{i + 1} キャラID不正: {characterId}");
                continue;
            }

            if (spawnPoints[i] == null)
            {
                Debug.LogError($"❌ P{i + 1} SpawnPoint が未設定");
                continue;
            }

            Instantiate(
                characterPrefabs[characterId],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            Debug.Log($"✅ P{i + 1} に キャラID {characterId} をスポーン");
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
            _ => -1
        };
    }
}
