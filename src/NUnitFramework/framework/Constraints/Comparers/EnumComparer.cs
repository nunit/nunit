// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Enum"/>s.
    /// </summary>
    internal static class EnumComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not Enum xEnum || y is not Enum yEnum)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            return xEnum.Equals(yEnum) ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
