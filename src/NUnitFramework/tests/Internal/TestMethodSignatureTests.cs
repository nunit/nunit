// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.TestData.TestMethodSignatureFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class TestMethodSignatureTests
    {
        private static Type fixtureType = typeof(TestMethodSignatureFixture);

        [Test]
        public void InstanceTestMethodIsRunnable()
        {
            TestAssert.IsRunnable( fixtureType, "InstanceTestMethod" );
        }

        [Test]
        public void StaticTestMethodIsRunnable()
        {
            TestAssert.IsRunnable( fixtureType, "StaticTestMethod" );
        }

        [Test]
        public void TestMethodWithoutParametersWithArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(fixtureType, "TestMethodWithoutParametersWithArgumentsProvided");
        }

        [Test]
        public void TestMethodWithArgumentsNotProvidedIsNotRunnable()
        {
            TestAssert.IsNotRunnable(fixtureType, "TestMethodWithArgumentsNotProvided");
        }

        [Test]
        public void TestMethodHasAttributesAppliedCorrectlyEvenIfNotRunnable()
        {
            var test = TestBuilder.MakeTestCase(fixtureType, "TestMethodWithArgumentsNotProvidedAndExtraAttributes");
            // NOTE: IgnoreAttribute has no effect, either on RunState or on the reason
            Assert.That(test.RunState == RunState.NotRunnable);
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("No arguments were provided"));
            Assert.That(test.Properties.Get(PropertyNames.Description), Is.EqualTo("My test"));
            Assert.That(test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(47));
        }

        [Test]
        public void TestMethodWithArgumentsProvidedIsRunnable()
        {
            TestAssert.IsRunnable(fixtureType, "TestMethodWithArgumentsProvided");
        }

        [Test]
        public void TestMethodWithWrongNumberOfArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(fixtureType, "TestMethodWithWrongNumberOfArgumentsProvided");
        }

        [Test]
        public void TestMethodWithWrongArgumentTypesProvidedGivesError()
        {
            TestAssert.IsRunnable(fixtureType, "TestMethodWithWrongArgumentTypesProvided", ResultState.Error);
        }

        [Test]
        public void StaticTestMethodWithArgumentsNotProvidedIsNotRunnable()
        {
            TestAssert.IsNotRunnable(fixtureType, "StaticTestMethodWithArgumentsNotProvided");
        }

        [Test]
        public void StaticTestMethodWithArgumentsProvidedIsRunnable()
        {
            TestAssert.IsRunnable(fixtureType, "StaticTestMethodWithArgumentsProvided");
        }

        [Test]
        public void StaticTestMethodWithWrongNumberOfArgumentsProvidedIsNotRunnable()
        {
            TestAssert.ChildNotRunnable(fixtureType, "StaticTestMethodWithWrongNumberOfArgumentsProvided");
        }

        [Test]
        public void StaticTestMethodWithWrongArgumentTypesProvidedGivesError()
        {
            TestAssert.IsRunnable(fixtureType, "StaticTestMethodWithWrongArgumentTypesProvided", ResultState.Error);
        }

        [Test]
        public void TestMethodWithConvertibleArgumentsIsRunnable()
        {
            TestAssert.IsRunnable(fixtureType, "TestMethodWithConvertibleArguments");
        }

        [Test]
        public void TestMethodWithNonConvertibleArgumentsGivesError()
        {
            TestAssert.IsRunnable(fixtureType, "TestMethodWithNonConvertibleArguments", ResultState.Error);
        }

        [Test]
        public void ProtectedTestMethodIsNotRunnable()
        {
            TestAssert.IsNotRunnable( fixtureType, "ProtectedTestMethod" );
        }

        [Test]
        public void PrivateTestMethodIsNotRunnable()
        {
            TestAssert.IsNotRunnable( fixtureType, "PrivateTestMethod" );
        }

        [Test]
        public void TestMethodWithReturnTypeIsNotRunnable()
        {
            TestAssert.IsNotRunnable( fixtureType, "TestMethodWithReturnType" );
        }

        [Test]
        public void TestMethodWithExpectedReturnTypeIsRunnable()
        {
            TestAssert.IsRunnable(fixtureType, "TestMethodWithExpectedReturnType");
        }

        [Test]
        public void TestMethodWithMultipleTestCasesExecutesMultipleTimes()
        {
            ITestResult result = TestBuilder.RunParameterizedMethodSuite(fixtureType, "TestMethodWithMultipleTestCases");

            Assert.That( result.ResultState, Is.EqualTo(ResultState.Success) );
            ResultSummary summary = new ResultSummary(result);
            Assert.That(summary.TestsRun, Is.EqualTo(3));
        }

        [Test]
        public void TestMethodWithMultipleTestCasesUsesCorrectNames()
        {
            string name = "TestMethodWithMultipleTestCases";
            string fullName = typeof (TestMethodSignatureFixture).FullName + "." + name;

            TestSuite suite = TestBuilder.MakeParameterizedMethodSuite(fixtureType, name);
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
            ITestResult result = TestBuilder.RunTestFixture(fixtureType);
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
