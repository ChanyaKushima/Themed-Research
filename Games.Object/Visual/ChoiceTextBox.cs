using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace Games.Object.Visual
{
	/// <summary>
	/// テキストを選択できるようなテキストボックスを表示するクラス。
	/// </summary>
	public class ChoiceTextBox : InfoTextBox
	{
		#region プロパティ・フィールド

		private int _selectLine;
		
		/// <summary>
		/// 選択している行を取得する。
		/// 行数は0から数える。
		/// </summary>
		public int SelectLine
		{
			get { return _selectLine; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int rem = value - value / ShowLine * ShowLine;
				_selectLine = value >= 0 ? rem : ShowLine + rem;
			}
		}

        public Brush SelectedAriaBorderBrush { get; set; } = null;
        public Thickness SelectedAriaBorderThickness { get; set; } = new Thickness();

        public Brush SelectedAria { get; set; } = Brushes.Gray;

		#endregion

		#region メソッド

		/// <summary>
		/// <see cref="ChoiceTextBox"/>を描画する。
		/// </summary>
		/// <param name="dc">描画するGDI</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void DrawChoiceTextBox(DrawingContext dc)
		{
			DrawTextBlock(dc);
			DrawSelectedAria(dc);
			DrawText(dc);
		}

		/// <summary>
		/// 次の行を選択する。
		/// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SelectNext() => SelectLine++;
		/// <summary>
		/// 前の行を選択する。
		/// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SelectPrev() => SelectLine--;

		#region プライベートメソッド

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawSelectedAria(DrawingContext dc)
		{
            Rect textAria = TextAria(_selectLine);
            dc.DrawBorder(SelectedAriaBorderBrush, textAria, SelectedAriaBorderThickness);
            dc.DrawRectangle(SelectedAria, null, textAria);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Rect TextAria(int line) =>
            new Rect(GetTextTop(line), new Size(TextWidth, FontHeight));

        #endregion

        #region オーバーライドしたメソッド

		protected override void OnRender(DrawingContext dc)
		{
			DrawChoiceTextBox(dc);
		}

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>
        /// <see cref="ChoiceTextBox"/>のコンストラクタ
        /// </summary>
        public ChoiceTextBox() : base()
		{

		}

		#endregion
	}
}
