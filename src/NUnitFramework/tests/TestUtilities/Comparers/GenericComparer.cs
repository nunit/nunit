// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// GenericComparer is used in testing to ensure that only
    /// the <see cref="IComparer{T}"/> interface is used.
    /// </summary>
    public class GenericComparer<T> : IComparer<T>
    {
        public bool WasCalled = false;

        int IComparer<T>.Compare(T x, T y)
        {
            WasCalled = true;
            return Comparer<T>.Default.Compare(x, y);
        }
    }
}
