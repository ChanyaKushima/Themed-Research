using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games.Objects.Visual
{
	/// <summary>
	/// テキストを選択できるようなテキストボックスを表示するクラス。
	/// </summary>
	public class ChoiceTextBox : InfoTextBox
	{
		#region プロパティ・フィールド

		private int _selectLine;
		private Color _ariaColor = Colors.Gray;
		private Lazy<Brush> _selectedAriaBrush = new Lazy<Brush>(() => Brushes.Gray);
		
		/// <summary>
		/// 選択している行を取得する。
		/// 行数は0から数える。
		/// </summary>
		public int SelectLine
		{
			get { return _selectLine; }
			set
			{
				int rem = value - value / Line * Line;
				_selectLine = value >= 0 ? rem : Line + rem;
			}
		}
		/// <summary>
		/// 選択範囲を示す色を取得または設定する。
		/// </summary>
		public Color SelectAriaColor
		{
			get { return _ariaColor; }
			set
			{
				_ariaColor = value;
				_selectedAriaBrush = new Lazy<Brush>(() => new SolidColorBrush(value));
			}
		}

		public Pen SelectedAriaPen { get; set; } 

		public Brush SelectedAriaBrush
		{
			get { return _selectedAriaBrush.Value; }
			set { _selectedAriaBrush = new Lazy<Brush>(() => value); }
		}

		#endregion

		#region メソッド

		/// <summary>
		/// <see cref="ChoiceTextBox"/>を描画する。
		/// </summary>
		/// <param name="dc">描画するGDI</param>
		public void DrawChoiceTextBox(DrawingContext dc)
		{
			DrawTextBlock(dc);
			DrawSelectedAria(dc);
			DrawText(dc);
		}

		/// <summary>
		/// 次の行を選択する。
		/// </summary>
		public void SelectNext() => SelectLine++;
		/// <summary>
		/// 前の行を選択する。
		/// </summary>
		public void SelectPrev() => SelectLine--;

		#region プライベートメソッド

		private void DrawSelectedAria(DrawingContext dc)
		{
			dc.DrawRectangle(SelectedAriaBrush, SelectedAriaPen, TextAria(_selectLine));
		}

		private Rect TextAria(int i) =>
			new Rect(TextLeft, TextTop + i * FontSize * (1 + TextLineMargin), _t_width, FontHeight);

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
