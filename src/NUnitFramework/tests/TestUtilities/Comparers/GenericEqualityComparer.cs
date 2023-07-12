// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Tests.TestUtilities.Comparers
{
    /// <summary>
    /// GenericEqualityComparer is used in testing to ensure that only
    /// the <see cref="IEqualityComparer{T}"/> interface is used.
    /// </summary>
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool WasCalled;

        bool IEqualityComparer<T>.Equals(T? x, T? y)
        {
            WasCalled = true;
            return Comparer<T>.Default.Compare(x, y) == 0;
        }

        int IEqualityComparer<T>.GetHashCode(T x)
        {
            return x is null ? 0 : x.GetHashCode();
        }
    }
}
