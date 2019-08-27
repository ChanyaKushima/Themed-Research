using System;
using System.Collections.Generic;
using System.Windows.Media;
using Games.Object.RPG;

namespace DeadlyOnline.Logic
{
    public class EnemyData : CharaBaseData
    {
        public Dictionary<string, FightAction> Actions { get; set; }

        public EnemyData(string name, int hp, ImageSource fightingImage) : base(name, hp)
        {
            FightingImageSource = fightingImage;
        }
        public EnemyData(string name, int hp, int lv, ImageSource fightingImage) : base(name, hp, lv)
        {
            FightingImageSource = fightingImage;
        }

        public override ImageSource FightingImageSource
        {
            get;
            internal set;
        }

        public override int Attack()
        {
            CheckAttackable(SPDGage);
            SPDGage -= Constants.SPDGageMax;
            return ATK;
        }
    }
}
