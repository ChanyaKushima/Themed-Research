using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;

namespace Games.Object.RPG
{
	public class RPGEnemy : RPGCharaBase, IEnemy
	{
		public ImageSource Image;

		protected List<FightAction> _actions = new List<FightAction>();

        #region コンストラクタ・デストラクタ


        public RPGEnemy(string name, int hp, int lv, int exp, ImageSource image) : base(name, hp, lv)
        {
            EXP = exp;
            Image = image;
        }

        public RPGEnemy(string name, int hp, int lv, int exp, Uri imageUri) :
            this(name, hp, lv, exp, new BitmapImage(imageUri))
        {
        }
        
        public RPGEnemy(string name, int hp, int lv, int exp, string imagePath) :
            this(name, hp, lv,exp, new BitmapImage(Logic.SolveUri(imagePath)))
        {
        }

		#endregion

		#region public methods

		public void ActionsAdd(FightAction action) => _actions.Add(action);
		public void SetActions(IEnumerable<FightAction> actions) => _actions = new List<FightAction>(actions);
		public override List<FightAction> GetActions() => new List<FightAction>(_actions);
		public override int Attack() => GetRandValue(max: Level << 1);

		#endregion
	}
}
