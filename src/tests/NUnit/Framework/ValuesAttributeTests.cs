// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

namespace NUnit.Framework.Tests
{
    public class ValuesAttributeTests
    {
        [Test]
        public void ValuesAttributeProvidesSpecifiedValues()
        {
            CheckValues("MethodWithValues", 1, 2, 3);
        }

        private void MethodWithValues( [Values(1, 2, 3)] int x) { }

        [Test]
        public void RangeAttributeWithIntRange()
        {
            CheckValues("MethodWithIntRange", 11, 12, 13, 14, 15);
        }

        private void MethodWithIntRange([Range(11, 15)] int x) { }

        [Test]
        public void RangeAttributeWithIntRangeAndStep()
        {
            CheckValues("MethodWithIntRangeAndStep", 11, 13, 15);
        }

        private void MethodWithIntRangeAndStep([Range(11, 15, 2)] int x) { }

        [Test]
        public void RangeAttributeWithLongRangeAndStep()
        {
            CheckValues("MethodWithLongRangeAndStep", 11L, 13L, 15L);
        }

        private void MethodWithLongRangeAndStep([Range(11L, 15L, 2)] long x) { }

        [Test]
        public void RangeAttributeWithDoubleRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithDoubleRangeAndStep", 0.7, 0.9, 1.1);
        }

        private void MethodWithDoubleRangeAndStep([Range(0.7, 1.2, 0.2)] double x) { }

        [Test]
        public void RangeAttributeWithFloatRangeAndStep()
        {
            CheckValuesWithinTolerance("MethodWithFloatRangeAndStep", 0.7f, 0.9f, 1.1f);
        }

        private void MethodWithFloatRangeAndStep([Range(0.7f, 1.2f, 0.2f)] float x) { }

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
