using UnityEngine;

/// <summary>
/// 全ての必殺技の共通基底クラス（抽象）
/// - 必ず Activate(GameObject player) を実装すること
/// 保存先: Assets/Scripts/Specials/SpecialBase.cs
/// </summary>

public abstract class SpecialBase : MonoBehaviour

{

    /// <summary>
    /// /// 必殺技を発動するための共通メソッド
    /// player: 発動者（通常はプレイヤーの GameObject）
    /// </summary>

    public abstract void Activate(GameObject player);

}

