using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Object
{
	public class Map2DHub<THubID, TImages> : ObjectHub<THubID, Map2D<THubID, TImages>>, ILinkable<THubID, Map2D<THubID, TImages>>
	{
		public new void Link(THubID id, Map2D<THubID, TImages> target)
		{
			base.Link(id, target, false);
		}

		public new void Link(THubID id, Map2D<THubID, TImages> target, bool overwrite)
		{
			base.Link(id, target, overwrite);
		}

		public static new void LinkEach(THubID id, Map2D<THubID, TImages> @this, Map2D<THubID, TImages> target)
		{
			@this.Hub.Link(id, @this, target, false);
		}
		public static new void LinkEach(THubID id, Map2D<THubID, TImages> @this, Map2D<THubID, TImages> target, bool overwrite)
		{
			@this.Hub.Link(id, @this, target, overwrite);
		}
	}

	public interface IMap2DHub<THubID, TImages> : IObjectHub<THubID, Map2D<THubID, TImages>, Map2DHub<THubID, TImages>>, IMapHub<THubID, MapPiece[,]>
	{
	}
}
