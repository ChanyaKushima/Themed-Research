using System.Collections.Generic;

namespace Games.Object
{ 
	public interface IPlayer : ICharaBase
	{
		int NeedEXP { get; }
		IEnumerable<string> AddEXP(int exp);
	}
}
