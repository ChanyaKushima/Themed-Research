using System;
using System.Collections.Generic;
using System.Drawing;

namespace Games.Objects.RPG
{
	public class RPGEnemy : RPGCharaBase, IEnemy, IDisposable
	{
		private readonly Image Image;

		protected List<FightAction> _actions = new List<FightAction>();
		
		public override FightAction SelectedAction { get; set; }
		public override RPGCharaBase TargetChara { get; set; }

		#region コンストラクタ・デストラクタ

		public RPGEnemy(string name, int hp, int lv, int exp, string imgPath) :
			this(name, hp, lv, exp, new Bitmap(imgPath))
		{ 
		}
		
		public RPGEnemy(string name, int hp, int lv, int exp, Image image) : base(name, hp, lv)
		{
			EXP = exp;
			Image = image;
		}
		/// <summary>
		/// デストラクタ
		/// </summary>
		~RPGEnemy()
		{
			Dispose(false);
		}

		#endregion

		#region public methods

		/// <summary>
		/// 描画する
		/// </summary>
		/// <param name="g">描画するGDI</param>
		/// <param name="x">描画する中心X座標</param>
		/// <param name="y">描画する中心Y座標</param>
		public void Draw(Graphics g, float x, float y)
		{
			var r = new RectangleF(x - Image.Width / 2.0f, y - Image.Height / 2.0f, Image.Width, Image.Height);
			g.DrawImage(Image, r);
		}
		/// <summary>
		/// 描画する
		/// </summary>
		/// <param name="g">描画するGDI</param>
		/// <param name="x">描画する中心X座標</param>
		/// <param name="y">描画する中心Y座標</param>
		public void Draw(Graphics g, float x, float y, float sizeRatio)
		{
			var r = new RectangleF(x - Image.Width * sizeRatio, y - Image.Height * sizeRatio, Image.Width, Image.Height);
			g.DrawImage(Image, r);
		}

		public void ActionsAdd(FightAction action) => _actions.Add(action);
		public void SetActions(IEnumerable<FightAction> actions) => _actions = new List<FightAction>(actions);
		public override List<FightAction> GetActions() => new List<FightAction>(_actions);
		public override int Attack() => GetRandValue(max: Level << 1);

		#endregion

		#region Disposeメソッド

		private bool disposedValue = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// マネージドオブジェクトの破棄
					Image.Dispose();
					GC.ReRegisterForFinalize(this);
				}
				// アンマネージドオブジェクトの破棄

				// 二重破棄の防止 
				disposedValue = true;
			}
		}

		#endregion
	}
}
