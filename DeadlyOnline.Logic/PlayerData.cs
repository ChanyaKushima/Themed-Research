using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Games.Objects;
using Games.Objects.RPG;

namespace DeadlyOnline.Logic
{
    [Serializable]
    public class PlayerData : CharaDataBase
    {
        #region 戦闘に関するデータ

        public bool IsFighting { get; }

        #endregion

        #region マップ上のデータ

        public Coordinate MapCoordinate { get; set; }
        public MapID CurrentMapID { get; set; }

        #endregion

        #region ステータス



        #endregion

        /// <summary>
        /// キャラの最大レベルを取得する。
        /// </summary>
        public override int MaxLevel { get => 999; protected set => base.MaxLevel = value; }
        /// <summary>
        /// キャラの最小レベルを取得する。
        /// </summary>
        public override int MinLevel { get => 0; protected set => base.MinLevel = value; }

        /// <summary>
        /// レベルアップに必要な経験値を取得する。
        /// </summary>
        public int NeedEXP => 0;



        public PlayerData(string name, int maxHP) : base(name, maxHP)
        {
        }

        public override List<FightAction> GetActions() => throw new NotImplementedException();
        public override int Attack() => throw new NotImplementedException();
    }

    public abstract class CharaDataBase : RPGCharaBase
    {
        public FightAction SelectedAction { get; set; }
        public RPGCharaBase TargetChara { get; set; }

        /// <summary>
        /// キャラが生きているかを取得する。
        /// </summary>
        public override bool IsAlive => base.IsAlive;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public int ATK { get; set; }
        /// <summary>
        /// 防御力
        /// </summary>
        public int DEF { get; set; }

        /// <summary>
        /// 俊敏性
        /// </summary>
        public int SPD { get; set; }

        /// <summary>
        /// 100貯まると攻撃できるようになる、行動順を決めるゲージ
        /// </summary>
        public decimal SPDGage{ get; set; }

        /// <summary>
        /// キャラのバフを取得する。
        /// </summary>
        public CharBuffs Buff { get; private set; }
        /// <summary>
        /// キャラのデバフを取得する。
        /// </summary>
        public CharDebuffs Debuff { get; private set; }

        protected CharaDataBase(string name, int hp) : base(name, hp)
        {
        // Add code here
        }
        protected internal CharaDataBase(string name, int hp, int lv) : base(name, hp, lv)
        {
            // Add code here
        }
    }

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
