// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// The supported partition types that can be used when creating a PartitionFilter
    /// </summary>
    file static class PartitionFilterTypes
    {
        public const string Test = "test";
        public const string Fixture = "fixture";
    }

    /// <summary>
    /// PartitionFilter filter matches a subset of tests based upon a chosen partition number and partition count
    ///
    /// This is helpful when you may want to run a subset of tests (eg, across 3 machines - or partitions), each with a separately assigned partition number and fixed partition count
    /// </summary>
    internal abstract class PartitionFilter : TestFilter
    {
        /// <summary>
        /// The matching partition number (between 1 and Partition Count, inclusive) this filter should match on
        /// </summary>
        public uint PartitionNumber { get; private set; }

        /// <summary>
        /// The number of partitions available to use when assigning a matching partition number for each test this filter should match on
        /// </summary>
        public uint PartitionCount { get; private set; }

#if NETFRAMEWORK
        private readonly ThreadLocal<SHA256> _sha256 = new(() => SHA256.Create());
#endif
        private readonly ThreadLocal<byte[]> _buffer = new(() => GC.AllocateUninitializedArray<byte>(4096));
        private readonly ThreadLocal<Encoder> _encoder = new(() => Encoding.UTF8.GetEncoder());

        /// <summary>
        /// Construct a PartitionFilter that matches tests that have the assigned partition number from the total partition count
        /// </summary>
        /// <param name="partitionNumber">The partition number this filter will recognize and match on.</param>
        /// <param name="partitionCount">The total number of partitions that should be configured when assigning each test to a partition number.</param>
        public PartitionFilter(uint partitionNumber, uint partitionCount)
        {
            PartitionNumber = partitionNumber;
            PartitionCount = partitionCount;
        }

        /// <summary>
        /// Create a new PartitionFilter from the provided string value, or return false if the value could not be parsed
        /// </summary>
        /// <param name="value">The partition value (eg, 1/10 to indicate partition 1 of 10)</param>
        /// <param name="partitionFilter">The created PartitionFilter if the parsing succeeded</param>
        /// <returns>True on successful parsing, or False if there is an error</returns>
        public static bool TryCreate(string value, [NotNullWhen(true)] out PartitionFilter? partitionFilter)
        {
            partitionFilter = null;

            // Split our numberWithCount into two parts, such that "1/10" becomes PartitionNumber 1, PartitionCount 10
            string[] parts = value.Split('/', ':');

            // Parts must be in the format of "number/count"
            // There may be an optional partition type after the count, such as "1/10:fixture" or "1/10:test"
            if (parts.Length is 2 or 3)
            {
                // First delimeter must be a '/', so check the character after the first part to ensure it is a '/'
                if (value[parts[0].Length] != '/')
                {
                    return false;
                }

                // First and second parts must be valid unsigned integers, so try to parse and validate them
                if (!uint.TryParse(parts[0], out uint number) || !uint.TryParse(parts[1], out uint count))
                {
                    return false;
                }
                else if (number < 1 || number > count)
                {
                    return false;
                }

                // Basic number/count parsing succeeded, so check if there is an optional partition type specified after the count, and create the appropriate PartitionFilter
                if (parts.Length == 2)
                {
                    partitionFilter = new TestPartitionFilter(number, count);
                    return true;
                }
                else if (parts.Length == 3 && value[parts[0].Length + parts[1].Length + 1] == ':')
                {
                    partitionFilter = CreateFilterInstance(number, count, parts[2]);
                    return partitionFilter is not null;
                }
            }

            // Could not parse partition information
            return false;

            static PartitionFilter? CreateFilterInstance(uint number, uint count, string partitionType)
            {
                if (partitionType.Equals(PartitionFilterTypes.Fixture, StringComparison.OrdinalIgnoreCase))
                {
                    return new FixturePartitionFilter(number, count);
                }
                else if (partitionType.Equals(PartitionFilterTypes.Test, StringComparison.OrdinalIgnoreCase))
                {
                    return new TestPartitionFilter(number, count);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            // Calculate the partition number for the provided Test
            var partitionForTest = ComputePartitionNumber(test);

            // Return a match if the calculated partition number matches our configured Partition Number
            return partitionForTest == PartitionNumber;
        }

        /// <summary>
        /// Adds a PartitionFilter XML node to the provided parentNode.
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            return parentNode.AddElement("partition", GetXmlValue());
        }

        public abstract string GetXmlValue();

        /// <summary>
        /// Computes the Partition Number that has been assigned to the provided ITest value (based upon the configured Partition Count)
        /// </summary>
        /// <param name="value">A partition value between 1 and PartitionCount, inclusive</param>
        /// <returns>A partition value between 1 and PartitionCount, inclusive</returns>
        public uint ComputePartitionNumber(ITest value)
        {
            return ComputeHashValue(value.FullName) % PartitionCount + 1;
        }

        /// <summary>
        /// Computes an unsigned integer hash value based upon the provided string
        /// </summary>
        private uint ComputeHashValue(string name)
        {
#if NETFRAMEWORK
            var buffer = _buffer.Value!;
            _encoder.Value!.Convert(name.ToCharArray(), 0, name.Length, buffer, 0, buffer.Length, flush: true, out _, out var bytesWritten, out _);

            var hashValue = _sha256.Value!.ComputeHash(buffer, 0, bytesWritten);

            return BitConverter.ToUInt32(hashValue, 0);
#else
            Span<byte> buffer = _buffer.Value;
            _encoder.Value!.Convert(name.AsSpan(), buffer, flush: true, out _, out var bytesWritten, out _);

            Span<byte> hashValue = stackalloc byte[32];
            SHA256.HashData(buffer[..bytesWritten], hashValue);

            return BitConverter.ToUInt32(hashValue[..4]);
#endif
        }
    }

    internal sealed class TestPartitionFilter : PartitionFilter
    {
        public TestPartitionFilter(uint partitionNumber, uint partitionCount) : base(partitionNumber, partitionCount)
        {
        }

        public override bool Match(ITest test)
        {
            // Do not match a test Suite, only match individual tests
            if (test.IsSuite)
                return false;

            return base.Match(test);
        }

        public override string GetXmlValue() => $"{PartitionNumber}/{PartitionCount}";
    }

    internal sealed class FixturePartitionFilter : PartitionFilter
    {
        public FixturePartitionFilter(uint partitionNumber, uint partitionCount) : base(partitionNumber, partitionCount)
        {
        }

        public override bool Match(ITest test)
        {
            // Only match TestFixtures, not individual tests
            if (test is not TestFixture)
                return false;

            return base.Match(test);
        }

        public override string GetXmlValue()
            => $"{PartitionNumber}/{PartitionCount}:{PartitionFilterTypes.Fixture}";
    }
}
