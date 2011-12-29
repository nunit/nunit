using System;
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    public class CommandRunner
    {
        private ITestListener listener;

        /// <summary>
        /// Runs a TestCommand, sending notifications to a listener.
        /// </summary>
        /// <param name="command">A TestCommand to be executed.</param>
        /// <param name="context">The context in which to execute the command.</param>
        /// <returns>A TestResult.</returns>
        public static TestResult Execute(TestCommand command)
        {
            TestResult testResult;

            TestExecutionContext.Save();
            TestExecutionContext context = TestExecutionContext.CurrentContext;
            //context = new TestExecutionContext(context);

            context.CurrentTest = command.Test;
            context.CurrentResult = command.Test.MakeTestResult();

            context.Listener.TestStarted(command.Test);
            long startTime = DateTime.Now.Ticks;

            try
            {
                TestSuiteCommand suiteCommand = command as TestSuiteCommand;
                if (suiteCommand != null)
                    testResult = ExecuteSuiteCommand(suiteCommand, context);
                //{
                //    suiteCommand.DoOneTimeSetup();
                //    foreach (TestCommand childCommand in suiteCommand.Children)
                //        Execute(childCommand, context);
                //    suiteCommand.DoOneTimeTearDown();
                //}
                else
                    testResult = command.Execute(context);

                testResult.AssertCount = context.AssertCount;

                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                testResult.Time = time;

                context.Listener.TestFinished(testResult);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                context.CurrentResult.RecordException(ex);
                return context.CurrentResult;
            }
            finally
            {
                TestExecutionContext.Restore();
                //context.ReverseChanges();
                //context = context.prior;
            }

            return testResult;
        }

        private static TestResult ExecuteSuiteCommand(TestSuiteCommand command, TestExecutionContext context)
        {
            //return command.Execute(context);
            TestSuiteResult suiteResult = context.CurrentResult as TestSuiteResult;
            System.Diagnostics.Debug.Assert(suiteResult != null);

            bool oneTimeSetUpComplete = false;
            try
            {
                // Temporary: this should be done by individual commands
                ApplyTestSettingsToExecutionContext(command.Test, context);

                command.DoOneTimeSetUp(context);
                oneTimeSetUpComplete = true;

                // SetUp may have changed some things
                context.Update();

                suiteResult = RunChildCommands(command, context);
            }
            catch (Exception ex)
            {
                if (ex is NUnitException || ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;


                if (oneTimeSetUpComplete)
                    suiteResult.RecordException(ex);
                else
                    suiteResult.RecordException(ex, FailureSite.SetUp);
            }
            finally
            {
                command.DoOneTimeTearDown(context);
            }

            return suiteResult;
        }

        public static TestSuiteResult RunChildCommands(TestSuiteCommand command, TestExecutionContext context)
        {
            TestSuiteResult suiteResult = TestExecutionContext.CurrentContext.CurrentResult as TestSuiteResult;
            suiteResult.SetResult(ResultState.Success);

            foreach (TestCommand childCommand in command.Children)
            {
                TestResult childResult = CommandRunner.Execute(childCommand);

                suiteResult.AddResult(childResult);

                if (childResult.ResultState == ResultState.Cancelled)
                    break;

                if (childResult.ResultState.Status == TestStatus.Failed && TestExecutionContext.CurrentContext.StopOnError)
                    break;
            }

            return suiteResult;
        }

        /// <summary>
        /// Applies the culture settings specified on the test
        /// to the TestExecutionContext.
        /// </summary>
        public static void ApplyTestSettingsToExecutionContext(Test test, TestExecutionContext context)
        {
#if !NETCF
            string setCulture = (string)test.Properties.Get(PropertyNames.SetCulture);
            if (setCulture != null)
                context.CurrentCulture = new System.Globalization.CultureInfo(setCulture);

            string setUICulture = (string)test.Properties.Get(PropertyNames.SetUICulture);
            if (setUICulture != null)
                context.CurrentUICulture = new System.Globalization.CultureInfo(setUICulture);
#endif

#if !NUNITLITE
            if (test.Properties.ContainsKey(PropertyNames.Timeout))
                context.TestCaseTimeout = (int)test.Properties.Get(PropertyNames.Timeout);
#endif
        }
    }
}

