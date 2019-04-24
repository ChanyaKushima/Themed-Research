using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSoft.Audio.Players
{
	internal class InternalMethods
	{
		internal static Uri ResolveUri(string partialUri)
		{
			Uri result = null;

			// try relative to appbase
			try
			{
				result = new Uri(System.IO.Path.GetFullPath(partialUri));
			}
			catch (UriFormatException)
			{
				// eat URI parse exceptions
			}
			if (result == null)
			{
				try
				{
					result = new Uri(partialUri);
				}
				catch (UriFormatException)
				{
					// eat URI parse exceptions
				}
			}
			return result;
		}
	}
}
