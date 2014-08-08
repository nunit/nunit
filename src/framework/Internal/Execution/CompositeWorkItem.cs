// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// A CompositeWorkItem represents a test suite and
    /// encapsulates the execution of the suite as well
    /// as all its child tests.
    /// </summary>
    public class CompositeWorkItem : WorkItem
    {
        static Logger log = InternalTrace.GetLogger("CompositeWorkItem");

        private TestSuite _suite;
        private ITestFilter _childFilter;
        private TestCommand _setupCommand;
        private TestCommand _teardownCommand;

        private CountdownEvent _childTestCountdown;

        /// <summary>
        /// Construct a CompositeWorkItem for executing a test suite
        /// using a filter to select child tests.
        /// </summary>
        /// <param name="suite">The TestSuite to be executed</param>
        /// <param name="context">The execution context to be used</param>
        /// <param name="childFilter">A filter used to select child tests</param>
        public CompositeWorkItem(TestSuite suite, TestExecutionContext context, ITestFilter childFilter)
            : base(suite, context)
        {
            _suite = suite;
            SetUpTearDownList setUpTearDown = null;
            if (suite.FixtureType != null)
                setUpTearDown =  new SetUpTearDownList(
                    suite.FixtureType, typeof(OneTimeSetUpAttribute), typeof(OneTimeTearDownAttribute));

            _setupCommand = MakeSetUpCommand(suite, setUpTearDown);
            _teardownCommand = MakeTearDownCommand(suite, setUpTearDown);
            _childFilter = childFilter;
        }

        /// <summary>
        /// Method that actually performs the work. Overridden
        /// in CompositeWorkItem to do setup, run all child
        /// items and then do teardown.
        /// </summary>
        protected override void PerformWork()
        {
            if (!CheckForCancellation())
                switch (Test.RunState)
                {
                    default:
                    case RunState.Runnable:
                    case RunState.Explicit:
                        // Assume success, since the result will be inconclusive
                        // if there is no setup method to run or if the
                        // context initialization fails.
                        Result.SetResult(ResultState.Success);

                        PerformOneTimeSetUp();

                        if (_suite.HasChildren && !CheckForCancellation())
                            switch (Result.ResultState.Status)
                            {
                                case TestStatus.Passed:
                                    RunChildren();
                                    return;
                                    // Just return: completion event will take care
                                    // of TestFixtureTearDown when all tests are done.

                                case TestStatus.Skipped:
                                case TestStatus.Inconclusive:
                                case TestStatus.Failed:
                                    SkipChildren();
                                    break;
                            }

                        // Directly execute the OneTimeFixtureTearDown for tests that
                        // were skipped, failed or set to inconclusive in one time setup
                        // unless we are aborting.
                        if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                            PerformOneTimeTearDown();
                        break;

                    case RunState.Skipped:
                        SkipFixture(ResultState.Skipped, GetSkipReason(), null);
                        break;

                    case RunState.Ignored:
                        SkipFixture(ResultState.Ignored, GetSkipReason(), null);
                        break;

                    case RunState.NotRunnable:
                        SkipFixture(ResultState.NotRunnable, GetSkipReason(), GetProviderStackTrace());
                        break;
                }

            // Fall through in case nothing was run.
            // Otherwise, this is done in the completion event.
            WorkItemComplete();
        
        }

        #region Helper Methods

        /// <summary>
        /// Gets the command to be executed before any of
        /// the child tests are run.
        /// </summary>
        /// <returns>A TestCommand</returns>
        private static TestCommand MakeSetUpCommand(TestSuite suite, SetUpTearDownList setUpTearDown)
        {
            if (suite.RunState != RunState.Runnable && suite.RunState != RunState.Explicit)
                return new SkipCommand(suite);

            TestCommand command = new OneTimeSetUpCommand(suite, setUpTearDown);

            if (suite.FixtureType != null)
            {
                IApplyToContext[] changes = (IApplyToContext[])suite.FixtureType.GetCustomAttributes(typeof(IApplyToContext), true);
                if (changes.Length > 0)
                    command = new ApplyChangesToContextCommand(command, changes);
            }

            return command;
        }

        /// <summary>
        /// Gets the command to be executed after all of the
        /// child tests are run.
        /// </summary>
        /// <returns>A TestCommand</returns>
        private static TestCommand MakeTearDownCommand(TestSuite suite, SetUpTearDownList setUpTearDown)
        {
            TestCommand command = new OneTimeTearDownCommand(suite, setUpTearDown);

            if (suite.TestType == "Theory")
                command = new TheoryResultCommand(command);

            return command;
        }

        private bool CheckForCancellation()
        {
            if (Context.ExecutionStatus != TestExecutionStatus.Running)
            {
                Result.SetResult(ResultState.Cancelled, "Test cancelled by user");
                return true;
            }

            return false;
        }

        private void PerformOneTimeSetUp()
        {
            try
            {
                _setupCommand.Execute(Context);

                // SetUp may have changed some things in the environment
                Context.UpdateContextFromEnvironment();
            }
            catch (Exception ex)
            {
                if (ex is NUnitException || ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;

                Result.RecordException(ex);
            }
        }

        private void RunChildren()
        {
            var children = new List<WorkItem>();

            foreach (Test test in _suite.Tests)
                if (_childFilter.Pass(test))
                    children.Add(WorkItem.CreateWorkItem(test, new TestExecutionContext(this.Context), _childFilter));

            if (children.Count > 0)
            {
                int childCount = children.Count;
                _childTestCountdown = new CountdownEvent(childCount);

                foreach (WorkItem child in children)
                {
                    if (CheckForCancellation())
                        break;

                    child.Completed += new EventHandler(OnChildCompleted);
                    Context.Dispatcher.Dispatch(child);
                    childCount--;
                }

                if (childCount > 0)
                {
                    while (childCount-- > 0)
                        CountDownChildTest();
                }
            }
        }

        private void SkipFixture(ResultState resultState, string message, string stackTrace)
        {
            Result.SetResult(resultState, message, stackTrace);
            SkipChildren();
        }

        private void SkipChildren()
        {
            // TODO: Extend this to skip recursively?
            foreach (Test child in _suite.Tests)
            {
                if (_childFilter.Pass(child))
                {
                    TestResult childResult = child.MakeTestResult();
                    var resultState = Result.ResultState.Status == TestStatus.Failed
                        ? ResultState.Failure // TODO: Converts errors to failures for the child. Should we do this?
                        : Result.ResultState;
                    childResult.SetResult(resultState, "TestFixtureSetUp: " + Result.Message);
                    Result.AddResult(childResult);
                }
            }
        }

        private void PerformOneTimeTearDown()
        {
            // Our child tests or even unrelated tests may have
            // executed on the same thread since the time that
            // this test started, so we have to re-establish
            // the proper execution environment
            this.Context.EstablishExecutionEnvironment();

            _teardownCommand.Execute(this.Context);
        }

        private string GetSkipReason()
        {
            return (string)Test.Properties.Get(PropertyNames.SkipReason);
        }

        private string GetProviderStackTrace()
        {
            return (string)Test.Properties.Get(PropertyNames.ProviderStackTrace);
        }

        private object _completionLock = new object();

        private void OnChildCompleted(object sender, EventArgs e)
        {
            lock (_completionLock)
            {
                WorkItem childTask = sender as WorkItem;
                if (childTask != null)
                {
                    childTask.Completed -= new EventHandler(OnChildCompleted);
                    Result.AddResult(childTask.Result);

                    // Check to see if all children completed
                    CountDownChildTest();
                }
            }
        }

        private void CountDownChildTest()
        {
            _childTestCountdown.Signal();
            if (_childTestCountdown.CurrentCount == 0)
            {
                if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                    PerformOneTimeTearDown();

                foreach (var childResult in this.Result.Children)
                    if (childResult.ResultState == ResultState.Cancelled)
                    {
                        this.Result.SetResult(ResultState.Cancelled, "Cancelled by user");
                        break;
                    }

                WorkItemComplete();
            }
        }

        #endregion
    }
}
