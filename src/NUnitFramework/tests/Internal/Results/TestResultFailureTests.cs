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
    public class TestResultFailureWithReasonAndStackGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithReasonAndStackGivenTests()
            : base(NonWhitespaceFailureMessage,
                  NonWhitespaceFailureStackTrace,
                  FailureNodeExistsAndIsNotNull,
                  tnode => MessageNodeExistsAndValueAsExpected(tnode, NonWhitespaceFailureMessage),
                  tnode => StackTraceNodeExistsAndValueAsExpected(tnode, NonWhitespaceFailureStackTrace))
        {
        }
    }

    public class TestResultFailureWithReasonGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithReasonGivenTests()
            : base(NonWhitespaceFailureMessage,
                  string.Empty,
                  FailureNodeExistsAndIsNotNull,
                  tnode => MessageNodeExistsAndValueAsExpected(tnode, NonWhitespaceFailureMessage),
                  StackTraceNodeDoesNotExist)
        {
        }
    }

    public class TestResultFailureWithStackTraceGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithStackTraceGivenTests()
            : base(string.Empty,
                  NonWhitespaceFailureStackTrace,
                  FailureNodeExistsAndIsNotNull,
                  MessageNodeDoesNotExist,
                  tnode => StackTraceNodeExistsAndValueAsExpected(tnode, NonWhitespaceFailureStackTrace))
        {
        }
    }

    public class TestResultFailureWithNullReasonAndStackTraceGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithNullReasonAndStackTraceGivenTests()
            : base(null,
                  null,
                  FailureNodeExistsAndIsNotNull,
                  MessageNodeDoesNotExist,
                  StackTraceNodeDoesNotExist)
        {
        }
    }

    public class TestResultFailureWithEmptyReasonAndStackTraceGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithEmptyReasonAndStackTraceGivenTests()
            : base(string.Empty,
                  string.Empty,
                  FailureNodeExistsAndIsNotNull,
                  MessageNodeDoesNotExist,
                  StackTraceNodeDoesNotExist)
        {
        }
    }

    public class TestResultFailureWithWhitespaceReasonAndStackTraceGivenTests : TestResultFailureTests
    {
        public TestResultFailureWithWhitespaceReasonAndStackTraceGivenTests()
            : base(" ",
                  "  ",
                  FailureNodeExistsAndIsNotNull,
                  MessageNodeDoesNotExist,
                  StackTraceNodeDoesNotExist)
        {
        }
    }

    public abstract class TestResultFailureTests : TestResultTests
    {
        protected const string NonWhitespaceFailureMessage = "message";
        protected const string NonWhitespaceFailureStackTrace = "stack_trace";

        protected string _failureReason;
        protected string _stackTrace;
        private readonly Func<TNode, TNode> _xmlFailureNodeValidation;
        private readonly Action<TNode> _xmlMessageNodeValidation;
        private readonly Action<TNode> _xmlStackTraceNodeValidation;

        protected TestResultFailureTests(string failureReason,
            string stackTrace,
            Func<TNode, TNode> xmlFailureNodeValidation,
            Action<TNode> xmlMessageNodeValidation,
            Action<TNode> xmlStackTraceNodeValidation)
        {
            _failureReason = failureReason;
            _stackTrace = stackTrace;
            _xmlFailureNodeValidation = xmlFailureNodeValidation;
            _xmlMessageNodeValidation = xmlMessageNodeValidation;
            _xmlStackTraceNodeValidation = xmlStackTraceNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Failure, _failureReason, _stackTrace);
            _testResult.AssertCount = 3;

            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, _testResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, _testResult.ResultState.Status);
            Assert.AreEqual(_failureReason, _testResult.Message);
            Assert.AreEqual(_stackTrace, _testResult.StackTrace);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.ChildFailure, _suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, _suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, _suiteResult.Message);
            Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.That(_suiteResult.StackTrace, Is.Null);

            Assert.AreEqual(0, _suiteResult.PassCount);
            Assert.AreEqual(1, _suiteResult.FailCount);
            Assert.AreEqual(0, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(0, _suiteResult.InconclusiveCount);
            Assert.AreEqual(3, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsFailure()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual("Failed", testNode.Attributes["result"]);
            Assert.AreEqual(null, testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);

            var failureNode = _xmlFailureNodeValidation(testNode);
            _xmlMessageNodeValidation(failureNode);
            _xmlStackTraceNodeValidation(failureNode);
        }

        [Test]
        public void TestResultXmlNodeEscapesInvalidXmlCharacters()
        {
            _testResult.SetResult( ResultState.Failure, "Invalid Characters: \u0001\u0008\u000b\u001f\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00" );
            TNode testNode = _testResult.ToXml( true );
            TNode failureNode = testNode.SelectSingleNode( "failure" );

            Assert.That( failureNode, Is.Not.Null, "No <failure> element found" );

            TNode messageNode = failureNode.SelectSingleNode( "message" );

            Assert.That( messageNode, Is.Not.Null, "No <message> element found" );
            Assert.That( messageNode.Value, Is.EqualTo( "Invalid Characters: \\u0001\\u0008\\u000b\\u001f\\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00" ) );
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);

            TNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.Null(stacktraceNode, "Unexpected <stack-trace> element found");

            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("3", suiteNode.Attributes["asserts"]);
        }

        protected static TNode FailureNodeExistsAndIsNotNull(TNode testNode)
        {
            TNode failureNode = testNode.SelectSingleNode("failure");
            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            return failureNode;
        }

        protected static void MessageNodeDoesNotExist(TNode failureNode)
        {
            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Null, "<message> element found but no such node was expected");
        }

        protected static void MessageNodeExistsAndValueAsExpected(TNode failureNode, string expectedFailureMessage)
        {
            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo(expectedFailureMessage));
        }

        protected static void StackTraceNodeDoesNotExist(TNode failureNode)
        {
            TNode stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.That(stacktraceNode, Is.Null, "<stack-trace> element found but was not expected");
        }

        protected static void StackTraceNodeExistsAndValueAsExpected(TNode failureNode, string expectedStackTrace)
        {
            TNode stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.That(stacktraceNode, Is.Not.Null, "No <stack-trace> element found");
            Assert.That(stacktraceNode.Value, Is.EqualTo(expectedStackTrace));
        }
    }
}
