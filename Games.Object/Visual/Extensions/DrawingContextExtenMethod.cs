using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;

namespace Games.Object.Visual
{
    internal static class DrawingContextExtenMethod
    {
        public static void DrawBorder(this DrawingContext dc, Brush brush, Rect drawAria, Thickness thickness)
        {
            Rect rect = drawAria;

            // Topの枠線の描画
            Point p1 = new Point(rect.X - thickness.Left, rect.Y - thickness.Top / 2.0);
            Point p2 = new Point(rect.X + rect.Width + thickness.Right, p1.Y);

            Pen pen = new Pen(brush, thickness.Top);
            dc.DrawLine(pen, p1, p2);

            // Bottomの枠線の描画
            p1.Y = p2.Y =
                rect.Y + rect.Height + thickness.Bottom / 2.0;
            pen.Thickness = thickness.Bottom;
            dc.DrawLine(pen, p1, p2);

            // Leftの枠線の描画
            p1 = new Point(rect.X - thickness.Left / 2.0, rect.Y - thickness.Top);
            p2 = new Point(p1.X, rect.Y + rect.Height + thickness.Bottom);
            pen.Thickness = thickness.Left;
            dc.DrawLine(pen, p1, p2);

            // Rightの枠線の描画
            p1.X = p2.X =
                rect.X + rect.Width + thickness.Right / 2.0;
            pen.Thickness = thickness.Right;
            dc.DrawLine(pen, p1, p2);
        }

        //private struct RectPoint
        //{
        //    public Rect Rect { get; }

        //    public Point UpperLeft => new Point(Rect.X, Rect.Y);
        //    public Point UpperRight => new Point(Rect.X + Rect.Width, Rect.Y);
        //    public Point LowerLeft => new Point(Rect.X, Rect.Y + Rect.Height);
        //    public Point LowerRight => new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height);

        //    public RectPoint(Rect rect) => Rect = rect;

        //    public static implicit operator Rect(RectPoint rp) => rp.Rect;
        //    public static implicit operator RectPoint(Rect rect) => new RectPoint(rect);
        //}
    }
}
