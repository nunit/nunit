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
    /// PartitionFilter filter matches a subset of tests based upon a chosen partition number and partition count
    ///
    /// This is helpful when you may want to run a subset of tests (eg, across 3 machines - or partitions), each with a separately assigned partition number and fixed partition count
    /// </summary>
    internal sealed class PartitionFilter : TestFilter
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
        private readonly ThreadLocal<byte[]> _buffer = new(() => new byte[4096]);
#else
        private readonly ThreadLocal<byte[]> _buffer = new(() => GC.AllocateUninitializedArray<byte>(4096));
#endif
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
            // Split our numberWithCount into two parts, such that "1/10" becomes PartitionNumber 1, PartitionCount 10
            string[] parts = value.Split('/');

            // Parts must be exactly 2, and be in the format of "number/count"
            if (parts.Length == 2 && uint.TryParse(parts[0], out uint number) && uint.TryParse(parts[1], out uint count))
            {
                // Number must be between 1 and Count, inclusive
                // Return a new PartitionFilter with the parsed values
                if (number >= 1 && number <= count)
                {
                    partitionFilter = new PartitionFilter(number, count);
                    return true;
                }
            }

            // Could not parse partition information
            partitionFilter = null;
            return false;
        }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            // Do not match a test Suite, only match individual tests
            if (test.IsSuite)
                return false;

            // Calculate the partition number for the provided Test
            var partitionForTest = ComputePartitionNumber(test);

            // Return a match if the calculated partition number matches our configured Partition Number
            return partitionForTest == PartitionNumber;
        }

        /// <summary>
        /// Adds a PartitionFilter XML node to the provided parentNode
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            return parentNode.AddElement("partition", $"{PartitionNumber}/{PartitionCount}");
        }

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
}
