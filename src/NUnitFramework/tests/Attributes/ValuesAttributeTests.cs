// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

#if NETCOREAPP1_1
using System.Linq;
#endif

namespace NUnit.Framework.Attributes
{
    public class ValuesAttributeTests
    {
        #region ValuesAttribute

        [Test]
        public void ValuesAttributeProvidesSpecifiedValues()
        {
            CheckValues("MethodWithValues", 1, 2, 3);
        }

        private void MethodWithValues([Values(1, 2, 3)] int x) { }

        #endregion

        #region Conversion Tests

        [Test]
        public void CanConvertSmallIntsToShort([Values(5)]short x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToByte([Values(5)]byte x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToSByte([Values(5)]sbyte x)
        {
        }

        [Test]
        public void CanConvertIntToDecimal([Values(12)]decimal x)
        {
        }

        [Test]
        public void CanConvertDoubleToDecimal([Values(12.5)]decimal x)
        {
        }

        [Test]
        public void CanConvertStringToDecimal([Values("12.5")]decimal x)
        {
        }

        #endregion

        #region Helper Methods

        private void CheckValues(string methodName, params object[] expected)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];
#if NETCOREAPP1_1
            var attr = param.GetCustomAttributes(typeof(ValuesAttribute), false).First() as ValuesAttribute;
#else
            var attr = param.GetCustomAttributes(typeof(ValuesAttribute), false)[0] as ValuesAttribute;
#endif
            Assert.That(attr.GetData(new ParameterWrapper(new MethodWrapper(GetType(), method), param)), Is.EqualTo(expected));
        }

        #endregion
    }
}
