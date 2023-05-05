// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    /// <summary>
    /// Abstract base for tests of TestResult
    /// </summary>
    [TestFixture]
    public abstract class TestResultTests
    {
        protected const string NonWhitespaceIgnoreReason = "because";
        protected TestMethod _test;
        protected TestResult _testResult;

        protected TestSuite _suite;
        protected TestSuiteResult _suiteResult;

        [SetUp]
        public void SetUp()
        {
            _test = new TestMethod(new MethodWrapper(typeof(DummySuite), "DummyMethod"));
            _testResult = _test.MakeTestResult();

            _suite = new TestSuite(typeof(DummySuite));
            _suiteResult = (TestSuiteResult)_suite.MakeTestResult();
        }

        protected static void ReasonNodeExpectedValidation(TNode testNode, string reasonMessage)
        {
            TNode? reason = testNode.SelectSingleNode("reason");
            Assert.That(reason, Is.Not.Null);
            Assert.That(reason.SelectSingleNode("message"), Is.Not.Null);
            Assert.That(reason.SelectSingleNode("message").Value, Is.EqualTo(reasonMessage));
            Assert.That(reason.SelectSingleNode("stack-trace"), Is.Null);
        }

        protected static void NoReasonNodeExpectedValidation(TNode testNode)
        {
            TNode? reason = testNode.SelectSingleNode("reason");
            Assert.That(reason, Is.Null, "This test expects no reason element to be present in the XML representation.");
        }

        #region Nested DummySuite

        public class DummySuite
        {
            public void DummyMethod() { }
        }

        #endregion
    }
 }
