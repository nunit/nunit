// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
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

        private readonly string? _failureReason;
        private readonly string? _stackTrace;
        private readonly Func<TNode, TNode> _xmlFailureNodeValidation;
        private readonly Action<TNode> _xmlMessageNodeValidation;
        private readonly Action<TNode> _xmlStackTraceNodeValidation;

        protected TestResultFailureTests(string? failureReason,
            string? stackTrace,
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
            TestResult.SetResult(ResultState.Failure, _failureReason, _stackTrace);
            TestResult.AssertCount = 3;

            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(TestResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(TestResult.Message, Is.EqualTo(_failureReason));
                Assert.That(TestResult.StackTrace, Is.EqualTo(_stackTrace));
            });
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(SuiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(SuiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(SuiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(SuiteResult.StackTrace, Is.Null);
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(0));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(1));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void TestResultXmlNodeIsFailure()
        {
            TNode testNode = TestResult.ToXml(true);

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
            TestResult.SetResult(ResultState.Failure, "Invalid Characters: \u0001\u0008\u000b\u001f\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00");
            TNode testNode = TestResult.ToXml(true);
            TNode? failureNode = testNode.SelectSingleNode("failure");

            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            TNode? messageNode = failureNode.SelectSingleNode("message");

            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo("Invalid Characters: \\u0001\\u0008\\u000b\\u001f\\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00"));
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
            });
            TNode? failureNode = suiteNode.SelectSingleNode("failure");
            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            TNode? messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));

            TNode? stacktraceNode = failureNode.SelectSingleNode("stacktrace");
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
            TNode? failureNode = testNode.SelectSingleNode("failure");
            Assert.That(failureNode, Is.Not.Null, "No <failure> element found");

            return failureNode;
        }

        protected static void MessageNodeDoesNotExist(TNode failureNode)
        {
            TNode? messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Null, "<message> element found but no such node was expected");
        }

        protected static void MessageNodeExistsAndValueAsExpected(TNode failureNode, string expectedFailureMessage)
        {
            TNode? messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Not.Null, "No <message> element found");
            Assert.That(messageNode.Value, Is.EqualTo(expectedFailureMessage));
        }

        protected static void StackTraceNodeDoesNotExist(TNode failureNode)
        {
            TNode? stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.That(stacktraceNode, Is.Null, "<stack-trace> element found but was not expected");
        }

        protected static void StackTraceNodeExistsAndValueAsExpected(TNode failureNode, string expectedStackTrace)
        {
            TNode? stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.That(stacktraceNode, Is.Not.Null, "No <stack-trace> element found");
            Assert.That(stacktraceNode.Value, Is.EqualTo(expectedStackTrace));
        }
    }
}
