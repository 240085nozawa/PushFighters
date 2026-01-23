using UnityEngine;
using System.Collections.Generic;

public class CharacterSpawner : MonoBehaviour
{
    public static Dictionary<int, int> SpawnBoxMap = new Dictionary<int, int>();
    // key = PlayerController の InstanceID
    // value = 箱番号

    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        SpawnCharacters();
    }

    void SpawnCharacters()
    {
        int playerCount = SkillSelectionData.playerCount;

        for (int boxIndex = 0; boxIndex < playerCount; boxIndex++)
        {
            int characterId = GetCharacterId(boxIndex);

            GameObject player = Instantiate(
                characterPrefabs[characterId],
                spawnPoints[boxIndex].position,
                spawnPoints[boxIndex].rotation
            );

            PlayerController pc = player.GetComponent<PlayerController>();

            if (pc != null)
            {
                int boxNumber = boxIndex + 1;

                // ★★★ これが決定打 ★★★
                pc.ApplySpawnBox(boxNumber);

                Debug.Log(
                    $"[Spawner] キャラID={characterId} → 箱={boxNumber}"
                );
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
            _ => -1
        };
    }
}
