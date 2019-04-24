using System;

namespace Games.Objects
{
	public abstract class CharaBase : ICharaBase
	{
		/// <summary>
		/// 直接触れちゃダメ
		/// </summary>
		private readonly Random rand = new Random();
		/// <summary>
		/// ランダムな値を確保する
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		protected int GetRandValue(int min = 0, int max = 0) => rand.Next(min, max);

		public virtual int MaxLevel { get; protected set; } = 100;
		public virtual int MinLevel { get; protected set; } = 1;


		public virtual string Name { get; }

		private int _m_hp;
		public int MaxHP
		{
			get { return _m_hp; }
			protected set
			{
				int m_hp_tmp = Math.Max(value, 0);
				if (_m_hp!=m_hp_tmp)
				{
					_m_hp = m_hp_tmp;
					MaxHPChanged?.Invoke(this, new EventArgs());
				}
				if (value < HP)
				{
					HP = value;
				}
			}
		}

		private int _hp;
		public int HP
		{
			get { return _hp; }
			protected set
			{
				int hp_tmp = Calc.FitInRange(value, MaxHP, 0);
				if (_hp != hp_tmp)
				{
					bool recovered = _hp < hp_tmp;
					bool preAlive = IsAlive;

					_hp = hp_tmp;
					HPChanged?.Invoke(this, new EventArgs());
					
					if (recovered)
					{
						Recovered?.Invoke(this, new EventArgs());
					}
					else
					{
						Damaged?.Invoke(this, new EventArgs());
					}
					if (preAlive && IsDead)
					{
						Died?.Invoke(this, new EventArgs());
					}
					else if (!preAlive&&IsAlive)
					{
						Revived?.Invoke(this, new EventArgs());
					}
				}
			}
		}

		private int _lv;
		public int Level
		{
			get { return _lv; }
			protected set
			{
				int lv_tmp = Calc.FitInRange(value, MaxLevel, MinLevel);
				if (_lv != lv_tmp)
				{
					bool lvUp = _lv < lv_tmp;

					_lv = lv_tmp;
					LevelChanged?.Invoke(this, new EventArgs());

					if (lvUp)
					{
						LevelUp?.Invoke(this, new EventArgs());
					}
					else
					{
						LevelDown?.Invoke(this, new EventArgs());
					}
				}
			}
		}
		public int EXP { get; protected set; }

		public virtual bool IsAlive => _hp > 0;
		public bool IsDead => !IsAlive;
		public abstract int Attack();
		public virtual void Damage(int amount) => HP -= amount;
		public virtual void Recover(int amount) => HP += amount;
		public int GetEXP() => EXP;

		public event EventHandler MaxHPChanged;
		public event EventHandler HPChanged;
		public event EventHandler Recovered;
		public event EventHandler Damaged;
		public event EventHandler LevelChanged;
		public event EventHandler LevelUp;
		public event EventHandler LevelDown;
		public event EventHandler Died;
		public event EventHandler Revived;

		protected CharaBase(string name, int hp)
		{
			Name = name;
			MaxHP = hp;
			HP = hp;
			_lv = MinLevel;
		}
		protected CharaBase(string name, int hp, int lv) : this(name, hp)
		{
			Level = lv;
		}
	}
}
