// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NUnitComparer encapsulates NUnit's default behavior
    /// in comparing two objects.
    /// </summary>
    public sealed class NUnitComparer : IComparer
    {
        /// <summary>
        /// Returns the default NUnitComparer.
        /// </summary>
        public static NUnitComparer Default
        {
            get { return new NUnitComparer(); }
        }

        /// <summary>
        /// Compares two objects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            else if (y == null)
                return +1;

#pragma warning disable CS0618 // 'Numerics' is only marked as obsolete for public use
            if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
                return Numerics.Compare(x, y);
#pragma warning restore CS0618

            Type xType = x.GetType();
            Type yType = y.GetType();

            // If we use BindingFlags.ExactBinding it will prevent us finding CompareTo(object)
            // It however also prevents finding CompareTo(TBase) when called with TDerived
            // Nor will it find CompareTo(int) when called with a short.
            // We fallback to explicitly exclude CompareTo(object)
            bool IsIComparable(MethodInfo method) => method.GetParameters()[0].ParameterType == typeof(object);

            MethodInfo method = xType.GetMethod("CompareTo", new Type[] { yType });
            if (method != null && !IsIComparable(method))
                return (int)method.Invoke(x, new object[] { y });

            method = yType.GetMethod("CompareTo", new Type[] { xType });
            if (method != null && !IsIComparable(method))
                return -(int)method.Invoke(y, new object[] { x });

            if (x is IComparable)
                return ((IComparable)x).CompareTo(y);

            if (y is IComparable)
                return -((IComparable)y).CompareTo(x);

            throw new ArgumentException("Neither value implements IComparable or IComparable<T>");
        }
    }
}
