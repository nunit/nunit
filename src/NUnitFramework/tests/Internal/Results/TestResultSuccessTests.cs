// ***********************************************************************
// Copyright (c) 2010-2016 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Interfaces;
using System;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultSuccessWithReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithReasonGivenTests() : base(TestPassedReason, node => ReasonNodeExpectedValidation(node, TestPassedReason))
        {
        }
    }

    public class TestResultSuccessWithNullReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithNullReasonGivenTests() : base(null, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultSuccessWithEmptyReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithEmptyReasonGivenTests() : base(string.Empty, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestSuccessInconclusiveWithWhitespaceReasonGivenTests : TestResultSuccessTests
    {
        public TestSuccessInconclusiveWithWhitespaceReasonGivenTests() : base(" ", NoReasonNodeExpectedValidation)
        {
        }
    }

    public abstract class TestResultSuccessTests : TestResultTests
    {
        protected string _successMessage;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        public const string TestPassedReason = "Test passed!";

        protected TestResultSuccessTests(string ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _successMessage = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Success, _successMessage);
            _testResult.AssertCount = 2;

            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsSuccess()
        {
            Assert.True(_testResult.ResultState == ResultState.Success);
            Assert.AreEqual(_successMessage, _testResult.Message);
        }

        [Test]
        public void SuiteResultIsSuccess()
        {
            Assert.True(_suiteResult.ResultState == ResultState.Success);

            Assert.AreEqual(1, _suiteResult.PassCount);
            Assert.AreEqual(0, _suiteResult.FailCount);
            Assert.AreEqual(0, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(0, _suiteResult.InconclusiveCount);
            Assert.AreEqual(2, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsSuccess()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual("Passed", testNode.Attributes["result"]);
            Assert.AreEqual(null, testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);
            Assert.AreEqual("2", testNode.Attributes["asserts"]);

            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsSuccess()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Passed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual(null, suiteNode.Attributes["site"]);
            Assert.AreEqual("1", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("2", suiteNode.Attributes["asserts"]);
        }
    }
}
