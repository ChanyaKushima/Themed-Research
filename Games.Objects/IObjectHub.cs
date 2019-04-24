using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Objects
{
	public interface IObjectHub<TID, TTarget> :
		IObjectHub<TID, TTarget, ObjectHub<TID, TTarget>>
			where TTarget : IObjectHub<TID, TTarget>
	{
	}

	public interface IObjectHub<in TID, in TTarget, out THub>
		where TTarget : IObjectHub<TID, TTarget, THub>
	{
		THub Hub { get; }
	}
}
