#nullable enable
using System;

namespace DeadlyOnline.Logic
{
    internal static class ThrowHelper
    {
        internal static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        internal static void ThrowArgumentException(string? message = null)
        {
            throw new ArgumentException(message);
        }


        internal static void ThrowNullReferenceException(string? message = null)
        {
            throw new NullReferenceException(message);
        }

        internal static void ThrowFileNotFoundException(string? message = null)
        {
            throw new System.IO.FileNotFoundException(message);
        }

        internal static void ThrowArgumentOutOfRengeException(string paramName)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        internal static void ThrowInvalidOperationException(string? message = null)
        {
            throw new InvalidOperationException(message);
        }


    }
}
#nullable restore