using System.Windows.Media;
using Games.Object.RPG;
using Games.Object;

namespace DeadlyOnline.Logic
{
    public abstract class CharaBaseData : CharaBase
    {
        public FightAction SelectedAction { get; set; }
        public CharaBaseData TargetChara { get; set; }

        public abstract ImageSource FightingImage { get; internal set; }

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
        public int SPDGage{ get; set; }

        /// <summary>
        /// キャラのバフを取得する。
        /// </summary>
        //public CharBuffs Buff { get; private set; }
        /// <summary>
        /// キャラのデバフを取得する。
        /// </summary>
        //public CharDebuffs Debuff { get; private set; }

        public bool CanAttack => SPDGage <= Constants.SPDGageMax;

        protected static void CheckAttackable(int spdGage)
        {
            if (spdGage < Constants.SPDGageMax)
            {
                throw new System.InvalidOperationException();
            }
        }

        protected CharaBaseData(string name, int hp) : base(name, hp)
        {
        // Add code here
        }
        protected internal CharaBaseData(string name, int hp, int lv) : base(name, hp, lv)
        {
            // Add code here
        }
    }
}
