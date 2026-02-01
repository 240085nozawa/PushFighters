using UnityEngine;
using System.Collections.Generic; // ★ Dictionary用に必要

public class GameData : MonoBehaviour
{
    public static int Player1CharacterIndex = 0;
    public static int Player2CharacterIndex = 0;
    public static int Player3CharacterIndex = 0;
    public static int Player4CharacterIndex = 0;

    public static int PlayerCount = 2; // デフォルトは2人モード

    // ★ 追加: リザルト画面へスコアを受け渡すための辞書
    public static Dictionary<int, int> FinalScores = new Dictionary<int, int>();

    // ★★★ これが重要！ ★★★
    // 各プレイヤーの画像を保存しておく場所
    public static Dictionary<int, Sprite> PlayerIcons = new Dictionary<int, Sprite>();
}
