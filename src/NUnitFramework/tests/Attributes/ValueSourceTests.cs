// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;
using static NUnit.TestData.ValueSourceFixture;

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

        [Test]
        public void NullValueSourceStillRunsOtherTests()
        {
            ValueSourceFixture fixture = new ValueSourceFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);

            Test? validTests = TestFinder.Find(nameof(ValueSourceFixture.Valid), suite, false)!;
            Assert.That(validTests.Tests, Has.Count.EqualTo(ValidSource.Length));

            ITestResult result = TestBuilder.RunTest(suite);
            ITestResult? validResults = TestFinder.Find(nameof(ValueSourceFixture.Valid), result, false)!;

            Assert.That(validResults.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(validResults.Children.ToArray(), Has.Length.EqualTo(ValidSource.Length));
        }

        [Test]
        public void NullValueSourceReportsFailure()
        {
            ValueSourceFixture fixture = new ValueSourceFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            Test? nullSourceTests = TestFinder.Find(nameof(ValueSourceFixture.UsingNullSources), suite, false)!;

            Assert.That(nullSourceTests.Tests, Has.Count.EqualTo(1));
            Assert.That(nullSourceTests.Tests[0].RunState, Is.EqualTo(RunState.NotRunnable));

            ITestResult result = TestBuilder.RunTest(nullSourceTests);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
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

        [Test]
        public void GenericNullableTest<TValue>(
            [ValueSource(nameof(GenericNullableSource))] TValue? value)
            where TValue : struct, IConvertible
        {
            Assert.That(value, Is.Not.Null);
            int index = value.Value.ToInt32(null);
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(value.Value, Is.EqualTo(index));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(value.Value, Is.InstanceOf(GenericNullableSource[index].GetType()));
        }

        private static readonly object[] GenericNullableSource = [0, 1L, 2UL, 3F, 4D];
    }

    public class ValueSourceMayBeInherited
    {
        protected static IEnumerable<bool> InheritedStaticProperty
        {
            get { yield return true; }
        }
    }
}
