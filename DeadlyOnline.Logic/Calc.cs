using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public static class Calc
	{
		public static int GetArrayEmptyTerritory<T>(T[] arr) where T : class
		{
			int i;
			for (i = 0; i < arr.Length; i++)
			{
				if (arr[i] == null) { break; }
			}
			return i;
		}
	}
}
