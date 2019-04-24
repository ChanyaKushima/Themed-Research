using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public struct ActionData
    {
		public ActionCommand Command;
		public List<string> Arguments;
		public object Data;
    }
}
