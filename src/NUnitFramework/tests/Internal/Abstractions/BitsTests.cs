// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Tests.Internal.Abstractions
{
    [TestFixture]
    internal sealed class BitsTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void Bits_WithCapacity_Throws_WhenCapacityIsLessThanOrEqualTo0(int capacity)
        {
            Assert.That(() => BitsFactory.Create(capacity), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase(10)]
        [TestCase(32)]
        public void Bits_WithCapacity_ReturnsInt32Bits_WhenCapacityIsLessThanOrEqualTo32(int capacity)
        {
            var bits = BitsFactory.Create(capacity);
            Assert.That(bits, Is.TypeOf<Int32Bits>());
        }

        [TestCase(33)]
        [TestCase(64)]
        public void Bits_WithCapacity_ReturnsInt64Bits_WhenCapacityIsGreaterThan32AndLessThanOrEqualTo64(int capacity)
        {
            var bits = BitsFactory.Create(capacity);
            Assert.That(bits, Is.TypeOf<Int64Bits>());
        }

        [TestCase(65)]
        [TestCase(100)]
        public void Bits_WithCapacity_ReturnsBitArrayBits_WhenCapacityIsGreaterThan64(int capacity)
        {
            var bits = BitsFactory.Create(capacity);
            Assert.That(bits, Is.TypeOf<BitArrayBits>());
        }

        [TestCase(10)]
        [TestCase(32)]
        [TestCase(50)]
        [TestCase(64)]
        [TestCase(100)]
        [TestCase(128)]
        public void Bits_SetAndGet_ReturnsTrue_WhenBitIsSet(int capacity)
        {
            var bits = BitsFactory.Create(capacity);
            bits.Set(0);
            bits.Set(capacity - 1);
            Assert.That(bits.Get(0), Is.True);
            Assert.That(bits.Get(capacity - 1), Is.True);
            Assert.That(bits.BitsSet, Is.EqualTo(2));
        }
    }
}
