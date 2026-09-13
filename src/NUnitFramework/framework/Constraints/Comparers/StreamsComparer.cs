// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Stream"/>s.
    /// </summary>
    internal static class StreamsComparer
    {
        private const int BUFFER_SIZE = 4096;

        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not Stream expectedStream || y is not Stream actualStream)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            if (expectedStream == actualStream)
                return EqualMethodResult.ComparedEqual;

            if (!expectedStream.CanRead)
                throw new ArgumentException("Stream is not readable", "expected");
            if (!actualStream.CanRead)
                throw new ArgumentException("Stream is not readable", "actual");

            bool bothSeekable = expectedStream.CanSeek && actualStream.CanSeek;

            if (bothSeekable)
            {
                if (expectedStream.Length != actualStream.Length)
                    return EqualMethodResult.ComparedNotEqual;

                if (expectedStream.Length == 0)
                    return EqualMethodResult.ComparedEqual;
            }

            long expectedPosition = bothSeekable ? expectedStream.Position : default;
            long actualPosition = bothSeekable ? actualStream.Position : default;

            byte[]? bufferExpected = null;
            byte[]? bufferActual = null;

            try
            {
                bufferExpected = LocalPool.Rent();
                bufferActual = LocalPool.Rent();

                if (expectedStream.CanSeek)
                {
                    expectedStream.Seek(0, SeekOrigin.Begin);
                }
                if (actualStream.CanSeek)
                {
                    actualStream.Seek(0, SeekOrigin.Begin);
                }

                int readExpected = 1;
                int readActual = 1;
                long readByte = 0;

                while (readExpected > 0 && readActual > 0)
                {
                    readExpected = ReadBuffer(expectedStream, bufferExpected);
                    readActual = ReadBuffer(actualStream, bufferActual);

#if !NETFRAMEWORK
                    if (readExpected == readActual && bufferExpected.AsSpan(0, readExpected).SequenceEqual(bufferActual.AsSpan(0, readActual)))
                    {
                        readByte += readActual;
                        continue;
                    }
#endif

                    int bytesToCompare = Math.Max(readExpected, readActual);
                    for (int count = 0; count < bytesToCompare; ++count)
                    {
                        if (count >= readExpected || count >= readActual || bufferExpected[count] != bufferActual[count])
                        {
                            var fp = new NUnitEqualityComparer.FailurePoint
                            {
                                Position = readByte + count,
                                ExpectedHasData = count < readExpected,
                                ExpectedValue = count < readExpected ? bufferExpected[count] : null,
                                ActualHasData = count < readActual,
                                ActualValue = count < readActual ? bufferActual[count] : null
                            };
                            equalityComparer.FailurePoints.Insert(0, fp);
                            return EqualMethodResult.ComparedNotEqual;
                        }
                    }

                    readByte += readActual;
                }
            }
            finally
            {
                if (expectedStream.CanSeek)
                {
                    expectedStream.Position = expectedPosition;
                }
                if (actualStream.CanSeek)
                {
                    actualStream.Position = actualPosition;
                }

                if (bufferExpected is not null)
                    LocalPool.Return(bufferExpected);

                if (bufferActual is not null)
                    LocalPool.Return(bufferActual);
            }

            return EqualMethodResult.ComparedEqual;
        }

        private static int ReadBuffer(Stream stream, byte[] buffer)
        {
            int total = 0;
            while (total < buffer.Length)
            {
                int read = stream.Read(buffer, total, buffer.Length - total);
                if (read == 0)
                    break;
                total += read;
            }

            return total;
        }

        internal static class LocalPool
        {
            private static readonly List<Buffer> Buffers = [new Buffer(), new Buffer()];

            internal static int RentedBuffers => Buffers.Count(b => b.Rented);
            internal static int AvailableBuffers => Buffers.Count(b => !b.Rented);

            public static byte[] Rent()
            {
                lock (Buffers)
                {
                    var buffer = Buffers.Find(b => !b.Rented);
                    if (buffer is null)
                    {
                        buffer = new Buffer();
                        Buffers.Add(buffer);
                    }

                    return buffer.Rent();
                }
            }

            public static void Return(byte[] data)
            {
                lock (Buffers)
                {
                    var buffer = Buffers.Find(b => ReferenceEquals(data, b.Data));
                    if (buffer is null)
                    {
                        throw new ArgumentException("Buffer not found in pool", nameof(data));
                    }
                    if (!buffer.Rented)
                    {
                        throw new ArgumentException("Buffer not rented out", nameof(data));
                    }

                    buffer.Return();
                }
            }

            private sealed class Buffer
            {
                public static int Where { get; internal set; }
                public byte[] Data { get; } = new byte[BUFFER_SIZE];

                public bool Rented { get; private set; }

                public byte[] Rent()
                {
                    Rented = true;
                    return Data;
                }

                public void Return()
                {
                    Rented = false;
                }
            }
        }
    }
}
