﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public struct ActionData
	{
		public ActionCommand Command { get; set; }
		public IEnumerable<object> Arguments { get; set; }
		public object Data { get; set; }
	}
}
