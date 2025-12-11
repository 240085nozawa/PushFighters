using UnityEngine;

/// <summary>
/// 攻撃オブジェクトの発動者情報を記録する共通スクリプト。
/// PunchHand, Beam, BombATK などにアタッチして使用。
/// </summary>
public class AttackSource : MonoBehaviour
{
    [Tooltip("この攻撃を放ったプレイヤーの番号（1?4）")]
    public int ownerTag;
}