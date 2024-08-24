// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class ParamAttributeTypeConversionTests
    {
        [Test]
        public static void NullDataArgumentShouldThrowArgumentNullException()
        {
            Assert.That(() => ParamAttributeTypeConversions.ConvertData(null!, typeof(object)), Throws.ArgumentNullException);
        }

        [Test]
        public static void NullTypeArgumentShouldThrowArgumentNullException()
        {
            Assert.That(() => ParamAttributeTypeConversions.ConvertData(Array.Empty<object>(), null!), Throws.ArgumentNullException);
        }

        [TestCase(typeof(short))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(decimal))]
        public void TestInts(Type targetType)
        {
            var data = new object[] { 1, 2, 3 };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }

        [Test]
        public void TestStringToDecimal()
        {
            var data = new object[] { "0.1", "1.0", "-1.0" };
            var expected = new[] { 0.1M, 1.0M, -1.0M };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(decimal));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<decimal>());
        }

        [Test]
        public void TestStringToDateTime()
        {
            var data = new object[] { "1970/01/01", "02/01/1980", "1999/12/31" };
            var expected = new[] { new DateTime(1970, 1, 1), new DateTime(1980, 2, 1), new DateTime(1999, 12, 31) };

            IEnumerable result = ParamAttributeTypeConversions.ConvertData(data, typeof(DateTime));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<DateTime>());
        }

        [Test]
        [TestCase(typeof(Tuple<int>))]
        [TestCase(typeof(ValueTuple<int>))]
        public void TestObjectToOneTuple(Type targetType)
        {
            var data = new object[] { 1, 2, 3 };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }

        [Test]
        [TestCase(typeof(Tuple<int>))]
        [TestCase(typeof(ValueTuple<int>))]
        public void TestArrayOfObjectsToOneTuple(Type targetType)
        {
            var data = new object[] { new object[] { 1 }, new object[] { 2 }, new object[] { 3 } };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }

        [Test]
        [TestCase(typeof(Tuple<int, string>))]
        [TestCase(typeof(ValueTuple<int, string>))]
        public void TestArrayOfObjectsToTwoTuple(Type targetType)
        {
            var data = new object[] { new object[] { 1, "a" }, new object[] { 2, "b" }, new object[] { 3, "c" } };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }

        [Test]
        [TestCase(typeof(Tuple<int, int, int, int, long, long, long, Tuple<string>>))]
        [TestCase(typeof((int, int, int, int, long, long, long, string)))]
        public void TestArrayOfObjectsToEightTuple(Type targetType)
        {
            var data = new object[] { new object[] { 1, 2, 3, 4, 5, 6, 7, "8" } };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }

        [Test]
        [TestCase(typeof(Tuple<int, int, int, int, long, long, long, Tuple<long, int, int, int, int, long, long, Tuple<long, long>>>))]
        [TestCase(typeof((int, int, int, int, long, long, long, long, int, int, int, int, long, long, long, long)))]
        public void TestArrayOfObjectsToTupleWithRestArgument(Type targetType)
        {
            var data = new object[] { new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 } };

            var result = ParamAttributeTypeConversions.ConvertData(data, targetType);

            Assert.That(result, Is.All.InstanceOf(targetType));
        }
    }
}
