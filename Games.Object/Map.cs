using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Games.Object
{
	public abstract class Map<TID, TData> : IMapHub<TID, TData>, ILinkable<TID, Map<TID, TData>>
	{
		public TData Data { get; set; }

		private MapHub<TID, TData> _hub;
		public  MapHub<TID, TData> Hub => _hub;

		public Map()
		{
			_hub = new MapHub<TID, TData>();
		}
		public Map(TData data)
		{
			Data = data;
			_hub = new MapHub<TID, TData>();
		}
		protected Map(TData data, bool makeHub)
		{
			Data = data;
			if (makeHub)
			{
				_hub = new MapHub<TID, TData>();
			}
		}

		public void SetData(TData data)
		{
			Data = data;
		}

		public void Link(TID id, Map<TID, TData> target)
		{
			Hub.Link(id, target, false);
		}
		public void Link(TID id, Map<TID, TData> target, bool overwrite)
		{
			Hub.Link(id, target, overwrite);
		}
	}
}
