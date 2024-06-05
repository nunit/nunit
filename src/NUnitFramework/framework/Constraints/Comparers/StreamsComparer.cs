// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;

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
                        readByte += readActual;
                        continue;
                    }

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
    }
}
