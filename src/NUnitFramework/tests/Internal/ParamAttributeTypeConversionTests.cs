// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Collections;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class ParamAttributeTypeConversionTests
    {
        [Test]
        public static void NullDataArgumentShouldThrowArgumentNullException()
        {
            Assert.That(() => ParamAttributeTypeConversions.ConvertData(null, typeof(object)), Throws.ArgumentNullException);
        }

        [Test]
        public static void NullTypeArgumentShouldThrowArgumentNullException()
        {
            Assert.That(() => ParamAttributeTypeConversions.ConvertData(new object[0], null), Throws.ArgumentNullException);
        }

        [TestCase(typeof(short))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(decimal))]
        public void TestInts(Type targetType)
        {
            var data = new object[] { 1, 2, 3 };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            CollectionAssert.AllItemsAreInstancesOfType(result, targetType);
        }

        [Test]
        public void TestStringToDecimal()
        {
            var data = new object[] { "0.1", "1.0", "-1.0" };
            var expected = new decimal[] { 0.1M, 1.0M, -1.0M };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(decimal));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<decimal>());
        }

        [Test]
        public void TestStringToDateTime()
        {
            var data = new object[] { "1970/01/01", "02/01/1980", "1999/12/31" };
            var expected = new DateTime[] { new DateTime(1970, 1, 1), new DateTime(1980, 2, 1), new DateTime(1999, 12, 31) };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(DateTime));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<DateTime>());
        }

        [Test]
        public void TestEmtpyDataToBool()
        {
            var data = new object[] { };
            var expected = new bool[] { true, false };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(bool));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestEmtpyDataToEnum()
        {
            var data = new object[] { };
            var expected = new TestEnum[] { TestEnum.Value1, TestEnum.Value2, TestEnum.value3 };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(TestEnum));

            Assert.That(result, Is.EquivalentTo(expected));
        }
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        value3
    }
}
