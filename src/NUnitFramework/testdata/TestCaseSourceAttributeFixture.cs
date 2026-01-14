// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NUnit.TestData.TestCaseSourceAttributeFixture
{
    [TestFixture]
    public class TestCaseSourceAttributeFixture
    {
        #region Test Calling Assert.Ignore

        [TestCaseSource(nameof(Source))]
        public void MethodCallsIgnore(int x, int y, int z)
        {
            Assert.Ignore("Ignore this");
        }

        private static readonly object[] Source = new object[]
        {
            new TestCaseData(2, 3, 4)
        };

        #endregion

        #region Test With Ignored TestCaseData

        [TestCaseSource(nameof(IgnoredSource))]
        [TestCaseSource(nameof(IgnoredWithDateSource))]
        public void MethodWithIgnoredTestCases(int num)
        {
        }

        private static IEnumerable IgnoredSource =>
            new object[]
            {
                new TestCaseData(1),
                new TestCaseData(2).Ignore("Don't Run Me!"),
            };

        private static IEnumerable IgnoredWithDateSource
        {
            get
            {
                DateTimeOffset utcTime = DateTimeOffset.UtcNow;
                TimeSpan timeZoneOffset = utcTime - utcTime.ToLocalTime();
                return new object[]
                {
                    new TestCaseData(3).Ignore("Ignore Me Until The Future").Until(new DateTimeOffset(4242, 01, 01, 0, 0, 0, timeZoneOffset)),
                    new TestCaseData(4).Ignore("I Was Ignored in the Past").Until(new DateTimeOffset(1492, 01, 01, 0, 0, 0, timeZoneOffset)),
                    new TestCaseData(5).Ignore("Ignore Me Until The Future").Until(new DateTimeOffset(4242, 01, 01, 12, 42, 33, timeZoneOffset)),
                };
            }
        }

        #endregion

        #region Test With Explicit TestCaseData

        [TestCaseSource(nameof(ExplicitSource))]
        public void MethodWithExplicitTestCases(int num)
        {
        }

        private static IEnumerable ExplicitSource =>
            new object[]
            {
                new TestCaseData(1),
                new TestCaseData(2).Explicit(),
                new TestCaseData(3).Explicit("Connection failing")
            };

        #endregion

        #region Tests Using Instance Members as Source

        [Test, TestCaseSource(nameof(InstanceProperty))]
        public void MethodWithInstancePropertyAsSource(string source)
        {
            Assert.That(source, Is.EqualTo(nameof(InstanceProperty)));
        }

        private IEnumerable InstanceProperty =>
            new object[]
            {
                new object[] { nameof(InstanceProperty) }
            };

        [Test, TestCaseSource(nameof(InstanceMethod))]
        public void MethodWithInstanceMethodAsSource(string source)
        {
            Assert.That(source, Is.EqualTo(nameof(InstanceMethod)));
        }

        private IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { nameof(InstanceMethod) } };
        }

        [Test, TestCaseSource(nameof(InstanceField))]
        public void MethodWithInstanceFieldAsSource(string source)
        {
            Assert.That(source, Is.EqualTo(nameof(InstanceField)));
        }

#pragma warning disable IDE1006 // Naming Styles
        private readonly object[] InstanceField = { new object[] { nameof(InstanceField) } };
