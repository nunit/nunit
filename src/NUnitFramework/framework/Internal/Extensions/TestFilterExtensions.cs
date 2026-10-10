// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Internal.Extensions
{
    /// <summary>
    /// Provides helpers for inspecting the partition a <see cref="TestFilter"/> selects.
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
        /// <exception cref="InvalidOperationException">The filter selects more than one partition.</exception>
        public static uint? GetPartitionNumber(this TestFilter filter)
        {
            switch (filter)
            {
                case PartitionFilter partitionFilter:
                    return partitionFilter.PartitionNumber;

                case NotFilter:
                    return null;

                case CompositeFilter compositeFilter:
                {
                    uint? partitionNumber = null;

                    foreach (TestFilter childFilter in compositeFilter.Filters)
                    {
                        uint? childPartitionNumber = childFilter.GetPartitionNumber();

                        if (childPartitionNumber is null)
                            continue;

                        // Tests cannot be in two partitions at once, so a filter asking for
                        // more than one cannot be turned into a single partition number.
                        if (partitionNumber is not null && partitionNumber != childPartitionNumber)
                        {
                            throw new InvalidOperationException(
                                $"The filter selects more than one partition ({partitionNumber} and {childPartitionNumber}).");
                        }

                        partitionNumber = childPartitionNumber;
                    }

                    return partitionNumber;
                }

                default:
                    return null;
            }
        }
    }
}
