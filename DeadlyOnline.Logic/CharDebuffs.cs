using System;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// キャラのデバフ(対象者にとって悪い付加効果)を表すビットフラグ
    /// </summary>
    [Flags]
    public enum CharDebuffs
    {
        None = 0,
        ATKDown = 0x0001,
        DEFDown = 0x0002,

        Poisn = 0x0100,

        Dead = 0x8000,
    }
}
