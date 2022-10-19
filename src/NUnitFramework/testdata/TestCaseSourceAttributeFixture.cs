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

        [TestCaseSource(nameof(source))]
        public void MethodCallsIgnore(int x, int y, int z)
        {
            Assert.Ignore("Ignore this");
        }

#pragma warning disable 414
        private static object[] source = new object[] {
            new TestCaseData( 2, 3, 4 ) };
#pragma warning restore 414

        #endregion

        #region Test With Ignored TestCaseData

        [TestCaseSource(nameof(IgnoredSource))]
        [TestCaseSource(nameof(IgnoredWithDateSource))]
        public void MethodWithIgnoredTestCases(int num)
        {
        }

        private static IEnumerable IgnoredSource
        {
            get
            {
                return new object[] {
                    new TestCaseData(1),
                    new TestCaseData(2).Ignore("Don't Run Me!"),

                };
            }
        }

        private static IEnumerable IgnoredWithDateSource
        {
            get
            {
                DateTimeOffset utcTime = DateTimeOffset.UtcNow;
                TimeSpan timeZoneOffset = utcTime - utcTime.ToLocalTime();
                return new object[] {
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

        private static IEnumerable ExplicitSource
        {
            get
            {
                return new object[] {
                    new TestCaseData(1),
                    new TestCaseData(2).Explicit(),
                    new TestCaseData(3).Explicit("Connection failing")
                };
            }
        }

        #endregion

        #region Tests Using Instance Members as Source

        [Test, TestCaseSource(nameof(InstanceProperty))]
        public void MethodWithInstancePropertyAsSource(string source)
        {
            Assert.AreEqual("InstanceProperty", source);
        }

        IEnumerable InstanceProperty
        {
            get { return new object[] { new object[] { "InstanceProperty" } }; }
        }

        [Test, TestCaseSource(nameof(InstanceMethod))]
        public void MethodWithInstanceMethodAsSource(string source)
        {
            Assert.AreEqual("InstanceMethod", source);
        }

        IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethod" } };
        }

        [Test, TestCaseSource(nameof(InstanceField))]
        public void MethodWithInstanceFieldAsSource(string source)
        {
            Assert.AreEqual("InstanceField", source);
        }

#pragma warning disable 414
        object[] InstanceField = { new object[] { "InstanceField" } };
#pragma warning restore 414

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

        static IEnumerable ExceptionSource
        {
            get
            {
                yield return new TestCaseData("a", "a");
                yield return new TestCaseData("b", "b");

                throw new System.Exception("my message");
            }
        }

        class DivideDataProvider
        {
#pragma warning disable 0169, 0649    // x is never assigned
            static object[] myObject;
            public static string MyField;
#pragma warning restore 0169, 0649
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

        static object[] ComplexArrayBasedTestInput = new[]
        {
            new[] { 1, "text", new object() },
            Array.Empty<object>(),
            new object[] { 1, new[] { 2, 3 }, 4 },
            new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            new object[] { new byte[,] { { 1, 2 }, { 2, 3 } } }
        };

        static IEnumerable<TestCaseData> ComplexArrayBasedTestInputTestCases()
        {
            foreach (var argumentValue in ComplexArrayBasedTestInput)
                yield return new TestCaseData(args: new[] { argumentValue });
        }

        #region Test name tests

        [TestCaseSource(nameof(TestCaseNameTestDataSource))]
        public static void TestCaseNameTestDataMethod(params object[] args) { }

        public static IEnumerable<TestCaseData> TestCaseNameTestDataSource() =>
            from spec in TestDataSpec.Specs
            select new TestCaseData(spec.Arguments)
                .SetArgDisplayNames(spec.ArgDisplayNames)
                .SetProperty("ExpectedTestName", spec.GetTestCaseName(nameof(TestCaseNameTestDataMethod)));

        #endregion
    }
}
