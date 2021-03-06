﻿using System;
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
        private static readonly Type _typeofThis = typeof(ChoiceTextBox);

        public static readonly DependencyProperty SelectedLineProperty =
            DependencyProperty.Register(nameof(SelectedLine), typeof(int), _typeofThis,
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsArrange
                )
        );

        public static readonly DependencyProperty SelectedAriaBorderBrushProperty =
            DependencyProperty.Register(nameof(SelectedAriaBorderBrush), typeof(Brush), _typeofThis,
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SelectedAriaProperty =
            DependencyProperty.Register(nameof(SelectedAria), typeof(Brush), _typeofThis,
                new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SelectedAriaBorderThicknessProperty =
            DependencyProperty.Register(nameof(SelectedAriaBorderThickness), typeof(Thickness), _typeofThis,
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));



        #region プロパティ・フィールド

        private int _selectLine;

        /// <summary>
        /// 選択している行を取得する。
        /// 行数は0から数える。
        /// </summary>
        public int SelectedLine
        {
            get => _selectLine;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int rem = value - value / DisplayLines * DisplayLines;
                _selectLine = value >= 0 ? rem : DisplayLines + rem;
                SetValue(SelectedLineProperty, _selectLine);
            }
        }

        public Brush SelectedAriaBorderBrush
        {
            get => (Brush)GetValue(SelectedAriaBorderBrushProperty);
            set => SetValue(SelectedAriaBorderBrushProperty, value);
        }
        public Thickness SelectedAriaBorderThickness
        {
            get => (Thickness)GetValue(SelectedAriaBorderThicknessProperty);
            set => SetValue(SelectedAriaBorderThicknessProperty, value);
        }
        public Brush SelectedAria
        {
            get => (Brush)GetValue(SelectedAriaProperty);
            set => SetValue(SelectedAriaProperty, value);
        }
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
        public void SelectNext() => SelectedLine++;
        /// <summary>
        /// 前の行を選択する。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SelectPrev() => SelectedLine--;

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
