using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games.Object.Visual
{
	/// <summary>
	/// 枠線と背景と、それを描画するメソッドを持った基底クラスを提供する。
	/// </summary>
	public abstract class TextBlock : Control
	{
        #region 表示位置・サイズ等のプロパティ

        /// <summary>
        /// 描画する範囲
        /// </summary>
        protected virtual Rect DrawArea =>
            new Rect(new Point(), RenderSize);

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
            dc.DrawImage(BackgroundImage, DrawArea);
            // 枠線描画
            dc.DrawBorder(BorderBrush, DrawArea, BorderThickness);
            // 矩形描画
            dc.DrawRectangle(Background, null, DrawArea);
        }

        protected abstract override void OnRender(DrawingContext drawingContext);

        #endregion
    }
}
