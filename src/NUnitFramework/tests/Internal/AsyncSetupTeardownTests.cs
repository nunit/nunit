#if NET_4_0 || NET_4_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.TestData;

namespace NUnit.Framework.Internal
{
    public class AsyncSetupTeardownTests
    {
        private AsyncSetupTearDownFixture _testObject;
        private TestExecutionContext _context;
        private static readonly IList<MethodInfo> Empty = new MethodInfo[0];

        [SetUp]
        public void Setup()
        {
            _testObject = new AsyncSetupTearDownFixture();
            _context = new TestExecutionContext {TestObject = _testObject, CurrentResult = new TestCaseResult(new TestMethod(Success.ElementAt(0)))};
        }

        [Test]
        public void SuccessfulSetup()
        {
            var item = new SetUpTearDownItem(Success.ToList(), Empty);
            item.RunSetUp(_context);

            Assert.That(_testObject.SuccessfulAsyncMethodRuns, Is.EqualTo(1));
        }

        [Test]
        public void FailingSetupShouldLetExceptionBubbleUpUnwrapped()
        {
            var item = new SetUpTearDownItem(Failure.ToList(), Empty);

            Assert.Throws<InvalidOperationException>(() => item.RunSetUp(_context));
        }

        [Test]
        public void FailingTeardownShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Empty, Failure.ToList());

            item.RunSetUp(_context);
            item.RunTearDown(_context);

            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Error));
        }

        [Test]
        public void SuccessfulThenFailingSetupShouldLetExceptionBubbleUpUnwrapped()
        {
            var item = new SetUpTearDownItem(Success.Concat(Failure).ToList(), Empty);

            Assert.Throws<InvalidOperationException>(() => item.RunSetUp(_context));
            Assert.That(_testObject.SuccessfulAsyncMethodRuns, Is.EqualTo(1));
        }

        [Test]
        public void SuccessfulThenFailingTeardownShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Empty, Success.Concat(Failure).ToList());

            item.RunSetUp(_context);
            item.RunTearDown(_context);

            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Error));
        }

        private IEnumerable<MethodInfo> Success
        {
            get { yield return Method("SuccessfulAsyncMethod"); }
        }
        private IEnumerable<MethodInfo> Failure
        {
            get { yield return Method("FailingAsyncMethod"); }
        }

        private MethodInfo Method(string methodName)
        {
            return _testObject.GetType().GetMethod(methodName);
        }
    }
}
#endif