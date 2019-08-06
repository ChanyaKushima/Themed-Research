using System;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// キャラのバフ(対象者にとって良い付加効果)を表すビットフラグ
    /// </summary>
    [Flags]
    public enum CharBuffs
    {
        None = 0,
        ATKUp = 1,
        DEFUp = 2,
    }
}
