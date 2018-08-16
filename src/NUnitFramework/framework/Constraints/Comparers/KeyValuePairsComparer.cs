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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="KeyValuePair{TKey, TValue}"/>s.
    /// </summary>
    internal sealed class KeyValuePairsComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal KeyValuePairsComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
        {
            // IDictionary<,> will eventually try to compare its key value pairs when using CollectionTally
            Type xType = x.GetType();
            Type yType = y.GetType();

            Type xGenericTypeDefinition = xType.GetTypeInfo().IsGenericType ? xType.GetGenericTypeDefinition() : null;
            Type yGenericTypeDefinition = yType.GetTypeInfo().IsGenericType ? yType.GetGenericTypeDefinition() : null;

            if (xGenericTypeDefinition != typeof(KeyValuePair<,>) ||
                yGenericTypeDefinition != typeof(KeyValuePair<,>))
                return null;

            var keyTolerance = Tolerance.Exact;
            object xKey = xType.GetProperty("Key").GetValue(x, null);
            object yKey = yType.GetProperty("Key").GetValue(y, null);
            object xValue = xType.GetProperty("Value").GetValue(x, null);
            object yValue = yType.GetProperty("Value").GetValue(y, null);

            return _equalityComparer.AreEqual(xKey, yKey, ref keyTolerance, false) 
                && _equalityComparer.AreEqual(xValue, yValue, ref tolerance, false);
        }
    }
}
