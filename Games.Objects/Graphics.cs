using System;
using System.Drawing;

using Games.Objects.Visual;
using Games.Objects.RPG;

namespace Games.Graphic
{
	public static class Extensions
	{
		#region DrawText Methods

		public static void DrawText(this Graphics g, string s, int color, float x, float y, Font font)
		{
			g.DrawText(s, Color.FromArgb(color), x, y, font);
		}
		public static void DrawText(this Graphics g, string s, int color, float x, float y, float size)
		{
			g.DrawText(s, Color.FromArgb(color), x, y, size);
		}
		public static void DrawText(this Graphics g, string s, uint color, float x, float y, Font font)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), x, y, font);
		}
		public static void DrawText(this Graphics g, string s, uint color, float x, float y, float size)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), x, y, size);
		}
		public static void DrawText(this Graphics g, string s, Color color, float x, float y, Font font)
		{
			g.DrawString(s, font, new SolidBrush(color), x, y);
		}
		public static void DrawText(this Graphics g, string s, Color color, float x, float y, float size)
		{
			g.DrawString(s, new Font("MS UI Gothic", size), new SolidBrush(color), x, y);
		}
		public static void DrawText(this Graphics g, string s, int color, float x, float y, Font font, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(color), x, y, font,format);
		}
		public static void DrawText(this Graphics g, string s, int color, float x, float y, float size, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(color), x, y, size, format);
		}
		public static void DrawText(this Graphics g, string s, uint color, float x, float y, Font font, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), x, y, font,format);
		}
		public static void DrawText(this Graphics g, string s, uint color, float x, float y, float size, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), x, y, size, format);
		}
		public static void DrawText(this Graphics g, string s, Color color, float x, float y, Font font, StringFormat format)
		{
			g.DrawString(s, font, new SolidBrush(color), x, y, format);
		}
		public static void DrawText(this Graphics g, string s, Color color, float x, float y, float size, StringFormat format)
		{
			g.DrawString(s, new Font("MS UI Gothic", size), new SolidBrush(color), x, y, format);
		}
		public static void DrawText(this Graphics g, string s, Color color, RectangleF rect, Font font, StringFormat format)
		{
			g.DrawString(s, font, new SolidBrush(color), rect, format);
		}
		public static void DrawText(this Graphics g, string s, Color color, RectangleF rect, float size, StringFormat format)
		{
			g.DrawString(s, new Font("MS UI Gothic", size), new SolidBrush(color), rect, format);
		}
		public static void DrawText(this Graphics g, string s, int color, RectangleF rect, Font font, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(color), rect, font, format);
		}
		public static void DrawText(this Graphics g, string s, int color, RectangleF rect, float size, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(color), rect, size, format);
		}
		public static void DrawText(this Graphics g, string s, uint color, RectangleF rect, Font font, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), rect, font, format);
		}
		public static void DrawText(this Graphics g, string s, uint color, RectangleF rect, float size, StringFormat format)
		{
			g.DrawText(s, Color.FromArgb(unchecked((int)color)), rect, size, format);
		}

		#endregion

		public static void DrawRPGMap<T>(this Graphics g, RPGMap<T> map, int x, int y)
		{
			map.Draw(g, x, y);
		}
		public static void DrawRPGMap<T>(this Graphics g, RPGMap<T> map, float x, float y)
		{
			map.Draw(g, x, y);
		}
	}
}
