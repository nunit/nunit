// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public class AssertionResultTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Success, "Test passed!");

            RecordExpectedAssertions(
                new AssertionResult(AssertionStatus.Passed, "Message 1", "Stack 1"),
                new AssertionResult(AssertionStatus.Failed, "Message 2", "Stack 2"));

            _suiteResult.AddResult(_testResult);
        }

        private readonly List<AssertionResult> _expectedAssertions = new List<AssertionResult>();

        protected void RecordExpectedAssertions(params AssertionResult[] expectedAssertions)
        {
            _expectedAssertions.AddRange(expectedAssertions);

            foreach (var assertion in _expectedAssertions)
                _testResult.RecordAssertion(assertion);
        }

        [Test]
        public void TestResult_AssertionResults()
        {
            Assert.AreEqual(_expectedAssertions, _testResult.AssertionResults);
        }

        [Test]
        public void SuiteResult_AssertionResults()
        {
            Assert.IsEmpty(_suiteResult.AssertionResults);
        }

        [Test]
        public void TestResultXml_AssertionResults()
        {
            TNode assertionResults = _testResult.ToXml(true).SelectSingleNode("assertions");

            if (_expectedAssertions.Count == 0)
            {
                Assert.Null(assertionResults, "No <assertions> element expected");
                return;
            }

            Assert.NotNull(assertionResults, "Expected <assertions> element");

            var assertionNodes = assertionResults.SelectNodes("assertion");
            Assert.NotNull(assertionNodes, "Empty <assertions> element");

            Assert.AreEqual(_expectedAssertions.Count, assertionNodes.Count, "Wrong number of <assertion> elements");

            for (int index = 0; index < _expectedAssertions.Count; index++)
            {
                AssertionResult expectedAssertion = _expectedAssertions[index];
                TNode assertionNode = assertionNodes[index];

                Assert.AreEqual(expectedAssertion.Status.ToString(), assertionNode.Attributes["result"]);

                if (expectedAssertion.Message != null)
                {
                    TNode messageNode = assertionNode.SelectSingleNode("message");
                    Assert.NotNull(messageNode);
                    Assert.That(messageNode.Value, Is.EqualTo(expectedAssertion.Message));
                }

                if (expectedAssertion.StackTrace != null)
                {
                    TNode stackNode = assertionNode.SelectSingleNode("stack-trace");
                    Assert.NotNull(stackNode);
                    Assert.That(stackNode.Value, Is.EqualTo(expectedAssertion.StackTrace));
                }
            }
        }

        [Test]
        public void SuiteResultXml_AssertionResults()
        {
            TNode suiteNode = _suiteResult.ToXml(true);
            Assert.Null(suiteNode.SelectSingleNode("assertions"));
        }
    }
}
