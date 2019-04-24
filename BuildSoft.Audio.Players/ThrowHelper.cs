using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSoft.Audio.Players
{
	internal static class ThrowHelper
	{
		internal static void ThrowArgumentException()
		{
			throw new ArgumentException();
		}
		internal static void ThrowArgumentNullException()
		{
			throw new ArgumentNullException();
		}
	}
}
