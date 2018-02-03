// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;
using NUnit.TestData.AssertMultipleData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    public class AssertMultipleTests
    {
        private static readonly ComplexNumber _complex = new ComplexNumber(5.2, 3.9);

        [OneTimeSetUp]
        public void CreateSampleFile()
        {

        }

        [TestCase("EmptyBlock", 0)]
        [TestCase("SingleAssertSucceeds", 1)]
        [TestCase("TwoAssertsSucceed", 2)]
        [TestCase("ThreeAssertsSucceed", 3)]
        [TestCase("NestedBlock_ThreeAssertsSucceed", 3)]
        [TestCase("TwoNestedBlocks_ThreeAssertsSucceed", 3)]
        [TestCase("NestedBlocksInMethodCalls", 3)]
#if ASYNC
        [TestCase("ThreeAssertsSucceed_Async", 3)]
        [TestCase("NestedBlock_ThreeAssertsSucceed_Async", 3)]
        [TestCase("TwoNestedBlocks_ThreeAssertsSucceed_Async", 3)]
#endif
        public void AssertMultipleSucceeds(string methodName, int asserts)
        {
            var result = TestBuilder.RunTestCase(typeof(AssertMultipleFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.AssertCount, Is.EqualTo(asserts));
            Assert.IsEmpty(result.AssertionResults);
        }

        [TestCase("TwoAsserts_FirstAssertFails", "RealPart")]
        [TestCase("TwoAsserts_SecondAssertFails", "ImaginaryPart")]
        [TestCase("TwoAsserts_BothAssertsFail", "RealPart", "ImaginaryPart")]
        [TestCase("NestedBlock_FirstAssertFails", "Expected: 5")]
        [TestCase("NestedBlock_TwoAssertsFail", "Expected: 5", "ImaginaryPart")]
        [TestCase("TwoNestedBlocks_FirstAssertFails", "Expected: 5")]
        [TestCase("TwoNestedBlocks_TwoAssertsFail", "Expected: 5", "ImaginaryPart")]
        [TestCase("MethodCallsFail", "Message from Assert.Fail")]
        [TestCase("MethodCallsFailAfterTwoAssertsFail", "Expected: 5", "ImaginaryPart", "Message from Assert.Fail")]
#if ASYNC
        [TestCase("TwoAsserts_BothAssertsFail_Async", "RealPart", "ImaginaryPart")]
        [TestCase("TwoNestedBlocks_TwoAssertsFail_Async", "Expected: 5", "ImaginaryPart")]
#endif
        public void AssertMultipleFails(string methodName, params string[] assertionMessageRegex)
        {
            CheckResult(methodName, ResultState.Failure, assertionMessageRegex);
        }

        [TestCase("ExceptionThrown")]
        [TestCase("ExceptionThrownAfterTwoFailures", "Failure 1", "Failure 2", "Simulated Error")]
        public void AssertMultipleErrorTests(string methodName, params string[] assertionMessageRegex)
        {
            ITestResult result = CheckResult(methodName, ResultState.Error, assertionMessageRegex);
            Assert.That(result.Message, Does.StartWith("System.Exception : Simulated Error"));//
        }

        [Test]
        public void AssertPassInBlockThrowsException()
        {
            ITestResult result = CheckResult("AssertPassInBlock", ResultState.Error);
            Assert.That(result.Message, Contains.Substring("Assert.Pass may not be used in a multiple assertion block."));
        }

        [Test]
        public void AssertIgnoreInBlockThrowsException()
        {
            ITestResult result = CheckResult("AssertIgnoreInBlock", ResultState.Error);
            Assert.That(result.Message, Contains.Substring("Assert.Ignore may not be used in a multiple assertion block."));
        }

        [Test]
        public void AssertInconclusiveInBlockThrowsException()
        {
            ITestResult result = CheckResult("AssertInconclusiveInBlock", ResultState.Error);
            Assert.That(result.Message, Contains.Substring("Assert.Inconclusive may not be used in a multiple assertion block."));
        }

        [Test]
        public void AssumptionInBlockThrowsException()
        {
            ITestResult result = CheckResult("AssumptionInBlock", ResultState.Error);
            Assert.That(result.Message, Contains.Substring("Assume.That may not be used in a multiple assertion block."));
        }

        private ITestResult CheckResult(string methodName, ResultState expectedResultState, params string[] assertionMessageRegex)
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(AssertMultipleFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(expectedResultState), "ResultState");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(assertionMessageRegex.Length), "Number of AssertionResults");
            Assert.That(result.StackTrace, Is.Not.Null.And.Contains(methodName), "StackTrace");

            if (result.AssertionResults.Count > 0)
            {
                int numFailures = result.AssertionResults.Count;
                if (expectedResultState == ResultState.Error)
                    --numFailures;

                if (numFailures > 1)
                    Assert.That(result.Message, Contains.Substring("Multiple failures or warnings in test:"));

                int i = 0;
                foreach (var assertion in result.AssertionResults)
                {
                    // Since the order of argument evaluation is not guaranteed, we don't
                    // want 'i' to appear more than once in the Assert statement.
                    string errmsg = string.Format("AssertionResult {0}", i + 1);
                    Assert.That(assertion.Message, Does.Match(assertionMessageRegex[i++]), errmsg);
                    Assert.That(result.Message, Contains.Substring(assertion.Message), errmsg);

                    // NOTE: This test expects the stack trace to contain the name of the method 
                    // that actually caused the failure. To ensure it is not optimized away, we
                    // compile the testdata assembly with optimizations disabled.
                    Assert.That(assertion.StackTrace, Is.Not.Null.And.Contains(methodName), errmsg);
                }
            }

            return result;
        }
    }

    [Explicit("Used to display error messages for visual confirmation")]
    public class MultipleAssertDemo
    {
        private static readonly ComplexNumber _complex = new ComplexNumber(5.2, 3.9);

        [Test]
        // Shows multiple failures including one from Assert.Fail
        public void MultipleAssertFailureDemo()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(_complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                Assert.Fail("Assert.Fail Called");
            });
        }

        [Test]
        // Shows two failures followed by an exception
        public void MultipleAssertErrorDemo()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(_complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                throw new Exception("Simulated Error");
            });
        }
    }

    internal class ComplexNumber
    {
        public ComplexNumber(double realPart, double imaginaryPart)
        {
            RealPart = realPart;
            ImaginaryPart = imaginaryPart;
        }

        public double RealPart;
        public double ImaginaryPart;
    }
}
