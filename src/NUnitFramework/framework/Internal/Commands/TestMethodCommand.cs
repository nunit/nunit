// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
                Assert.AreEqual(_testMethod.ExpectedResult, result);

            context.CurrentResult.SetResult(ResultState.Success);

            if (context.CurrentResult.AssertionResults.Count > 0)
                context.CurrentResult.RecordTestCompletion();

            return context.CurrentResult;
        }

        private object? RunTestMethod(TestExecutionContext context)
        {
            var methodInfo = MethodInfoCache.Get(_testMethod.Method);

            if (methodInfo.IsAsyncOperation)
            {
                return AsyncToSyncAdapter.Await(() => InvokeTestMethod(context));
            }

            return InvokeTestMethod(context);
        }

        private object? InvokeTestMethod(TestExecutionContext context)
        {
            return _testMethod.Method.Invoke(context.TestObject, _arguments);
        }
    }
}
