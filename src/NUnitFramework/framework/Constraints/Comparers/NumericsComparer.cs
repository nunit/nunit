// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Numerics"/>s.
    /// </summary>
    internal sealed class NumericsComparer : IChainComparer
    {
        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!Numerics.IsNumericType(x) || !Numerics.IsNumericType(y))
                return null;

            return Numerics.AreEqual(x, y, ref tolerance);
        }
    }
}
