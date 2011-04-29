using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    public class CommandRunner
    {
        private ITestListener listener;

        /// <summary>
        /// Runs a TestCommand, sending notifications to a listener.
        /// </summary>
        /// <param name="command">A TestCommand to be executed</param>
        /// <param name="testObject">The object on which to run the tests</param>
        /// <param name="listener">An event listener to receive notifications</param>
        /// <returns>A TestResult.</returns>
        public static TestResult Execute(TestCommand command, object testObject, ITestListener listener)
        {
            TestResult testResult;

            listener.TestStarted(command.Test);

            long startTime = DateTime.Now.Ticks;

            TestExecutionContext.Save();

            TestExecutionContext.CurrentContext.CurrentTest = command.Test;
            TestExecutionContext.CurrentContext.CurrentResult = new TestResult(command.Test);

            try
            {
                return command.Execute(testObject);
            }
            // TODO: Ensure no exceptions escape
            finally
            {
                TestExecutionContext.Restore();
            }

            long stopTime = DateTime.Now.Ticks;
            double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
            testResult.Time = time;

            listener.TestFinished(testResult);
            return testResult;
        }
    }
}
