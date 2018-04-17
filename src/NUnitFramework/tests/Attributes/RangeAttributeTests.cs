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

            Assert.That(from testCase in test.Tests select testCase.Arguments[0], Is.EqualTo(new[] { 1, 2, 3, 10, 11, 12 }));
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
    }
}
