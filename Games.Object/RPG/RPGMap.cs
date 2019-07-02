using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Games.Object.RPG
{
	using static System.Math;


	public class RPGMap<TID> : Map2D<TID, Image[]>, ILinkable<TID, RPGMap<TID>>, IRPGMapHub<TID>, IObjectHub<TID, RPGMap<TID>, RPGMapHub<TID>>
	{
		public static Dictionary<string, Action> TerrainEffects { get; set; } = new Dictionary<string, Action>();

		public new RPGMapHub<TID> Hub { get; }

		/// <summary>
		/// マップを描画する
		/// </summary>
		/// <param name="g"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void Draw(Graphics g, int x, int y)
		{
			if (Data is null)
			{
				ThrowHelper.ThrowNullReferenceException($"{nameof(Data)}がnullです。");
			}
			else if (g is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(g)}がnullです。");
			}

			int beginDrawX = Max(0, x);
			int beginDrawY = Max(0, y);
			int endDrawX = Min(Width, Width + x);
			int endDrawY = Min(Height, Height + y);

			for (int i = beginDrawX; i < endDrawX; i++)
			{
				for (int j = beginDrawY; j < endDrawY; j++)
				{
					Data[i - x, j - y].Draw(g, MapImages, i * DrawSize, j * DrawSize, DrawSize);
				}
			}
		}

		/// <summary>
		/// マップを描画する。
		/// </summary>
		/// <param name="g"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void Draw(Graphics g, float x, float y)
		{
			if (Data is null)
			{
				ThrowHelper.ThrowNullReferenceException($"{nameof(Data)}がnullです。");
			}
			else if (g is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(g)}がnullです。");
			}

			int x_IntPart = (int)x;
			int y_IntPart = (int)y;
			int x_DrawStartPos = (int)(DrawSize * (x - (int)x));
			int y_DrawStartPos = (int)(DrawSize * (y - (int)y));

			int beginDrawX = Max(-1, x_IntPart);
			int beginDrawY = Max(-1, y_IntPart);
			int endDrawX = Min(Width, Width + x_IntPart);
			int endDrawY = Min(Height, Height + y_IntPart);

			for (int i = beginDrawX; i < endDrawX; i++)
			{
				for (int j = beginDrawY; j < endDrawY; j++)
				{
					Data[i - x_IntPart, j - y_IntPart].Draw(g, MapImages, i * DrawSize + x_DrawStartPos, j * DrawSize + y_DrawStartPos, DrawSize);
				}
			}
		}

		public void ChipImageRead(Bitmap bitmap, int cutSize)
		{
			MapImages = ChipImage.Read(bitmap, cutSize);
		}

		public void SetMapImages(Image[] images, int drawSize)
		{
			DrawSize = drawSize;
			MapImages = images;
		}

		public void Link(TID id, RPGMap<TID> target)
		{
			Hub.Link(id, target, false);
		}
		public void Link(TID id, RPGMap<TID> target, bool overwrite)
		{
			Hub.Link(id, target, overwrite);
		}


		#region コンストラクタ

		public RPGMap()
		{
		}

		public RPGMap(Bitmap chipSource, int cutSize, int drawSize) :
			this(chipSource, cutSize, drawSize, null, 0, 0)
		{
		}

		public RPGMap(Bitmap chipSource, int cutSize, int drawSize, MapPiece[,] mapData, int drawWidth, int drawHeight) :
			this(ChipImage.Read(chipSource, cutSize), drawSize, mapData, drawWidth, drawHeight)
		{
		}

		public RPGMap(Image[] mapImages, int drawSize, MapPiece[,] mapData, int drawWidth, int drawHeight)
		{
			MapImages = mapImages;
			DrawSize = drawSize;
			Data = mapData;
			DrawingWidth = drawWidth;
			DrawingHeight = drawHeight;
		}

		#endregion
	}
}
