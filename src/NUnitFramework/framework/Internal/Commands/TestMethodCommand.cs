// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestMethodCommand is the lowest level concrete command
    /// used to run actual test cases.
    /// </summary>
    public class TestMethodCommand : TestCommand
    {
        private readonly TestMethod _testMethod;
        private readonly object?[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodCommand"/> class.
        /// </summary>
        /// <param name="testMethod">The test.</param>
        public TestMethodCommand(TestMethod testMethod) : base(testMethod)
        {
            _testMethod = testMethod;
            _arguments = testMethod.Arguments;
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

            var result = RunTestMethod(context);

            if (_testMethod.HasExpectedResult)
                Assert.That(result, Is.EqualTo(_testMethod.ExpectedResult));

            if (context.MultipleAssertLevel > 0)
                context.CurrentResult.SetResult(ResultState.Error, $"Test completed with {context.MultipleAssertLevel} active assertion scopes.");
            else
                context.CurrentResult.SetResult(ResultState.Success);

            if (context.CurrentResult.AssertionResults.Count > 0)
                context.CurrentResult.RecordTestCompletion();

            return context.CurrentResult;
        }

        private object? RunTestMethod(TestExecutionContext context)
        {
            var methodInfo = MethodInfoCache.Get(_testMethod.Method);

            bool lastParameterAcceptsCancellationToken = methodInfo.Parameters.LastParameterAcceptsCancellationToken();

            if (methodInfo.IsAsyncOperation)
            {
                return AsyncToSyncAdapter.Await(() => InvokeTestMethod(context, lastParameterAcceptsCancellationToken));
            }

            return InvokeTestMethod(context, lastParameterAcceptsCancellationToken);
        }

        private object? InvokeTestMethod(TestExecutionContext context, bool lastParameterAcceptsCancellationToken)
        {
            object?[] arguments = _arguments;

            if (lastParameterAcceptsCancellationToken &&
                !arguments.LastArgumentIsCancellationToken())
            {
                // Add our CancellationToken as an extra argument to the test method
                arguments = new object?[_arguments.Length + 1];
                Array.Copy(_arguments, arguments, _arguments.Length);
                arguments[_arguments.Length] = context.CancellationToken;
            }

            try
            {
                context.HookExtension?.OnBeforeTest(context, _testMethod);
                return _testMethod.Method.Invoke(context.TestObject, arguments);
            }
            finally
            {
                context.HookExtension?.OnAfterTest(context, _testMethod);
            }
        }
    }
}
