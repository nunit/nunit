// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Tests.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectEqualityComparer is used in testing to ensure that only
    /// methods of the IEqualityComparer interface are used.
    /// </summary>
    public class ObjectEqualityComparer : IEqualityComparer
    {
        public bool Called;

        bool IEqualityComparer.Equals(object? x, object? y)
        {
            Called = true;
            return Comparer.Default.Compare(x, y) == 0;
        }

        int IEqualityComparer.GetHashCode(object x)
        {
            return x.GetHashCode();
        }
    }
}
