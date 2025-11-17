// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestCaseSourceAttributeFixture;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class TestCaseSourceTests : TestSourceMayBeInherited
    {
        #region Tests With Static and Instance Members as Source

        [Test, TestCaseSource(nameof(StaticProperty))]
        public void SourceCanBeStaticProperty(string source)
        {
            Assert.That(source, Is.EqualTo("StaticProperty"));
        }

        [Test, TestCaseSource(nameof(InheritedStaticProperty))]
        public void TestSourceCanBeInheritedStaticProperty(bool source)
        {
            Assert.That(source, Is.EqualTo(true));
        }

        private static IEnumerable StaticProperty =>
            new object[]
            {
                new object[] { "StaticProperty" }
            };

        [Test, TestCaseSource(nameof(StaticAsyncMethod))]
        public void SourceCanBeStaticAsyncMethod(string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncMethod"));
        }

        private static Task<IEnumerable?> StaticAsyncMethod()
        {
            var result = new object[] { new object[] { nameof(StaticAsyncMethod) } };
            return Task.FromResult((IEnumerable?)result);
        }

        [Test, TestCaseSource(nameof(StaticAsyncEnumerableMethod))]
        public void SourceCanBeStaticAsyncEnumerableMethod(string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncEnumerableMethod"));
        }

        [Test, TestCaseSource(nameof(StaticAsyncEnumerableMethodReturningTask))]
        public void SourceCanBeStaticAsyncEnumerableMethodReturningTask(string source)
        {
            Assert.That(source, Is.EqualTo("StaticAsyncEnumerableMethodReturningTask"));
        }
#pragma warning restore NUnit1019 // The source specified by the TestCaseSource does not return an IEnumerable or a type that implements IEnumerable

        private static IAsyncEnumerable<object> StaticAsyncEnumerableMethod()
        {
            var result = new object[] { new object[] { nameof(StaticAsyncEnumerableMethod) } };
            return result.AsAsyncEnumerable();
        }

        private static Task<IAsyncEnumerable<object>> StaticAsyncEnumerableMethodReturningTask()
        {
            var result = new object[] { new object[] { nameof(StaticAsyncEnumerableMethodReturningTask) } };
            return Task.FromResult(result.AsAsyncEnumerable());
        }

        [Test]
        public void SourceUsingInstancePropertyIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstancePropertyAsSource));
            Assert.That(ResultState.NotRunnable, Is.EqualTo(result.Children.ToArray()[0].ResultState));
        }

        [Test, TestCaseSource(nameof(StaticMethod))]
        public void SourceCanBeStaticMethod(string source)
        {
            Assert.That(source, Is.EqualTo("StaticMethod"));
        }

        private static IEnumerable StaticMethod()
        {
            return new object[] { new object[] { "StaticMethod" } };
        }

        [Test]
        public void SourceUsingInstanceMethodIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstanceMethodAsSource));
            Assert.That(ResultState.NotRunnable, Is.EqualTo(result.Children.ToArray()[0].ResultState));
        }

        private IEnumerable InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethod" } };
        }

        [Test, TestCaseSource(nameof(StaticField))]
        public void SourceCanBeStaticField(string source)
        {
            Assert.That(source, Is.EqualTo("StaticField"));
        }

        private static readonly object[] StaticField =
            {
                new object[] { "StaticField" }
            };

        [Test]
        public void SourceUsingInstanceFieldIsNotRunnable()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithInstanceFieldAsSource));
            Assert.That(ResultState.NotRunnable, Is.EqualTo(result.Children.ToArray()[0].ResultState));
        }

        #endregion

        #region Test With IEnumerable Class as Source

        [Test, TestCaseSource(typeof(DataSourceClass))]
        public void SourceCanBeInstanceOfIEnumerable(string source)
        {
            Assert.That(source, Is.EqualTo("DataSourceClass"));
        }

        private class DataSourceClass : IEnumerable
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
            Assert.That(n / d, Is.EqualTo(q));
        }

        [TestCaseSource(nameof(MyData))]
        public void TestAttributeIsOptional(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test, TestCaseSource(nameof(MyIntData))]
        public void SourceMayReturnArgumentsAsIntArray(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test, TestCaseSource(nameof(MyArrayData))]
        public void SourceMayReturnArrayForArray(int[] array)
        {
            Assert.Multiple(() =>
            {
                Assert.That(array, Is.Not.Null);
                Assert.That(true);
            });
        }

        [Test, TestCaseSource(nameof(EvenNumbers))]
        public void SourceMayReturnSinglePrimitiveArgumentAlone(int n)
        {
            Assert.That(n % 2, Is.EqualTo(0));
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
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test, TestCaseSource(nameof(FourArgs))]
        public void TestWithFourArguments(int n, int d, int q, int r)
        {
            Assert.Multiple(() =>
            {
                Assert.That(n / d, Is.EqualTo(q));
                Assert.That(n % d, Is.EqualTo(r));
            });
        }

        [Test, Category("Top"), TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.HereIsTheData))]
        public void SourceMayBeInAnotherClass(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test, Category("Top"), TestCaseSource(typeof(DivideDataProvider), nameof(DivideDataProvider.HereIsTheDataWithParameters), new object[] { 100, 4, 25 })]
        public void SourceInAnotherClassPassingSomeDataToConstructor(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test, Category("Top"), TestCaseSource(nameof(StaticMethodDataWithParameters), new object[] { 8000, 8, 1000 })]
        public void SourceCanBeStaticMethodPassingSomeDataToConstructor(int n, int d, int q)
        {
            Assert.That(n / d, Is.EqualTo(q));
        }

        [Test]
        public void SourceInAnotherClassPassingParamsToField()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingParamsToField)).Tests[0];
            Assert.That(testMethod.RunState, Is.EqualTo(RunState.NotRunnable));
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(result.Message, Is.EqualTo("You have specified a data source field but also given a set of parameters. Fields cannot take parameters, " +
                                "please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove " +
                                "it or specify a method."));
            });
        }

        [Test]
        public void SourceInAnotherClassPassingParamsToProperty()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingParamsToProperty)).Tests[0];
            Assert.That(testMethod.RunState, Is.EqualTo(RunState.NotRunnable));
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(result.Message, Is.EqualTo("You have specified a data source property but also given a set of parameters. " +
                                "Properties cannot take parameters, please revise the 3rd parameter passed to the " +
                                "TestCaseSource attribute and either remove it or specify a method."));
            });
        }

        [Test]
        public void SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.SourceInAnotherClassPassingSomeDataToConstructorWrongNumberParam)).Tests[0];
            Assert.That(testMethod.RunState, Is.EqualTo(RunState.NotRunnable));
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(result.Message, Is.EqualTo("You have given the wrong number of arguments to the method in the TestCaseSourceAttribute" +
                                ", please check the number of parameters passed in the object is correct in the 3rd parameter for the " +
                                "TestCaseSourceAttribute and this matches the number of parameters in the target method and try again."));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(result.Message, Is.EqualTo("Ignore this"));
            });
        }

        [Test]
        public void CanIgnoreIndividualTestCases()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithIgnoredTestCases));
            Test? testCase = TestFinder.Find("MethodWithIgnoredTestCases(1)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find("MethodWithIgnoredTestCases(2)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
                Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Don't Run Me!"));
            });
        }

        [Test]
        public void CanIgnoreIndividualTestCasesWithUntilDate()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithIgnoredTestCases));

            DateTimeOffset untilDate = DateTimeOffset.Parse("4242-01-01 00:00:00", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            Test? testCase = TestFinder.Find("MethodWithIgnoredTestCases(3)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
                Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo($"Ignoring until {untilDate:u}. Ignore Me Until The Future"));
                Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDate.ToString("u")));
            });
            untilDate = DateTimeOffset.Parse("1492-01-01", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            testCase = TestFinder.Find("MethodWithIgnoredTestCases(4)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDate.ToString("u")));
            });
            untilDate = DateTimeOffset.Parse("4242-01-01 12:42:33Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            testCase = TestFinder.Find("MethodWithIgnoredTestCases(5)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testCase.RunState, Is.EqualTo(RunState.Ignored));
                Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo($"Ignoring until {untilDate:u}. Ignore Me Until The Future"));
                Assert.That(testCase.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo(untilDate.ToString("u")));
            });
        }

        [Test]
        public void CanMarkIndividualTestCasesExplicit()
        {
            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithExplicitTestCases));

            Test? testCase = TestFinder.Find("MethodWithExplicitTestCases(1)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));

            testCase = TestFinder.Find("MethodWithExplicitTestCases(2)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));

            testCase = TestFinder.Find("MethodWithExplicitTestCases(3)", suite, false);
            Assert.That(testCase, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testCase.RunState, Is.EqualTo(RunState.Explicit));
                Assert.That(testCase.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Connection failing"));
            });
        }

        [Test]
        public void HandlesExceptionInTestCaseSource()
        {
            var testMethod = (TestMethod)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture), nameof(TestCaseSourceAttributeFixture.MethodWithSourceThrowingException)).Tests[0];
            Assert.That(testMethod.RunState, Is.EqualTo(RunState.NotRunnable));
            ITestResult result = TestBuilder.RunTest(testMethod, null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(result.Message, Is.EqualTo("System.Exception : my message"));
            });
        }

        [TestCaseSource(nameof(ExceptionSource)), Explicit("Used for GUI tests")]
        public void HandlesExceptionInTestCaseSource_GuiDisplay(string lhs, string rhs)
        {
            Assert.That(rhs, Is.EqualTo(lhs));
        }

