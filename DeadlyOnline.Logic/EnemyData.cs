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
            FightingImage = fightingImage;
        }
        public EnemyData(string name, int hp, int lv, ImageSource fightingImage) : base(name, hp, lv)
        {
            FightingImage = fightingImage;
        }

        public override ImageSource FightingImage
        {
            get;
            internal set;
        }
    }
}
