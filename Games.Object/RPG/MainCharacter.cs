using System.Drawing;

namespace Games.Object.RPG
{
	public sealed class RPGMainCharacter : RPGPlayer
	{
		public Coordinate Coordinate { get; set; }
		public Direction Direction { get; set; }

		private readonly Image[] _images;

		public RPGMainCharacter(string name, int hp, Coordinate coordinate, Image[] images) : base(name, hp)
		{
			Coordinate = coordinate;
			_images = images;
		}

		public void Draw(Graphics g, float x, float y, float width, float height)
		{
			g.DrawImage(_images[(int)Direction], x, y, width, height);
		}
	}

	public enum Direction { Down, Left, Right, Up }
}
