using UnityEngine;

public class SpawnPointInfo : MonoBehaviour
{
    [Header("この場所から生まれたキャラのプレイヤー番号")]
    public int playerNumber = 1; // 1, 2, 3, 4

    [Header("Input Managerの名前をここに登録")]
    public string horizontalAxis = "P1_Horizontal";
    public string verticalAxis = "P1_Vertical";
    public string punchButton = "P1_Punch";
    public string dashButton = "P1_Dash";
    public string ultButton = "P1_Special"; // 必要なら
}