// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Internal
{
    public class AsyncSetupTeardownTests
    {
        private AsyncSetupTearDownFixture _testObject;
        private TestExecutionContext _context;
        private static readonly IList<IMethodInfo> Empty = Array.Empty<IMethodInfo>();

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
        public void FailingSetupShouldShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Failure.ToList(), Empty);

            item.RunSetUp(_context);

            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.SetUpError));
        }

        [Test]
        public void FailingTeardownShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Empty, Failure.ToList());

            item.RunSetUp(_context);
            item.RunTearDown(_context);

            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.TearDownError));
        }

        [Test]
        public void SuccessfulThenFailingSetupShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Success.Concat(Failure).ToList(), Empty);

            item.RunSetUp(_context);
            Assert.That(_testObject.SuccessfulAsyncMethodRuns, Is.EqualTo(1));
            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.SetUpError));
        }

        [Test]
        public void SuccessfulThenFailingTeardownShouldRecordFailureInTestContext()
        {
            var item = new SetUpTearDownItem(Empty, Success.Concat(Failure).ToList());

            item.RunSetUp(_context);
            item.RunTearDown(_context);

            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.TearDownError));
        }

        private IEnumerable<IMethodInfo> Success
        {
            get { yield return Method(nameof(AsyncSetupTearDownFixture.SuccessfulAsyncMethod)); }
        }
        private IEnumerable<IMethodInfo> Failure
        {
            get { yield return Method(nameof(AsyncSetupTearDownFixture.FailingAsyncMethod)); }
        }

        private IMethodInfo Method(string methodName)
        {
            return new MethodWrapper(_testObject.GetType(), methodName);
        }
    }
}
