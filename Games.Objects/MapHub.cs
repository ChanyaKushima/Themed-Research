using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Objects
{
	public class MapHub<TID, TData> : ObjectHub<TID, Map<TID, TData>>, ILinkable<TID, Map<TID, TData>>
	{
		public MapHub()
		{

		}
		public MapHub(IMapHub<TID, TData> hub)
		{
			Contents = hub.Hub.Contents;
		}

		public static new void LinkEach(TID id, Map<TID, TData> @this, Map<TID, TData> target)
		{
			@this.Hub.Link(id, @this, target, false);
		}
		public static new void LinkEach(TID id, Map<TID, TData> @this, Map<TID, TData> target, bool overwrite)
		{
			@this.Hub.Link(id, @this, target, overwrite);
		}
	}

	public interface IMapHub<TID, TData> : IObjectHub<TID, Map<TID, TData>, MapHub<TID, TData>>
	{
	}
}
