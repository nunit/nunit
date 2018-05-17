// ***********************************************************************
// Copyright (c) 2009-2018 Charlie Poole, Rob Prouse
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
using System.Linq;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    public partial class RangeAttributeTests
    {
        #region Shared specs

        public static IEnumerable<Type> TestedParameterTypes() => new[]
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        // See XML docs for the ParamAttributeTypeConversions class.
        private static readonly Type[] Int32RangeConvertibleToParameterTypes = { typeof(int), typeof(sbyte), typeof(byte), typeof(short), typeof(decimal) };
        private static readonly Type[] UInt32RangeConvertibleToParameterTypes = { typeof(uint) };
        private static readonly Type[] Int64RangeConvertibleToParameterTypes = { typeof(long) };
        private static readonly Type[] UInt64RangeConvertibleToParameterTypes = { typeof(ulong) };
        private static readonly Type[] SingleRangeConvertibleToParameterTypes = { typeof(float) };
        private static readonly Type[] DoubleRangeConvertibleToParameterTypes = { typeof(double), typeof(decimal) };

        #endregion

        [Test]
        public void MultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(typeof(RangeTestFixture), nameof(RangeTestFixture.MethodWithMultipleRanges));

            var arguments = from testCase in test.Tests select testCase.Arguments[0];
            Assert.That(arguments, Is.EquivalentTo(new[] { 1, 2, 3, 10, 11, 12 }));
        }

        #region Forward

        public static IEnumerable<RangeWithExpectedConversions> ForwardRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(11, 15), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11u, 15u), UInt32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11L, 15L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11UL, 15UL), UInt64RangeConvertibleToParameterTypes)
        };

        [Test]
        public static void ForwardRange(
            [ValueSource(nameof(ForwardRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 11, 12, 13, 14, 15 });
        }

        #endregion

        #region Backward

        public static IEnumerable<RangeWithExpectedConversions> BackwardRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(15, 11), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(15L, 11L), Int64RangeConvertibleToParameterTypes)
        };

        [Test]
        public static void BackwardRange(
            [ValueSource(nameof(BackwardRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 15, 14, 13, 12, 11 });
        }

        [Test]
        public static void BackwardRangeDisallowed_UInt32()
        {
            Assert.That(() => new RangeAttribute(15u, 11u), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void BackwardRangeDisallowed_UInt64()
        {
            Assert.That(() => new RangeAttribute(15UL, 11UL), Throws.InstanceOf<ArgumentException>());
        }

        #endregion

        #region Degenerate

        public static IEnumerable<RangeWithExpectedConversions> DegenerateRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(11, 11), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11u, 11u), UInt32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11L, 11L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11UL, 11UL), UInt64RangeConvertibleToParameterTypes)
        };

        [Test]
        public static void DegenerateRange(
            [ValueSource(nameof(DegenerateRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 11 });
        }

        public static IEnumerable<RangeWithExpectedConversions> DegeneratePositiveStepRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(11, 11, 2), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11u, 11u, 2u), UInt32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11L, 11L, 2L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11UL, 11UL, 2UL), UInt64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11f, 11f, 2f), SingleRangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11d, 11d, 2d), DoubleRangeConvertibleToParameterTypes)
        };

        [Test]
        public static void DegeneratePositiveStepRangeDisallowed_Int32(
            [ValueSource(nameof(DegeneratePositiveStepRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 11 });
        }

        public static IEnumerable<RangeWithExpectedConversions> DegenerateNegativeStepRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(11, 11, -2), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11L, 11L, -2L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11f, 11f, -2f), SingleRangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11d, 11d, -2d), DoubleRangeConvertibleToParameterTypes)
        };

        [Test]
        public static void DegenerateNegativeStepRange(
            [ValueSource(nameof(DegenerateNegativeStepRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 11 });
        }

        [Test]
        public static void DegenerateZeroStepRangeDisallowed_Int32()
        {
            Assert.That(() => new RangeAttribute(11, 11, 0), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void DegenerateZeroStepRangeDisallowed_Int64()
        {
            Assert.That(() => new RangeAttribute(11L, 11L, 0L), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void DegenerateZeroStepRangeDisallowed_Single()
        {
            Assert.That(() => new RangeAttribute(11f, 11f, 0f), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void DegenerateZeroStepRangeDisallowed_Double()
        {
            Assert.That(() => new RangeAttribute(11d, 11d, 0d), Throws.InstanceOf<ArgumentException>());
        }

        #endregion

        #region Forward step

        public static IEnumerable<RangeWithExpectedConversions> ForwardStepRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(11, 15, 2), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11u, 15u, 2u), UInt32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11L, 15L, 2L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11UL, 15UL, 2UL), UInt64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11f, 15f, 2f), SingleRangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(11d, 15d, 2d), DoubleRangeConvertibleToParameterTypes)
        };

        [Test]
        public static void ForwardStepRange(
            [ValueSource(nameof(ForwardStepRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 11, 13, 15 });
        }

        #endregion

        #region Backward step

        public static IEnumerable<RangeWithExpectedConversions> BackwardStepRangeCases => new[]
        {
            new RangeWithExpectedConversions(new RangeAttribute(15, 11, -2), Int32RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(15L, 11L, -2L), Int64RangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(15f, 11f, -2f), SingleRangeConvertibleToParameterTypes),
            new RangeWithExpectedConversions(new RangeAttribute(15d, 11d, -2d), DoubleRangeConvertibleToParameterTypes)
        };

        [Test]
        public static void BackwardStepRange(
            [ValueSource(nameof(BackwardStepRangeCases))] RangeWithExpectedConversions rangeWithExpectedConversions,
            [ValueSource(nameof(TestedParameterTypes))] Type parameterType)
        {
            rangeWithExpectedConversions.AssertCoercionErrorOrMatchingSequence(
                parameterType, new[] { 15, 13, 11 });
        }

        #endregion

        #region Zero step

        [Test]
        public static void ZeroStepRangeDisallowed_Int32()
        {
            Assert.That(() => new RangeAttribute(11, 15, 0), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void ZeroStepRangeDisallowed_UInt32()
        {
            Assert.That(() => new RangeAttribute(11u, 15u, 0u), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void ZeroStepRangeDisallowed_Int64()
        {
            Assert.That(() => new RangeAttribute(11L, 15L, 0L), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void ZeroStepRangeDisallowed_UInt64()
        {
            Assert.That(() => new RangeAttribute(11UL, 15UL, 0UL), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void ZeroStepRangeDisallowed_Single()
        {
            Assert.That(() => new RangeAttribute(11f, 15f, 0f), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void ZeroStepRangeDisallowed_Double()
        {
            Assert.That(() => new RangeAttribute(11d, 15d, 0d), Throws.InstanceOf<ArgumentException>());
        }

        #endregion

        #region Opposing step

        [Test]
        public static void OpposingStepForwardRangeDisallowed_Int32()
        {
            Assert.That(() => new RangeAttribute(11, 15, -2), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OpposingStepBackwardRangeDisallowed_Int32()
        {
            Assert.That(() => new RangeAttribute(15, 11, 2), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OppositeStepForwardRangeDisallowed_Int64()
        {
            Assert.That(() => new RangeAttribute(11L, 15L, -2L), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OpposingStepBackwardRangeDisallowed_Int64()
        {
            Assert.That(() => new RangeAttribute(15L, 11L, 2L), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OppositeStepForwardRangeDisallowed_Single()
        {
            Assert.That(() => new RangeAttribute(11f, 15f, -2f), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OpposingStepBackwardRangeDisallowed_Single()
        {
            Assert.That(() => new RangeAttribute(15f, 11f, 2f), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OppositeForwardRangeDisallowed_Double()
        {
            Assert.That(() => new RangeAttribute(11d, 15d, -2d), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public static void OpposingStepBackwardRangeDisallowed_Double()
        {
            Assert.That(() => new RangeAttribute(15d, 11d, 2d), Throws.InstanceOf<ArgumentException>());
        }

        #endregion

        // The smallest distance from MaxValue or MinValue which results in a different number,
        // overcoming loss of precision.
        // Calculated by hand by flipping bits. Used the round-trip format and double-checked.
        private const float SingleExtremaEpsilon = 2.028241E+31f;
        private const double DoubleExtremaEpsilon = 1.99584030953472E+292;
        private const double LargestDoubleConvertibleToDecimal = 7.9228162514264329E+28;
        private const double NextLargestDoubleConvertibleToLowerDecimal = 7.92281625142642E+28;

        #region MaxValue

        [Test]
        public static void MaxValueRange_Int32()
        {
            Assert.That(
                GetData(new RangeAttribute(int.MaxValue - 2, int.MaxValue), typeof(int)),
                Is.EqualTo(new[] { int.MaxValue - 2, int.MaxValue - 1, int.MaxValue }));
        }

        [Test]
        public static void MaxValueRange_UInt32()
        {
            Assert.That(
                GetData(new RangeAttribute(uint.MaxValue - 2, uint.MaxValue), typeof(uint)),
                Is.EqualTo(new[] { uint.MaxValue - 2, uint.MaxValue - 1, uint.MaxValue }));
        }

        [Test]
        public static void MaxValueRange_Int64()
        {
            Assert.That(
                GetData(new RangeAttribute(long.MaxValue - 2, long.MaxValue), typeof(long)),
                Is.EqualTo(new[] { long.MaxValue - 2, long.MaxValue - 1, long.MaxValue }));
        }

        [Test]
        public static void MaxValueRange_UInt64()
        {
            Assert.That(
                GetData(new RangeAttribute(ulong.MaxValue - 2, ulong.MaxValue), typeof(ulong)),
                Is.EqualTo(new[] { ulong.MaxValue - 2, ulong.MaxValue - 1, ulong.MaxValue }));
        }

        [Test]
        public static void MaxValueRange_Single()
        {
            Assert.That(
                GetData(new RangeAttribute(float.MaxValue - SingleExtremaEpsilon * 2, float.MaxValue, SingleExtremaEpsilon), typeof(float)),
                Is.EqualTo(new[] { float.MaxValue - SingleExtremaEpsilon * 2, float.MaxValue - SingleExtremaEpsilon, float.MaxValue }));
        }

        [Test]
        public static void MaxValueRangeLosingPrecision_Single()
        {
            Assert.That(() => GetData(new RangeAttribute(float.MaxValue - SingleExtremaEpsilon, float.MaxValue, SingleExtremaEpsilon / 2), typeof(float)),
                Throws.InstanceOf<ArithmeticException>());
        }

        [Test]
        public static void MaxValueRange_Double()
        {
            Assert.That(
                GetData(new RangeAttribute(double.MaxValue - DoubleExtremaEpsilon * 2, double.MaxValue, DoubleExtremaEpsilon), typeof(double)),
                Is.EqualTo(new[] { double.MaxValue - DoubleExtremaEpsilon * 2, double.MaxValue - DoubleExtremaEpsilon, double.MaxValue }));
        }

        [Test]
        public static void MaxValueRangeLosingPrecision_Double()
        {
            Assert.That(() => GetData(new RangeAttribute(double.MaxValue - DoubleExtremaEpsilon, double.MaxValue, DoubleExtremaEpsilon / 2), typeof(double)),
                Throws.InstanceOf<ArithmeticException>());
        }

        [Test]
        public static void MaxValueRange_Decimal()
        {
            const decimal fromDecimal = (decimal)NextLargestDoubleConvertibleToLowerDecimal;
            const decimal toDecimal = (decimal)LargestDoubleConvertibleToDecimal;
            const double step = (double)((toDecimal - fromDecimal) / 2);

            Assert.That(
                GetData(new RangeAttribute(NextLargestDoubleConvertibleToLowerDecimal, LargestDoubleConvertibleToDecimal, step), typeof(decimal)),
                Is.EqualTo(new[]
                {
                    fromDecimal,
                    fromDecimal + (decimal)step,
                    fromDecimal + (decimal)step * 2
                }));
        }

        [Test]
        public static void MaxValueRangeLosingPrecision_Decimal()
        {
            Assert.That(() => GetData(new RangeAttribute(NextLargestDoubleConvertibleToLowerDecimal, LargestDoubleConvertibleToDecimal, 0.1), typeof(decimal)),
                Throws.InstanceOf<ArithmeticException>());
        }

        #endregion

        #region MinValue

        [Test]
        public static void MinValueRange_Int32()
        {
            Assert.That(
                GetData(new RangeAttribute(int.MinValue + 2, int.MinValue), typeof(int)),
                Is.EqualTo(new[] { int.MinValue + 2, int.MinValue + 1, int.MinValue }));
        }

        [Test]
        public static void MinValueRange_Int64()
        {
            Assert.That(
                GetData(new RangeAttribute(long.MinValue + 2, long.MinValue), typeof(long)),
                Is.EqualTo(new[] { long.MinValue + 2, long.MinValue + 1, long.MinValue }));
        }

        [Test]
        public static void MinValueRange_Single()
        {
            Assert.That(
                GetData(new RangeAttribute(float.MinValue + SingleExtremaEpsilon * 2, float.MinValue, -SingleExtremaEpsilon), typeof(float)),
                Is.EqualTo(new[] { float.MinValue + SingleExtremaEpsilon * 2, float.MinValue + SingleExtremaEpsilon, float.MinValue }));
        }

        [Test]
        public static void MinValueRangeLosingPrecision_Single()
        {
            Assert.That(() => GetData(new RangeAttribute(float.MinValue + SingleExtremaEpsilon, float.MinValue, -SingleExtremaEpsilon / 2), typeof(float)),
                Throws.InstanceOf<ArithmeticException>());
        }

        [Test]
        public static void MinValueRange_Double()
        {
            Assert.That(
                GetData(new RangeAttribute(double.MinValue + DoubleExtremaEpsilon * 2, double.MinValue, -DoubleExtremaEpsilon), typeof(double)),
                Is.EqualTo(new[] { double.MinValue + DoubleExtremaEpsilon * 2, double.MinValue + DoubleExtremaEpsilon, double.MinValue }));
        }

        [Test]
        public static void MinValueRangeLosingPrecision_Double()
        {
            Assert.That(() => GetData(new RangeAttribute(double.MinValue + DoubleExtremaEpsilon, double.MinValue, -DoubleExtremaEpsilon / 2), typeof(double)),
                Throws.InstanceOf<ArithmeticException>());
        }

        [Test]
        public static void MinValueRange_Decimal()
        {
            const decimal fromDecimal = -(decimal)NextLargestDoubleConvertibleToLowerDecimal;
            const decimal toDecimal = -(decimal)LargestDoubleConvertibleToDecimal;
            const double step = (double)((toDecimal - fromDecimal) / 2);

            Assert.That(
                GetData(new RangeAttribute(-NextLargestDoubleConvertibleToLowerDecimal, -LargestDoubleConvertibleToDecimal, step), typeof(decimal)),
                Is.EqualTo(new[]
                {
                    fromDecimal,
                    fromDecimal + (decimal)step,
                    fromDecimal + (decimal)step * 2
                }));
        }

        [Test]
        public static void MinValueRangeLosingPrecision_Decimal()
        {
            Assert.That(() => GetData(new RangeAttribute(-NextLargestDoubleConvertibleToLowerDecimal, -LargestDoubleConvertibleToDecimal, -0.1), typeof(decimal)),
                Throws.InstanceOf<ArithmeticException>());
        }

        #endregion

        private static object[] GetData(RangeAttribute rangeAttribute, Type parameterType)
        {
            return rangeAttribute.GetData(null, StubParameterInfo.OfType(parameterType)).Cast<object>().ToArray();
        }
    }
}
