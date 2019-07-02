using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Object
{
	internal static class Calc
	{
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
		internal static T FitInRange<T>(T value, in T max, in T min) where T : IComparable, IComparable<T>
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
	}
}
