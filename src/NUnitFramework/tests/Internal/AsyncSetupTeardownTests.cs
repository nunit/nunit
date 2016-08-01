// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

#if NET_4_0 || NET_4_5 || PORTABLE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
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
            var method = new MethodWrapper(typeof(AsyncSetupTearDownFixture), Success.ElementAt(0));
            _context = new TestExecutionContext {TestObject = _testObject, CurrentResult = new TestCaseResult(new TestMethod(method))};
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