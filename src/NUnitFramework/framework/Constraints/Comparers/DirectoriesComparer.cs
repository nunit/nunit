// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DirectoryInfo"/>s.
    /// </summary>
    internal sealed class DirectoriesComparer : IChainComparer
    {
        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is DirectoryInfo xDirectoryInfo) || !(y is DirectoryInfo yDirectoryInfo))
                return null;

            // Do quick compares first
            if (xDirectoryInfo.Attributes != yDirectoryInfo.Attributes ||
                xDirectoryInfo.CreationTime != yDirectoryInfo.CreationTime ||
                xDirectoryInfo.LastAccessTime != yDirectoryInfo.LastAccessTime)
            {
                return false;
            }

            // TODO: Find a cleaner way to do this
            return new SamePathConstraint(xDirectoryInfo.FullName).ApplyTo(yDirectoryInfo.FullName).IsSuccess;
        }
    }
}
