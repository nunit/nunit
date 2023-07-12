// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.TestUtilities.Comparers
{
    internal class GenericEqualityComparison<T>
    {
        public bool WasCalled = false;

        public Func<T, T, bool> Delegate => new Func<T, T, bool>(Compare);

        public bool Compare(T? x, T? y)
        {
            WasCalled = true;

            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }
    }
}
