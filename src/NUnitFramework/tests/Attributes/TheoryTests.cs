// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TheoryFixture;

namespace NUnit.Framework.Tests.Attributes
{
    public class TheoryTests
    {
        private static readonly Type FixtureType = typeof(TheoryFixture);

        #region Issue 4426 - Non-static DatapointSource with throwing constructor

        // Issue #4426: Originally, tests with non-static DatapointSource and a throwing constructor
        // were silently ignored (no test discovered, no error). This has been fixed - tests are now
        // discovered and failures are reported.
        //
        // However, as Manfred notes: with a non-static DatapointSource, the datapoint values cannot
        // be read without an instance. When the constructor throws, no instance exists, so the Theory
        // cannot be expanded into parameterized test cases. It is marked NotRunnable instead.
        //
        // Future consideration: NotRunnable tests can be "lost between other skipped tests."
        // Perhaps DatapointSource should require static fields (like TestCaseSource/ValueSource),
        // or the failure should be more prominent.

        /// <summary>
        /// Issue #4426: With a non-static DatapointSource, NUnit cannot read the datapoint values
        /// without creating an instance. When the constructor throws, no instance can be created,
        /// so the Theory cannot be expanded into parameterized test cases.
        /// The Theory method is discovered but marked NotRunnable.
        /// </summary>
        [Test]
        public void TheoryWithNonStaticDatapointSource_ConstructorThrows_TheoryIsNotRunnable()
        {
            var fixture = TestBuilder.MakeFixture(typeof(TheoryWithNonStaticDatapointSourceAndThrowingConstructor));

            // Find the TheoryMethod test
            var theoryMethod = (Test)fixture.Tests.Single(t => t.Name == "TheoryMethod");

            Assert.Multiple(() =>
            {
                // The Theory is discovered but cannot be expanded into parameterized cases
                // because the non-static DatapointSource cannot be read without an instance
                Assert.That(theoryMethod.RunState, Is.EqualTo(RunState.NotRunnable),
                    "Theory should be NotRunnable when datapoints cannot be accessed");
                Assert.That(theoryMethod.TestCaseCount, Is.EqualTo(1),
                    "Theory should have single placeholder test case, not expanded parameterized cases");
                Assert.That(theoryMethod.Properties.Get(PropertyNames.SkipReason)?.ToString(),
                    Does.Contain("Failure building Test"),
                    "Should indicate failure building test");
            });
        }

