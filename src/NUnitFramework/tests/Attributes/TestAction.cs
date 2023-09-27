// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Attributes
{
    public sealed class TestAction
    {
        public string Name { get; }
        public Action Action { get; }
        public Action<ITestResult> Assertion { get; }

        public TestAction(Action action, Action<ITestResult> assertion, string name)
        {
            Action = action;
            Assertion = assertion;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        private const string FailureMessage = "The test has failed";
        private const string IgnoreMessage = "The test was ignored";
        private const string InconclusiveMessage = "The test was inconclusive";

        internal static IEnumerable<TestAction> PossibleTestOutcomes
        {
            get
            {
                yield return new TestAction(Assert.Pass, AssertPassedResult, "Pass");
                yield return new TestAction(() => throw new Exception(), AssertExceptionResult, "Exception");
                yield return new TestAction(() => Assert.Fail(FailureMessage), AssertFailResult, "Fail");
                yield return new TestAction(() => Assert.Ignore(IgnoreMessage), AssertIgnoreResult, "Ignore");
                yield return new TestAction(() => Assert.Inconclusive(InconclusiveMessage), AssertInconclusiveResult, "Inconclusive");
                yield return new TestAction(MultipleFail, AssertFailResult, "Multiple > Fail");
                yield return new TestAction(AsynchronousMultipleFail, AssertFailResult, "Multiple > Async Fail");
            }
        }

        private static void MultipleFail()
        {
            Assert.Multiple(() => Assert.Fail(FailureMessage));
        }

        private static void AsynchronousMultipleFail()
        {
            Assert.Multiple(async () =>
            {
                await Task.Yield();
                Assert.Fail(FailureMessage);
            });
        }

        private static void AssertPassedResult(ITestResult result)
        {
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        private static void AssertExceptionResult(ITestResult result)
        {
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.ResultState.Label, Is.EqualTo(ResultState.Error.Label));
        }

        private static void AssertFailResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Failed, FailureMessage);
        }

        private static void AssertIgnoreResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Skipped, IgnoreMessage);
        }

        private static void AssertInconclusiveResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Inconclusive, InconclusiveMessage);
        }

        private static void AssertOutcome(ITestResult result, TestStatus status, string message)
        {
            Assert.That(result.ResultState.Status, Is.EqualTo(status));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Is.EqualTo(message));
        }
    }
}
