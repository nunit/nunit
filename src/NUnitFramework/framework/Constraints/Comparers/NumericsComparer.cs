// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Numerics"/>s.
    /// </summary>
    internal static class NumericsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!Numerics.IsNumericType(x) || !Numerics.IsNumericType(y))
                return EqualMethodResult.TypesNotSupported;

            return Numerics.AreEqual(x, y, ref tolerance) ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
