using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Object.RPG
{
	/// <summary>
	/// RPGゲームで登場するプレイヤー。
	/// </summary>
	public class RPGPlayer : RPGCharaBase, IPlayer
	{
		protected List<FightCommand> _fightCommands = new List<FightCommand>();
		public List<FightCommand> FightCommands => _fightCommands;

		public virtual int NeedEXP => Level * Level + 10;
		protected bool CanLevelUp => NeedEXP <= EXP && MaxLevel > Level;

		#region コンストラクタ 

		public RPGPlayer(string name, int hp) : base(name, hp)
		{
		}
		private RPGPlayer(string name, int hp, int lv) : base(name, hp, lv)
		{
		}

		#endregion

		#region メソッド

		#region パブリックメソッド

		public override List<FightAction> GetActions() => FightCommands.Select(fc => fc.Action).ToList();
		public IEnumerable<string> GetCommandNames() => FightCommands.Select(fc => fc.Name);
		public int Attack() => GetRandValue(max: Level << 1) + Level;
		public void SetFightCommands(IEnumerable<FightCommand> fightCmds) => _fightCommands = new List<FightCommand>(fightCmds);

		/// <summary>
		/// 経験値を獲得する
		/// レベルアップしたらメッセージを返す
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public IEnumerable<string> AddEXP(int exp)
		{
			EXP += exp;
			while (CanLevelUp)
			{
				yield return DoLevelUp();
			}
		}

		/// <summary>
		/// プレイヤーを作り出す。
		/// 通常はコンストラクタを使う事。
		/// </summary>
		/// <example>
		/// ゲーム途中でキャラクターを作成したい時に使う。
		/// <code>
		/// Player newPlayer = 
		///		RPGPlayer.CreatePlayer("クラウド", 2000, 1, 54);
		///		
		/// Console.WriteLine($"{newPlayer.Name}が仲間に加わった!");
		/// </code>
		/// </example>
		/// <param name="name"></param>
		/// <param name="maxHP"></param>
		/// <param name="nowHP"></param>
		/// <param name="lv"></param>
		/// <returns></returns>
		public static RPGPlayer CreatePlayer(string name, int maxHP, int nowHP, int lv)
		{
			RPGPlayer rp = new RPGPlayer(name, maxHP, lv)
			{
				HP = nowHP,
			};
			return rp;
		}

        #endregion

        /// <summary>
        /// レベルアップさせる
        /// </summary>
        /// <returns>レベルアップメッセージ</returns>
        protected virtual string DoLevelUp()
        {
            EXP = Math.Max(EXP - NeedEXP, 0);
            Level++;
            MaxHP += Level * 3;
            HP = MaxHP;

            return $"HPが{Level * 3}上がった";
        }

		#endregion
	}
}
