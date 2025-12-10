// IGimmick.cs
using System.Collections.Generic;

public interface IGimmick
{
    // 色変化時に呼ばれる（タイル番号リストが渡る）
    void Activate(System.Collections.Generic.List<int> tileNumbers);

    // 色が戻ったときに呼ばれる
    void Deactivate();

    // ギミック名（任意。デバッグ用）
    string GetName();
}
