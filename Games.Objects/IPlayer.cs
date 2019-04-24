using System.Collections.Generic;

namespace Games.Objects
{
	public interface IPlayer : ICharaBase
	{
		int NeedEXP { get; }
		IEnumerable<string> AddEXP(int exp);
	}
}
