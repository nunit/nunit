// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class ValueSourceTests : ValueSourceMayBeInherited
    {
        [Test]
        public void ValueSourceCanBeStaticProperty(
            [ValueSource(nameof(StaticProperty))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticProperty"));
        }

        private static IEnumerable StaticProperty
        {
            get
            {
                yield return "StaticProperty";
            }
        }

        [Test]
        public void ValueSourceCanBeInheritedStaticProperty(
            [ValueSource(nameof(InheritedStaticProperty))] bool source)
        {
            Assert.That(source, Is.EqualTo(true));
        }

        [Test]
        public void ValueSourceMayNotBeInstanceProperty()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), nameof(MethodWithValueSourceInstanceProperty));
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        private void MethodWithValueSourceInstanceProperty(
#pragma warning disable NUnit1022 // The specified source is not static
            [ValueSource(nameof(InstanceProperty))] string source)
#pragma warning restore NUnit1022 // The specified source is not static
        {
            Assert.Fail("This is not a valid test case: " + source);
        }

        private IEnumerable InstanceProperty => new object[] { "InstanceProperty" };

        [Test]
        public void ValueSourceCanBeStaticMethod(
            [ValueSource(nameof(StaticMethod))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticMethod"));
        }

        private static IEnumerable StaticMethod()
        {
            return new object[] { "StaticMethod" };
        }

#pragma warning disable NUnit1024 // The source specified by the ValueSource does not return an IEnumerable or a type that implements IEnumerable
        [Test]
        public void ValueSourceCanBeStaticAsyncMethod(
            [ValueSource(nameof(StaticAsyncMethod))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncMethod"));
        }

        [Test]
        public void SourceCanBeStaticAsyncEnumerableMethod(
            [ValueSource(nameof(StaticAsyncEnumerableMethod))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncEnumerableMethod"));
        }

        [Test]
        public void SourceCanBeStaticAsyncEnumerableMethodReturningTask(
            [ValueSource(nameof(StaticAsyncEnumerableMethodReturningTask))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncEnumerableMethodReturningTask"));
        }
#pragma warning restore NUnit1024 // The source specified by the ValueSource does not return an IEnumerable or a type that implements IEnumerable

        private static Task<IEnumerable?> StaticAsyncMethod()
        {
            var result = new object[] { nameof(StaticAsyncMethod) };
            return Task.FromResult((IEnumerable?)result);
        }
        private static IAsyncEnumerable<object> StaticAsyncEnumerableMethod()
        {
            var result = new object[] { nameof(StaticAsyncEnumerableMethod) };
            return result.AsAsyncEnumerable();
        }

        private static Task<IAsyncEnumerable<object>> StaticAsyncEnumerableMethodReturningTask()
        {
            var result = new object[] { nameof(StaticAsyncEnumerableMethodReturningTask) };
            return Task.FromResult(result.AsAsyncEnumerable());
        }

        [Test]
        public void ValueSourceMayNotBeInstanceMethod()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), nameof(MethodWithValueSourceInstanceMethod));
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        private void MethodWithValueSourceInstanceMethod(
#pragma warning disable NUnit1022 // The specified source is not static
            [ValueSource(nameof(InstanceMethod))] string source)
#pragma warning restore NUnit1022 // The specified source is not static
        {
            Assert.Fail("This is not a valid test case: " + source);
        }

        private IEnumerable InstanceMethod()
        {
            return new object[] { "InstanceMethod" };
        }

        [Test]
        public void ValueSourceCanBeStaticField(
            [ValueSource(nameof(StaticField))] string source)
        {
            Assert.That(source, Is.EqualTo("StaticField"));
        }

        internal static object[] StaticField = { "StaticField" };

        [Test]
        public void ValueSourceMayNotBeInstanceField()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), nameof(MethodWithValueSourceInstanceField));
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        private void MethodWithValueSourceInstanceField(
#pragma warning disable NUnit1022 // The specified source is not static
            [ValueSource(nameof(InstanceField))] string source)
