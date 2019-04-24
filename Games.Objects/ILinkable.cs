using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Objects
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TID">IDの型</typeparam>
	/// <typeparam name="TTarget">リンク対象の型</typeparam>
	public interface ILinkable<in TID, in TTarget>
	{
		void Link(TID id, TTarget target);
		void Link(TID id, TTarget target, bool overwrite);
	}
}
