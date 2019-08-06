namespace Games.Object
{
	using System.Drawing;
    using System.Diagnostics;

	/// <summary>
	/// マップの欠片
	/// </summary>
	/// <!-- (なんかアイテムみたいだね) -->
	/// <!-- サイズが16Byte以上になったらclassに変える -->
	/// <!-- classに改変しても, コンストラクタで全てのフィールドに初期値を与える様にする -->
	/// <!-- もしくはファイルパスを引数として, 2次元・3次元配列を返す静的メソッドを定義する -->
	/// <!-- 後記の場合, このクラスをinternalにする事をお勧めする -->
    [DebuggerDisplay("Layer = [{Layer[0]}, {Layer[1]}, {Layer[2]}]")]
	public class MapPiece
	{
		#region 定数

		/// <summary>
		/// 透明なレイヤーの値
		/// </summary>
		public const int TransparentLayer = -1;
		/// <summary>
		/// 透明なレイヤーの値
		/// <see cref="TransparentLayer"/>の省略バージョン
		/// </summary>
		public const int TransLayer = TransparentLayer;

		/// <summary>
		/// レイヤーの数
		/// </summary>
		public static readonly int LayerNo = 3;

        #endregion

        public int[] Layer { get; set; }
		/// <summary>
		/// <see cref="MapPiece"/>を識別するID文字列。
		/// 識別する必要がないならば<see langword="null"/>。
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 通り抜け出来るか
		/// </summary>
		public bool Passable { get; private set; }

		public void ChangeLayer(int no, int data)
		{
			if (no >= Layer.Length || no < 0)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(no)}の値が異常です。\n{0}～{Layer.Length - 1}の範囲で使用してください。");
			}

			Layer[no] = data;
		}

		internal void Draw(Graphics g, Image[] resourses, int x, int y, int size)
		{
			for (int i = 0; i < Layer.Length; i++)
			{
				if (Layer[i] >= 0)
				{
					g.DrawImage(resourses[Layer[i]], x, y, size, size);
				}
			}
		}
		internal void Draw(Graphics g, Image[,] resourses, int x, int y, int size)
		{
			for (int i = 0; i < Layer.Length; i++)
			{
				if (Layer[i] >= 0)
				{
					g.DrawImage(resourses[i, Layer[i]], x, y, size, size);
				}
			}
		}
		internal void Draw(Graphics g, Image[][] resourses, int x, int y, int size)
		{
			for (int i = 0; i < Layer.Length; i++)
			{
				if (Layer[i] >= 0)
				{
					g.DrawImage(resourses[i][Layer[i]], x, y, size, size);
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is MapPiece p)
			{
				return Equals(p);
			}
			return false;
		}
		public bool Equals(MapPiece piece)
		{
			return (piece.Layer[0] == Layer[0]) && (piece.Layer[1] == Layer[1]) &&
				   (piece.Layer[2] == Layer[2]) && (piece.Passable == Passable) &&
				   (piece.ID == ID);
		}

		public override int GetHashCode()
		{
			return Layer[0] ^ Layer[1] ^ Layer[2] ^ Passable.GetHashCode() ^ ID.GetHashCode();
		}

		/// <summary>
		/// <see cref="MapPiece"/>のコンストラクタ。
		/// </summary>
		/// <param name="d0"><see cref="Layer[0]"/>のデータ</param>
		/// <param name="d1"><see cref="Layer[1]"/>のデータ</param>
		/// <param name="d2"><see cref="Layer[2]"/>のデータ</param>
		/// <param name="passable"><see cref="Passable"/>のデータ</param>
		public MapPiece(int d0, int d1, int d2, bool passable, string id = null) :
			this(new[] { d0, d1, d2 }, passable, id)
		{
		}

		public MapPiece(int[] data, bool passable, string id = null)
		{
			if (data is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(data)}がnullです。");
			}
			else if (data.Length != LayerNo)
			{
				ThrowHelper.ThrowArgumentException(
					$"{nameof(data)}の要素数が不正です。\n" +
					$"{nameof(data.Length)}が{data.Length}になっています。\n" +
					$"正しくは{LayerNo}である必要があります。"
				);
			}
			Layer = data;
			Passable = passable;
			ID = id;
		}

		#region Defined of operators

		public static bool operator ==(MapPiece left, MapPiece right) => left.Equals(right);
		public static bool operator ==(MapPiece left, object   right) => left.Equals(right);
		public static bool operator ==(object   left, MapPiece right) => right.Equals(left);
		public static bool operator !=(MapPiece left, MapPiece right) => !left.Equals(right);
		public static bool operator !=(MapPiece left, object   right) => !left.Equals(right);
		public static bool operator !=(object   left, MapPiece right) => !right.Equals(left);

		#endregion
	}
}
