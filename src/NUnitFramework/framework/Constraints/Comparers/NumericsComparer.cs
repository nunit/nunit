// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Numerics"/>s.
    /// </summary>
    internal static class NumericsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!Numerics.IsNumericType(x) || !Numerics.IsNumericType(y))
                return null;

            return Numerics.AreEqual(x, y, ref tolerance);
        }
    }
}
