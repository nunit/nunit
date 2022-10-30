// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionCommand handles a single ITestAction applied
    /// to a test. It runs the BeforeTest method, then runs the
    /// test and finally runs the AfterTest method.
    /// </summary>
    public abstract class BeforeAndAfterTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public BeforeAndAfterTestCommand(TestCommand innerCommand) : base(innerCommand) { }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            Guard.OperationValid(BeforeTest != null, "BeforeTest was not set by the derived class constructor");
            Guard.OperationValid(AfterTest != null, "AfterTest was not set by the derived class constructor");

            Test.Fixture ??= context.TestObject;

            RunTestMethodInThreadAbortSafeZone(context, () =>
            {
                BeforeTest(context);
                context.CurrentResult = innerCommand.Execute(context);
            });

            if (context.ExecutionStatus != TestExecutionStatus.AbortRequested)
            {
                RunTestMethodInThreadAbortSafeZone(context, () => AfterTest(context));
            }

            return context.CurrentResult;
        }

        /// <summary>
        /// Perform the before test action
        /// </summary>
        protected Action<TestExecutionContext>? BeforeTest;

        /// <summary>
        /// Perform the after test action
        /// </summary>
        protected Action<TestExecutionContext>? AfterTest;
    }
}