#pragma warning disable NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method
        [TestCaseSource(nameof(ParamsArrayTwoStringArguments))]
        public void HandlesParamsArrayAsSoleArgument(params string[] array)
        {
            Assert.Multiple(() =>
            {
                Assert.That(array[0], Is.EqualTo("a"));
                Assert.That(array[1], Is.EqualTo("b"));
            });
        }

        [TestCaseSource(nameof(ParamsArrayOneStringArgument))]
        public void HandlesParamsArrayWithOneItemAsSoleArgument(params string[] array)
        {
            Assert.That(array[0], Is.EqualTo("a"));
        }

        [TestCaseSource(nameof(ParamsArrayFourStringArguments))]
        public void HandlesParamsArrayAsLastArgument(string s1, string s2, params object[] array)
        {
            Assert.That(s1, Is.EqualTo("a"));
            Assert.That(s2, Is.EqualTo("b"));
            Assert.That(array[0], Is.EqualTo("c"));
            Assert.That(array[1], Is.EqualTo("d"));
        }

        [TestCaseSource(nameof(ParamsArrayTwoStringArguments))]
        public void HandlesParamsArrayWithNoItemsAsLastArgument(string s1, string s2, params string[] array)
        {
            Assert.That(s1, Is.EqualTo("a"));
            Assert.That(s2, Is.EqualTo("b"));
            Assert.That(array, Is.Empty);
        }
