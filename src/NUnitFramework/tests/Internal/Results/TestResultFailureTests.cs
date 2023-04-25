// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            Assert.Multiple(() =>
            {
                Assert.That(_testResult.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(_testResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(_testResult.Message, Is.EqualTo(_failureReason));
                Assert.That(_testResult.StackTrace, Is.EqualTo(_stackTrace));
            });
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(_suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(_suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(_suiteResult.StackTrace, Is.Null);
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(0));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(1));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void TestResultXmlNodeIsFailure()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(testNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(testNode.Attributes["site"], Is.EqualTo(null));
            });

            var failureNode = _xmlFailureNodeValidation(testNode);
            _xmlMessageNodeValidation(failureNode);
            _xmlStackTraceNodeValidation(failureNode);
        }

        [Test]
        public void TestResultXmlNodeEscapesInvalidXmlCharacters()
        {
            _testResult.SetResult(ResultState.Failure, "Invalid Characters: \u0001\u0008\u000b\u001f\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00");
            TNode testNode = _testResult.ToXml(true);
            TNode failureNode = testNode.SelectSingleNode("failure");

            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");

            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo("Invalid Characters: \\u0001\\u0008\\u000b\\u001f\\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00"));
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
            });
            TNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));

            TNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.That(stacktraceNode, Is.Null, "Unexpected <stack-trace> element found");

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("3"));
            });
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
