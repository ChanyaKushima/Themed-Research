namespace Games.Objects
{
	using System;
	using System.Drawing;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	[Serializable]
	public abstract class Map2D<THubID, TImages> : Map<THubID, MapPiece[,]>, IMap2DHub<THubID, TImages>
	{
		[NonSerialized]
		private TImages mapImages;
		public TImages MapImages { get => mapImages; internal protected set => mapImages = value; }

		public virtual int Width => (Data is null) ? 0 : Data.GetLength(0);
		public virtual int Height => (Data is null) ? 0 : Data.GetLength(1);
		public int DrawingWidth { get; set; }
		public int DrawingHeight { get; set; }
		public int DrawSize { get; set; }
		public new Map2DHub<THubID, TImages> Hub { get; }

		public virtual void Draw(Graphics g, int x, int y) => Draw(g, (float)x, (float)y);
		public abstract void Draw(Graphics g, float x, float y);

		public Map2D()
		{
			Hub = new Map2DHub<THubID, TImages>();
		}

		//public static MapPiece[,] ReadMapDataFile(string FilePath)
		//{
		//	if (FilePath is null)
		//	{
		//		ThrowHelper.ThrowArgumentException($"{FilePath}がnullです");
		//	}
		//	if (!File.Exists(FilePath))
		//	{
		//		ThrowHelper.ThrowFileNotFoundException($"\"{FilePath}\"が見つかりませんでした");
		//	}
		//	MapPiece[,] data;

		//	using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None))
		//	{
		//		BinaryFormatter formatter = new BinaryFormatter();
		//		data = (MapPiece[,])formatter.Deserialize(stream);
		//	}
		//	return data;
		//}
	}
}