#pragma warning restore NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method

        [TestCaseSource(nameof(OptionalArgumentsTestCasesSource))]
        public string[] HandlesOptionalArguments(string s1 = "a", string s2 = "b", string s3 = "c")
        {
            return [s1, s2, s3];
        }

        private static IEnumerable ParamsArrayOneStringArgument
        {
            get
            {
                yield return new TestCaseData("a").SetArgDisplayNames("new TestCaseData(\"a\")");
                yield return new TestCaseData<string>("a").SetArgDisplayNames("new TestCaseData<string>(\"a\")");
                yield return new string[] { "a" };
            }
        }

        private static IEnumerable ParamsArrayTwoStringArguments
        {
            get
            {
                yield return new TestCaseData("a", "b").SetArgDisplayNames("new TestCaseData(\"a\", \"b\")");
                yield return new TestCaseData<string>("a", "b").SetArgDisplayNames("new TestCaseData<string>(\"a\", \"b\")");
                yield return new string[] { "a", "b" };
            }
        }

        private static IEnumerable ParamsArrayFourStringArguments
        {
            get
            {
                yield return new TestCaseData("a", "b", "c", "d").SetArgDisplayNames("new TestCaseData(\"a\", \"b\", \"c\", \"d\")");
                yield return new string[] { "a", "b", "c", "d" };
            }
        }

        private static IEnumerable OptionalArgumentsTestCasesSource
        {
            get
            {
                yield return new TestCaseData()
                    .SetArgDisplayNames("new TestCaseData()")
                    .Returns(new[] { "a", "b", "c" });

                yield return new TestCaseData("x")
                    .SetArgDisplayNames("new TestCaseData(\"x\")")
                    .Returns(new[] { "x", "b", "c" });

                yield return new TestCaseData("x", "y")
                    .SetArgDisplayNames("new TestCaseData(\"x\", \"y\")")
                    .Returns(new[] { "x", "y", "c" });

                yield return new TestCaseData("x", "y", "z")
                    .SetArgDisplayNames("new TestCaseData(\"x\", \"y\", \"z\")")
                    .Returns(new[] { "x", "y", "z" });
            }
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

            Assert.That(suiteToTest.Tests, Has.Count.EqualTo(1));
            Assert.That(suiteToTest.Tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        private static readonly object[] TestCases =
        {
            new TestCaseData(
                new[] { "A" },
                new[] { "B" })
        };

        [Test, TestCaseSource(nameof(TestCases))]
        public void MethodTakingTwoStringArrays(string[] a, string[] b)
        {
            Assert.Multiple(() =>
            {
                Assert.That(a, Is.TypeOf(typeof(string[])));
                Assert.That(b, Is.TypeOf(typeof(string[])));
            });
        }

        [TestCaseSource(nameof(SingleMemberArrayAsArgument))]
        public void Issue1337SingleMemberArrayAsArgument(string[] args)
        {
            Assert.That(args.Length == 1 && args[0] == "1");
        }

        private static readonly string[][] SingleMemberArrayAsArgument = { new[] { "1" } };

        #region Test name tests

        private static IEnumerable<TestCaseData> IndividualInstanceNameTestDataSource()
        {
            var suite = (ParameterizedMethodSuite)TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture),
                nameof(TestCaseSourceAttributeFixture.TestCaseNameTestDataMethod));

            foreach (var test in suite.Tests)
            {
                var expectedName = (string?)test.Properties.Get("ExpectedTestName");
                Assert.That(expectedName, Is.Not.Null);

                var d = new TestCaseData(test, expectedName)
                    .SetArgDisplayNames(expectedName); // SetArgDisplayNames (here) is purely cosmetic for the purposes of these tests
                yield return d;
            }
        }

        [TestCaseSource(nameof(IndividualInstanceNameTestDataSource))]
        public static void IndividualInstanceName(ITest test, string expectedName)
        {
            Assert.That(test.Name, Is.EqualTo(expectedName));
        }

        [TestCaseSource(nameof(IndividualInstanceNameTestDataSource))]
        public static void IndividualInstanceFullName(ITest test, string expectedName)
        {
            var expectedFullName = typeof(TestCaseSourceAttributeFixture).FullName + "." + expectedName;
            Assert.That(test.FullName, Is.EqualTo(expectedFullName));
        }

        #endregion

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

        [TestCaseSource(nameof(ExplicitTypeArgsWithUnrelatedParametersTestCases))]
        public void ExplicitTypeArgsWithUnrelatedParameters<T>(string input)
        {
            Assert.That(typeof(T), Is.EqualTo(typeof(long)));
            Assert.That(input, Is.EqualTo("2"));
        }

        private static IEnumerable<TestCaseData> ExplicitTypeArgsWithUnrelatedParametersTestCases()
        {
            yield return new TestCaseData("2") { TypeArgs = new[] { typeof(long) } };
        }

        [TestCaseSource(nameof(GenericMethodAndParameterWithExplicitOrImplicitTypingTestCases))]
        public Type GenericMethodAndParameterWithExplicitOrImplicitTyping<T>(T input)
            => typeof(T);

        private static IEnumerable<TestCaseData> GenericMethodAndParameterWithExplicitOrImplicitTypingTestCases()
        {
            yield return new TestCaseData(2)
            {
                TypeArgs = new[] { typeof(long) },
                ExpectedResult = typeof(long)
            };
            yield return new TestCaseData(2L)
            {
                TypeArgs = new[] { typeof(long) },
                ExpectedResult = typeof(long)
            };
            yield return new TestCaseData(2)
            {
                ExpectedResult = typeof(int)
            };
            yield return new TestCaseData(2L)
            {
                ExpectedResult = typeof(long)
            };
            yield return new TestCaseData<long>(2)
            {
                ExpectedResult = typeof(long)
            };
            yield return new TestCaseData<long>(2L)
            {
                ExpectedResult = typeof(long)
            };
            yield return new TestCaseData<int>(2)
            {
                ExpectedResult = typeof(int)
            };
        }

        [Test]
        public void ExplicitTypeArgsWithUnassignableParametersFailsAtRuntime()
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(
                typeof(TestCaseSourceAttributeFixture),
                nameof(TestCaseSourceAttributeFixture.MethodWithIncompatibleGenericTypeAndArgument));

            var test = (Test)suite.Tests[0];

            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));

            var result = TestBuilder.RunTest(test);

            Assert.That(result.FailCount, Is.EqualTo(1));
            Assert.That(result.Message, Does.Contain("Object of type 'System.String' cannot be converted to type 'System.Int32'."));
        }

        [TestCaseSource(nameof(ExplicitTypeArgsWithGenericConstraintSatisfiedTestCases))]
        public void ExplicitTypeArgsWithGenericConstraintSatisfied<T1, T2>(T1 a, T2 b)
            where T1 : IComparer<T2>
        {
            Assert.That(typeof(T1), Is.EqualTo(typeof(IntConverter)));
            Assert.That(a, Is.TypeOf<DerivedIntConverter>());
        }

        public class IntConverter : IComparer<int>
        {
            public int Compare(int x, int y) => x - y;
        }

        public class DerivedIntConverter : IntConverter
        {
        }

        private static IEnumerable<TestCaseData> ExplicitTypeArgsWithGenericConstraintSatisfiedTestCases()
        {
            yield return new TestCaseData(new DerivedIntConverter(), 2)
            {
                TypeArgs = new[] { typeof(IntConverter), typeof(int) }
            };
#if NET6_0_OR_GREATER
            yield return new TestCaseData<IntConverter, int>(new DerivedIntConverter(), 2);
#endif
        }

