using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    [Header("キャラクタープレハブ（選択画面と同じ並び順にすること）")]
    public GameObject[] characterPrefabs;

    [Header("生成位置")]
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    public Transform player3SpawnPoint;
    public Transform player4SpawnPoint;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // P1〜P4のインデックスを格納する配列
        int[] selectedIndices = new int[4];
        selectedIndices[0] = GameData.Player1CharacterIndex;
        selectedIndices[1] = GameData.Player2CharacterIndex;
        selectedIndices[2] = GameData.Player3CharacterIndex;
        selectedIndices[3] = GameData.Player4CharacterIndex;

        // P1, P2, P3, P4 の生成位置 (Inspectorで設定が必要)
        Transform[] spawnPoints = { player1SpawnPoint, player2SpawnPoint, player3SpawnPoint, player4SpawnPoint };
        // ↑ この配列はInspectorで設定できるように変数定義を別途追加してください

        for (int i = 0; i < GameData.PlayerCount; i++)
        {
            int playerIndex = i + 1; // 1, 2, 3, 4
            int charIndex = selectedIndices[i];

            Transform spawn = (i < spawnPoints.Length) ? spawnPoints[i] : spawnPoints[0];

            // プレイヤー生成
            GameObject player = Instantiate(characterPrefabs[charIndex], spawn.position, spawn.rotation);

            // PlayerControllerの設定
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.PlayerTag = playerIndex; // PlayerTagを1, 2, 3, 4に設定

                // 4人対戦の場合、P3とP4の入力軸も設定が必要
                if (playerIndex == 3) pc.horizontalAxis = "P3_Horizontal"; // Input ManagerにP3軸が必要
                if (playerIndex == 4) pc.horizontalAxis = "P4_Horizontal"; // Input ManagerにP4軸が必要

                // ... (verticalAxisも同様にP3, P4の設定が必要) ...
            }

            Debug.Log($"Player {playerIndex} ({player.name}) を生成しました。");
        }
    }
}