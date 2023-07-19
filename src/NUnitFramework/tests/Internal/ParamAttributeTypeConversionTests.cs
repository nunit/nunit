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
    }
}
