using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Object
{
    internal static class ThrowHelper
    {
        internal static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }
        internal static void ThrowArgumentNullException()
        {
            throw new ArgumentNullException();
        }

        internal static void ThrowArgumentException(string message)
        {
            throw new ArgumentException(message);
        }

        internal static void ThrowNullReferenceException(string message)
        {
            throw new NullReferenceException(message);
        }

        internal static void ThrowIdNotFoundException(string message)
        {
            throw new IDNotFoundException(message);
        }

        internal static void ThrowFileNotFoundException(string message)
        {
            throw new System.IO.FileNotFoundException(message);
        }

        internal static void ThrowArgumentOutOfRangeException(string message)
        {
            throw new ArgumentOutOfRangeException(message);
        }

        internal static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }

        internal static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        internal static void ThrowInvalidOperationException(string message, Exception innerException)
        {
            throw new InvalidOperationException(message, innerException);
        }
    }
}