#pragma warning restore NUnit1022 // The specified source is not static
        {
            Assert.Fail("This is not a valid test case: " + source);
        }

        internal object[] InstanceField = { "InstanceField" };

        [Test, Sequential]
        public void MultipleArguments(
            [ValueSource(nameof(Numerators))] int n,
            [ValueSource(nameof(Denominators))] int d,
            [ValueSource(nameof(Quotients))] int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        internal static int[] Numerators = new[] { 12, 12, 12 };
        internal static int[] Denominators = new[] { 3, 4, 6 };
        internal static int[] Quotients = new[] { 4, 3, 2 };

        [Test, Sequential]
        public void ValueSourceMayBeInAnotherClass(
            [ValueSource(typeof(DivideDataProvider), nameof(DivideDataProvider.Numerators))] int n,
            [ValueSource(typeof(DivideDataProvider), nameof(DivideDataProvider.Denominators))] int d,
            [ValueSource(typeof(DivideDataProvider), nameof(DivideDataProvider.Quotients))] int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        private class DivideDataProvider
        {
            internal static int[] Numerators = new[] { 12, 12, 12 };
            internal static int[] Denominators = new[] { 3, 4, 6 };
            internal static int[] Quotients = new[] { 4, 3, 2 };
        }

        [Test]
        public void ValueSourceMayBeGeneric(
            [ValueSource(typeof(ValueProvider), nameof(ValueProvider.IntegerProvider))] int val)
        {
            Assert.That(2 * val, Is.EqualTo(val + val));
        }

        public class ValueProvider
        {
            public static IEnumerable<int> IntegerProvider()
            {
                List<int> dataList = new List<int>();

                dataList.Add(1);
                dataList.Add(2);
                dataList.Add(4);
                dataList.Add(8);

                return dataList;
            }

            public static IEnumerable<int>? ForeignNullResultProvider()
            {
                return null;
            }
        }

        private static readonly string? NullSource;

        private static IEnumerable<int>? NullDataSourceProvider()
        {
            return null;
        }

        public static IEnumerable<int>? NullDataSourceProperty => null;

        [Test, Explicit("Null or nonexistent data sources definitions should not prevent other tests from run #1121")]
        public void ValueSourceMayNotBeNull(
            [ValueSource(nameof(NullSource))] string nullSource,
            [ValueSource(nameof(NullDataSourceProvider))] string nullDataSourceProvided,
            [ValueSource(typeof(ValueProvider), nameof(ValueProvider.ForeignNullResultProvider))] string nullDataSourceProvider,
            [ValueSource(typeof(object), sourceName: null)] string typeNotImplementingIEnumerableAndNullSourceName,
            [ValueSource(nameof(NullDataSourceProperty))] int nullDataSourceProperty,
#pragma warning disable NUnit1025 // The ValueSource argument does not specify an existing member
            [ValueSource("SomeNonExistingMemberSource")] int nonExistingMember)
#pragma warning restore NUnit1025 // The ValueSource argument does not specify an existing member
        {
            Assert.Fail();
        }

        [Test]
        public void ValueSourceAttributeShouldThrowInsteadOfReturningNull()
        {
            var method = new MethodWrapper(GetType(), "ValueSourceMayNotBeNull");
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var dataSource = parameter.GetCustomAttributes<IParameterDataSource>(false)[0];
                Assert.Throws<InvalidDataSourceException>(() => dataSource.GetData(parameter));
            }
        }

        [Test]
        public void MethodWithArrayArguments([ValueSource(nameof(ComplexArrayBasedTestInput))] object o)
        {
        }

        private static readonly object[] ComplexArrayBasedTestInput = new[]
        {
            new[] { 1, "text", new object() },
            Array.Empty<object>(),
            new object[] { 1, new[] { 2, 3 }, 4 },
            new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            new object[]
            {
                new byte[,]
                {
                    { 1, 2 },
                    { 2, 3 }
                }
            }
        };

        [Test]
        public void TestNameIntrospectsArrayValues()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                GetType(), nameof(MethodWithArrayArguments));

            Assert.That(suite.TestCaseCount, Is.EqualTo(5));

            Assert.Multiple(() =>
            {
                Assert.That(suite.Tests[0].Name, Is.EqualTo(@"MethodWithArrayArguments([1, ""text"", System.Object])"));
                Assert.That(suite.Tests[1].Name, Is.EqualTo(@"MethodWithArrayArguments([])"));
                Assert.That(suite.Tests[2].Name, Is.EqualTo(@"MethodWithArrayArguments([1, Int32[], 4])"));
                Assert.That(suite.Tests[3].Name, Is.EqualTo(@"MethodWithArrayArguments([1, 2, 3, 4, 5, ...])"));
                Assert.That(suite.Tests[4].Name, Is.EqualTo(@"MethodWithArrayArguments([System.Byte[,]])"));
            });
        }
    }

    public class ValueSourceMayBeInherited
    {
        protected static IEnumerable<bool> InheritedStaticProperty
        {
            get { yield return true; }
        }
    }
}
