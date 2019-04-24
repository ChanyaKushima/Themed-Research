using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games.Objects.Visual
{
	/// <summary>
	/// キャラクターの画像と枠を表示するクラス。
	/// </summary>
	/// <!-- 未使用 バグ発生可能性: 大 -->
	public class CharacterImageBox : TextBlock
	{
		#region 描画位置等のプロパティ

		/// <summary>
		/// 描画する画像
		/// </summary>
		public ImageSource Image { get; set; }

		/// <summary>
		/// <see cref="CharacterImageBox"/>の描画位置の左端から,
		/// テキストの表示位置の左端までの距離をピクセルで取得または設定する。
		/// </summary>
		public double ImageLeft { get; set; }
		/// <summary>
		/// <see cref="CharacterImageBox"/>の描画位置の上端から,
		/// テキストの表示位置の上端までの距離をピクセルで取得または設定する。
		/// </summary>
		public double ImageTop { get; set; }

		/// <summary>
		/// 行間の幅を行単位でを取得または設定する。
		/// </summary>
		public double ImageMargin { get; set; }

		#endregion

		#region プライベートプロパティ

		/// <summary>
		/// テキスト描画領域の幅
		/// </summary>
		protected double _i_width => Width - D_ImageLeft;
		/// <summary>
		/// テキスト描画領域の高さ
		/// </summary>
		protected double _i_height => Height - D_ImageTop;
		/// <summary>
		/// テキストを描画する範囲
		/// </summary>
		protected virtual Rect ImageDrawingArea =>
			new Rect(ImageLeft, ImageTop, _i_width, _i_height);


		/// <summary>
		/// <see cref="ImageLeft"/>の二倍
		/// </summary>
		private double D_ImageLeft => ImageLeft * 2;
		/// <summary>
		/// <see cref="ImageTop"/>の二倍
		/// </summary>
		private double D_ImageTop => ImageTop * 2;

		#endregion

		#region メソッド

		/// <summary>
		/// <see cref="CharacterImageBox"/>を描画する。
		/// </summary>
		/// <param name="dc">描画するGDI</param>
		protected void DrawCharacterImageBox(DrawingContext dc)
		{
			DrawTextBlock(dc);
			dc.DrawImage(Image, ImageDrawingArea);
		}

		protected override void OnRender(DrawingContext dc)
		{
			DrawCharacterImageBox(dc);
		}

		#endregion

		#region コンストラクタ
		/// <summary>
		/// <see cref="CharacterImageBox"/>のコンストラクタ
		/// </summary>
		/// <param name="image">描画する画像</param>
		public CharacterImageBox()
		{
		}

		#endregion
	}
}
