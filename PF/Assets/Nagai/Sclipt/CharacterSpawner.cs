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

            GameObject player = Instantiate(
                characterPrefabs[characterId],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                int playerNumber = i + 1;

                // ★ Player番号
                controller.PlayerTag = playerNumber;

                // ★ Input名を全部作り直す（ここが超重要）
                controller.horizontalAxis = $"P{playerNumber}_Horizontal";
                controller.verticalAxis = $"P{playerNumber}_Vertical";
                controller.punchButton = $"P{playerNumber}_Punch";
                controller.dashButton = $"P{playerNumber}_Dash";

                Debug.Log(
                    $"🎮 P{playerNumber} 設定完了\n" +
                    $"{controller.horizontalAxis}, {controller.punchButton}"
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
