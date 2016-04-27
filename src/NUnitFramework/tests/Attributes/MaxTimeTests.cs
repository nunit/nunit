// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
#if !PORTABLE
using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Tests for MaxTime decoration.
    /// </summary>
    [TestFixture, Parallelizable(ParallelScope.None)]
    public class MaxTimeTests
    {
        [Test,MaxTime(1000)]
        public void MaxTimeNotExceeded()
        {
        }

        // TODO: We need a way to simulate the clock reliably
        [Test]
        public void MaxTimeExceeded()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixture));
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.Message, Does.Contain("exceeds maximum of 1ms"));
        }

        [Test]
        public void MaxTimeExceededOnTestCase()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithTestCase));
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            ITestResult result = suiteResult.Children.ToArray()[0].Children.ToArray()[0];
            Assert.That(result.Message, Does.Contain("exceeds maximum of 1ms"));
        }

        [Test, MaxTime(1000)]
        public void FailureReport()
        {
            Assert.That(
                () => Assert.Fail("Intentional Failure"),
                Throws.TypeOf<AssertionException>().With.Message.EqualTo("Intentional Failure"));
        }

        [Test]
        public void FailureReportHasPriorityOverMaxTime()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithFailure));
            Assert.AreEqual(ResultState.ChildFailure, result.ResultState);
            result = (TestResult)result.Children.ToArray()[0];
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.That(result.Message, Is.EqualTo("Intentional Failure"));
        }

        [Test]
        public void ErrorReportHasPriorityOverMaxTime()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithError));
            Assert.AreEqual(ResultState.ChildFailure, result.ResultState);
            result = result.Children.ToArray()[0];
            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.That(result.Message, Does.Contain("Exception message"));
        }
    }
}
#endif