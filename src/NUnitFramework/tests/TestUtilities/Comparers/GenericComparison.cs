// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.TestUtilities.Comparers
{
    internal class GenericComparison<T>
    {
        public bool WasCalled = false;

        public Comparison<T> Delegate
        {
            get { return new Comparison<T>(Compare); }
        }

        public int Compare(T x, T y)
        {
            WasCalled = true;
            return Comparer<T>.Default.Compare(x, y);
        }
    }
}
