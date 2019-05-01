using System;

namespace DeadlyOnline.Logic
{
	internal static class ThrowHelper
	{
		internal static void ThrowArgumentNullException(string paramName)
		{
			throw new ArgumentNullException(paramName);
		}

		internal static void ThrowArgumentException(string message)
		{
			throw new ArgumentException(message);
		}

		internal static void ThrowNullReferenceException(string message)
		{
			throw new NullReferenceException(message);
		}

		internal static void ThrowFileNotFoundException(string message)
		{
			throw new System.IO.FileNotFoundException(message);
		}

		internal static void ThrowArgumentOutOfRengeException(string paramName)
		{
			throw new ArgumentOutOfRangeException(paramName);
		}
	}
}
