// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    public class ValuesAttributeTests
    {
        #region ValuesAttribute

        [Test]
        public void ValuesAttributeProvidesSpecifiedValues()
        {
            CheckValues("MethodWithValues", 1, 2, 3);
        }

        private void MethodWithValues([Values(1, 2, 3)] int x)
        {
        }

        #endregion

        #region Conversion Tests

        [Test]
        public void CanConvertIntsToLong([Values(5, int.MaxValue)] long x)
        {
            Assert.That(x, Is.Not.EqualTo(default(long)));
        }

        [Test]
        public void CanConvertIntsToNullableLong([Values(5, int.MaxValue)] long? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertSmallIntsToShort([Values(5)] short x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToNullableShort([Values(5)] short? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertSmallIntsToByte([Values(5)] byte x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToNullableByte([Values(5)] byte? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertSmallIntsToSByte([Values(5)] sbyte x)
        {
        }

        [Test]
        public void CanConvertSmallIntsToNullableSByte([Values(5)] sbyte? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertValuesToDecimal([Values(12, 12.5, "12.5")] decimal x)
        {
            Assert.That(x, Is.Not.EqualTo(default(decimal)));
        }

        [Test]
        public void CanConvertValuesToNullableDecimal([Values(12, 12.5, "12.5")] decimal? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertStringToDateTimeOffset([Values("2018-10-09 15:15:00+02:30")] DateTimeOffset x)
        {
            Assert.That(x, Is.Not.EqualTo(default(DateTimeOffset)));
        }

        [Test]
        public void CanConvertStringToNullableDateTimeOffset([Values("2018-10-09 15:15:00+02:30")] DateTimeOffset? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertStringToTimeSpan([Values("4:44:15")] TimeSpan x)
        {
            Assert.That(x, Is.Not.EqualTo(default(TimeSpan)));
        }

        [Test]
        public void CanConvertStringToNullableTimeSpan([Values("4:44:15")] TimeSpan? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        [Test]
        public void CanConvertStringToDateTime([Values("2018-10-10")] DateTime x)
        {
            Assert.That(x, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void CanConvertStringToNullableDateTime([Values("2018-10-10")] DateTime? x)
        {
            Assert.That(x.HasValue, Is.True);
        }

        #endregion

        #region Helper Methods

        private void CheckValues(string methodName, params object[] expected)
        {
            MethodInfo? method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(method, Is.Not.Null);
            ParameterInfo param = method.GetParameters()[0];

            var attr = param.GetAttributes<ValuesAttribute>(false).Single();

            Assert.That(attr.GetData(new ParameterWrapper(new MethodWrapper(GetType(), method), param)), Is.EqualTo(expected));
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
            Assert.That(a, Is.EqualTo(1));
        }

        [Test]
        public void NullableSimpleFormalParametersWithNullArgument([Values(null)] int? a)
        {
            Assert.That(a, Is.Null);
        }

        [Test]
        public void MethodWithArrayArguments([Values(
            new object?[] { 1, "text", null },
            new object[0],
            new object[] { 1, new[] { 2, 3 }, 4 },
            new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })] object o)
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
