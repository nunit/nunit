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
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

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
            MethodInfo method = GetType().GetTypeInfo().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            ParameterInfo param = method.GetParameters()[0];

            var attr = param.GetAttributes<ValuesAttribute>(false).Single();

            Assert.That(attr.GetData(GetType(), param), Is.EqualTo(expected));
        }

        #endregion

        [Test]
        public void SupportsNullableDecimal([Values(null)] decimal? x)
        {
            Assert.That(x.HasValue, Is.False);
        }

        [Test]
        public void SupportsNullableDateTime([Values(null)] DateTime? dt)
        {
            Assert.That(dt.HasValue, Is.False);
        }

        [Test]
        public void SupportsNullableTimeSpan([Values(null)] TimeSpan? dt)
        {
            Assert.That(dt.HasValue, Is.False);
        }

        [Test]
        public void NullableSimpleFormalParametersWithArgument([Values(1)] int? a)
        {
            Assert.AreEqual(1, a);
        }

        [Test]
        public void NullableSimpleFormalParametersWithNullArgument([Values(null)] int? a)
        {
            Assert.IsNull(a);
        }


        [Test]
        public void MethodWithArrayArguments([Values(
            (object)new object[] { 1, "text", null },
            (object)new object[0],
            (object)new object[] { 1, new int[] { 2, 3 }, 4 },
            (object)new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })] object o)
        {
        }

        [Test]
        public void TestNameIntrospectsArrayValues()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                GetType(), nameof(MethodWithArrayArguments));

            Assert.That(suite.TestCaseCount, Is.EqualTo(4));

            Assert.Multiple(() =>
            {
                Assert.That(suite.Tests[0].Name, Is.EqualTo(@"MethodWithArrayArguments([1, ""text"", null])"));
                Assert.That(suite.Tests[1].Name, Is.EqualTo(@"MethodWithArrayArguments([])"));
                Assert.That(suite.Tests[2].Name, Is.EqualTo(@"MethodWithArrayArguments([1, Int32[], 4])"));
                Assert.That(suite.Tests[3].Name, Is.EqualTo(@"MethodWithArrayArguments([1, 2, 3, 4, 5, ...])"));
            });
        }
    }
}
