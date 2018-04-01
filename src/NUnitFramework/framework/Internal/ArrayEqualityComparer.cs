// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    internal sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        public static ArrayEqualityComparer<T> Default { get; } = new ArrayEqualityComparer<T>();

        private readonly IEqualityComparer<T> elementComparer;

        public ArrayEqualityComparer() : this(null)
        {
        }

        public ArrayEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(T[] x, T[] y)
        {
            if (x == y) return true;
            if (x == null | y == null) return false;
            if (x.Length != y.Length) return false;

            for (var i = 0; i < x.Length; i++)
                if (!elementComparer.Equals(x[i], y[i]))
                    return false;

            return true;
        }

        #region Compare https://github.com/dotnet/coreclr/blob/246409db8b910c8bd6d04b3fb0783995d00d8be8/src/mscorlib/src/System/Array.cs#L679-L697

        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        public int GetHashCode(T[] obj)
        {
            var ret = 0;

            for (var i = (obj.Length >= 8 ? obj.Length - 8 : 0); i < obj.Length; i++)
            {
                ret = CombineHashCodes(ret, elementComparer.GetHashCode(obj[i]));
            }

            return ret;
        }

        #endregion
    }
}
