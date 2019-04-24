using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Objects.RPG
{
	public struct FightCommand
	{
		public FightCommand(string name, FightAction command)
		{
			Name = name;
			Action = command;
		}

		public string Name { get; set; }
		public FightAction Action { get; set; }

		public static implicit operator (string, FightAction) (FightCommand fc)
		{
			return (fc.Name, fc.Action);
		}
		public static implicit operator FightCommand((string Name, FightAction Cmd) t)
		{
			return new FightCommand(t.Name, t.Cmd);
		}
	}
}
