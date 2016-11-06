// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

namespace NUnit.Framework.Assertions.Tests
{
    public class AssertMultipleTests
    {
        private static readonly ComplexNumber number = new ComplexNumber(5.2, 3.9);

        [TestCase("EmptyBlock", 0)]
        [TestCase("SingleAssert", 1)]
        [TestCase("TwoAsserts", 2)]
        [TestCase("ThreeAsserts", 3)]
        [TestCase("NestedBlock", 3)]
        [TestCase("TwoNestedBlocks", 3)]
        [TestCase("NestedBlocksInMethodCalls", 3)]
        public void AssertMultipleSucceeds(string methodName, int asserts)
        {
            var result = TestBuilder.RunTestCase(typeof(AssertMultipleSuccessFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.AssertCount, Is.EqualTo(asserts));
            Assert.IsEmpty(result.AssertionResults);
        }

        [TestCase("TwoAsserts_FirstAssertFails", 2, "RealPart")]
        [TestCase("TwoAsserts_SecondAssertFails", 2, "ImaginaryPart")]
        [TestCase("TwoAsserts_BothAssertsFail", 2, "RealPart", "ImaginaryPart")]
        [TestCase("NestedBlock_FirstAssertFails", 3, "Expected: 5")]
        [TestCase("NestedBlock_TwoAssertsFail", 3, "Expected: 5", "ImaginaryPart")]
        [TestCase("TwoNestedBlocks_FirstAssertFails", 3, "Expected: 5")]
        [TestCase("TwoNestedBlocks_TwoAssertsFail", 3, "Expected: 5", "ImaginaryPart")]
        public void AssertMultipleFails(string methodName, int asserts, params string[] failureMessageRegex)
        {
            var result = TestBuilder.RunTestCase(typeof(AssertMultipleFailureFixture), methodName);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.AssertCount, Is.EqualTo(asserts), "AssertCount");

            int expectedFailures = failureMessageRegex.Length;
            int actualFailures = result.AssertionResults.Count;
            Assert.That(actualFailures, Is.EqualTo(expectedFailures), "FailureCount");

            if (actualFailures > 0)
            {
                Assert.That(result.Message, Contains.Substring(
                    string.Format("Multiple Assert block had {0} failure(s)", actualFailures)));

                int i = 0;
                foreach (var failure in result.AssertionResults)
                {
                    // Since the order of argument evaluation is not guaranteed, we don't
                    // want 'i' to appear more than once in the Assert statement.
                    string errmsg = string.Format("AssertionResult {0}", i + 1);
                    Assert.That(failure.Message, Does.Match(failureMessageRegex[i++]), errmsg);
                    Assert.That(result.Message, Contains.Substring(failure.Message),
                        "Failure message should contain AssertionResult message");

                    // NOTE: This test expects the stack trace to contain the name of the method 
                    // that actually caused the failure. To ensure it is not optimized away, we
                    // compile the testdata assembly with optimizations disabled.
                    Assert.That(failure.StackTrace, Is.Not.Null.And.Contains(methodName));
                }

                Assert.That(result.StackTrace, Is.Not.Null.And.Contains(methodName));
            }
        }

        [Test, Explicit("Used to display error message for visual confirmation")]
        public void MultipleAssertFailureDemo()
        {
            Assert.Multiple(() =>
            {
                Assert.That(number.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(number.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        private class ComplexNumber
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
}
