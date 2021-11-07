// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.TestData;

namespace NUnit.Framework.Internal
{
    public class AsyncSetupTeardownTests
    {
        private AsyncSetupTearDownFixture _testObject;
        private TestExecutionContext _context;
        private static readonly IList<IMethodInfo> Empty = new IMethodInfo[0];

        [SetUp]
        public void Setup()
        {
            _testObject = new AsyncSetupTearDownFixture();
            var method = Success.ElementAt(0);
            _context = new TestExecutionContext { TestObject = _testObject, CurrentResult = new TestCaseResult(new TestMethod(method)) };
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

        private IEnumerable<IMethodInfo> Success
        {
            get { yield return Method("SuccessfulAsyncMethod"); }
        }
        private IEnumerable<IMethodInfo> Failure
        {
            get { yield return Method("FailingAsyncMethod"); }
        }

        private IMethodInfo Method(string methodName)
        {
            return new MethodWrapper(_testObject.GetType(), _testObject.GetType().GetMethod(methodName));
        }
    }
}
