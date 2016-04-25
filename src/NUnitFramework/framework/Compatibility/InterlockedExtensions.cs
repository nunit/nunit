using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Threading
	{
	/// <summary>Provides atomic operations for variables that are shared by multiple threads.</summary>
	public static class InterlockedEx
		{
		/// <summary>
		///     Adds a 32-bit signed integer to a 32-bit signed integer and replaces the first integer with the sum, as an
		///     atomic operation.
		/// </summary>
		/// <param name="target">
		///     A variable containing the first value to be added. The sum of the two values is stored in
		///     <paramref name="target" />.
		/// </param>
		/// <param name="value">The value to be added to the integer at <paramref name="target" />.</param>
		/// <returns>The new value stored at <paramref name="target" />.</returns>
		public static Int32 Add (ref Int32 target, Int32 value)
			{
			Int32 i, j = target, n;
			do
				{
				i = j;
				n = unchecked (i + value);
				j = Interlocked.CompareExchange (ref target, n, i);
				}
			while (i != j);
			return n;
			}
		}
	}
