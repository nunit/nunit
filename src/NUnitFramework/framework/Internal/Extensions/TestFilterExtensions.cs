// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Internal.Extensions
{
    /// <summary>
    /// Provides helpers for inspecting the partition a <see cref="ITestFilter"/> selects.
    /// </summary>
    internal static class TestFilterExtensions
    {
        /// <summary>
        /// Gets the partition number the filter selects tests from, or <see langword="null"/>
        /// if the filter does not identify a single partition.
        /// </summary>
        /// <remarks>
        /// A negated partition selects everything except one partition, so it does not
        /// identify a single one and is ignored.
        /// </remarks>
        /// <param name="filter">The filter to inspect.</param>
        /// <returns>The partition number, or <see langword="null"/> if there isn't a single one.</returns>
        public static uint? GetPartitionNumber(this ITestFilter filter)
        {
            switch (filter)
            {
                case PartitionFilter partitionFilter:
                    return partitionFilter.PartitionNumber;

                case NotFilter:
                    return null;

                case CompositeFilter compositeFilter:
                    return compositeFilter.Filters
                        .Select(GetPartitionNumber)
                        .FirstOrDefault(partitionNumber => partitionNumber is not null);

                default:
                    return null;
            }
        }
    }
}
