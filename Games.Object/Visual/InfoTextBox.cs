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
        #region 

        private static readonly Type _typeofThis = typeof(InfoTextBox);

        public static readonly DependencyProperty TextLeftProperty =
            DependencyProperty.Register(nameof(TextLeft), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(30.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextTopProperty =
            DependencyProperty.Register(nameof(TextTop), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextLineMarginProperty =
            DependencyProperty.Register(nameof(TextLineMargin), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DisplayLinesProperty =
            DependencyProperty.Register(nameof(DisplayLines), typeof(int), _typeofThis,
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextsProperty =
            DependencyProperty.Register(nameof(Texts), typeof(ItemList<string>), _typeofThis,
                new FrameworkPropertyMetadata(new ItemList<string>(i => i + 8), FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region 定数

        public const double DefaultPixelsPerDpi = 1.5;
        private static readonly string _newLineString = "\n";

        #endregion

        #region 描画位置・サイズ等のプロパティ

        /// <summary>
        /// <see cref="InfoTextBox"/>の描画位置の左端から,
        /// テキストの表示位置の左端までの距離をピクセルで取得または設定する。
        /// </summary>
        public double TextLeft
        {
            get => (double)GetValue(TextLeftProperty);
            set => SetValue(TextLeftProperty, value);
        }

        /// <summary>
        /// <see cref="InfoTextBox"/>の描画位置の上端から,
        /// テキストの表示位置の上端までの距離をピクセルで取得または設定する。
        /// </summary>
        public double TextTop
        {
            get => (double)GetValue(TextTopProperty);
            set => SetValue(TextTopProperty, value);
        }

        /// <summary>
        /// 行間の幅を行単位でを取得または設定する。
        /// </summary>
        public double TextLineMargin
        {
            get => (double)GetValue(TextLineMarginProperty);
            set => SetValue(TextLineMarginProperty, value);
        }

        /// <summary>
        /// 表示する行数を取得または設定する。
        /// </summary>
        public int DisplayLines
        {
            get => (int)GetValue(DisplayLinesProperty);
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                SetValue(DisplayLinesProperty, value);
            }
        }

        public string Text
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Texts.Count != 0) ? Texts[0] : null;
        }

        /// <summary>
        /// 現在表示中の文を取得する。
        /// </summary>
        public ItemList<string> Texts => (ItemList<string>)GetValue(TextsProperty);

        /// <summary>
        /// <see cref="NextText"/>が次に表示する文を持っているかどうかを取得する。
        /// </summary>
        public bool HasNextText => Texts.Count > DisplayLines;

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
        /// <param name="texts"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetText(params string[] texts)
        {
            SetText(texts);
        }
        /// <summary>
        /// 描画するテキストを設定する。
        /// </summary>
        /// <param name="visible"><seealso cref="TextBlock.Visibility"/>を変更する。(オプション)</param>
        /// <param name="texts"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetText(Visibility visibility, params string[] texts)
        {
            SetText((IEnumerable<string>)texts);
            Visibility = visibility;
        }
        /// <summary>
        /// 描画するテキストを設定する。
        /// </summary>
        /// <param name="texts"><see cref="InfoTextBox"/>に表示する文字(改行はしない)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetText(IEnumerable<string> texts)
        {
            Texts.Clear();
            Texts.AddRange(texts);
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
            int len = Math.Min(DisplayLines, Texts.Count);
            Texts.RemoveRange(0, len);
        }

        /// <summary>
        /// 描画するテキストを追加する。
        /// </summary>
        /// <param name="texts">追加するテキスト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddText(params string[] texts)
        {
            AddText((IEnumerable<string>)texts);
        }
        /// <summary>
        /// 描画するテキストを追加する。
        /// </summary>
        /// <param name="texts">追加するテキスト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddText(IEnumerable<string> texts)
        {
            Texts.AddRange(texts);
            FillInTextblanks();
        }

        /// <summary>
        /// <see cref="Texts"/>プロパティを規定値にリセットする。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetText()
        {
            Texts.Clear();
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
            int len = Math.Min(DisplayLines, Texts.Count);
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
            int cnt = DisplayLines - Texts.Count % DisplayLines;
            if (cnt != DisplayLines)
            {
                for (int i = 0; i < cnt; i++)
                {
                    Texts.Add(_newLineString);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Point GetTextTop(int line) => new Point(TextLeft, TextTop + line * (FontHeight + TextLineMargin));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetSerialText(int len)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin('\n', Texts.Take(len));
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
