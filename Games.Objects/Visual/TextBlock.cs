using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games.Objects.Visual
{
	/// <summary>
	/// 枠線と背景と、それを描画するメソッドを持った基底クラスを提供する。
	/// </summary>
	public abstract class TextBlock : FrameworkElement
	{
		#region 表示位置・サイズ等のプロパティ

		/// <summary>
		/// 描画する範囲
		/// </summary>
		protected virtual Rect DrawingArea =>
			new Rect(new Point(), RenderSize);

		#endregion

		#region 背景・枠線の情報のプロパティ

		/// <summary>
		/// 枠線の太さを取得または設定する。
		/// </summary>
		public double Border
		{
			get { return BorderPen.Thickness; }
			set { BorderPen.Thickness = value; }
		}
		/// <summary>
		/// <see cref="InfoTextBox"/>の枠線を描画するペンを取得または設定する。
		/// </summary>
		
		public Pen BorderPen { get; set; }
		/// <summary>
		/// <see cref="InfoTextBox"/>の枠線の色を取得または設定する。
		/// </summary>
		public Brush BorderBrush
		{
			get { return BorderPen.Brush; }
			set { BorderPen.Brush = value; }
		}
		/// <summary>
		/// <see cref="InfoTextBox"/>の背景色を取得または設定する。
		/// </summary>
		public Brush BackgroundBrush { get; set; }

		#endregion

		#region その他のプロパティ

		public ImageSource BackgroundImage{ get; set; }

		#endregion

		#region メソッド

		/// <summary>
		/// <see cref="TextBlock"/>を描画する
		/// </summary>
		/// <param name="dc">描画するGDI</param>
		protected void DrawTextBlock(DrawingContext dc)
		{
			dc.DrawImage(BackgroundImage, DrawingArea);
			// 矩形描画
			dc.DrawRectangle(BackgroundBrush, BorderPen, DrawingArea);
		}

		protected override void OnRender(DrawingContext dc)
		{
			DrawTextBlock(dc);
		}

		#endregion
	}
}
