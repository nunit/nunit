// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace NUnit.Framework
{
    public class ByteAllocationBenchmark
    {
        private const int BUFFER_SIZE = 4096;

        [Params(4096 * 8)]
        public int Size { get; set; }
        private Stream? XStream { get; set; }
        private Stream? YStream { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var buffer = new byte[BUFFER_SIZE];

            XStream = new MemoryStream(Size);
            for (var i = 0; i < Size; i += BUFFER_SIZE)
                XStream.Write(buffer, 0, BUFFER_SIZE);
            XStream.Seek(0, SeekOrigin.Begin);

            YStream = new MemoryStream(Size);
            for (var i = 0; i < Size; i += BUFFER_SIZE)
                YStream.Write(buffer, 0, BUFFER_SIZE);
            YStream.Seek(0, SeekOrigin.Begin);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            XStream?.Dispose();

            YStream?.Dispose();
        }

        [Benchmark(Baseline = true)]
        public bool NewByteArray()
        {
            var equal = Equal_Original(XStream!, YStream!, out _);
            return equal == EqualMethodResult.ComparedEqual;
        }

        [Benchmark]
        public bool ArrayPool()
        {
            var equal = Equal_ArrayPool(XStream!, YStream!, out _);
            return equal == EqualMethodResult.ComparedEqual;
        }

        [Benchmark]
        public bool StreamLocalPool()
        {
            var equal = Equal_LocalPool(XStream!, YStream!, out _);
            return equal == EqualMethodResult.ComparedEqual;
        }

        public static EqualMethodResult Equal_Original(Stream xStream, Stream yStream, out long? failurePoint)
        {
            failurePoint = null;
            bool bothSeekable = xStream.CanSeek && yStream.CanSeek;

            if (bothSeekable && xStream.Length != yStream.Length)
                return EqualMethodResult.ComparedNotEqual;

            byte[] bufferExpected = new byte[BUFFER_SIZE];
            byte[] bufferActual = new byte[BUFFER_SIZE];

            BinaryReader binaryReaderExpected = new BinaryReader(xStream);
            BinaryReader binaryReaderActual = new BinaryReader(yStream);

            long expectedPosition = bothSeekable ? xStream.Position : default;
            long actualPosition = bothSeekable ? yStream.Position : default;

            try
            {
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

                    if (MemoryExtensions.SequenceEqual<byte>(bufferExpected, bufferActual))
                    {
                        readByte += BUFFER_SIZE;
                        continue;
                    }

                    for (int count = 0; count < BUFFER_SIZE; ++count)
                    {
                        if (bufferExpected[count] != bufferActual[count])
                        {
                            failurePoint = readByte + count;
                            return EqualMethodResult.ComparedNotEqual;
                        }
                    }
                    readByte += BUFFER_SIZE;
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
            }

            return EqualMethodResult.ComparedEqual;
        }

        public static EqualMethodResult Equal_ArrayPool(Stream xStream, Stream yStream, out long? failurePoint)
        {
            failurePoint = null;
            bool bothSeekable = xStream.CanSeek && yStream.CanSeek;

            if (bothSeekable && xStream.Length != yStream.Length)
                return EqualMethodResult.ComparedNotEqual;

            byte[]? bufferExpected = null;
            byte[]? bufferActual = null;

            BinaryReader binaryReaderExpected = new BinaryReader(xStream);
            BinaryReader binaryReaderActual = new BinaryReader(yStream);

            long expectedPosition = bothSeekable ? xStream.Position : default;
            long actualPosition = bothSeekable ? yStream.Position : default;

            try
            {
                bufferExpected = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
                bufferActual = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

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

                    if (MemoryExtensions.SequenceEqual<byte>(bufferExpected.AsSpan(0, readExpected), bufferActual.AsSpan(0, readActual)))
                    {
                        readByte += BUFFER_SIZE;
                        continue;
                    }

                    for (int count = 0; count < BUFFER_SIZE; ++count)
                    {
                        if (bufferExpected[count] != bufferActual[count])
                        {
                            failurePoint = readByte + count;
                            return EqualMethodResult.ComparedNotEqual;
                        }
                    }
                    readByte += BUFFER_SIZE;
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
                    ArrayPool<byte>.Shared.Return(bufferExpected);

                if (bufferActual is not null)
                    ArrayPool<byte>.Shared.Return(bufferActual);
            }

            return EqualMethodResult.ComparedEqual;
        }

        public static EqualMethodResult Equal_LocalPool(Stream xStream, Stream yStream, out long? failurePoint)
        {
            failurePoint = null;
            bool bothSeekable = xStream.CanSeek && yStream.CanSeek;

            if (bothSeekable && xStream.Length != yStream.Length)
                return EqualMethodResult.ComparedNotEqual;

            byte[]? bufferExpected = null;
            byte[]? bufferActual = null;

            BinaryReader binaryReaderExpected = new BinaryReader(xStream);
            BinaryReader binaryReaderActual = new BinaryReader(yStream);

            long expectedPosition = bothSeekable ? xStream.Position : default;
            long actualPosition = bothSeekable ? yStream.Position : default;

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

                    if (MemoryExtensions.SequenceEqual<byte>(bufferExpected.AsSpan(0, readExpected), bufferActual.AsSpan(0, readActual)))
                    {
                        readByte += BUFFER_SIZE;
                        continue;
                    }

                    for (int count = 0; count < BUFFER_SIZE; ++count)
                    {
                        if (bufferExpected[count] != bufferActual[count])
                        {
                            failurePoint = readByte + count;
                            return EqualMethodResult.ComparedNotEqual;
                        }
                    }
                    readByte += BUFFER_SIZE;
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

        private static class LocalPool
        {
            private static readonly List<Buffer> Buffers = [new Buffer(), new Buffer()];

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
                        throw new ArgumentException("Buffer not found in local pool", nameof(data));
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

        /// <summary>
        /// Result of the Equal comparison method.
        /// </summary>
        public enum EqualMethodResult
        {
            /// <summary>
            /// Method does not support the instances being compared.
            /// </summary>
            TypesNotSupported,

            /// <summary>
            /// Method is appropriate for the data types, but doesn't support the specified tolerance.
            /// </summary>
            ToleranceNotSupported,

            /// <summary>
            /// Method is appropriate and the items are considered equal.
            /// </summary>
            ComparedEqual,

            /// <summary>
            /// Method is appropriate and the items are considered different.
            /// </summary>
            ComparedNotEqual,

            /// <summary>
            /// Method is appropriate but the class has cyclic references.
            /// </summary>
            ComparisonPending
        }
    }
}
