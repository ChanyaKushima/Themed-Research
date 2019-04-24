namespace Games.Objects.RPG
{
	using System;
	using System.Collections.Generic;

	public abstract class RPGCharaBase : CharaBase, ICharaBase
	{
		public RPGCharaBase(string name, int hp) : base(name, hp)
		{
		}

		public RPGCharaBase(string name, int hp, int lv) : base(name, hp, lv)
		{
		}

		public abstract List<FightAction> GetActions();
		public abstract FightAction SelectedAction { get; set; }
		public abstract RPGCharaBase TargetChara{ get; set; }

		public override abstract int Attack();

		public void ActionInvoke()
		{
			SelectedAction(this, TargetChara);
		}
	}

	public delegate void FightAction(RPGCharaBase self, RPGCharaBase target);
}
