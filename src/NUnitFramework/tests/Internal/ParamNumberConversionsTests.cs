using System;
using System.Collections;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class ParamNumberConversionTests
    {
        [TestCase(typeof(short))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(decimal))]
        public void TestInts(Type targetType)
        {
            var data = new object[] { 1, 2, 3 };

            IEnumerable result = ParamNumberConversions.ConvertDataToNumeric(data, targetType);

            CollectionAssert.AllItemsAreInstancesOfType(result, targetType);
        }

        [Test]
        public void TestStringToDecimal()
        {
            var data = new object[] { "0.1", "1.0", "-1.0" };
            var expected = new decimal[] { 0.1M, 1.0M, -1.0M };

            IEnumerable result = ParamNumberConversions.ConvertDataToNumeric(data, typeof(decimal));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestStringToDateTime()
        {
            var data = new object[] { "1970/01/01", "02/01/1980", "1999/12/31" };
            var expected = new DateTime[] { new DateTime(1970, 1, 1), new DateTime(1980, 2, 1), new DateTime(1999, 12, 31) };

            IEnumerable result = ParamNumberConversions.ConvertDataToNumeric(data, typeof(DateTime));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestEmtpyDataToBool()
        {
            var data = new object[] { };
            var expected = new bool[] { true, false };

            IEnumerable result = ParamNumberConversions.ConvertDataToNumeric(data, typeof(bool));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestEmtpyDataToEnum()
        {
            var data = new object[] { };
            var expected = new TestEnum[] { TestEnum.Value1, TestEnum.Value2, TestEnum.value3 };

            IEnumerable result = ParamNumberConversions.ConvertDataToNumeric(data, typeof(TestEnum));

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