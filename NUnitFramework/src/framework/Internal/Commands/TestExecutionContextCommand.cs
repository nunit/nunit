using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestExecutionContextCommand : DelegatingTestCommand
    {
        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="innerCommand"></param>
        public TestExecutionContextCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
        }

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(object testObject, ITestListener listener)
        {
            listener.TestStarted(Test);

            long startTime = DateTime.Now.Ticks;

            TestExecutionContext.Save();

            TestExecutionContext.CurrentContext.CurrentTest = this.Test;
            TestExecutionContext.CurrentContext.CurrentResult = this.Test.MakeTestResult();

            try
            {
                return innerCommand.Execute(testObject, listener);
            }
                // TODO: Ensure no exceptions escape
            finally
            {
                CurrentResult.AssertCount = TestExecutionContext.CurrentContext.AssertCount;

                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                CurrentResult.Time = time;

                listener.TestFinished(CurrentResult);

                TestExecutionContext.Restore();
            }
        }
    }
}
