using System;

namespace Games.Object
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

        /// <summary>
        /// 最大レベルの設定。
        /// <see cref="MaxLevel"/>を含む。
        /// </summary>
		public virtual int MaxLevel { get; protected set; } = 100;
        /// <summary>
        /// 最低レベルの設定。
        /// <see cref="MaxLevel"/>を含む。
        /// </summary>
		public virtual int MinLevel { get; protected set; } = 1;

        /// <summary>
        /// 名前
        /// </summary>
		public virtual string Name { get; }

		private int _m_hp;
		public int MaxHP
		{
			get { return _m_hp; }
            protected set
            {
                int tmp_m_hp = Math.Max(value, 0);
                if (_m_hp != tmp_m_hp)
                {
                    _m_hp = tmp_m_hp;
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
				int tmp_hp = Calc.FitInRange(value, MaxHP, 0);
				if (_hp != tmp_hp)
				{
					bool recovered = _hp < tmp_hp;
					bool preAlive = IsAlive;

					_hp = tmp_hp;
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
                    else if (!preAlive && IsAlive)
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
				int tmp_lv = Calc.FitInRange(value, MaxLevel, MinLevel);
				if (_lv != tmp_lv)
				{
					bool lvUp = _lv < tmp_lv;

					_lv = tmp_lv;
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
		//public abstract int Attack();
		public virtual void Damage(int amount) => HP -= amount;
		public virtual void Recover(int amount) => HP += amount;
		public int GetEXP() => EXP;

		public virtual event EventHandler MaxHPChanged;
		public virtual event EventHandler HPChanged;
        public virtual event EventHandler Recovered;
        public virtual event EventHandler Damaged;
        public virtual event EventHandler LevelChanged;
        public virtual event EventHandler LevelUp;
        public virtual event EventHandler LevelDown;
        public virtual event EventHandler Died;
        public virtual event EventHandler Revived;

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
