// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole
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

namespace NUnit.Framework.Attributes
{
    public class RangeAttributeTests
    {
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
        public void LongRangeAndStep()
        {
            CheckValues("MethodWithLongRangeAndStep", 11L, 13L, 15L);
        }

        private void MethodWithLongRangeAndStep([Range(11L, 15L, 2)] long x) { }

        [Test]
        public void DoubleRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithDoubleRangeAndStep", 0.7, 0.9, 1.1);
        }

        private void MethodWithDoubleRangeAndStep([Range(0.7, 1.2, 0.2)] double x) { }

        [Test]
        public void FloatRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithFloatRangeAndStep", 0.7f, 0.9f, 1.1f);
        }

        private void MethodWithFloatRangeAndStep([Range(0.7f, 1.2f, 0.2f)] float x) { }

        [Test]
        public void CanConvertIntRangeToShort([Range(1, 3)] short x) { }

        [Test]
        public void CanConvertIntRangeToByte([Range(1, 3)] byte x) { }

        [Test]
        public void CanConvertIntRangeToSByte([Range(1, 3)] sbyte x) { }

        [Test]
        public void CanConvertIntRangeToDecimal([Range(1, 3)] decimal x) { }

        [Test]
        public void CanConvertDoubleRangeToDecimal([Range(1.0, 1.3, 0.1)] decimal x) { }

        #region Helper Methods
        private void CheckValues(string methodName, params object[] expected)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];
            ValuesAttribute attr = param.GetCustomAttributes(typeof(ValuesAttribute), false)[0] as ValuesAttribute;
            Assert.That(attr.GetData(param), Is.EqualTo(expected));
        }

        private void CheckValuesWithinTolerance(string methodName, params object[] expected)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];
            ValuesAttribute attr = param.GetCustomAttributes(typeof(ValuesAttribute), false)[0] as ValuesAttribute;
            Assert.That(attr.GetData(param), Is.EqualTo(expected).Within(0.000001));
        }
        #endregion
    }
}
