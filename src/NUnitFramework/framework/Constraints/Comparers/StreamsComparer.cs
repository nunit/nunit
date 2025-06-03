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
            if (x is not Stream xStream || y is not Stream yStream)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            if (xStream == yStream)
                return EqualMethodResult.ComparedEqual;

            if (!xStream.CanRead)
                throw new ArgumentException("Stream is not readable", "expected");
            if (!yStream.CanRead)
                throw new ArgumentException("Stream is not readable", "actual");

            bool bothSeekable = xStream.CanSeek && yStream.CanSeek;

            if (bothSeekable)
            {
                if (xStream.Length != yStream.Length)
                    return EqualMethodResult.ComparedNotEqual;

                if (xStream.Length == 0)
                    return EqualMethodResult.ComparedEqual;
            }

            BinaryReader binaryReaderExpected = new BinaryReader(xStream);
            BinaryReader binaryReaderActual = new BinaryReader(yStream);

            long expectedPosition = bothSeekable ? xStream.Position : default;
            long actualPosition = bothSeekable ? yStream.Position : default;

            byte[]? bufferExpected = null;
            byte[]? bufferActual = null;

            try
            {
                bufferExpected = LocalPool.Rent();
                bufferActual = LocalPool.Rent();

                if (xStream.CanSeek)
                {
                    binaryReaderExpected.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                if (yStream.CanSeek)
                {
                    binaryReaderActual.BaseStream.Seek(0, SeekOrigin.Begin);
                }

                int readExpected = 1;
                int readActual = 1;
                long readByte = 0;

                while (readExpected > 0 && readActual > 0)
                {
                    readExpected = binaryReaderExpected.Read(bufferExpected, 0, BUFFER_SIZE);
                    readActual = binaryReaderActual.Read(bufferActual, 0, BUFFER_SIZE);

#if !NETFRAMEWORK
                    if (bufferExpected.SequenceEqual(bufferActual))
                    {
                        readByte += readActual;
                        continue;
                    }
#endif

                    for (int count = 0; count < BUFFER_SIZE; ++count)
                    {
                        if (bufferExpected[count] != bufferActual[count])
                        {
                            NUnitEqualityComparer.FailurePoint fp = new NUnitEqualityComparer.FailurePoint();
                            fp.Position = readByte + count;
                            fp.ExpectedHasData = true;
                            fp.ExpectedValue = bufferExpected[count];
                            fp.ActualHasData = true;
                            fp.ActualValue = bufferActual[count];
                            equalityComparer.FailurePoints.Insert(0, fp);
                            return EqualMethodResult.ComparedNotEqual;
                        }
                    }

#if NETFRAMEWORK
                    readByte += readActual;
#endif
                }
            }
            finally
            {
                if (xStream.CanSeek)
                {
                    xStream.Position = expectedPosition;
                }
                if (yStream.CanSeek)
                {
                    yStream.Position = actualPosition;
                }

                if (bufferExpected is not null)
                    LocalPool.Return(bufferExpected);

                if (bufferActual is not null)
                    LocalPool.Return(bufferActual);
            }

            return EqualMethodResult.ComparedEqual;
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
