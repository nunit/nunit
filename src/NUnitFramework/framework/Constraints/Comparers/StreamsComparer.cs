// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Stream"/>s.
    /// </summary>
    internal sealed class StreamsComparer : IChainComparer
    {
        private static readonly int BUFFER_SIZE = 4096;

        private readonly NUnitEqualityComparer _equalityComparer;

        internal StreamsComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
        {
            if (!(x is Stream) || !(y is Stream))
                return null;

            Stream xStream = (Stream)x;
            Stream yStream = (Stream)y;

            if (xStream == yStream) return true;

            if (!xStream.CanRead)
                throw new ArgumentException("Stream is not readable", "expected");
            if (!yStream.CanRead)
                throw new ArgumentException("Stream is not readable", "actual");
            if (!xStream.CanSeek)
                throw new ArgumentException("Stream is not seekable", "expected");
            if (!yStream.CanSeek)
                throw new ArgumentException("Stream is not seekable", "actual");

            if (xStream.Length != yStream.Length) return false;

            byte[] bufferExpected = new byte[BUFFER_SIZE];
            byte[] bufferActual = new byte[BUFFER_SIZE];

            BinaryReader binaryReaderExpected = new BinaryReader(xStream);
            BinaryReader binaryReaderActual = new BinaryReader(yStream);

            long expectedPosition = xStream.Position;
            long actualPosition = yStream.Position;

            try
            {
                binaryReaderExpected.BaseStream.Seek(0, SeekOrigin.Begin);
                binaryReaderActual.BaseStream.Seek(0, SeekOrigin.Begin);

                for (long readByte = 0; readByte < xStream.Length; readByte += BUFFER_SIZE)
                {
                    binaryReaderExpected.Read(bufferExpected, 0, BUFFER_SIZE);
                    binaryReaderActual.Read(bufferActual, 0, BUFFER_SIZE);

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
                            _equalityComparer.FailurePoints.Insert(0, fp);
                            return false;
                        }
                    }
                }
            }
            finally
            {
                xStream.Position = expectedPosition;
                yStream.Position = actualPosition;
            }

            return true;
        }
    }
}
