// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertWarnTests
    {
        [Test]
        public void AssertWarnWorksWithMessage()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "CallAssertWarnWithMessage");

            Assert.AreEqual(ResultState.Warning, result.ResultState);
            Assert.AreEqual("MESSAGE", result.Message);
        }

        [Test]
        public void AssertWarnWorksWithMessageAndArgs()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "CallAssertWarnWithMessageAndArgs");

            Assert.AreEqual(ResultState.Warning, result.ResultState);
            Assert.AreEqual("MESSAGE: 2+2=4", result.Message);
        }

        [Test]
        public void WarningsAreDisplayedWithFailure()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndFailure");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("This fails"));
        }

        [Test, Ignore("Currently Fails: Ignored message is displayed without the warnings")]
        public void WarningsAreDisplayedWithIgnore()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndIgnore");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("Ignore this"));
        }

        [Test, Ignore("Currently Fails: Inconclusive message is displayed without the warnings")]
        public void WarningsAreDisplayedWithInconclusive()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndInconclusive");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("This is inconclusive"));
        }
    }
}
