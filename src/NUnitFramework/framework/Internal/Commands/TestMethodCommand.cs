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

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestMethodCommand is the lowest level concrete command
    /// used to run actual test cases.
    /// </summary>
    public class TestMethodCommand : TestCommand
    {
        private readonly TestMethod testMethod;
        private readonly object[] arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodCommand"/> class.
        /// </summary>
        /// <param name="testMethod">The test.</param>
        public TestMethodCommand(TestMethod testMethod) : base(testMethod)
        {
            this.testMethod = testMethod;
            this.arguments = testMethod.Arguments;
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

            if (testMethod.HasExpectedResult)
                NUnit.Framework.Assert.AreEqual(testMethod.ExpectedResult, result);

            context.CurrentResult.SetResult(ResultState.Success);

            if (context.CurrentResult.AssertionResults.Count > 0)
                context.CurrentResult.RecordTestCompletion();

            return context.CurrentResult;
        }

        private object RunTestMethod(TestExecutionContext context)
        {
#if ASYNC
            if (AsyncToSyncAdapter.IsAsyncOperation(testMethod.Method))
            {
                try
                {
                    return AsyncToSyncAdapter.Await(() => InvokeTestMethod(context));
                }
                catch (Exception e)
                {
                    throw new NUnitException("Rethrown", e);
                }
            }
#endif
            return InvokeTestMethod(context);
        }

        private object InvokeTestMethod(TestExecutionContext context)
        {
            return Reflect.InvokeMethod(testMethod.Method, context.TestObject, arguments);
        }
    }
}
