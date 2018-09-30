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

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Array"/>s.
    /// </summary>
    internal sealed class ArraysComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;
        private readonly EnumerablesComparer _enumerablesComparer;

        internal ArraysComparer(NUnitEqualityComparer equalityComparer, EnumerablesComparer enumerablesComparer)
        {
            _equalityComparer = equalityComparer;
            _enumerablesComparer = enumerablesComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
        {
            if (!x.GetType().IsArray || !y.GetType().IsArray || _equalityComparer.CompareAsCollection)
                return null;

            Array xArray = (Array)x;
            Array yArray = (Array)y;

            int rank = xArray.Rank;

            if (rank != yArray.Rank)
                return false;

            for (int r = 1; r < rank; r++)
                if (xArray.GetLength(r) != yArray.GetLength(r))
                    return false;

            return _enumerablesComparer.Equal(xArray, yArray, ref tolerance);
        }
    }
}
