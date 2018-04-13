// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using NUnit.Framework;

namespace NUnit.TestData.RandomAttributeTests
{
    public class RandomAttributeFixture
    {
        public const int COUNT = 3;

        #region Int

        private readonly List<int> previousIntValues = new List<int>();

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

        [Test]
        public void RandomInt_IntRange_Distinct([Random(1, 4, COUNT, Distinct = true)] int x)
        {
            Assert.That(previousIntValues, Does.Not.Contain(x));
            previousIntValues.Add(x);
        }

        #endregion

        #region Unsigned Int

        private readonly List<uint> previousUIntValues = new List<uint>();

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

        [Test]
        public void RandomUInt_UIntRange_Distinct([Random(1u, 4u, COUNT, Distinct = true)] uint x)
        {
            Assert.That(previousUIntValues, Does.Not.Contain(x));
            previousUIntValues.Add(x);
        }

        #endregion

        #region Short

        private readonly List<short> previousShortValues = new List<short>();

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

        [Test]
        public void RandomShort_ShortRange_Distinct([Random((short)1, (short)4, COUNT, Distinct = true)] short x)
        {
            Assert.That(previousShortValues, Does.Not.Contain(x));
            previousShortValues.Add(x);
        }

        #endregion

        #region Unsigned Short

        private readonly List<ushort> previousUShortValues = new List<ushort>();

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

        [Test]
        public void RandomShort_UShortRange_Distinct([Random((ushort)1, (ushort)4, COUNT, Distinct = true)] ushort x)
        {
            Assert.That(previousUShortValues, Does.Not.Contain(x));
            previousUShortValues.Add(x);
        }

        #endregion

        #region Long

        private readonly List<long> previousLongValues = new List<long>();

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

        [Test]
        public void RandomLong_LongRange_Distinct([Random(1L, 4L, COUNT, Distinct = true)] long x)
        {
            Assert.That(previousLongValues, Does.Not.Contain(x));
            previousLongValues.Add(x);
        }

        #endregion

        #region Unsigned Long

        private readonly List<ulong> previousULongValues = new List<ulong>();

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

        [Test]
        public void RandomULong_ULongRange_Distinct([Random(1uL, 4uL, COUNT, Distinct = true)] ulong x)
        {
            Assert.That(previousULongValues, Does.Not.Contain(x));
            previousULongValues.Add(x);
        }

        #endregion

        #region Double

        private readonly List<double> previousDoubleValues = new List<double>();

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

        [Test]
        public void RandomDouble_DoubleRange_Distinct([Random(1.0d, 4.0d, COUNT, Distinct = true)] double x)
        {
            Assert.That(previousDoubleValues, Does.Not.Contain(x));
            previousDoubleValues.Add(x);
        }

        #endregion

        #region Float

        private readonly List<float> previousFloatValues = new List<float>();

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

        [Test]
        public void RandomFloat_FloatRange_Distinct([Random(1.0f, 4.0f, COUNT, Distinct = true)] float x)
        {
            Assert.That(previousFloatValues, Does.Not.Contain(x));
            previousFloatValues.Add(x);
        }

        #endregion

        #region Enum

        private readonly List<TestEnum> previousEnumValues = new List<TestEnum>();

        [Test]
        public void RandomEnum([Random(COUNT)] TestEnum x)
        {
            Assert.That(x, Is.EqualTo(TestEnum.A).Or.EqualTo(TestEnum.B).Or.EqualTo(TestEnum.C));
        }

        [Test]
        public void RandomEnum_EnumRange_Distinct([Random(COUNT, Distinct = true)] TestEnum x)
        {
            Assert.That(previousEnumValues, Does.Not.Contain(x));
            previousEnumValues.Add(x);
        }

        public enum TestEnum
        {
            A, B, C
        }

        #endregion

        #region Decimal

        private readonly List<decimal> previousDecimalValues = new List<decimal>();

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

        [Test]
        public void RandomDecimal_IntRange_Distinct([Random(1, 4, COUNT, Distinct = true)] decimal x)
        {
            Assert.That(previousDecimalValues, Does.Not.Contain(x));
            previousDecimalValues.Add(x);
        }

        #endregion

        #region Byte

        private readonly List<decimal> previousByteValues = new List<decimal>();

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

        [Test]
        public void RandomByte_IntRange_Distinct([Random(1, 4, COUNT, Distinct = true)] byte x)
        {
            Assert.That(previousByteValues, Does.Not.Contain(x));
            previousByteValues.Add(x);
        }

        #endregion

        #region SByte

        private readonly List<decimal> previousSByteValues = new List<decimal>();

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

        [Test]
        public void RandomSByte_IntRange_Distinct([Random(1, 4, COUNT, Distinct = true)] sbyte x)
        {
            Assert.That(previousSByteValues, Does.Not.Contain(x));
            previousSByteValues.Add(x);
        }

        #endregion
    }
}
