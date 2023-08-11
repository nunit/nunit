// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.TestUtilities.Comparers
{
    internal class GenericComparison<T>
    {
        public bool WasCalled = false;

        public Comparison<T> Delegate => new Comparison<T>(Compare);

        public int Compare(T x, T y)
        {
            WasCalled = true;
            return Comparer<T>.Default.Compare(x, y);
        }
    }
}
