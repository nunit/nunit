// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestMethodSignatureFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class TestMethodSignatureTests
    {
        private static readonly Type FixtureType = typeof(TestMethodSignatureFixture);

        [Test]
        public void InstanceTestMethodIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.InstanceTestMethod) );
        }

        [Test]
        public void StaticTestMethodIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.StaticTestMethod) );
        }

        [Test]
        public void TestMethodWithoutParametersWithArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithoutParametersWithArgumentsProvided));
        }

        [Test]
        public void TestMethodWithArgumentsNotProvidedIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithArgumentsNotProvided));
        }

        [Test]
        public void TestMethodHasAttributesAppliedCorrectlyEvenIfNotRunnable()
        {
            var test = TestBuilder.MakeTestCase(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithArgumentsNotProvidedAndExtraAttributes));
            Assert.Multiple(() =>
            {
                // NOTE: IgnoreAttribute has no effect, either on RunState or on the reason
                Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("No arguments were provided"));
                Assert.That(test.Properties.Get(PropertyNames.Description), Is.EqualTo("My test"));
                Assert.That(test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(47));
            });
        }

        [Test]
        public void TestMethodWithArgumentsProvidedIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithArgumentsProvided));
        }

        [Test]
        public void TestMethodWithWrongNumberOfArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithWrongNumberOfArgumentsProvided));
        }

        [Test]
        public void TestMethodWithWrongArgumentTypesProvidedGivesError()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithWrongArgumentTypesProvided), ResultState.Error);
        }

        [Test]
        public void StaticTestMethodWithArgumentsNotProvidedIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.StaticTestMethodWithArgumentsNotProvided));
        }

        [Test]
        public void StaticTestMethodWithArgumentsProvidedIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.StaticTestMethodWithArgumentsProvided));
        }

        [Test]
        public void StaticTestMethodWithWrongNumberOfArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.StaticTestMethodWithWrongNumberOfArgumentsProvided));
        }

        [Test]
        public void StaticTestMethodWithWrongArgumentTypesProvidedGivesError()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.StaticTestMethodWithWrongArgumentTypesProvided), ResultState.Error);
        }

        [Test]
        public void TestMethodWithConvertibleArgumentsIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithConvertibleArguments));
        }

        [Test]
        public void TestMethodWithNonConvertibleArgumentsGivesError()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithNonConvertibleArguments), ResultState.Error);
        }

        [Test]
        public void ProtectedTestMethodIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, "ProtectedTestMethod");
        }

        [Test]
        public void PrivateTestMethodIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, "PrivateTestMethod");
        }

        [Test]
        public void TestMethodWithReturnTypeIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithReturnValue_WithoutExpectedResult));
        }

        [Test]
        public void TestMethodWithExpectedReturnTypeIsRunnable()
        {
            TestAssert.IsRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithReturnValue_WithExpectedResult));
        }

        [Test]
        public void TestMethodWithExpectedReturnAndArgumentsIsNotRunnable()
        {
            TestAssert.IsNotRunnable(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithReturnValueAndArgs_WithExpectedResult));
        }

        [Test]
        public void TestMethodWithMultipleTestCasesExecutesMultipleTimes()
        {
            ITestResult result = TestBuilder.RunParameterizedMethodSuite(FixtureType, nameof(TestMethodSignatureFixture.TestMethodWithMultipleTestCases));

            Assert.That( result.ResultState, Is.EqualTo(ResultState.Success) );
            ResultSummary summary = new ResultSummary(result);
            Assert.That(summary.TestsRun, Is.EqualTo(3));
        }

        [Test]
        public void TestMethodWithMultipleTestCasesUsesCorrectNames()
        {
            string name = nameof(TestMethodSignatureFixture.TestMethodWithMultipleTestCases);
            string fullName = typeof (TestMethodSignatureFixture).FullName + "." + name;

            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, name);
            Assert.That(suite.TestCaseCount, Is.EqualTo(3));

            var names = new List<string>();
            var fullNames = new List<string>();

            foreach (Test test in suite.Tests)
            {
                names.Add(test.Name);
                fullNames.Add(test.FullName);
            }

            Assert.That(names, Has.Member(name + "(12,3,4)"));
            Assert.That(names, Has.Member(name + "(12,2,6)"));
            Assert.That(names, Has.Member(name + "(12,4,3)"));

            Assert.That(fullNames, Has.Member(fullName + "(12,3,4)"));
            Assert.That(fullNames, Has.Member(fullName + "(12,2,6)"));
            Assert.That(fullNames, Has.Member(fullName + "(12,4,3)"));
        }

        [Test]
        public void RunningTestsThroughFixtureGivesCorrectResults()
        {
            ITestResult result = TestBuilder.RunTestFixture(FixtureType);
            ResultSummary summary = new ResultSummary(result);

            Assert.That(
                summary.ResultCount,
                Is.EqualTo(TestMethodSignatureFixture.Tests));
            Assert.That(
                summary.TestsRun,
                Is.EqualTo(TestMethodSignatureFixture.Tests));
            Assert.That(
                summary.Failed,
                Is.EqualTo(TestMethodSignatureFixture.Failures + TestMethodSignatureFixture.Errors + TestMethodSignatureFixture.NotRunnable));
            Assert.That(
                summary.Skipped,
                Is.EqualTo(0));
        }
    }
}
