// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TheoryFixture;

namespace NUnit.Framework.Tests.Attributes
{
    public class TheoryTests
    {
        private static readonly Type FixtureType = typeof(TheoryFixture);

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
        private readonly string[] _vals = new string[] { "xyz1", "xyz2", "xyz3" };

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

        public class SqrtTests
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

            [Theory]
            public void SqrtTimesItselfGivesOriginal(double num)
            {
                Assume.That(num >= 0.0 && num < double.MaxValue);

                double sqrt = Math.Sqrt(num);

                Assert.That(sqrt, Is.GreaterThanOrEqualTo(0.0));
                Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
            }
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