#pragma warning restore IDE1006 // Naming Styles

        #endregion

        [Test, TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.MyField), new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingParamsToField(int n, int d, int q)
        {
        }

        [Test, TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.MyProperty), new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingParamsToProperty(int n, int d, int q)
        {
        }

        [Test, TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.HereIsTheDataWithParameters), new object[] { 100, 4 })]
        public void SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam(int n, int d, int q)
        {
        }

        [TestCaseSource(nameof(ExceptionSource))]
        public void MethodWithSourceThrowingException(string lhs, string rhs)
        {
        }

        [TestCaseSource("NonExistingSource")]
        public void MethodWithNonExistingSource(object param)
        {
        }

        [TestCaseSource(nameof(ComplexArrayBasedTestInputTestCases))]
        public void MethodWithArrayArguments(object o)
        {
        }

        [TestCaseSource(nameof(IncompatibleGenericTypeAndArgumentTestCases))]
        public static void MethodWithIncompatibleGenericTypeAndArgument<T>(T o)
        {
        }

        private static IEnumerable ExceptionSource
        {
            get
            {
                yield return new TestCaseData("a", "a");
                yield return new TestCaseData("b", "b");

                throw new System.Exception("my message");
            }
        }

        private class DivideDataProvider
        {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
            public static string? MyField;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
            public static int MyProperty { get; set; }
            public static IEnumerable HereIsTheDataWithParameters(int inject1, int inject2, int inject3)
            {
                yield return new object[] { inject1, inject2, inject3 };
            }
            public static IEnumerable HereIsTheData
            {
                get
                {
                    yield return new object[] { 100, 20, 5 };
                    yield return new object[] { 100, 4, 25 };
                }
            }
        }

        private static readonly object[] ComplexArrayBasedTestInput = new[]
        {
#pragma warning disable SA1500 // Braces for multi-line statements should not share line
            new[] { 1, "text", new object() },
            Array.Empty<object>(),
            new object[] { 1, new[] { 2, 3 }, 4 },
            new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            new object[] { new byte[,] { { 1, 2 }, { 2, 3 } } }
#pragma warning restore SA1500 // Braces for multi-line statements should not share line
        };

        private static IEnumerable<TestCaseData> ComplexArrayBasedTestInputTestCases()
        {
            foreach (var argumentValue in ComplexArrayBasedTestInput)
                yield return new TestCaseData(args: new[] { argumentValue });
        }

        public static IEnumerable<TestCaseData> IncompatibleGenericTypeAndArgumentTestCases()
        {
            yield return new TestCaseData("doesn't work")
            {
                TypeArgs = new[] { typeof(int) }
            };
        }

        #region Test name tests

        [TestCaseSource(nameof(TestCaseNameTestDataSource))]
        public static void TestCaseNameTestDataMethod(params object[] args)
        {
        }

        public static IEnumerable<TestCaseData> TestCaseNameTestDataSource() =>
            from spec in TestDataSpec.Specs
            select new TestCaseData(spec.Arguments)
                .SetArgDisplayNames(spec.ArgDisplayNames)
                .SetProperty("ExpectedTestName", spec.GetTestCaseName(nameof(TestCaseNameTestDataMethod)));

        #endregion

        #region Multiline selection failure
        [TestFixture(Description = "https://github.com/nunit/nunit/issues/4584")]
        public class SelectionFail
        {
            [TestCaseSource(nameof(Data))]
            public static void Test(int actual, int expected)
            {
                Assert.That(actual, Is.EqualTo(expected));
            }

            public static IEnumerable<TestCaseData> Data
            {
                get
                {
                    yield return new TestCaseData(42, 42);
                    yield return new TestCaseData(42, 42).SetName("42 == 42");
                    yield return new TestCaseData(42, 42).SetName("42\r\n42");
                    yield return new TestCaseData(42, 42).SetArgDisplayNames("42\n", "42\r\n");
                }
            }
        }
        #endregion

        [TestCaseSource(nameof(DoubleTestData))]
        [TestCaseSource(nameof(StringTestData))]
        [TestCaseSource(nameof(TypedTestData))]
        [TestCaseSource(nameof(BaseClassData))]
        [TestCase(typeof(double), 4, 5.5, 6)] // Infers 'double'
        [TestCase(typeof(double), 7, 8, 9, TypeArgs = [typeof(double)])] // Will convert to specified type.
        [TestCase(typeof(double), 7, TypeArgs = [typeof(double)])] // Use different path
        [TestCase(typeof(double), new double[] { 1.1, 2.2 })] // Argument is already an array
        public void GenericParamsSuccess<T>(Type t, params T[] x)
        {
            Assert.That(x.Length, Is.GreaterThanOrEqualTo(0));
            Assert.That(typeof(T), Is.EqualTo(t));
        }

        private static IEnumerable<object[]> DoubleTestData()
        {
            yield return new object[] { typeof(double), 1.0 };
            yield return new object[] { typeof(double), 1, 2.2, 3.3 };
        }

        private static IEnumerable<object[]> StringTestData()
        {
            yield return new object[] { typeof(string), "1" };
            yield return new object[] { typeof(string), "1", "2", "3" };
        }

        private static IEnumerable<TestCaseData> TypedTestData()
        {
            yield return new TestCaseData([typeof(double)]) { TypeArgs = [typeof(double)] };
            yield return new TestCaseData([typeof(string)]) { TypeArgs = [typeof(string)] };
        }

        private static IEnumerable<TestCaseData> BaseClassData()
        {
            yield return new TestCaseData(typeof(D1), new D1(), new D2(), new D3());
            yield return new TestCaseData(typeof(D1), new D2(), new D3());
            yield return new TestCaseData(typeof(object), new D2(), string.Empty) { TypeArgs = [typeof(object)] };
        }

        [TestCaseSource(nameof(NoCommonBaseClassData))]
        public void GenericParamsNotRunnable<T>(params T[] x)
        {
            Assert.Fail("Should not be runnable");
        }

        private static IEnumerable<TestCaseData> NoCommonBaseClassData()
        {
            yield return new TestCaseData(1, "1");
            yield return new TestCaseData(new D2(), string.Empty);
        }

        private class D1
        {
        }

        private class D2 : D1
        {
        }

        private class D3 : D1
        {
        }
    }
}
