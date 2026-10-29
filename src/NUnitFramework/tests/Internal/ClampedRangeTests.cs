// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System;
#else
using System.Numerics;
#endif
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    internal sealed class ClampedRangeTests
    {
        [TestCaseSource(nameof(InRangeValuesAbsolute))]
        public void RangeIsWithinLimitsAbsolute<T>(T value, T tolerance, T lowerBound, T upperBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForAbsolute(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.False);
                Assert.That(range.LowerBound, Is.EqualTo(lowerBound));
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.False);
                Assert.That(range.UpperBound, Is.EqualTo(upperBound));
            }
        }

        private static readonly TestCaseData[] InRangeValuesAbsolute =
        [
            new TestCaseData(-10, 2, -12, -8),
            new TestCaseData(10U, 2U, 8U, 12U),
            new TestCaseData(-10L, 2L, -12L, -8L),
            new TestCaseData(10UL, 2UL, 8UL, 12UL),
            new TestCaseData((decimal)10, (decimal)2, (decimal)8, (decimal)12),
        ];

        [TestCaseSource(nameof(OutsideLowerTypeLimitsValuesAbsolute))]
        public void RangIsOutsideLowerLimitsAbsolute<T>(T value, T tolerance, T lowerBound, T upperBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForAbsolute(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.True);
                Assert.That(range.LowerBound, Is.EqualTo(lowerBound));
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.False);
                Assert.That(range.UpperBound, Is.EqualTo(upperBound));
            }
        }

        private static readonly TestCaseData[] OutsideLowerTypeLimitsValuesAbsolute =
        [
            new TestCaseData(int.MinValue + 1, 2, int.MinValue, int.MinValue + 3),
            new TestCaseData(uint.MinValue + 1, 2U, uint.MinValue, uint.MinValue + 3),
            new TestCaseData(long.MinValue + 1, 2L, long.MinValue, long.MinValue + 3),
            new TestCaseData(ulong.MinValue + 1, 2UL, ulong.MinValue, ulong.MinValue + 3),
            new TestCaseData(decimal.MinValue + 1, (decimal)2, decimal.MinValue, decimal.MinValue + 3),
        ];

        [TestCaseSource(nameof(OutsideUpperTypeLimitsValuesAbsolute))]
        public void RangIsOutsideUpperLimitsAbsolute<T>(T value, T tolerance, T lowerBound, T upperBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForAbsolute(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.False);
                Assert.That(range.LowerBound, Is.EqualTo(lowerBound));
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.True);
                Assert.That(range.UpperBound, Is.EqualTo(upperBound));
            }
        }

        private static readonly TestCaseData[] OutsideUpperTypeLimitsValuesAbsolute =
        [
            new TestCaseData(int.MaxValue - 1, 2, int.MaxValue - 3, int.MaxValue),
            new TestCaseData(uint.MaxValue - 1, 2U, uint.MaxValue - 3, uint.MaxValue),
            new TestCaseData(long.MaxValue - 1, 2L, long.MaxValue - 3, long.MaxValue),
            new TestCaseData(ulong.MaxValue - 1, 2UL, ulong.MaxValue - 3, ulong.MaxValue),
            new TestCaseData(decimal.MaxValue - 1, (decimal)2, decimal.MaxValue - 3, decimal.MaxValue),
        ];

        [Test]
        public void ValueChecksAbsoluteRangeNormalRange()
        {
            var range = ClampedRange.ForAbsolute(10, 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.ValueIsWithinRange(int.MinValue), Is.False);
                Assert.That(range.ValueIsWithinRange(7), Is.False);
                Assert.That(range.ValueIsWithinRange(8), Is.True);
                Assert.That(range.ValueIsWithinRange(9), Is.True);
                Assert.That(range.ValueIsWithinRange(10), Is.True);
                Assert.That(range.ValueIsWithinRange(11), Is.True);
                Assert.That(range.ValueIsWithinRange(12), Is.True);
                Assert.That(range.ValueIsWithinRange(13), Is.False);
                Assert.That(range.ValueIsWithinRange(int.MaxValue), Is.False);

                // Lower bound is 8
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(7), Is.False);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(8), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(13), Is.True);

                Assert.That(range.ValueIsGreaterThanLowerBound(7), Is.False);
                Assert.That(range.ValueIsGreaterThanLowerBound(8), Is.False);
                Assert.That(range.ValueIsGreaterThanLowerBound(13), Is.True);

                // Upper bound is 12
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(11), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(12), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(13), Is.False);

                Assert.That(range.ValueIsLessThanUpperBound(11), Is.True);
                Assert.That(range.ValueIsLessThanUpperBound(12), Is.False);
                Assert.That(range.ValueIsLessThanUpperBound(13), Is.False);
            }
        }

        [Test]
        public void ValueChecksAbsoluteRangeLowerBoundExceedsTypeLimits()
        {
            var range = ClampedRange.ForAbsolute(1UL, 2UL);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.True);
                Assert.That(range.ValueIsWithinRange(0UL), Is.True);
                Assert.That(range.ValueIsWithinRange(1UL), Is.True);
                Assert.That(range.ValueIsWithinRange(2UL), Is.True);
                Assert.That(range.ValueIsWithinRange(3UL), Is.True);
                Assert.That(range.ValueIsWithinRange(4UL), Is.False);
                Assert.That(range.ValueIsWithinRange(ulong.MaxValue), Is.False);

                // Lower bound is -1, but that is outside the range of ulong, so we can only test values >= 0UL
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(0UL), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(1L), Is.True);

                // Lower bound is -1 so even 0 is less than the lower bound
                Assert.That(range.ValueIsGreaterThanLowerBound(0UL), Is.True);
                Assert.That(range.ValueIsGreaterThanLowerBound(1UL), Is.True);

                // Upper bound is 3
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(2), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(3), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(4), Is.False);

                Assert.That(range.ValueIsLessThanUpperBound(2), Is.True);
                Assert.That(range.ValueIsLessThanUpperBound(3), Is.False);
                Assert.That(range.ValueIsLessThanUpperBound(4), Is.False);
            }
        }

        [Test]
        public void ValueChecksAbsoluteRangeUpperBoundExceedsTypeLimits()
        {
            const int value = int.MaxValue - 1;
            var range = ClampedRange.ForAbsolute(value, 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.True);

                Assert.That(range.ValueIsWithinRange(int.MinValue), Is.False);
                Assert.That(range.ValueIsWithinRange(value - 3), Is.False);
                Assert.That(range.ValueIsWithinRange(value - 2), Is.True);
                Assert.That(range.ValueIsWithinRange(value - 1), Is.True);
                Assert.That(range.ValueIsWithinRange(value), Is.True);
                Assert.That(range.ValueIsWithinRange(value + 1), Is.True);
                Assert.That(range.ValueIsWithinRange(int.MaxValue), Is.True);

                // Lower bound is value - 2
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(value - 3), Is.False);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(value - 2), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(value - 1), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(value), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(value + 1), Is.True);
                Assert.That(range.ValueIsGreaterThanOrEqualToLowerBound(int.MaxValue), Is.True);

                Assert.That(range.ValueIsGreaterThanLowerBound(value - 3), Is.False);
                Assert.That(range.ValueIsGreaterThanLowerBound(value - 2), Is.False);
                Assert.That(range.ValueIsGreaterThanLowerBound(value - 1), Is.True);
                Assert.That(range.ValueIsGreaterThanLowerBound(value), Is.True);
                Assert.That(range.ValueIsGreaterThanLowerBound(value + 1), Is.True);
                Assert.That(range.ValueIsGreaterThanLowerBound(int.MaxValue), Is.True);

                // Upper bound is int.MaxValue, so all values are less than or equal to the upper bound
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(int.MinValue), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(value), Is.True);
                Assert.That(range.ValueIsLessThanOrEqualToUpperBound(int.MaxValue), Is.True);

                Assert.That(range.ValueIsLessThanUpperBound(int.MinValue), Is.True);
                Assert.That(range.ValueIsLessThanUpperBound(value), Is.True);

                // UpperBound is MaxValue + 1, so even MaxValue is less than the upper bound
                Assert.That(range.ValueIsLessThanUpperBound(int.MaxValue), Is.True);
            }
        }

        [TestCaseSource(nameof(InRangeValuesRelative))]
        public void RangeIsWithinLimitsRelative<T>(T value, double tolerance, T lowerBound, T upperBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForRelative(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.False);
                Assert.That(range.LowerBound, Is.EqualTo(lowerBound));
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.False);
                Assert.That(range.UpperBound, Is.EqualTo(upperBound));
            }
        }

        private static readonly TestCaseData[] InRangeValuesRelative =
        [
            new TestCaseData(-10, 0.2, -12, -8),
            new TestCaseData(10U, 0.2, 8U, 12U),
            new TestCaseData(-10L, 0.2, -12L, -8L),
            new TestCaseData(10UL, 0.2, 8UL, 12UL),
            new TestCaseData((decimal)10, 0.2, (decimal)8, (decimal)12),
        ];

        [TestCaseSource(nameof(OutsideLowerTypeLimitsValuesRelative))]
        public void RangIsOutsideLowerLimitsRelative<T>(T value, double tolerance, T lowerBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForRelative(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.True);
                Assert.That(range.LowerBound, Is.EqualTo(lowerBound));
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.False);
            }
        }

        private static readonly TestCaseData[] OutsideLowerTypeLimitsValuesRelative =
        [
            new TestCaseData(int.MinValue, 0.1, int.MinValue),
            new TestCaseData(10U, 2.0, uint.MinValue),
            new TestCaseData(long.MinValue, 0.2, long.MinValue),
            new TestCaseData(10UL, 1.1, ulong.MinValue),
            new TestCaseData(decimal.MinValue, 0.2, decimal.MinValue),
        ];

        [TestCaseSource(nameof(OutsideUpperTypeLimitsValuesRelative))]
        public void RangIsOutsideUpperLimitsRelative<T>(T value, double tolerance, T upperBound)
#if NETFRAMEWORK
            where T : struct, IComparable<T>, IConvertible
#else
            where T : struct, INumber<T>, IMinMaxValue<T>
#endif
        {
            var range = ClampedRange.ForRelative(value, tolerance);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBoundExceedsTypeLimits, Is.False);
                Assert.That(range.UpperBoundExceedsTypeLimits, Is.True);
                Assert.That(range.UpperBound, Is.EqualTo(upperBound));
            }
        }

        private static readonly TestCaseData[] OutsideUpperTypeLimitsValuesRelative =
        [
            new TestCaseData(int.MaxValue - 100, 0.2, int.MaxValue),
            new TestCaseData(uint.MaxValue - 100, 0.2, uint.MaxValue),
            new TestCaseData(long.MaxValue - 100, 0.2, long.MaxValue),
            new TestCaseData(ulong.MaxValue - 100, 0.2, ulong.MaxValue),
            new TestCaseData(decimal.MaxValue - 100, 0.2, decimal.MaxValue),
        ];
    }
}
