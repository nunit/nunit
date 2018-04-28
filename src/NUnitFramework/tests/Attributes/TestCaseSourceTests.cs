// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.TestCaseSourceAttributeFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestCaseSourceTests : TestSourceMayBeInherited
    {
        #region Tests With Static and Instance Members as Source

        [Test, TestCaseSource(nameof(StaticProperty))]
        public void SourceCanBeStaticProperty(string source)
        {
            Assert.AreEqual("StaticProperty", source);
        }

        [Test, TestCaseSource(nameof(InheritedStaticProperty))]
        public void TestSourceCanBeInheritedStaticProperty(bool source)
        {
            Assert.AreEqual(true, source);
        }

        static IEnumerable StaticProperty
        {
            get { return new object[] { new object[] { "StaticProperty" } }; }
        }

        [Test]
        public void SourceUsingInstancePropertyIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstancePropertyAsSource));
            Assert.AreEqual(result.Children.ToArray()[0].ResultState, ResultState.NotRunnable);
        }

        [Test, TestCaseSource(nameof(StaticMethod))]
        public void SourceCanBeStaticMethod(string source)
        {
            Assert.AreEqual("StaticMethod", source);
        }

        static IEnumerable StaticMethod()
        {
            return new object[] { new object[] { "StaticMethod" } };
        }

        [Test]
        public void SourceUsingInstanceMethodIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstanceMethodAsSource));
            Assert.AreEqual(result.Children.ToArray()[0].ResultState, ResultState.NotRunnable);
        }

        IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethod" } };
        }

        [Test, TestCaseSource(nameof(StaticField))]
        public void SourceCanBeStaticField(string source)
        {
            Assert.AreEqual("StaticField", source);
        }

        static object[] StaticField =
            { new object[] { "StaticField" } };

        [Test]
        public void SourceUsingInstanceFieldIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstanceFieldAsSource));
            Assert.AreEqual(result.Children.ToArray()[0].ResultState, ResultState.NotRunnable);
        }

        #endregion

        #region Test With IEnumerable Class as Source

        [Test, TestCaseSource(typeof(DataSourceClass))]
        public void SourceCanBeInstanceOfIEnumerable(string source)
        {
            Assert.AreEqual("DataSourceClass", source);
        }

        class DataSourceClass : IEnumerable
        {
            public DataSourceClass()
            {
            }

            public IEnumerator GetEnumerator()
            {
                yield return "DataSourceClass";
            }
        }

        #endregion

        [Test, TestCaseSource(nameof(MyData))]
        public void SourceMayReturnArgumentsAsObjectArray(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [TestCaseSource(nameof(MyData))]
        public void TestAttributeIsOptional(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource(nameof(MyIntData))]
        public void SourceMayReturnArgumentsAsIntArray(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource(nameof(MyArrayData))]
        public void SourceMayReturnArrayForArray(int[] array)
        {
            Assert.That(true);
        }

        [Test, TestCaseSource(nameof(EvenNumbers))]
        public void SourceMayReturnSinglePrimitiveArgumentAlone(int n)
        {
            Assert.AreEqual(0, n % 2);
        }

        [Test, TestCaseSource(nameof(Params))]
        public int SourceMayReturnArgumentsAsParamSet(int n, int d)
        {
            return n / d;
        }

        [Test]
        [TestCaseSource(nameof(MyData))]
        [TestCaseSource(nameof(MoreData), Category = "Extra")]
        [TestCase(12, 2, 6)]
        public void TestMayUseMultipleSourceAttributes(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, TestCaseSource(nameof(FourArgs))]
        public void TestWithFourArguments(int n, int d, int q, int r)
        {
            Assert.AreEqual(q, n / d);
            Assert.AreEqual(r, n % d);
        }

        [Test, Category("Top"), TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.HereIsTheData))]
        public void SourceMayBeInAnotherClass(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, Category("Top"), TestCaseSource(typeof(DivideDataProvider), "HereIsTheDataWithParameters", new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingSomeDataToConstructor(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test, Category("Top"), TestCaseSource(nameof(StaticMethodDataWithParameters), new object[] { 8000, 8, 1000 })]
        public void SourceCanBeStaticMethodPassingSomeDataToConstructor(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test]
        public void SourceInAnotherClassPassingParamsToField()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingParamsToField)).Tests[0];
            Assert.AreEqual(RunState.NotRunnable, testMethod.RunState);
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
            Assert.AreEqual("You have specified a data source field but also given a set of parameters. Fields cannot take parameters, " +
                            "please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove " +
                            "it or specify a method.", result.Message);
        }

        [Test]
        public void SourceInAnotherClassPassingParamsToProperty()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingParamsToProperty)).Tests[0];
            Assert.AreEqual(RunState.NotRunnable, testMethod.RunState);
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
            Assert.AreEqual("You have specified a data source property but also given a set of parameters. " +
                            "Properties cannot take parameters, please revise the 3rd parameter passed to the " +
                            "TestCaseSource attribute and either remove it or specify a method.", result.Message);
        }

        [Test]
        public void SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam)).Tests[0];
            Assert.AreEqual(RunState.NotRunnable, testMethod.RunState);
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
            Assert.AreEqual("You have given the wrong number of arguments to the method in the TestCaseSourceAttribute" +
                            ", please check the number of parameters passed in the object is correct in the 3rd parameter for the " +
                            "TestCaseSourceAttribute and this matches the number of parameters in the target method and try again.", result.Message);
        }

        [Test, TestCaseSource(typeof(DivideDataProviderWithReturnValue), nameof(DivideDataProviderWithReturnValue.TestCases))]
        public int SourceMayBeInAnotherClassWithReturn(int n, int d)
        {
            return n / d;
        }

        [Test]
        public void IgnoreTakesPrecedenceOverExpectedException()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodCallsIgnore)).Children.ToArray()[0];
            Assert.AreEqual(ResultState.Ignored, result.ResultState);
            Assert.AreEqual("Ignore this", result.Message);
        }

        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithIgnoredTestCases));

            Test testCase = TestFinder.Find("MethodWithIgnoredTestCases(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find("MethodWithIgnoredTestCases(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Don't Run Me!"));
        }

        [Test]
        public void CanMarkIndividualTestCasesExplicit()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithExplicitTestCases));

            Test testCase = TestFinder.Find("MethodWithExplicitTestCases(1)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find("MethodWithExplicitTestCases(2)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));

            testCase = TestFinder.Find("MethodWithExplicitTestCases(3)", suite, false);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
        }

        [Test]
        public void HandlesExceptionInTestCaseSource()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithSourceThrowingException)).Tests[0];
            Assert.AreEqual(RunState.NotRunnable, testMethod.RunState);
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
            Assert.AreEqual("System.Exception : my message", result.Message);
        }

        [TestCaseSource(nameof(exception_source)), Explicit("Used for GUI tests")]
        public void HandlesExceptionInTestCaseSource_GuiDisplay(string lhs, string rhs)
        {
            Assert.AreEqual(lhs, rhs);
        }


        private static IEnumerable<TestCaseData> ZeroTestCasesSource() => Enumerable.Empty<TestCaseData>();

        [TestCaseSource(nameof(ZeroTestCasesSource))]
        public void TestWithZeroTestSourceCasesShouldPassWithoutRequiringArguments(int requiredParameter)
        {
        }

        [Test]
        public void TestMethodIsNotRunnableWhenSourceDoesNotExist()
        {
            TestSuite suiteToTest = TestBuilder.MakeParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithNonExistingSource));
            
            Assert.That(suiteToTest.Tests.Count == 1);
            Assert.AreEqual(RunState.NotRunnable, suiteToTest.Tests[0].RunState);
        }

        static object[] testCases =
        {
            new TestCaseData(
                new string[] { "A" },
                new string[] { "B" })
        };

        [Test, TestCaseSource(nameof(testCases))]
        public void MethodTakingTwoStringArrays(string[] a, string[] b)
        {
            Assert.That(a, Is.TypeOf(typeof(string[])));
            Assert.That(b, Is.TypeOf(typeof(string[])));
        }

        [TestCaseSource(nameof(SingleMemberArrayAsArgument))]
        public void Issue1337SingleMemberArrayAsArgument(string[] args)
        {
            Assert.That(args.Length == 1 && args[0] == "1");
        }

        static string[][] SingleMemberArrayAsArgument = { new[] { "1" }  };

        [Test]
        public void TestNameIntrospectsArrayValues()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithArrayArguments));

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

        #region Sources used by the tests
        static object[] MyData = new object[] {
            new object[] { 12, 3, 4 },
            new object[] { 12, 4, 3 },
            new object[] { 12, 6, 2 } };

        static object[] MyIntData = new object[] {
            new int[] { 12, 3, 4 },
            new int[] { 12, 4, 3 },
            new int[] { 12, 6, 2 } };

        static object[] MyArrayData = new object[]
        {
            new int[] { 12 },
            new int[] { 12, 4 },
            new int[] { 12, 6, 2 }
        };

        public static IEnumerable StaticMethodDataWithParameters(int inject1, int inject2, int inject3)
        {
            yield return new object[] { inject1, inject2, inject3 };
        }

        static object[] FourArgs = new object[] {
            new TestCaseData( 12, 3, 4, 0 ),
            new TestCaseData( 12, 4, 3, 0 ),
            new TestCaseData( 12, 5, 2, 2 ) };

        static int[] EvenNumbers = new int[] { 2, 4, 6, 8 };

        static object[] MoreData = new object[] {
            new object[] { 12, 1, 12 },
            new object[] { 12, 2, 6 } };

        static object[] Params = new object[] {
            new TestCaseData(24, 3).Returns(8),
            new TestCaseData(24, 2).Returns(12) };

        private class DivideDataProvider
        {
#pragma warning disable 0169    // x is never assigned
            private static object[] myObject;
#pragma warning restore 0169

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

        public class DivideDataProviderWithReturnValue
        {
            public static IEnumerable TestCases
            {
                get
                {
                    return new object[] {
                        new TestCaseData(12, 3).Returns(4).SetName("TC1"),
                        new TestCaseData(12, 2).Returns(6).SetName("TC2"),
                        new TestCaseData(12, 4).Returns(3).SetName("TC3")
                    };
                }
            }
        }

        private static IEnumerable exception_source
        {
            get
            {
                yield return new TestCaseData("a", "a");
                yield return new TestCaseData("b", "b");

                throw new System.Exception("my message");
            }
        }
        #endregion
    }

    public class TestSourceMayBeInherited
    {
        protected static IEnumerable<bool> InheritedStaticProperty
        {
            get { yield return true; }
        }
    }
}
