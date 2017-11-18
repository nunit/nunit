// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

#if NETSTANDARD1_3 || NETSTANDARD1_6
using System.Linq;
#endif

namespace NUnit.Framework.Attributes
{
    public class RangeAttributeTests
    {
        #region Ints

        [Test]
        public void IntRange()
        {
            CheckValues("MethodWithIntRange", 11, 12, 13, 14, 15);
        }

        private void MethodWithIntRange([Range(11, 15)] int x) { }

        [Test]
        public void IntRange_Reversed()
        {
            CheckValues("MethodWithIntRange_Reversed", 15, 14, 13, 12, 11);
        }

        private void MethodWithIntRange_Reversed([Range(15, 11)] int x) { }

        [Test]
        public void IntRange_FromEqualsTo()
        {
            CheckValues("MethodWithIntRange_FromEqualsTo", 11);
        }

        private void MethodWithIntRange_FromEqualsTo([Range(11, 11)] int x) { }

        [Test]
        public void IntRangeAndStep()
        {
            CheckValues("MethodWithIntRangeAndStep", 11, 13, 15);
        }

        private void MethodWithIntRangeAndStep([Range(11, 15, 2)] int x) { }

        [Test]
        public void IntRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithIntRangeAndZeroStep", 11, 12, 13));
        }

        private void MethodWithIntRangeAndZeroStep([Range(11, 15, 0)] int x) { }

