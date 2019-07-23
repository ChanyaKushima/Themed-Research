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

        internal static T[][] GetJuggedArrayFrom<T>(T[,] arr)
        {
            if (arr is null) { return null; }

            int len0 = arr.GetLength(0);
            int len1 = arr.GetLength(1);

            T[][] juggedArr;
            if (len0 == 0 || len1 == 0)
            {
                if (len0 == 0)
                {
                    return Array.Empty<T[]>();
                }
                else
                {
                    juggedArr = new T[len0][];
                    T[] emptyArray = Array.Empty<T>();
                    for (int i = 0; i < len0; i++)
                    {
                        juggedArr[i] = emptyArray;
                    }
                }
            }
            else
            {
                juggedArr = new T[len0][];
                for (int i = 0; i < len0; i++)
                {
                    juggedArr[i] = new T[len1];
                    for (int j = 0; j < len1; j++)
                    {
                        juggedArr[i][j] = arr[i, j];
                    }
                }
            }
            return juggedArr;
        }
        internal static T[][] GetJuggedArrayFromParallel<T>(T[,] arr)
        {
            if (arr is null) { return null; }

            int len0 = arr.GetLength(0);
            int len1 = arr.GetLength(1);
            
            T[][] juggedArr;
            
            if (len0 == 0 || len1 == 0)
            {
                if (len0 == 0)
                {
                    return Array.Empty<T[]>();
                }
                else
                {
                    juggedArr = new T[len0][];
                    T[] emptyArray = Array.Empty<T>();
                    for (int i = 0; i < len0; i++)
                    {
                        juggedArr[i] = emptyArray;
                    }
                }
            }
            else
            {
                juggedArr = new T[len0][];
                Parallel.For(0, len0, i =>
                 {
                     juggedArr[i] = new T[len1];
                     for (int j = 0; j < len1; j++)
                     {
                         juggedArr[i][j] = arr[i, j];
                     }
                 });
            }
            return juggedArr;
        }
    }
}
