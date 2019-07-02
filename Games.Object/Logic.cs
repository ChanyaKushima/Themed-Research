using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Games.Object
{
    internal static class Logic
    {
        public static Uri SolveUri(string path)
        {
            Uri resultUri = null;

            try
            {
                resultUri = new Uri(path);
            }
            catch (UriFormatException)
            {
                // eat UriFormatException
            }

            if (resultUri is null)
            {
                try
                {
                    resultUri = new Uri(Path.GetFileName(path));
                }
                catch (UriFormatException)
                {
                    // eat UriFormatException
                }
            }

            return resultUri;
        }
    }
}
