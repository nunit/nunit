// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.TestUtilities.Comparers
{
    internal class TestComparer : IComparer
    {
        public int CallCount = 0;

        #region IComparer Members
        public int Compare(object? x, object? y)
        {
            CallCount++;

            if (x is null && y is null)
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return +1;

            if (x.Equals(y))
                return 0;

            return -1;
        }
        #endregion
    }
}
