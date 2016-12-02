// ***********************************************************************
// Copyright (c) 2010-2016 Charlie Poole
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

namespace NUnit.Framework.Internal.Results
{
    public class TestResultFailureTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Failure, "message", "stack_trace");
            _testResult.AssertCount = 3;

            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, _testResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, _testResult.ResultState.Status);
            Assert.AreEqual("message", _testResult.Message);
            Assert.AreEqual("stack_trace", _testResult.StackTrace);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.ChildFailure, _suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, _suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, _suiteResult.Message);
            Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.Null(_suiteResult.StackTrace);

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

            TNode failureNode = testNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual("message", messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.NotNull(stacktraceNode, "No <stack-trace> element found");
            Assert.That(stacktraceNode.Value, Is.EqualTo(_testResult.StackTrace));
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
    }
 }
