using System;
using System.Collections;
using System.Collections.Generic;

namespace Games.Objects
{
	public abstract class ObjectHub<TID, TTarget> :
		IEnumerable<KeyValuePair<TID, TTarget>>, ILinkable<TID, TTarget>
			where TTarget : IObjectHub<TID, TTarget, ObjectHub<TID, TTarget>>
	{
		#region プロパティ

		internal Dictionary<TID, TTarget> Contents { get; set; } = new Dictionary<TID, TTarget>();

		public int Count => Contents.Count;
		public IEnumerable<TID> IDs => Contents.Keys;
		public IEnumerable<TTarget> Targets => Contents.Values;

		#endregion

		#region コンストラクタ

		public ObjectHub()
		{
		}

		#endregion

		#region 静的メソッド

		public static void LinkEach(TID id, TTarget @this, TTarget target)
		{
			@this.Hub.Link(id, @this, target, false);
		}
		public static void LinkEach(TID id, TTarget @this, TTarget target, bool overwrite)
		{
			@this.Hub.Link(id, @this, target, overwrite);
		}

		#endregion

		#region インデクサ

		public TTarget this[TID id]
		{
			get
			{
				if (id is null)
				{
					ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
				}
				if (Contents.ContainsKey(id))
				{
					ThrowHelper.ThrowIdNotFoundException($"{id}は登録されていません");
				}

				return Contents[id];
			}
		}

		#endregion

		#region メソッド

		public virtual void Link(TID id, TTarget target)
		{
			Link(id, target, false);
		}
		public virtual void Link(TID id, TTarget @this, TTarget target)
		{
			Link(id, @this, target, false);
		}
		public virtual void Link(TID id, TTarget target, bool overwrite)
		{
			#region 引数検証
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			if (target is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(target)}がnullです");
			}
			if (!overwrite && Contents.ContainsKey(id))
			{
				ThrowHelper.ThrowArgumentException("{" + $"{nameof(id)}:{id}" + "}は既に登録されています");
			}
			#endregion

			Contents[id] = target;
		}
		public virtual void Link(TID id, TTarget @this, TTarget target, bool overwrite)
		{
			#region 引数検証
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			if (@this is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(@this)}がnullです");
			}
			if (target is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(target)}がnullです");
			}
			if (@this.Hub != this)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(@this)}は親オブジェクトでなければいけません");
			}
			if (!overwrite && (Contents.ContainsKey(id) || target.Hub.Contents.ContainsKey(id)))
			{
				ThrowHelper.ThrowArgumentException("{" + $"{nameof(id)}:{id}" + "}は既に登録されています");
			}
			#endregion

			Contents[id] = target;
			target.Hub.Contents[id] = @this;
		}

		public TTarget GetTarget(TID id)
		{
			#region 引数検証
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			if (!Contents.ContainsKey(id))
			{
				ThrowHelper.ThrowIdNotFoundException($"{id}は登録されていません");
			}
			#endregion

			return Contents[id];
		}

		public bool TryGetTarget(TID id, out TTarget target)
		{
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			return Contents.TryGetValue(id, out target);
		}

		public bool ContainsID(TID id)
		{
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}

			return Contents.ContainsKey(id);
		}

		public bool ContainsTarget(TTarget target)
		{
			return Contents.ContainsValue(target);
		}

		public void Clear()
		{
			Contents.Clear();
		}

		public bool Remove(TID id)
		{
			if (id is null)
			{
				ThrowHelper.ThrowArgumentNullException($"{nameof(id)}がnullです");
			}
			return Contents.Remove(id);
		}

		public IEnumerator<KeyValuePair<TID, TTarget>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TID, TTarget>>)Contents).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		#endregion
	}
}
