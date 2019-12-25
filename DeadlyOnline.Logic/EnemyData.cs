using System;
using System.Collections.Generic;
using System.Windows.Media;
using Games.Object.RPG;

namespace DeadlyOnline.Logic
{
    [Serializable]
    public class EnemyData : CharaBaseData
    {
        public Dictionary<string, FightAction> Actions { get; set; }

        public EnemyData(string name, int hp, string fightingImagePath) : base(name, hp)
        {
            _fightingImagePath = fightingImagePath;
        }
        public EnemyData(string name, int hp, int lv, string fightingImagePath) : base(name, hp, lv)
        {
            _fightingImagePath = fightingImagePath;
        }

        private readonly string _fightingImagePath;
        
        [NonSerialized]
        private ImageSource _fightingImage;

        public override ImageSource FightingImage
        {
            get
            {
                if (_fightingImage == null)
                {
                    _fightingImage = GameObjectGenerator.CreateBitmap(_fightingImagePath);
                }
                return _fightingImage;
            }
        }
    }
}
