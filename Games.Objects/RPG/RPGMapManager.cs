using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Objects.RPG
{
	public class RPGMapManager<TID>
	{
		/// <summary>
		/// 使用するマップのリスト
		/// </summary>
		public List<RPGMap<TID>> Maps { get; }

		public RPGMapCollection<TID> TestMaps { get; }

		/// <summary>
		/// メインキャラ
		/// </summary>
		public RPGMainCharacter MainCharacter{ get; set; }

		private RPGMap<TID> _activeMap;
		/// <summary>
		/// <see cref="MainCharacter"/>が現在いるマップ
		/// </summary>
		public RPGMap<TID> ActiveMap
		{
			get { return _activeMap; }
			private set
			{
				if (!Maps.Contains(value))
				{
					ThrowHelper.ThrowArgumentException($"{nameof(ActiveMap)}は{nameof(Maps)}に含まれている必要があります");
				}
				_activeMap = value;
			}
		}

		#region コンストラクタ

		/// <summary>
		/// <see cref="RPGMapManager{TID}"/>のコンストラクタ
		/// </summary>
		/// <param name="mainChara"></param>
		public RPGMapManager(RPGMainCharacter mainChara) : this(mainChara, Enumerable.Empty<RPGMap<TID>>(), null)
		{
		}
		/// <summary>
		/// <see cref="RPGMapManager{TID}"/>のコンストラクタ
		/// </summary>
		/// <param name="mainChara">メインキャラ</param>
		/// <param name="maps">使用するマップのリスト</param>
		/// <param name="activeMap">最初にアクティブにするマップ</param>
		public RPGMapManager(RPGMainCharacter mainChara, IEnumerable<RPGMap<TID>> maps, RPGMap<TID> activeMap)
		{
			if (maps is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(maps)}がnullです");
			}
			if (mainChara is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(mainChara)}がnullです");
			}
			MainCharacter = mainChara;
			Maps = new List<RPGMap<TID>>(maps);
			TestMaps = new RPGMapCollection<TID>(maps);
			ActiveMap = activeMap;
		}

		#endregion

		#region パブリックメソッド

		public void ChangeActiveMapByID(TID id)
		{
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			if (!ActiveMap.Hub.ContainsID(id))
			{
				ThrowHelper.ThrowArgumentException("{" + $"{nameof(id)}:{id}" + "}" + $"が{nameof(ActiveMap)}.{nameof(ActiveMap.Hub)}にありません");
			}
			ActiveMap = _activeMap.Hub[id];
		}

		public void Teleport(TID id, Coordinate coordinate)
		{
			ActiveMap = _activeMap.Hub[id];
			MainCharacter.Coordinate = coordinate;
		}
		public void Teleport(RPGMap<TID> map, Coordinate coordinate)
		{
			ActiveMap = map;
			MainCharacter.Coordinate = coordinate;
		}

		#endregion
	}

	public class RPGMapCollection<TID> : IList<RPGMap<TID>>, IEnumerable<RPGMap<TID>>, ICollection<RPGMap<TID>>
	{
		private readonly List<RPGMap<TID>> _items;

		#region イベント

		internal event EventHandler CollectionChanged;
		internal event ItemAddingEventHandler<RPGMap<TID>> ItemAdding;
		internal event EventHandler ItemAdded;
		internal event ItemRemovingEventHandler<RPGMap<TID>> ItemRemoving;
		internal event EventHandler ItemRemoved;
		internal event EventHandler ItemCleared;

		#endregion

		#region プロパティ

		public int Count => _items.Count;
		public int Capacity => _items.Capacity;
		public bool IsReadOnly => false;

		#endregion

		#region コンストラクタ

		/// <summary>
		/// 指定したコレクションからコピーした要素を格納した、<see cref="RPGMapCollection{TID}"/>クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="maps">新たなコレクションに追加する<see cref="RPGMap{TID}"/>のコレクション</param>
		public RPGMapCollection(IEnumerable<RPGMap<TID>> maps)
		{
			_items = new List<RPGMap<TID>>(maps);
		}

		/// <summary>
		/// 空で、指定した初期量だけ備えた<see cref="RPGMapCollection{TID}"/>クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="capacity">新しいコレクションに格納できる要素の数</param>
		public RPGMapCollection(int capacity)
		{
			_items = new List<RPGMap<TID>>(capacity);
		}

		/// <summary>
		/// 空で、既定の初期量を備えた、<see cref="RPGMapCollection{TID}"/>クラスの新しいインスタンスを初期化します。
		/// </summary>
		public RPGMapCollection()
		{
			_items = new List<RPGMap<TID>>();
		}

		#endregion

		#region indexer

		/// <summary>
		/// 指定したインデックスの要素を取得、または設定します。
		/// </summary>
		/// <param name="index">取得、または設定する、0から始まるインデックス番号</param>
		/// <returns></returns>
		public RPGMap<TID> this[int index]
		{
			get { return _items[index]; }
			set
			{
				if (_items[index] != value)
				{
					_items[index] = value;
					CollectionChanged?.Invoke(this, new EventArgs());
				}
			}
		}

		#endregion

		#region publicメソッド

		public void Add(RPGMap<TID> item)
		{
			ItemAddingEventArgs<RPGMap<TID>> e = new ItemAddingEventArgs<RPGMap<TID>>(item);

			ItemAdding?.Invoke(this, e);

			if (!e.Cancel)
			{
				_items.Add(item);
				ItemAdded?.Invoke(this, new EventArgs());
				CollectionChanged?.Invoke(this, new EventArgs());
			}
		}

		public void Clear()
		{
			_items.Clear();
			ItemCleared?.Invoke(this, new EventArgs());
			CollectionChanged?.Invoke(this, new EventArgs());
		}
		public bool Contains(RPGMap<TID> item) => _items.Contains(item);
		public void CopyTo(RPGMap<TID>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
		public int IndexOf(RPGMap<TID> item) => _items.IndexOf(item);
		public void Insert(int index, RPGMap<TID> item)
		{
			if (index >= _items.Count || index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRengeException($"{nameof(index)}が境界外です");
			}

			ItemAddingEventArgs<RPGMap<TID>> e = new ItemAddingEventArgs<RPGMap<TID>>(item);
			ItemAdding?.Invoke(this, e);
			if (!e.Cancel)
			{
				_items.Insert(index, item);
				ItemAdded?.Invoke(this, new EventArgs());
				CollectionChanged?.Invoke(this, new EventArgs());
			}
		}
		public bool Remove(RPGMap<TID> item)
		{
			if (_items.Contains(item))
			{
				ItemRemovingEventArgs<RPGMap<TID>> e = new ItemRemovingEventArgs<RPGMap<TID>>(item);

				ItemRemoving?.Invoke(this, e);
				if (!e.Cancel)
				{
					bool res = _items.Remove(item);
					ItemRemoved?.Invoke(this, new EventArgs());
					CollectionChanged?.Invoke(this, new EventArgs());
					return res;
				}
			}
			return false;
		}
		public void RemoveAt(int index)
		{
			if (index >= _items.Count || index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRengeException($"{nameof(index)}が境界外です");
			}

			ItemRemovingEventArgs<RPGMap<TID>> e = new ItemRemovingEventArgs<RPGMap<TID>>(_items[index]);
			ItemRemoving?.Invoke(this, e);
			if (!e.Cancel)
			{
				_items.RemoveAt(index);
				ItemRemoved?.Invoke(this, new EventArgs());
				CollectionChanged?.Invoke(this, new EventArgs());
			}
		}

		public IEnumerator<RPGMap<TID>> GetEnumerator() => _items.GetEnumerator();

		#endregion

		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
	}


	internal delegate void ItemAddingEventHandler<T>(object sender, ItemAddingEventArgs<T> e);
	internal delegate void ItemRemovingEventHandler<T>(object sender, ItemRemovingEventArgs<T> e);

	internal class ItemAddingEventArgs<T> : EventArgs
	{
		/// <summary>
		/// 追加をキャンセルするか
		/// </summary>
		public bool Cancel { get; set; } = false;
		/// <summary>
		/// 追加されるアイテム
		/// </summary>
		public T Item { get; }

		public ItemAddingEventArgs() : base()
		{
		}
		public ItemAddingEventArgs(T item) : base()
		{
			Item = item;
		}
	}
	internal class ItemRemovingEventArgs<T> : EventArgs
	{
		/// <summary>
		/// 削除をキャンセルするか
		/// </summary>
		public bool Cancel { get; set; } = false;
		/// <summary>
		/// 削除されるアイテム
		/// </summary>
		public T Item { get; }

		public ItemRemovingEventArgs() : base()
		{
		}
		public ItemRemovingEventArgs(T item) : base()
		{
			Item = item;
		}
	}
}
