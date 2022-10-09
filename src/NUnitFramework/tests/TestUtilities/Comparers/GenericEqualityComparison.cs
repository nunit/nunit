﻿// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities.Comparers
{
    internal class GenericEqualityComparison<T>
    {
        public bool WasCalled = false;

        public Func<T, T, bool> Delegate
        {
            get { return new Func<T, T, bool>(Compare); }
        }

        public bool Compare(T x, T y)
        {
            WasCalled = true;
            return x.Equals(y);
        }
    }
}
