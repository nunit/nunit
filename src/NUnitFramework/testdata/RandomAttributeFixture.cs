// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.TestData.RandomAttributeTests
{
    public class RandomAttributeFixture
    {
        public const int COUNT = 3;

        #region Int

        [Test]
        public void RandomInt([Random(COUNT)] int x)
        {
            Assert.That(x, Is.InRange(0, int.MaxValue-1));
        }

        [Test]
        public void RandomInt_IntRange([Random(7, 47, COUNT)] int x)
        {
            Assert.That(x, Is.InRange(7, 46));
        }

        #endregion

        #region Unsigned Int

        [Test]
        public void RandomUInt([Random(COUNT)] uint x)
        {
            Assert.That(x, Is.InRange(0u, uint.MaxValue-1));
        }

        [Test]
        public void RandomUInt_UIntRange([Random(7u, 47u, COUNT)] uint x)
        {
            Assert.That(x, Is.InRange(7u, 46u));
        }

        #endregion

        #region Short

        [Test]
        public void RandomShort([Random(COUNT)] short x)
        {
            Assert.That(x, Is.InRange(0, short.MaxValue - 1));
        }

        [Test]
        public void RandomShort_ShortRange([Random((short)7, (short)47, COUNT)] short x)
        {
            Assert.That(x, Is.InRange((short)7, (short)46));
        }

        [Test]
        public void RandomShort_IntRange([Random(7, 47, COUNT)] short x)
        {
            Assert.That(x, Is.InRange((short)7, (short)46));
        }

        #endregion

        #region Unsigned Short

        [Test]
        public void RandomUShort([Random(COUNT)] ushort x)
        {
            Assert.That(x, Is.InRange(0, ushort.MaxValue-1));
        }

        [Test]
        public void RandomUShort_UShortRange([Random((ushort)7, (ushort)47, COUNT)] ushort x)
        {
            Assert.That(x, Is.InRange((ushort)7, (ushort)46));
        }

        [Test]
        public void RandomUShort_IntRange([Random(7, 47, COUNT)] ushort x)
        {
            Assert.That(x, Is.InRange((ushort)7, (ushort)46));
        }

        #endregion

        #region Long

        [Test]
        public void RandomLong([Random(COUNT)] long x)
        {
            Assert.That(x, Is.InRange(0L, long.MaxValue - 1));
        }

        [Test]
        public void RandomLong_LongRange([Random(7L, 47L, COUNT)] long x)
        {
            Assert.That(x, Is.InRange(7L, 46L));
        }

        #endregion

        #region Unsigned Long

        [Test]
        public void RandomULong([Random(COUNT)] ulong x)
        {
            Assert.That(x, Is.InRange(0UL, ulong.MaxValue - 1));
        }

        [Test]
        public void RandomULong_ULongRange([Random(7ul, 47ul, COUNT)] ulong x)
        {
            Assert.That(x, Is.InRange(7ul, 46ul));
        }

        #endregion

        #region Double

        [Test]
        public void RandomDouble([Random(COUNT)] double x)
        {
            Assert.That(x, Is.InRange(0.0d, 1.0d));
        }

        [Test]
        public void RandomDouble_DoubleRange([Random(7.0d, 47.0d, COUNT)] double x)
        {
            Assert.That(x, Is.InRange(7.0d, 47.0d));
        }

        #endregion

        #region Float

        [Test]
        public void RandomFloat([Random(COUNT)] float x)
        {
            Assert.That(x, Is.InRange(0.0f, 1.0f));
        }

        [Test]
        public void RandomFloat_FloatRange([Random(7.0f, 47.0f, COUNT)] float x)
        {
            Assert.That(x, Is.InRange(7.0f, 47.0f));
        }

        #endregion

        #region Enum

        [Test]
        public void RandomEnum([Random(COUNT)] TestEnum x)
        {
            Assert.That(x, Is.EqualTo(TestEnum.A).Or.EqualTo(TestEnum.B).Or.EqualTo(TestEnum.C));
        }

        public enum TestEnum
        {
            A, B, C
        }

        #endregion

        #region Decimal

        [Test]
        public void RandomDecimal([Random(COUNT)] decimal x)
        {
            Assert.That(x, Is.InRange(0M, decimal.MaxValue));
        }

        [Test]
        public void RandomDecimal_IntRange([Random(7, 47, COUNT)] decimal x)
        {
            Assert.That(x, Is.InRange(7M, 46M));
        }

        [Test]
        public void RandomDecimal_DoubleRange([Random(7.0, 47.0, COUNT)] decimal x)
        {
            Assert.That(x, Is.InRange(7M, 47M));
        }

        #endregion

        #region Byte

        [Test]
        public void RandomByte([Random(COUNT)] byte x)
        {
            Assert.That(x, Is.InRange(0, byte.MaxValue - 1));
        }

        [Test]
        public void RandomByte_ByteRange([Random((byte)7, (byte)47, COUNT)] byte x)
        {
            Assert.That(x, Is.InRange((byte)7, (byte)46));
        }

        [Test]
        public void RandomByte_IntRange([Random(7, 47, COUNT)] byte x)
        {
            Assert.That(x, Is.InRange((byte)7, (byte)46));
        }

        #endregion

        #region SByte

        [Test]
        public void RandomSByte([Random(COUNT)] sbyte x)
        {
            Assert.That(x, Is.InRange(0, sbyte.MaxValue - 1));
        }

        [Test]
        public void RandomSByte_SByteRange([Random((sbyte)7, (sbyte)47, COUNT)] sbyte x)
        {
            Assert.That(x, Is.InRange((sbyte)7, (sbyte)46));
        }

        [Test]
        public void RandomSByte_IntRange([Random(7, 47, COUNT)] sbyte x)
        {
            Assert.That(x, Is.InRange((sbyte)7, (sbyte)46));
        }

        #endregion
    }
}