#if NET6_0_OR_GREATER
        [TestCaseSource(nameof(GenericDataWithGenericConstraint1))]
        public void ExplicitGenericDataWithCompatibleParameters<T>(T input)
        {
            Assert.That(input, Is.InstanceOf<T>());
        }

        private static IEnumerable<TestCaseData> GenericDataWithGenericConstraint1()
        {
            yield return new TestCaseData<int>(2);
            yield return new TestCaseData<double>(2);
        }

        [TestCaseSource(nameof(GenericDataWithGenericConstraint2))]
        public void ExplicitGenericDataWithCompatibleParameters<T1, T2>(T1 input1, T2 input2)
        {
            Assert.Multiple(() =>
            {
                Assert.That(input1, Is.InstanceOf<T1>());
                Assert.That(input2, Is.InstanceOf<T2>());
            });
        }

        private static IEnumerable<TestCaseData> GenericDataWithGenericConstraint2()
        {
            yield return new TestCaseData<int, double>(2, 2.0);
            yield return new TestCaseData<double, int>(2.0, 2);
        }

        [TestCaseSource(nameof(GenericDataWithGenericConstraint3))]
        public void ExplicitGenericDataWithCompatibleParameters<T1, T2, T3>(T1 input1, T2 input2, T3 input3)
        {
            Assert.Multiple(() =>
            {
                Assert.That(input1, Is.InstanceOf<T1>());
                Assert.That(input2, Is.InstanceOf<T2>());
                Assert.That(input3, Is.InstanceOf<T3>());
            });
        }

        private static IEnumerable<TestCaseData> GenericDataWithGenericConstraint3()
        {
            yield return new TestCaseData<string, int, double>("2", 2, 2.0);
            yield return new TestCaseData<double, int, string>(2.0, 2, "2");
        }

        [TestCaseSource(nameof(GenericDataWithGenericConstraint4))]
        public void ExplicitGenericDataWithCompatibleParameters<T1, T2, T3, T4>(T1 input1, T2 input2, T3 input3, T4 input4)
        {
            Assert.Multiple(() =>
            {
                Assert.That(input1, Is.InstanceOf<T1>());
                Assert.That(input2, Is.InstanceOf<T2>());
                Assert.That(input3, Is.InstanceOf<T3>());
                Assert.That(input4, Is.InstanceOf<T4>());
            });
        }

        private static IEnumerable<TestCaseData> GenericDataWithGenericConstraint4()
        {
            yield return new TestCaseData<bool, string, int, double>(true, "2", 2, 2.0);
            yield return new TestCaseData<double, int, string, bool>(2.0, 2, "2", true);
        }

        [TestCaseSource(nameof(GenericDataWithGenericConstraint5))]
        public void ExplicitGenericDataWithCompatibleParameters<T1, T2, T3, T4, T5>(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5)
        {
            Assert.Multiple(() =>
            {
                Assert.That(input1, Is.InstanceOf<T1>());
                Assert.That(input2, Is.InstanceOf<T2>());
                Assert.That(input3, Is.InstanceOf<T3>());
                Assert.That(input4, Is.InstanceOf<T4>());
                Assert.That(input5, Is.InstanceOf<T5>());
            });
        }

        private static IEnumerable<TestCaseData> GenericDataWithGenericConstraint5()
        {
            yield return new TestCaseData<bool, char, string, int, double>(true, 'N', "2", 2, 2.0);
            yield return new TestCaseData<double, int, string, char, bool>(2.0, 2, "2", 'N', true);
        }