        [Test]
        public void IntRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithIntRangeAndStep_Reversed", 11, 13, 15));
        }

        private void MethodWithIntRangeAndStep_Reversed([Range(15, 11, 2)] int x) { }

        [Test]
        public void IntRangeAndNegativeStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithIntRangeAndNegativeStep", 11, 13, 15));
        }

        private void MethodWithIntRangeAndNegativeStep([Range(11, 15, -2)] int x) { }

        [Test]
        public void IntRangeAndNegativeStep_Reversed()
        {
            CheckValues("MethodWithIntRangeAndNegativeStep_Reversed", 15, 13, 11);
        }

        private void MethodWithIntRangeAndNegativeStep_Reversed([Range(15, 11, -2)] int x) { }

        [Test]
        public void IntRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleIntRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleIntRange([Range(1, 3)][Range(10, 12)]int x) { }

        #endregion

        #region Unsigned Ints

        [Test]
        public void UintRange()
        {
            CheckValues("MethodWithUintRange", 11u, 12u, 13u, 14u, 15u);
        }

        private void MethodWithUintRange([Range(11u, 15u)] uint x) { }

        [Test]
        public void UintRange_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUintRange_Reversed", 15u, 14u, 13u, 12u, 11u));
        }

        private void MethodWithUintRange_Reversed([Range(15u, 11u)] uint x) { }

        [Test]
        public void UintRange_FromEqualsTo()
        {
            CheckValues("MethodWithUintRange_FromEqualsTo", 11u);
        }

        private void MethodWithUintRange_FromEqualsTo([Range(11u, 11u)] uint x) { }

        [Test]
        public void UintRangeAndStep()
        {
            CheckValues("MethodWithUintRangeAndStep", 11u, 13u, 15u);
        }

        private void MethodWithUintRangeAndStep([Range(11u, 15u, 2u)] uint x) { }

        [Test]
        public void UintRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUintRangeAndZeroStep", 11u, 12u, 13u));
        }

        private void MethodWithUintRangeAndZeroStep([Range(11u, 15u, 0u)] uint x) { }

        [Test]
        public void UintRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUintRangeAndStep_Reversed", 11u, 13u, 15u));
        }

        private void MethodWithUintRangeAndStep_Reversed([Range(15u, 11u, 2u)] uint x) { }

        [Test]
        public void UnsignedIntRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleUnsignedIntRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleUnsignedIntRange([Range(1u, 3u)] [Range(10u, 12u)] uint x) { }

        #endregion

        #region Longs

        [Test]
        public void LongRange()
        {
            CheckValues("MethodWithLongRange", 11L, 12L, 13L, 14L, 15L);
        }

        private void MethodWithLongRange([Range(11L, 15L)] long x) { }

        [Test]
        public void LongRange_Reversed()
        {
            CheckValues("MethodWithLongRange_Reversed", 15L, 14L, 13L, 12L, 11L);
        }

        private void MethodWithLongRange_Reversed([Range(15L, 11L)] long x) { }

        [Test]
        public void LongRange_FromEqualsTo()
        {
            CheckValues("MethodWithLongRange_FromEqualsTo", 11L);
        }

        private void MethodWithLongRange_FromEqualsTo([Range(11L, 11L)] long x) { }

        [Test]
        public void LongRangeAndStep()
        {
            CheckValues("MethodWithLongRangeAndStep", 11L, 13L, 15L);
        }

        private void MethodWithLongRangeAndStep([Range(11L, 15L, 2)] long x) { }

        [Test]
        public void LongRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithLongRangeAndZeroStep", 11L, 12L, 13L));
        }

        private void MethodWithLongRangeAndZeroStep([Range(11L, 15L, 0L)] long x) { }

        [Test]
        public void LongRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithLongRangeAndStep_Reversed", 11L, 13L, 15L));
        }

        private void MethodWithLongRangeAndStep_Reversed([Range(15L, 11L, 2L)] long x) { }

        [Test]
        public void LongRangeAndNegativeStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithLongRangeAndNegativeStep", 11L, 13L, 15L));
        }

        private void MethodWithLongRangeAndNegativeStep([Range(11L, 15L, -2L)] long x) { }

        [Test]
        public void LongRangeAndNegativeStep_Reversed()
        {
            CheckValues("MethodWithLongRangeAndNegativeStep_Reversed", 15L, 13L, 11L);
        }

        private void MethodWithLongRangeAndNegativeStep_Reversed([Range(15L, 11L, -2L)] long x) { }

        [Test]
        public void LongRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleLongRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleLongRange([Range(1L, 3L)] [Range(10L, 12L)] long x) { }

        #endregion

        #region Unsigned Longs

        [Test]
        public void UlongRange()
        {
            CheckValues("MethodWithUlongRange", 11ul, 12ul, 13ul, 14ul, 15ul);
        }

        private void MethodWithUlongRange([Range(11ul, 15ul)] ulong x) { }

        [Test]
        public void UlongRange_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUlongRange_Reversed", 15ul, 14ul, 13ul, 12ul, 11ul));
        }

        private void MethodWithUlongRange_Reversed([Range(15ul, 11ul)] ulong x) { }

        [Test]
        public void UlongRange_FromEqualsTo()
        {
            CheckValues("MethodWithUlongRange_FromEqualsTo", 11ul);
        }

        private void MethodWithUlongRange_FromEqualsTo([Range(11ul, 11ul)] ulong x) { }

        [Test]
        public void UlongRangeAndStep()
        {
            CheckValues("MethodWithUlongRangeAndStep", 11ul, 13ul, 15ul);
        }

        private void MethodWithUlongRangeAndStep([Range(11ul, 15ul, 2ul)] ulong x) { }

        [Test]
        public void UlongRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUlongRangeAndZeroStep", 11ul, 12ul, 13ul));
        }

        private void MethodWithUlongRangeAndZeroStep([Range(11ul, 15ul, 0ul)] ulong x) { }

        [Test]
        public void UlongRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValues("MethodWithUlongRangeAndStep_Reversed", 11ul, 13ul, 15ul));
        }

        private void MethodWithUlongRangeAndStep_Reversed([Range(15ul, 11ul, 2ul)] ulong x) { }

        [Test]
        public void UnsignedLongRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleUnsignedLongRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleUnsignedLongRange([Range(1ul, 3ul)] [Range(10ul, 12ul)] ulong x) { }

        #endregion

        #region Doubles

        [Test]
        public void DoubleRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithDoubleRangeAndStep", 0.7, 0.9, 1.1);
        }

        private void MethodWithDoubleRangeAndStep([Range(0.7, 1.2, 0.2)] double x) { }

        [Test]
        public void DoubleRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithDoubleRangeAndZeroStep", 0.7, 0.9, 1.1));
        }

        private void MethodWithDoubleRangeAndZeroStep([Range(0.7, 1.2, 0.0)] double x) { }

        [Test]
        public void DoubleRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithDoubleRangeAndStep_Reversed", 0.7, 0.9, 1.1));
        }

        private void MethodWithDoubleRangeAndStep_Reversed([Range(1.2, 0.7, 0.2)] double x) { }

        [Test]
        public void DoubleRangeAndNegativeStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithDoubleRangeAndNegativeStep", 0.7, 0.9, 1.1));
        }

        private void MethodWithDoubleRangeAndNegativeStep([Range(0.7, 1.2, -0.2)] double x) { }

        [Test]
        public void DoubleRangeAndNegativeStep_Reversed()
        {
            CheckValuesWithinTolerance("MethodWithDoubleRangeAndNegativeStep_Reversed", 1.2, 1.0, 0.8);
        }

        private void MethodWithDoubleRangeAndNegativeStep_Reversed([Range(1.2, 0.7, -0.2)] double x) { }

        [Test]
        public void DoubleRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleDoubleRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleDoubleRange([Range(1.0, 3.0, 1.0)] [Range(10.0, 12.0, 1.0)] double x) { }

        #endregion

        #region Floats

        [Test]
        public void FloatRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithFloatRangeAndStep", 0.7f, 0.9f, 1.1f);
        }

        private void MethodWithFloatRangeAndStep([Range(0.7f, 1.2f, 0.2f)] float x) { }

        [Test]
        public void FloatRangeAndZeroStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithFloatRangeAndZeroStep", 0.7f, 0.9f, 1.1f));
        }

        private void MethodWithFloatRangeAndZeroStep([Range(0.7f, 1.2f, 0.0f)] float x) { }

        [Test]
        public void FloatRangeAndStep_Reversed()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithFloatRangeAndStep_Reversed", 0.7f, 0.9f, 1.1f));
        }

        private void MethodWithFloatRangeAndStep_Reversed([Range(1.2f, 0.7, 0.2f)] float x) { }

        [Test]
        public void FloatRangeAndNegativeStep()
        {
            Assert.Throws<ArgumentException>(() => CheckValuesWithinTolerance("MethodWithFloatRangeAndNegativeStep", 0.7f, 0.9f, 1.1f));
        }

        private void MethodWithFloatRangeAndNegativeStep([Range(0.7f, 1.2f, -0.2f)] float x) { }

        [Test]
        public void FloatRangeAndNegativeStep_Reversed()
        {
            CheckValuesWithinTolerance("MethodWithFloatRangeAndNegativeStep_Reversed", 1.2f, 1.0f, 0.8f);
        }

        private void MethodWithFloatRangeAndNegativeStep_Reversed([Range(1.2f, 0.7, -0.2f)] float x) { }

        [Test]
        public void FloatRangeWithMultipleAttributes()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(GetType(), "MethodWithMultipleFloatRange");

            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        private void MethodWithMultipleFloatRange([Range(1.0f, 3.0f, 1.0f)] [Range(10.0f, 12.0f, 1.0f)] float x) { }

        #endregion

        #region Conversions

        [Test]
        public void CanConvertIntRangeToShort()
        {
            CheckValues("MethodWithIntRangeAndShortArgument", (short)1, (short)2, (short)3);
        }

        private void MethodWithIntRangeAndShortArgument([Range(1, 3)] short x) { }

        [Test]
        public void CanConvertIntRangeToByte() 
        {
            CheckValues("MethodWithIntRangeAndByteArgument", (byte)1, (byte)2, (byte)3);
        }
        
        private void MethodWithIntRangeAndByteArgument([Range(1, 3)] byte x) { }

        [Test]
        public void CanConvertIntRangeToSByte() 
        {
            CheckValues("MethodWithIntRangeAndSByteArgument", (sbyte)1, (sbyte)2, (sbyte)3);
        }
        
        private void MethodWithIntRangeAndSByteArgument([Range(1, 3)] sbyte x) { }

        [Test]
        public void CanConvertIntRangeToDecimal()
        {
            CheckValues("MethodWithIntRangeAndDecimalArgument", 1M, 2M, 3M);
        }
        
        private void MethodWithIntRangeAndDecimalArgument([Range(1, 3)] decimal x) { }

        [Test]
        public void CanConvertDoubleRangeToDecimal() 
        {
            CheckValues("MethodWithDoubleRangeAndDecimalArgument", 1.0M, 1.1M, 1.2M);
        }

        // Use max of 1.21 rather than 1.3 so rounding won't give an extra value
        private void MethodWithDoubleRangeAndDecimalArgument([Range(1.0, 1.21, 0.1)] decimal x) { }

        #endregion

        #region Helper Methods
        
        private void CheckValues(string methodName, params object[] expected)
        {
            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            var param = method.GetParameters()[0];
#if NETSTANDARD1_3 || NETSTANDARD1_6
            var attr = param.GetCustomAttributes(typeof(RangeAttribute), false).First() as RangeAttribute;
#else
            var attr = param.GetCustomAttributes(typeof(RangeAttribute), false)[0] as RangeAttribute;
#endif
            Assert.That(attr.GetData(new ParameterWrapper(new MethodWrapper(GetType(), method), param)), Is.EqualTo(expected));
        }

        private void CheckValuesWithinTolerance(string methodName, params object[] expected)
        {
            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            var param = method.GetParameters()[0];
#if NETSTANDARD1_3 || NETSTANDARD1_6
            var attr = param.GetCustomAttributes(typeof(RangeAttribute), false).First() as RangeAttribute;
#else
            var attr = param.GetCustomAttributes(typeof(RangeAttribute), false)[0] as RangeAttribute;
#endif
            Assert.That(attr.GetData(new ParameterWrapper(new MethodWrapper(GetType(), method), param)), 
                Is.EqualTo(expected).Within(0.000001));
        }
        
        #endregion
    }
}
