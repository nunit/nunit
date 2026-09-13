// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Linq;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture, NonParallelizable]
    public class StreamsComparerTests
    {
        [Test]
        public void EqualShortStreamsAfterUnequalLongStreams()
        {
            byte[] first = new byte[4096];
            byte[] second = new byte[4096];
            second[100] = 1;
            using (var expected = new MemoryStream(first))
            using (var actual = new MemoryStream(second))
                Assert.That(actual, Is.Not.EqualTo(expected));

            using (var expected = new MemoryStream(new byte[] { 1, 2, 3 }))
            using (var actual = new MemoryStream(new byte[] { 1, 2, 3 }))
                Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 1, 7)]
        [TestCase(3, 1, 2)]
        [TestCase(4096, 7, 4096)]
        [TestCase(4097, 4096, 3)]
        [TestCase(8193, 13, 7)]
        public void EqualStreamsWithDifferentReadSizes(int length, int expectedReadSize, int actualReadSize)
        {
            byte[] bytes = CreateData(length);
            using var expected = new ChunkedReadStream(bytes, expectedReadSize);
            using var actual = new ChunkedReadStream(bytes, actualReadSize);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4096, 4097)]
        [TestCase(4097, 4096)]
        public void DifferentLengthStreamsWithMatchingPrefixes(int expectedLength, int actualLength)
        {
            // Zero padding must be distinguished from the absence of data at EOF.
            using var expected = new ChunkedReadStream(new byte[expectedLength], 7);
            using var actual = new ChunkedReadStream(new byte[actualLength], 13);
            var constraint = Is.EqualTo(expected);

            Assert.That(constraint.ApplyTo(actual).IsSuccess, Is.False);
            Assert.That(constraint.FailurePoints, Has.Count.EqualTo(1));
            NUnitEqualityComparer.FailurePoint failure = constraint.FailurePoints[0];
            Assert.Multiple(() =>
            {
                Assert.That(failure.Position, Is.EqualTo(Math.Min(expectedLength, actualLength)));
                Assert.That(failure.ExpectedHasData, Is.EqualTo(expectedLength > actualLength));
                Assert.That(failure.ActualHasData, Is.EqualTo(actualLength > expectedLength));
            });
        }

        [TestCase(0)]
        [TestCase(8)]
        [TestCase(4096)]
        [TestCase(8192)]
        public void DifferentReadSizesReportTheFirstMismatch(int position)
        {
            byte[] expectedBytes = CreateData(8193);
            byte[] actualBytes = (byte[])expectedBytes.Clone();
            actualBytes[position] ^= 0xff;
            using var expected = new ChunkedReadStream(expectedBytes, 7);
            using var actual = new ChunkedReadStream(actualBytes, 13);
            var constraint = Is.EqualTo(expected);

            Assert.That(constraint.ApplyTo(actual).IsSuccess, Is.False);
            Assert.That(constraint.FailurePoints, Has.Count.EqualTo(1));
            NUnitEqualityComparer.FailurePoint failure = constraint.FailurePoints[0];
            Assert.Multiple(() =>
            {
                Assert.That(failure.Position, Is.EqualTo(position));
                Assert.That(failure.ExpectedValue, Is.EqualTo(expectedBytes[position]));
                Assert.That(failure.ActualValue, Is.EqualTo(actualBytes[position]));
                Assert.That(failure.ExpectedHasData, Is.True);
                Assert.That(failure.ActualHasData, Is.True);
            });
        }

        private static byte[] CreateData(int length) => Enumerable.Range(0, length).Select(value => (byte)(value % 251)).ToArray();

        private sealed class ChunkedReadStream : Stream
        {
            private readonly MemoryStream _inner;
            private readonly int _readSize;

            public ChunkedReadStream(byte[] bytes, int readSize)
            {
                _inner = new MemoryStream(bytes);
                _readSize = readSize;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, Math.Min(count, _readSize));

            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _inner.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