#endif

        #region Sources used by the tests
        private static readonly object[] MyData = new object[]
        {
            new object[] { 12, 3, 4 },
            new object[] { 12, 4, 3 },
            new object[] { 12, 6, 2 }
        };
        private static readonly object[] MyIntData = new object[]
        {
            new[] { 12, 3, 4 },
            new[] { 12, 4, 3 },
            new[] { 12, 6, 2 }
        };
        private static readonly object[] MyArrayData = new object[]
        {
            new[] { 12 },
            new[] { 12, 4 },
            new[] { 12, 6, 2 }
        };

        private static IEnumerable StaticMethodDataWithParameters(int inject1, int inject2, int inject3)
        {
            yield return new object[] { inject1, inject2, inject3 };
        }

        private static readonly object[] FourArgs = new object[]
        {
            new TestCaseData(12, 3, 4, 0),
            new TestCaseData(12, 4, 3, 0),
            new TestCaseData(12, 5, 2, 2)
        };
        private static readonly int[] EvenNumbers = new[] { 2, 4, 6, 8 };
        private static readonly object[] MoreData = new object[]
        {
            new object[] { 12, 1, 12 },
            new object[] { 12, 2, 6 }
        };
        private static readonly object[] Params = new object[]
        {
            new TestCaseData(24, 3).Returns(8),
            new TestCaseData(24, 2).Returns(12)
        };

        private class DivideDataProvider
        {
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
            public static IEnumerable TestCases =>
                new object[]
                {
                    new TestCaseData(12, 3).Returns(4).SetName("TC1"),
                    new TestCaseData(12, 2).Returns(6).SetName("TC2"),
                    new TestCaseData(12, 4).Returns(3).SetName("TC3")
                };
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
        #endregion

        [TestCaseSource(nameof(AsyncEnumerableTestCases))]
        public async Task TestAsyncEnumerable(int expected, int actual)
        {
            await Task.Delay(100); // Simulate an asynchronous operation
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static async IAsyncEnumerable<TestCaseData> AsyncEnumerableTestCases()
        {
            await Task.Delay(100); // Simulate an asynchronous operation
            yield return new TestCaseData(42, 42);
            await Task.Delay(100); // Simulate another asynchronous operation
            yield return new TestCaseData(51, 51);
        }

        #region Generic Nullable

        [TestCaseSource(nameof(GenericNullableSource))]
        public void GenericNullableTest<TValue>(TValue? value)
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
