// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class AssertionResultTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.Success, "Test passed!");

            RecordExpectedAssertions(
                new AssertionResult(AssertionStatus.Passed, "Message 1", "Stack 1"),
                new AssertionResult(AssertionStatus.Failed, "Message 2", "Stack 2"));

            SuiteResult.AddResult(TestResult);
        }

        private readonly List<AssertionResult> _expectedAssertions = new List<AssertionResult>();

        protected void RecordExpectedAssertions(params AssertionResult[] expectedAssertions)
        {
            _expectedAssertions.AddRange(expectedAssertions);

            foreach (var assertion in _expectedAssertions)
                TestResult.RecordAssertion(assertion);
        }

        [Test]
        public void TestResult_AssertionResults()
        {
            Assert.That(TestResult.AssertionResults, Is.EqualTo(_expectedAssertions));
        }

        [Test]
        public void SuiteResult_AssertionResults()
        {
            Assert.That(SuiteResult.AssertionResults, Is.Empty);
        }

        [Test]
        public void TestResultXml_AssertionResults()
        {
            TNode? assertionResults = TestResult.ToXml(true).SelectSingleNode("assertions");

            if (_expectedAssertions.Count == 0)
            {
                Assert.That(assertionResults, Is.Null, "No <assertions> element expected");
                return;
            }

            Assert.That(assertionResults, Is.Not.Null, "Expected <assertions> element");

            var assertionNodes = assertionResults.SelectNodes("assertion");
            Assert.That(assertionNodes, Is.Not.Null, "Empty <assertions> element");

            Assert.That(assertionNodes, Has.Count.EqualTo(_expectedAssertions.Count), "Wrong number of <assertion> elements");

            for (int index = 0; index < _expectedAssertions.Count; index++)
            {
                AssertionResult expectedAssertion = _expectedAssertions[index];
                TNode assertionNode = assertionNodes[index];

                Assert.That(assertionNode.Attributes["result"], Is.EqualTo(expectedAssertion.Status.ToString()));

                if (expectedAssertion.Message is not null)
                {
                    TNode? messageNode = assertionNode.SelectSingleNode("message");
                    Assert.That(messageNode, Is.Not.Null);
                    Assert.That(messageNode.Value, Is.EqualTo(expectedAssertion.Message));
                }

                if (expectedAssertion.StackTrace is not null)
                {
                    TNode? stackNode = assertionNode.SelectSingleNode("stack-trace");
                    Assert.That(stackNode, Is.Not.Null);
                    Assert.That(stackNode.Value, Is.EqualTo(expectedAssertion.StackTrace));
                }
            }
        }

        [Test]
        public void SuiteResultXml_AssertionResults()
        {
            TNode suiteNode = SuiteResult.ToXml(true);
            Assert.That(suiteNode.SelectSingleNode("assertions"), Is.Null);
        }
    }
}
