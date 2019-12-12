﻿using System.Windows.Media;
using Games.Object.RPG;
using Games.Object;

namespace DeadlyOnline.Logic
{
    public abstract class CharaBaseData : CharaBase
    {
        public static readonly decimal MaxSpdGage = 100;
        public static readonly int MaxSpdCount = 5;

        private decimal _spdGage;
        private int _spdCount;

        public BehaviorInfo SelectedBehavior { get; set; }

        public abstract ImageSource FightingImage { get; internal set; }

        /// <summary>
        /// キャラが生きているかを取得する。
        /// </summary>
        public override bool IsAlive => base.IsAlive;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public int AttackPower { get; set; }
        /// <summary>
        /// 防御力
        /// </summary>
        public int Defence { get; set; }

        /// <summary>
        /// 俊敏性
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 100貯まると攻撃できるようになる、行動順を決めるゲージ
        /// </summary>
        public decimal SpdGage
        {
            get => _spdGage;
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentException();
                }

                if (value >= MaxSpdGage)
                {
                    if (SpdCount >= MaxSpdCount)
                    {
                        value = MaxSpdGage;
                    }
                    else
                    {
                        value -= MaxSpdGage;
                        SpdCount++;
                    }
                }
                _spdGage = value;
            }
        }

        public int SpdCount
        {
            get => _spdCount;
            internal set
            {
                if (value > MaxSpdCount || value < 0)
                {
                    ThrowHelper.ThrowArgumentException();
                }
                _spdCount = value;
            }
        }

        ///// <summary>
        ///// キャラのバフを取得する。
        ///// </summary>
        //public CharBuffs Buff { get; private set; }
        ///// <summary>
        ///// キャラのデバフを取得する。
        ///// </summary>
        //public CharDebuffs Debuff { get; private set; }


        public bool CanAttack
        {
            get
            {
                BehaviorInfo behavior = SelectedBehavior;
                return behavior != null && behavior.GageUsage >= SpdGage;
            }
        }

        protected static void CheckAttackable(CharaBaseData chara)
        {
            if (chara.CanAttack)
            {
                throw new System.InvalidOperationException();
            }
        }

        public void InvokeBehavior(CharaBaseData target)
        {
            var @this = this;
            CheckAttackable(@this);
            BehaviorInfo behavior = @this.SelectedBehavior;
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