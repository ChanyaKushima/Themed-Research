using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace Games.Objects.Visual
{
	/// <summary>
	/// 「敵があらわれた！」などと言ったゲーム情報を表示する
	/// ツールを取りそろえたクラス
	/// </summary>
	public class InfoTextBox : TextBlock
	{
		#region 定数

		public const double DefaultPixelsPerDpi = 1.5;

		#endregion

		private double _pixelsPerDpi=DefaultPixelsPerDpi;
		#region 描画位置・サイズ等のプロパティ

		/// <summary>
		/// 描画するテキストの色
		/// </summary>
		public Brush TextBrush { get; set; } = Brushes.White;
		/// <summary>
		/// <see cref="InfoTextBox"/>の描画位置の左端から,
		/// テキストの表示位置の左端までの距離をピクセルで取得または設定する。
		/// </summary>
		public double TextLeft { get; set; } = 30;
		/// <summary>
		/// <see cref="InfoTextBox"/>の描画位置の上端から,
		/// テキストの表示位置の上端までの距離をピクセルで取得または設定する。
		/// </summary>
		public double TextTop { get; set; } = 15;

		/// <summary>
		/// 行間の幅を行単位でを取得または設定する。
		/// </summary>
		public double TextLineMargin { get; set; }

		/// <summary>
		/// 規定の描画するテキストのフォーマットを取得または設定する。
		/// </summary>
		public Typeface Typeface { get; set; }

		/// <summary>
		/// 表示する行数を取得または設定する。
		/// </summary>
		public int Line { get; set; }

		/// <summary>
		/// 現在表示中の文を取得する。
		/// </summary>
		public List<string> Text { get; private set; } = new List<string>();
		/// <summary>
		/// <see cref="NextText"/>が次に表示する文を持っているかどうかを取得する。
		/// </summary>
		public bool HasNextText => Text.Count > Line;

		/// <summary>
		/// 表示するテキストのフォントを取得または設定する。
		/// </summary>
		public FontFamily Font { get; set; }
		/// <summary>
		/// <see cref="Font.Size"/>を取得または設定する。
		/// </summary>
		public double FontSize { get; set; }
		public double FontHeight => Math.Ceiling(_pixelsPerDpi * Font.LineSpacing);
		public double PixelsPerDpi
		{
			get { return _pixelsPerDpi; }
			set
			{
				if (value <= 0)
				{
					ThrowHelper.ThrowArgumentException("入力値は0より大きくなければいけません。");
				}
				_pixelsPerDpi = value;
			}
		}
		#endregion

		#region プライベートプロパティ

		/// <summary>
		/// テキスト描画領域の幅
		/// </summary>
		protected double _t_width => Width - D_TextLeft;
		/// <summary>
		/// テキスト描画領域の高さ
		/// </summary>
		protected double _t_height => Height - D_TextTop;

		/// <summary>
		/// テキストを描画する範囲
		/// </summary>
		protected virtual Rect TextDrawingArea =>
			new Rect(TextLeft, TextTop, _t_width, _t_height);


		/// <summary>
		/// <see cref="TextLeft"/>の二倍
		/// </summary>
		private double D_TextLeft => TextLeft * 2;
		/// <summary>
		/// <see cref="TextTop"/>の二倍
		/// </summary>
		private double D_TextTop => TextTop * 2;

		#endregion

		#region メソッド

		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
		public void SetText(params string[] str)
		{
			Text = new List<string>(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="visible"><seealso cref="TextBlock.Visibility"/>を変更する。(オプション)</param>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
		public void SetText(Visibility visibility, params string[] str)
		{
			SetText(str);
			Visibility = visibility;
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
		public void SetText(IEnumerable<string> str)
		{
			Text = new List<string>(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="visibility"><seealso cref="UIElement.Visibility"/>を変更する。(オプション)</param>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
		public void SetText(Visibility visibility, IEnumerable<string> str)
		{
			SetText(str);
			Visibility = visibility;
		}


		/// <summary>
		/// <see cref="NextText"/>を描画するテキストに設定する。
		/// </summary>
		/// <param name="visible"><seealso cref="UIElement.Visibility"/>を変更する。(オプション)</param>
		public void GoToNextText(Visibility visibility)
		{
			GoToNextText();
			Visibility = visibility;
		}
		/// <summary>
		/// <see cref="NextText"/>を描画するテキストに設定する。
		/// </summary>
		public void GoToNextText()
		{
			int len = Math.Min(Line, Text.Count);
			Text.RemoveRange(0, len);
		}

		/// <summary>
		/// 描画するテキストを追加する。
		/// </summary>
		/// <param name="str">追加するテキスト</param>
		public void AddText(params string[] str)
		{
			Text.AddRange(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを追加する。
		/// </summary>
		/// <param name="str">追加するテキスト</param>
		public void AddText(IEnumerable<string> str)
		{
			Text.AddRange(str);
			FillInTextblanks();
		}

		/// <summary>
		/// <see cref="InfoTextBox"/>を描画する。
		/// </summary>
		/// <param name="dc">描画するGDI</param>
		protected void DrawInfoTextBox(DrawingContext dc)
		{
			DrawTextBlock(dc);

			DrawText(dc);
		}

		/// <summary>
		/// テキストの描画
		/// <see cref="InfoTextBox"/>の機能
		/// </summary>
		/// <param name="dc">描画するGDI</param>
		protected void DrawText(DrawingContext dc)
		{
			int len = Math.Min(Line, Text.Count);
			// テキスト描画
			for (int i = 0; i < len; i++)
			{
				var ft = new FormattedText(Text[i], CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
					Typeface, FontSize, TextBrush, null, TextFormattingMode.Ideal, PixelsPerDpi);
				dc.DrawText(ft, new Point(TextLeft, TextTop + i * FontSize * (1 + TextLineMargin)));
			}
		}

		/// <summary>
		/// <see cref="Text"/>プロパティを規定値にリセットする。
		/// </summary>
		public void ResetText()
		{
			Text.Clear();
		}

		private void FillInTextblanks()
		{
			int cnt = Line - Text.Count % Line;
			if (cnt != Line)
			{
				for (int i = 0; i < cnt; i++)
				{
					Text.Add(null);
				}
			}
		}

		protected override void OnRender(DrawingContext dc)
		{
			DrawInfoTextBox(dc);
		}

		#endregion

		#region コンストラクタ

		public InfoTextBox()
		{

		}

		#endregion

	}
}
