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
            _setupCommand = suite.GetOneTimeSetUpCommand();
            _teardownCommand = suite.GetOneTimeTearDownCommand();
            _childFilter = childFilter;
        }

        /// <summary>
        /// Method that actually performs the work. Overridden
        /// in CompositeWorkItem to do setup, run all child
        /// items and then do teardown.
        /// </summary>
        protected override void PerformWork()
        {
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

                    if (_suite.HasChildren)
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
                    // Directly execute the OneTimeFixtureTearDown or tests that
                    // were skipped, failed or set to inconclusive in one time setup.
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
                _childTestCountdown = new CountdownEvent(children.Count);

                foreach (WorkItem child in children)
                {
                    child.Completed += new EventHandler(OnChildCompleted);
#if NUNITLITE
                    child.Execute();
#else
                    if (Context.Dispatcher == null)
                        child.Execute();
                    // For now, run all test cases on the same thread as the fixture
                    // in order to handle ApartmentState preferences set on the fixture.
                    else if (child is SimpleWorkItem || child.Test is ParameterizedMethodSuite)
                    {
                        log.Debug("Executing WorkItem for {0}", child.Test.FullName);
                        child.Execute();
                    }
                    else
                        Context.Dispatcher.Dispatch(child);
#endif
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
            foreach (Test test in _suite.Tests)
            {
                if (_childFilter.Pass(test))
                {
                    TestResult result = test.MakeTestResult();
                    if (Result.ResultState.Status == TestStatus.Failed)
                        result.SetResult(ResultState.Failure, "TestFixtureSetUp Failed");
                    else
                        result.SetResult(Result.ResultState, Result.Message);
                    Result.AddResult(result);
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
                    _childTestCountdown.Signal();
                    if (_childTestCountdown.CurrentCount == 0)
                    {
                        PerformOneTimeTearDown();
                        WorkItemComplete();
                    }
                }
            }
        }

        #endregion
    }
}
