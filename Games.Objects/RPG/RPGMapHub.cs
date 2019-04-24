using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Games.Objects.RPG
{
	public class RPGMapHub<TID> : ObjectHub<TID, RPGMap<TID>>
	{
	}

	public interface IRPGMapHub<TID>: IMap2DHub<TID,Image[]>, IObjectHub<TID, RPGMap<TID>, RPGMapHub<TID>>
	{

	}
}
