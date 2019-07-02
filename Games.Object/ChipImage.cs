using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Games.Object
{
	public static class ChipImage
	{
		/// <summary>
		/// チップ読み込み
		/// </summary>
		/// <param name="bitmap">チップ画像</param>
		/// <param name="chipWidth">チップ1つ当たりの幅</param>
		/// <param name="chipHeight">チップ1つ当たりの高さ</param>
		/// <returns>チップにした画像の配列</returns>
		public static Bitmap[] Read(Bitmap bitmap, int chipWidth, int chipHeight)
		{
			if (bitmap is null)
			{
				ThrowHelper.ThrowArgumentNullException(nameof(bitmap));
			}

			int cWid = bitmap.Width / chipWidth;
			int cHei = bitmap.Height / chipHeight;

			var images = new Bitmap[cWid * cHei];
			var size = new Size(chipWidth, chipHeight);

			for (int i = 0; i < cWid; i++)
			{
				for (int j = 0; j < cHei; j++)
				{
					var point = new Point(chipWidth * i, chipHeight * j);
					var r = new Rectangle(point, size);
					images[i + j * cWid] = bitmap.Clone(r, bitmap.PixelFormat);
				}
			}
			return images;
		}
		/// <summary>
		/// チップ読み込み
		/// </summary>
		/// <param name="bitmap">チップ画像</param>
		/// <param name="chipSize">チップ1つ当たりの1辺の長さ</param>
		/// <returns>チップにした画像の配列</returns>
		public static Bitmap[] Read(Bitmap bitmap, int chipSize) =>
			Read(bitmap, chipSize, chipSize);
		/// <summary>
		/// チップ読み込み
		/// </summary>
		/// <param name="imagePath">チップ画像のパス</param>
		/// <param name="chipWidth">チップ1つ当たりの幅</param>
		/// <param name="chipHeight">チップ1つ当たりの高さ</param>
		/// <returns>チップにした画像の配列</returns>
		public static Bitmap[] Read(string imagePath, int chipWidth, int chipHeight)
		{
			if (!System.IO.File.Exists(imagePath))
			{
				ThrowHelper.ThrowFileNotFoundException($"{imagePath}が見つかりませんでした");
			}
			if (!BitmapTryParse(imagePath, out Bitmap result))
			{
				ThrowHelper.ThrowArgumentException($"{imagePath}は画像ファイルではありません");
			}
			return Read(result, chipWidth, chipHeight);
		}
		/// <summary>
		/// チップ読み込み
		/// </summary>
		/// <param name="imagePath">チップ画像のパス</param>
		/// <param name="chipSize">チップ1つ当たりの1辺の長さ</param>
		/// <returns>チップにした画像の配列</returns>
		public static Bitmap[] Read(string imagePath, int chipSize)
		{
			if (!System.IO.File.Exists(imagePath))
			{
				ThrowHelper.ThrowFileNotFoundException($"{imagePath}が見つかりませんでした");
			}
			if (!BitmapTryParse(imagePath, out Bitmap result))
			{
				ThrowHelper.ThrowArgumentException($"{imagePath}は画像ファイルではありません");
			}
			return Read(result, chipSize, chipSize);
		}

		private static bool BitmapTryParse(string imagePath, out Bitmap result)
		{
			try
			{
				result = new Bitmap(imagePath);
			}
			catch (ArgumentException)
			{
				result = null;
				return false;
			}
			return true;
		}
	}
}
