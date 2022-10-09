// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData.AssertMultipleData;
using NUnit.TestUtilities;
using AM = NUnit.TestData.AssertMultipleData.AssertMultipleFixture;

namespace NUnit.Framework.Assertions
{
    public class AssertMultipleTests
    {
        private static readonly ComplexNumber _complex = new ComplexNumber(5.2, 3.9);

        [TestCase(nameof(AM.EmptyBlock), 0)]
        [TestCase(nameof(AM.SingleAssertSucceeds), 1)]
        [TestCase(nameof(AM.TwoAssertsSucceed), 2)]
        [TestCase(nameof(AM.ThreeAssertsSucceed), 3)]
        [TestCase(nameof(AM.NestedBlock_ThreeAssertsSucceed), 3)]
        [TestCase(nameof(AM.TwoNestedBlocks_ThreeAssertsSucceed), 3)]
        [TestCase(nameof(AM.NestedBlocksInMethodCalls), 3)]
        [TestCase(nameof(AM.ThreeWarnIf_AllPass), 3)]
        [TestCase(nameof(AM.ThreeWarnUnless_AllPass), 3)]
        [TestCase(nameof(AM.ThreeAssertsSucceed_Async), 3)]
        [TestCase(nameof(AM.NestedBlock_ThreeAssertsSucceed_Async), 3)]
        [TestCase(nameof(AM.TwoNestedBlocks_ThreeAssertsSucceed_Async), 3)]
        public void AssertMultipleSucceeds(string methodName, int asserts)
        {
            CheckResult(methodName, ResultState.Success, asserts);
        }

        [TestCase(nameof(AM.TwoAsserts_FirstAssertFails), 2, "RealPart")]
        [TestCase(nameof(AM.TwoAsserts_SecondAssertFails), 2, "ImaginaryPart")]
        [TestCase(nameof(AM.TwoAsserts_BothAssertsFail), 2, "RealPart", "ImaginaryPart")]
        [TestCase(nameof(AM.NestedBlock_FirstAssertFails), 3, "Expected: 5")]
        [TestCase(nameof(AM.NestedBlock_TwoAssertsFail), 3, "Expected: 5", "ImaginaryPart")]
        [TestCase(nameof(AM.TwoNestedBlocks_FirstAssertFails), 3, "Expected: 5")]
        [TestCase(nameof(AM.TwoNestedBlocks_TwoAssertsFail), 3, "Expected: 5", "ImaginaryPart")]
        [TestCase(nameof(AM.MethodCallsFail), 0, "Message from Assert.Fail")]
        [TestCase(nameof(AM.MethodCallsFailAfterTwoAssertsFail), 2, "Expected: 5", "ImaginaryPart", "Message from Assert.Fail")]
        [TestCase(nameof(AM.TwoAssertsFailAfterWarning), 2, "WARNING", "Expected: 5", "ImaginaryPart")]
        [TestCase(nameof(AM.WarningAfterTwoAssertsFail), 2, "Expected: 5", "ImaginaryPart", "WARNING")]
        [TestCase(nameof(AM.TwoAsserts_BothAssertsFail_Async), 2, "RealPart", "ImaginaryPart")]
        [TestCase(nameof(AM.TwoNestedBlocks_TwoAssertsFail_Async), 3, "Expected: 5", "ImaginaryPart")]
        public void AssertMultipleFails(string methodName, int asserts, params string[] assertionMessages)
        {
            CheckResult(methodName, ResultState.Failure, asserts, assertionMessages);
        }

        [TestCase(nameof(AM.ThreeAssertWarns), 0, "WARNING1", "WARNING2", "WARNING3")]
        [TestCase(nameof(AM.ThreeWarnIf_TwoFail), 3, "WARNING1", "WARNING3")]
        [TestCase(nameof(AM.ThreeWarnUnless_TwoFail), 3, "WARNING1", "WARNING3")]
        public void AssertMultipleWarns(string methodName, int asserts, params string[] assertionMessages)
        {
            CheckResult(methodName, ResultState.Warning, asserts, assertionMessages);
        }

        [TestCase(nameof(AM.ExceptionThrown), 0)]
        [TestCase(nameof(AM.ExceptionThrownAfterTwoFailures), 2, "Failure 1", "Failure 2", "Simulated Error")]
        [TestCase(nameof(AM.ExceptionThrownAfterWarning), 0, "WARNING", "Simulated Error")]
        public void AssertMultipleErrorTests(string methodName, int asserts, params string[] assertionMessages)
        {
            ITestResult result = CheckResult(methodName, ResultState.Error, asserts, assertionMessages);
            Assert.That(result.Message, Does.StartWith("System.Exception : Simulated Error"));//
        }

        [TestCase(nameof(AM.AssertPassInBlock), "Assert.Pass")]
        [TestCase(nameof(AM.AssertIgnoreInBlock), "Assert.Ignore")]
        [TestCase(nameof(AM.AssertInconclusiveInBlock), "Assert.Inconclusive")]
        [TestCase(nameof(AM.AssumptionInBlock), "Assume.That")]
        public void AssertMultiple_InvalidAssertThrowsException(string methodName, string invalidAssert)
        {
            ITestResult result = CheckResult(methodName, ResultState.Error, 0);
            Assert.That(result.Message, Contains.Substring($"{invalidAssert} may not be used in a multiple assertion block."));
        }

        private ITestResult CheckResult(string methodName, ResultState expectedResultState, int expectedAsserts, params string[] assertionMessageRegex)
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(AssertMultipleFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(expectedResultState), "ResultState");
            Assert.That(result.AssertCount, Is.EqualTo(expectedAsserts), "AssertCount");
            Assert.That(result.AssertionResults.Count, Is.EqualTo(assertionMessageRegex.Length), "Number of AssertionResults");

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                if (result.ResultState.Status == TestStatus.Failed)
                    Assert.That(result.StackTrace, Is.Not.Null.And.Contains(methodName), "StackTrace");
            });

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
                    string errmsg = $"AssertionResult {i + 1}";
                    Assert.That(assertion.Message, Does.Match(assertionMessageRegex[i++]), errmsg);
                    Assert.That(result.Message, Contains.Substring(assertion.Message), errmsg);

                    // NOTE: This test expects the stack trace to contain the name of the method 
                    // that actually caused the failure. To ensure it is not optimized away, we
                    // compile the testdata assembly with optimizations disabled.

                    PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
                    {
                        Assert.That(assertion.StackTrace, Is.Not.Null.And.Contains(methodName), errmsg);
                    });
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
