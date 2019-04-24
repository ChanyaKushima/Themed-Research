using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Games.Objects
{
	public struct Coordinate
	{
		public static readonly Coordinate Empty = new Coordinate();

		public bool IsEmpty
		{
			get { return X == 0 && Y == 0; }
		}

		public int X { get; set; }
		public int Y { get; set; }

		public bool Equals(Coordinate c)
		{
			return c.X == X && c.Y == Y;
		}
		public override bool Equals(object obj)
		{
			if (obj is Coordinate c)
			{
				return Equals(c);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X ^ Y;
		}

		public override string ToString() => "{" + X + ", " + Y + "}";

		public Coordinate(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
		}



		public static implicit operator (int, int) (Coordinate c) => (c.X, c.Y);
		public static implicit operator Coordinate((int X, int Y) t) => new Coordinate(t.X, t.Y);

		public static bool operator ==(Coordinate l, Coordinate r) => l.Equals(r);
		public static bool operator !=(Coordinate l, Coordinate r) => !l.Equals(r);
		public static bool operator ==(Coordinate l, (int X, int Y) r) => l.Equals(r);
		public static bool operator !=(Coordinate l, (int X, int Y) r) => !l.Equals(r);
	}
}
