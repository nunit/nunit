// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
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

#if !THREAD_ABORT
using System;
using System.Threading;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestMethodCommand is the lowest level concrete command
    /// used to run actual test cases.
    /// </summary>
    public class TestMethodCommand : TestCommand
    {
        private readonly TestMethod _testMethod;
        private readonly object[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodCommand"/> class.
        /// </summary>
        /// <param name="testMethod">The test.</param>
        public TestMethodCommand(TestMethod testMethod) : base(testMethod)
        {
            this._testMethod = testMethod;
            this._arguments = testMethod.Arguments;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the execution context, as
        /// well as returning it. If the test has an expected result, it
        /// is asserts on that value. Since failed tests and errors throw
        /// an exception, this command must be wrapped in an outer command,
        /// will handle that exception and records the failure. This role
        /// is usually played by the SetUpTearDown command.
        /// </summary>
        /// <param name="context">The execution context</param>
        public override TestResult Execute(TestExecutionContext context)
        {
            // NOTE: Things would be cleaner if we could handle
            // exceptions in this command. However, that would
            // make it impossible to write a wrapper command to
            // implement ExpectedException, among other things.

            object result = RunTestMethod(context);

            if (_testMethod.HasExpectedResult)
                Assert.AreEqual(_testMethod.ExpectedResult, result);

            context.CurrentResult.SetResult(ResultState.Success);

            if (context.CurrentResult.AssertionResults.Count > 0)
                context.CurrentResult.RecordTestCompletion();

            return context.CurrentResult;
        }

        private object RunTestMethod(TestExecutionContext context)
        {
            var methodInfo = MethodInfoCache.Get(_testMethod.Method);

            if (methodInfo.IsAsyncOperation)
            {
                return AsyncToSyncAdapter.Await(() => InvokeTestMethod(context));
            }

            return InvokeTestMethod(context);
        }

        private object InvokeTestMethod(TestExecutionContext context)
        {
		    var methodInfo = MethodInfoCache.Get(_testMethod.Method);
		
            object[] arguments = _arguments;

#if !THREAD_ABORT
            if (AsyncToSyncAdapter.AcceptsCancellationToken(_testMethod.Method.MethodInfo) &&
                !AsyncToSyncAdapter.LastArgumentIsCancellationToken(_arguments))
            {
                arguments = new object[_arguments.Length + 1];
                Array.Copy(_arguments, arguments, _arguments.Length);
                arguments[_arguments.Length] = context.CancellationToken;
            }
#endif

            return _testMethod.Method.Invoke(context.TestObject, arguments);
        }
    }
}
