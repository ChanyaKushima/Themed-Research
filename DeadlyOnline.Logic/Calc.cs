﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DeadlyOnline.Logic
{
	public static class Calc
	{
        private static Regex _correctPlayerID = new Regex(@"^[^\W\s\x00-\x1f<>:""/\\|?*]+$");
        public static bool CanUseAsPlayerID(this string playerID) => _correctPlayerID.IsMatch(playerID);

		public static int GetArrayEmptyTerritory<T>(T[] arr) where T : class
		{
			int i;
			for (i = 0; i < arr.Length; i++)
			{
				if (arr[i] == null) { break; }
			}
			return i;
		}

        /// <summary>
        /// <paramref name="value"/>の値が<paramref name="max"/>より大きければ<paramref name="max"/>,
        /// <paramref name="min"/>より小さければ<paramref name="min"/>,
        /// <paramref name="min"/>~<paramref name="max"/>の範囲内であれば<paramref name="value"/>をそのまま返す
        /// </summary>
        /// <typeparam name="T">引数, 戻り値の型</typeparam>
        /// <param name="value">比較する値</param>
        /// <param name="max">戻り値の最大値</param>
        /// <param name="min">戻り値の最小値</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="min"/>が<paramref name="max"/>より大きい
        /// </exception>
        /// <returns>
        /// <paramref name="min"/>~<paramref name="max"/>の範囲内に丸められた<paramref name="value"/>
        /// </returns>
        public static T FitInRange<T>(T value, T max, T min) where T : IComparable, IComparable<T>
        {
            if (max.CompareTo(min) < 0)
            {
                ThrowHelper.ThrowArgumentException($"{nameof(max)}の値が{nameof(min)}より大きいです");
            }

            if (value.CompareTo(max) > 0)
            {
                value = max;
            }
            else if (value.CompareTo(min) < 0)
            {
                value = min;
            }

            return value;
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

        public static byte[] HashPassword(string password)
        {
            var bytePassword = Encoding.UTF8.GetBytes(password);
            
            byte[] hashedPassword1;
            byte[] hashedPassword2;

            using (var hmac = new HMACSHA512(bytePassword))
            {
                hashedPassword1 = hmac.ComputeHash(bytePassword);
            }
            using (var hmac = new HMACSHA384(hashedPassword1))
            {
                hashedPassword2 = hmac.ComputeHash(bytePassword);
            }

            byte[] result = new byte[hashedPassword1.Length + hashedPassword2.Length];
            Array.Copy(hashedPassword1, 0, result, 0, hashedPassword1.Length);
            Array.Copy(hashedPassword2, 0, result, hashedPassword1.Length, hashedPassword2.Length);

            return result;
        }

        public static bool EqualsAsArray(this byte[] left, byte[] right)
        {
            int leftLen = left.Length;
            if (leftLen != right.Length)
            {
                return false;
            }

            for (int i = 0; i < leftLen; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
