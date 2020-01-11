using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeadlyOnline.Server
{
    internal static class InternalMethods
    {
        // https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file に基づく正規表現
        private static readonly Regex _incorrectNaming = new Regex(
            "[\\x00-\\x1f<>:\"/\\\\|?*]|^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9]|CLOCK\\$)(\\.|$)|[\\. ]$",
            RegexOptions.IgnoreCase);

        public static bool CanUseAsFileName(this string name) => !_incorrectNaming.IsMatch(name);

    }
}