        /// <summary>
        /// Issue #4426: When the fixture is executed, the constructor exception should be reported.
        /// Both the NotRunnable theory and the regular test should fail due to the constructor exception.
        /// </summary>
        [Test]
        public void TheoryWithNonStaticDatapointSource_ConstructorThrows_ReportsError()
        {
            var result = TestBuilder.RunTestFixture(typeof(TheoryWithNonStaticDatapointSourceAndThrowingConstructor));

            Assert.Multiple(() =>
            {
                // The fixture should fail due to constructor exception
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed),
                    "Should report failure due to constructor exception");
                Assert.That(result.Message, Does.Contain("Constructor throws"),
                    "Should contain the constructor exception message");
            });
        }

        /// <summary>
        /// Issue #4426: With a static DatapointSource, NUnit can read the datapoint values
        /// without creating an instance. The Theory is properly expanded into parameterized test cases.
        /// Contrast with non-static case where this is not possible.
        /// </summary>
        [Test]
        public void TheoryWithStaticDatapointSource_ConstructorThrows_TestsAreDiscovered()
        {
            var fixture = TestBuilder.MakeFixture(typeof(TheoryWithStaticDatapointSourceAndThrowingConstructor));

            // Find the TheoryMethod test
            var theoryMethod = (Test)fixture.Tests.Single(t => t.Name == "TheoryMethod");

            Assert.Multiple(() =>
            {
                // With static DatapointSource, all test cases can be discovered
                // (5 datapoint values + 1 regular test = 6 total)
                Assert.That(fixture.TestCaseCount, Is.EqualTo(6),
                    "All 6 test cases should be discovered with static DatapointSource");
                Assert.That(theoryMethod.RunState, Is.EqualTo(RunState.Runnable),
                    "Theory should be Runnable with static DatapointSource");
                Assert.That(theoryMethod.TestCaseCount, Is.EqualTo(5),
                    "Theory should have 5 parameterized test cases from datapoints");
            });
        }

        /// <summary>
        /// Issue #4426: Even with static DatapointSource, execution fails because the constructor throws.
        /// The difference from non-static is that tests ARE properly discovered.
        /// </summary>
        [Test]
        public void TheoryWithStaticDatapointSource_ConstructorThrows_ReportsError()
        {
            var result = TestBuilder.RunTestFixture(typeof(TheoryWithStaticDatapointSourceAndThrowingConstructor));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed),
                    "Should report failure due to constructor exception");
                Assert.That(result.Message, Does.Contain("Constructor throws"),
                    "Should contain the constructor exception message");
            });
        }

        #endregion

        [Test]
        public void TheoryWithNoArgumentsIsTreatedAsTest()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TheoryFixture.TheoryWithNoArguments));
        }

        [Test]
        public void TheoryWithNoDatapointsIsRunnableButFails()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TheoryFixture.TheoryWithArgumentsButNoDatapoints), ResultState.Failure);
        }

        [Test]
        public void UnsupportedNullableTypeArgumentWithNoDatapointsIsRunnableButFails()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TheoryFixture.TestWithUnsupportedNullableTypeArgumentWithNoDataPoints), ResultState.Failure);
        }

        [Test]
        public void TheoryWithDatapointsIsRunnable()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TheoryWithArgumentsAndDatapoints));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(9));
        }

        [Test]
        public void BooleanArgumentsAreSuppliedAutomatically()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithBooleanArguments));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(4));
        }

        [Test]
        public void NullableBooleanArgumentsAreSuppliedAutomatically()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithNullableBooleanArguments));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(9));
        }

        [Test]
        public void DatapointAndAttributeDataMayBeCombined()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithBothDatapointAndAttributeData));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(6));
        }

        [Datapoint] private readonly object? _nullObj = null;

        [Theory]
        public void NullDatapointIsOK(object o)
        {
            Assert.That(o, Is.Null);
            Assert.That(_nullObj, Is.Null); // to avoid a warning
        }

        [Test]
        public void EnumArgumentsAreSuppliedAutomatically()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithEnumAsArgument));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(16));
        }

        [Test]
        public void NullableEnumArgumentsAreSuppliedAutomatically()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithNullableEnumAsArgument));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(17));
        }

        [Test]
        public void AllValuesMayBeSuppliedByAttributes()
        {
            // NOTE: This test was failing with a count of 8 because both
            // TheoryAttribute and CombinatorialAttribute were adding cases.
            // Solution is to make TheoryAttribute a CombiningAttribute so
            // that no extra attribute is added to the method.
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithAllDataSuppliedByAttributes));
            TestAssert.IsRunnable(test);
            Assert.That(test.TestCaseCount, Is.EqualTo(4));
        }

        [Theory]
        public void SquareRootWithAllGoodValues(
            [Values(12.0, 4.0, 9.0)] double d)
        {
            SquareRootTest(d);
        }

        [Theory]
        public void SquareRootWithOneBadValue(
            [Values(12.0, -4.0, 9.0)] double d)
        {
            SquareRootTest(d);
        }

        [Datapoints]
        private readonly string[] _vals = ["xyz1", "xyz2", "xyz3"];

        [Theory]
        public void ArrayWithDatapointsAttributeIsUsed(string s)
        {
            Assert.That(s, Does.StartWith("xyz"));
        }

        private static void SquareRootTest(double d)
        {
            Assume.That(d > 0);
            double root = Math.Sqrt(d);
            Assert.That(root * root, Is.EqualTo(d).Within(0.000001));
            Assert.That(root, Is.GreaterThan(0));
        }

        [Test]
        public void SimpleTestIgnoresDataPoints()
        {
            Test test = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithArguments));
            Assert.That(test.TestCaseCount, Is.EqualTo(2));
        }

        [Test]
        public void TheoryFailsIfAllTestsAreInconclusive()
        {
            ITestResult result = TestBuilder.RunParameterizedMethodSuite(FixtureType, nameof(TheoryFixture.TestWithAllBadValues));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("All test cases were inconclusive"));
        }

        public class NestedTheoryThatSearchesInDeclaringTypes
        {
            private static readonly Type NestedType = typeof(TheoryFixture.NestedWhileSearchingInDeclaringType);

            [Test]
            public void WithNoArgumentsIsTreatedAsTest()
            {
                TestAssert.IsRunnable(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithNoArguments));
            }

            [Test]
            public void WithNoDatapointsIsRunnableButFails()
            {
                TestAssert.IsRunnable(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithArgumentsButNoDatapoints), ResultState.Failure);
            }

            [Test]
            public void WithUnsupportedNullableTypeArgumentWithNoDatapointsIsRunnableButFails()
            {
                TestAssert.IsRunnable(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithUnsupportedNullableTypeArgumentWithNoDataPoints), ResultState.Failure);
            }

            [Test]
            public void WithDatapointsIsRunnable()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithArgumentsAndDatapoints));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(9));
            }

            [Test]
            public void WithBooleanArgumentsHasValuesSuppliedAutomatically()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithBooleanArguments));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(4));
            }

            [Test]
            public void WithNullableBooleanArgumentsHasValuesSuppliedAutomatically()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithNullableBooleanArguments));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(9));
            }

            [Test]
            public void WithDatapointAndAttributeDataIsRunnable()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithBothDatapointAndAttributeData));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(6));
            }

            [Test]
            public void WithEnumArgumentsHasValuesSuppliedAutomatically()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithEnumAsArgument));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(16));
            }

            [Test]
            public void WithNullableEnumArgumentsHasValuesSuppliedAutomatically()
            {
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithNullableEnumAsArgument));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(17));
            }

            [Test]
            public void WithAllValuesSuppliedByAttributesIsRunnable()
            {
                // NOTE: This test was failing with a count of 8 because both
                // TheoryAttribute and CombinatorialAttribute were adding cases.
                // Solution is to make TheoryAttribute a CombiningAttribute so
                // that no extra attribute is added to the method.
                Test test = TestBuilder.MakeParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithAllDataSuppliedByAttributes));
                TestAssert.IsRunnable(test);
                Assert.That(test.TestCaseCount, Is.EqualTo(4));
            }

            [Test]
            public void FailsIfAllTestsAreInconclusive()
            {
                ITestResult result = TestBuilder.RunParameterizedMethodSuite(NestedType, nameof(TheoryFixture.NestedWhileSearchingInDeclaringType.WithAllBadValues));
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.Message, Is.EqualTo("All test cases were inconclusive"));
            }
        }

        public class NestedSqrtTests
        {
            [Datapoint]
            public double Zero = 0;

            [Datapoint]
            public double Positive = 1;

            [Datapoint]
            public double Negative = -1;

            [Datapoint]
            public double Max = double.MaxValue;

            [Datapoint]
            public double Infinity = double.PositiveInfinity;

            public class NestedTestFixture
            {
                [Theory(true)]
                public void SqrtTimesItselfGivesOriginal(double num)
                {
                    Assume.That(num >= 0.0 && num < double.MaxValue);

                    double sqrt = Math.Sqrt(num);

                    Assert.That(sqrt, Is.GreaterThanOrEqualTo(0.0));
                    Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
                }
            }
        }
    }
}
