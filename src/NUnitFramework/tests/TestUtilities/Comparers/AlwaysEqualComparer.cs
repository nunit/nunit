﻿// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities.Comparers
{
    internal class AlwaysEqualComparer : IComparer
    {
        public int CallCount = 0;

        int IComparer.Compare(object x, object y)
        {
            CallCount++;

            // This comparer ALWAYS returns zero (equal)!
            return 0;
        }
    }
}
