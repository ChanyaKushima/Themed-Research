using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Games.Object.Visual
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

		#region 描画位置・サイズ等のプロパティ

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
        /// 表示する行数を取得または設定する。
        /// </summary>
        public int ShowLine { get; set; } = 1;

        public string Text
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (TextList.Count != 0) ? TextList[0] : null;
            }
        }

		/// <summary>
		/// 現在表示中の文を取得する。
		/// </summary>
		public List<string> TextList { get; private set; } = new List<string>();
		/// <summary>
		/// <see cref="NextText"/>が次に表示する文を持っているかどうかを取得する。
		/// </summary>
		public bool HasNextText => TextList.Count > ShowLine;

        /// <summary>
        /// 文字のHeight
        /// </summary>
        public double FontHeight => Math.Ceiling(FontSize * 1.5);

		#endregion

		#region プライベートプロパティ

		/// <summary>
		/// テキスト描画領域の幅
		/// </summary>
		protected double TextWidth => RenderSize.Width - D_TextLeft;
		/// <summary>
		/// テキスト描画領域の高さ
		/// </summary>
		protected double TextHeight => RenderSize.Height - D_TextTop;

		/// <summary>
		/// テキストを描画する範囲
		/// </summary>
		protected virtual Rect TextDrawingArea =>
			new Rect(TextLeft, TextTop, TextWidth, TextHeight);


		/// <summary>
		/// <see cref="TextLeft"/>の二倍
		/// </summary>
		private double D_TextLeft => TextLeft * 2;
		/// <summary>
		/// <see cref="TextTop"/>の二倍
		/// </summary>
		private double D_TextTop => TextTop * 2;

		#endregion

		#region パブリックメソッド

		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetText(params string[] str)
		{
			TextList = new List<string>(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="visible"><seealso cref="TextBlock.Visibility"/>を変更する。(オプション)</param>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetText(Visibility visibility, params string[] str)
		{
			SetText(str);
			Visibility = visibility;
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetText(IEnumerable<string> str)
		{
			TextList = new List<string>(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを設定する。
		/// </summary>
		/// <param name="visibility"><seealso cref="UIElement.Visibility"/>を変更する。(オプション)</param>
		/// <param name="str"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetText(Visibility visibility, IEnumerable<string> str)
		{
			SetText(str);
			Visibility = visibility;
		}


		/// <summary>
		/// <see cref="NextText"/>を描画するテキストに設定する。
		/// </summary>
		/// <param name="visible"><seealso cref="UIElement.Visibility"/>を変更する。(オプション)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GoToNextText(Visibility visibility)
		{
			GoToNextText();
			Visibility = visibility;
		}
		/// <summary>
		/// <see cref="NextText"/>を描画するテキストに設定する。
		/// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GoToNextText()
		{
			int len = Math.Min(ShowLine, TextList.Count);
			TextList.RemoveRange(0, len);
		}

		/// <summary>
		/// 描画するテキストを追加する。
		/// </summary>
		/// <param name="str">追加するテキスト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddText(params string[] str)
		{
			TextList.AddRange(str);
			FillInTextblanks();
		}
		/// <summary>
		/// 描画するテキストを追加する。
		/// </summary>
		/// <param name="str">追加するテキスト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddText(IEnumerable<string> str)
		{
			TextList.AddRange(str);
			FillInTextblanks();
		}

		/// <summary>
		/// <see cref="TextList"/>プロパティを規定値にリセットする。
		/// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetText()
		{
			TextList.Clear();
		}

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// <see cref="InfoTextBox"/>を描画する。
        /// </summary>
        /// <param name="dc">描画するGDI</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawText(DrawingContext dc)
        {
            int len = Math.Min(ShowLine, TextList.Count);
            var tf = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var location = new Point(TextLeft, TextTop - TextLineMargin);
            // テキスト描画

            var ft = new FormattedText(GetSerialText(len), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                tf, FontSize, Foreground, DefaultPixelsPerDpi)
            {
                LineHeight = FontHeight + TextLineMargin 
            };

            dc.DrawText(ft, location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FillInTextblanks()
		{
			int cnt = ShowLine - TextList.Count % ShowLine;
			if (cnt != ShowLine)
			{
				for (int i = 0; i < cnt; i++)
				{
					TextList.Add(null);
				}
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Point GetTextTop(int line) => new Point(TextLeft, TextTop + line * (FontHeight + TextLineMargin));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetSerialText(int len)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin('\n', TextList.Take(len));
            return sb.ToString();
        }

        #endregion

        #region Overrided Method(s)

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
