// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DirectoryInfo"/>s.
    /// </summary>
    internal static class DirectoriesComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not DirectoryInfo xDirectoryInfo || y is not DirectoryInfo yDirectoryInfo)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            // Do quick compares first
            if (xDirectoryInfo.Attributes != yDirectoryInfo.Attributes ||
                xDirectoryInfo.CreationTime != yDirectoryInfo.CreationTime ||
                xDirectoryInfo.LastAccessTime != yDirectoryInfo.LastAccessTime)
            {
                return EqualMethodResult.ComparedNotEqual;
            }

            // TODO: Find a cleaner way to do this
            return new SamePathConstraint(xDirectoryInfo.FullName).ApplyTo(yDirectoryInfo.FullName).IsSuccess ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
