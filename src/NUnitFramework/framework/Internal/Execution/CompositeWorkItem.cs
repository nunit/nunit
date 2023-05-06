// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// A CompositeWorkItem represents a test suite and
    /// encapsulates the execution of the suite as well
    /// as all its child tests.
    /// </summary>
    public class CompositeWorkItem : WorkItem
    {
        //        static Logger log = InternalTrace.GetLogger("CompositeWorkItem");

        private readonly TestSuite _suite;
        private readonly TestSuiteResult _suiteResult;
        private TestCommand? _setupCommand;
        private TestCommand? _teardownCommand;

        /// <summary>
        /// List of Child WorkItems
        /// </summary>
        public List<WorkItem> Children { get; } = new();

        /// <summary>
        /// Indicates whether this work item should use a separate dispatcher.
        /// </summary>
        public override bool IsolateChildTests => ExecutionStrategy == ParallelExecutionStrategy.NonParallel && Context.Dispatcher.LevelOfParallelism > 0;

        private CountdownEvent? _childTestCountdown;

        /// <summary>
        /// Construct a CompositeWorkItem for executing a test suite
        /// using a filter to select child tests.
        /// </summary>
        /// <param name="suite">The TestSuite to be executed</param>
        /// <param name="childFilter">A filter used to select child tests</param>
        public CompositeWorkItem(TestSuite suite, ITestFilter childFilter)
            : base(suite, childFilter)
        {
            _suite = suite;
            _suiteResult = (TestSuiteResult)Result;
        }

        /// <summary>
        /// Method that actually performs the work. Overridden
        /// in CompositeWorkItem to do one-time setup, run all child
        /// items and then dispatch the one-time teardown work item.
        /// </summary>
        protected override void PerformWork()
        {
            if (!CheckForCancellation())
                if (Test.RunState == RunState.Explicit && !Filter.IsExplicitMatch(Test))
                    SkipFixture(ResultState.Explicit, GetSkipReason(), null);
                else
                    switch (Test.RunState)
                    {
                        default:
                        case RunState.Runnable:
                        case RunState.Explicit:
                            // Assume success, since the result will otherwise
                            // default to inconclusive.
                            Result.SetResult(ResultState.Success);

                            if (Children.Count > 0)
                            {
                                InitializeSetUpAndTearDownCommands();

                                PerformOneTimeSetUp();

                                if (!CheckForCancellation())
                                {
                                    switch (Result.ResultState.Status)
                                    {
                                        case TestStatus.Passed:
                                        case TestStatus.Warning:
                                            RunChildren();
                                            return;
                                        // Just return: completion event will take care
                                        // of OneTimeTearDown when all tests are done.

                                        case TestStatus.Skipped:
                                        case TestStatus.Inconclusive:
                                        case TestStatus.Failed:
                                            SkipChildren(this, Result.ResultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + Result.Message);
                                            break;
                                    }
                                }

                                // Directly execute the OneTimeFixtureTearDown for tests that
                                // were skipped, failed or set to inconclusive in one time setup
                                // unless we are aborting.
                                if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                                    PerformOneTimeTearDown();
                            }
                            else if (Test.TestType == "Theory")
                                Result.SetResult(ResultState.Failure, "No test cases were provided");
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

        private bool CheckForCancellation()
        {
            if (Context.ExecutionStatus != TestExecutionStatus.Running)
            {
                Result.SetResult(ResultState.Cancelled, "Test cancelled by user");
                return true;
            }

            return false;
        }

        private void InitializeSetUpAndTearDownCommands()
        {
            var methodValidator = Test.HasLifeCycle(LifeCycle.InstancePerTestCase)
                ? new StaticMethodValidator(
                    $"Only static OneTimeSetUp and OneTimeTearDown are allowed for {nameof(LifeCycle.InstancePerTestCase)} mode.")
                : null;

            List<SetUpTearDownItem> setUpTearDownItems =
                BuildSetUpTearDownList(_suite.OneTimeSetUpMethods, _suite.OneTimeTearDownMethods, methodValidator);

            var actionItems = new List<TestActionItem>();
            foreach (ITestAction action in Test.Actions)
            {
                // We need to go through all the actions on the test to determine which ones
                // will be used immediately and which will go into the context for use by
                // lower level tests.
                //
                // Special handling here for ParameterizedMethodSuite is a bit ugly. However,
                // it is needed because Tests are not supposed to know anything about Action
                // Attributes (or any attribute) and Attributes don't know where they were
                // initially applied unless we tell them.
                //
                // ParameterizedMethodSuites and individual test cases both use the same
                // MethodInfo as a source of attributes. We handle the Test and Default targets
                // in the test case, so we don't want to doubly handle it here.
                bool applyToSuite =  action.Targets.HasFlag(ActionTargets.Suite)
                    || action.Targets == ActionTargets.Default && !(Test is ParameterizedMethodSuite);

                bool applyToTest = action.Targets.HasFlag(ActionTargets.Test)
                    && !(Test is ParameterizedMethodSuite);

                if (applyToSuite)
                    actionItems.Add(new TestActionItem(action));

                if (applyToTest)
                    Context.UpstreamActions.Add(action);
            }

            _setupCommand = MakeOneTimeSetUpCommand(setUpTearDownItems, actionItems);

            _teardownCommand = MakeOneTimeTearDownCommand(setUpTearDownItems, actionItems);
        }

        private TestCommand MakeOneTimeSetUpCommand(List<SetUpTearDownItem> setUpTearDown, List<TestActionItem> actions)
        {
            TestCommand command = new EmptyTestCommand(Test);

            // Add Action Commands
            int index = actions.Count;
            while (--index >= 0)
                command = new BeforeTestActionCommand(command, actions[index]);

            if (Test.TypeInfo is not null)
            {
                // Build the OneTimeSetUpCommands
                foreach (SetUpTearDownItem item in setUpTearDown)
                    command = new OneTimeSetUpCommand(command, item);

                // Construct the fixture if necessary
                if (!Test.TypeInfo.IsStaticClass && !Test.HasLifeCycle(LifeCycle.InstancePerTestCase))
                    command = new ConstructFixtureCommand(command);
            }

            // Prefix with any IApplyToContext items from attributes
            foreach (var attr in Test.GetCustomAttributes<IApplyToContext>(true))
                command = new ApplyChangesToContextCommand(command, attr);

            return command;
        }

        private TestCommand MakeOneTimeTearDownCommand(List<SetUpTearDownItem> setUpTearDownItems, List<TestActionItem> actions)
        {
            TestCommand command = new EmptyTestCommand(Test);

            // For Theories, follow with TheoryResultCommand to adjust result as needed
            if (Test.TestType == "Theory")
                command = new TheoryResultCommand(command);

            // Create the AfterTestAction commands
            int index = actions.Count;
            while (--index >= 0)
                command = new AfterTestActionCommand(command, actions[index]);

            // Create the OneTimeTearDown commands
            foreach (SetUpTearDownItem item in setUpTearDownItems)
                command = new OneTimeTearDownCommand(command, item);

            // Dispose of fixture if necessary
            if (Test is IDisposableFixture && Test.TypeInfo is not null && typeof(IDisposable).IsAssignableFrom(Test.TypeInfo.Type) && !Test.HasLifeCycle(LifeCycle.InstancePerTestCase))
                command = new DisposeFixtureCommand(command);

            return command;
        }

        private void PerformOneTimeSetUp()
        {
            try
            {
                _setupCommand?.Execute(Context);

                // SetUp may have changed some things in the environment
                Context.UpdateContextFromEnvironment();
            }
            catch (Exception ex)
            {
                if (ex is NUnitException || ex is TargetInvocationException)
                    ex = ex.InnerException!;

                Result.RecordException(ex, FailureSite.SetUp);
            }
        }

        private void RunChildren()
        {
            if (Test.TestType == "Theory")
                Result.SetResult(ResultState.Inconclusive);

            int childCount = Children.Count;
            if (childCount == 0)
                throw new InvalidOperationException("RunChildren called but item has no children");

            _childTestCountdown = new CountdownEvent(childCount);

            foreach (WorkItem child in Children)
            {
                if (CheckForCancellation())
                    break;

                child.Completed += new EventHandler(OnChildItemCompleted);
                child.InitializeContext(new TestExecutionContext(Context));

                // In case we run directly, on same thread
                child.TestWorker = TestWorker;

                Context.Dispatcher.Dispatch(child);
                childCount--;
            }

            // If run was cancelled, reduce countdown by number of
            // child items not yet staged and check if we are done.
            if (childCount > 0)
                lock(_childCompletionLock)
                {
                    _childTestCountdown.Signal(childCount);
                    if (_childTestCountdown.CurrentCount == 0)
                        OnAllChildItemsCompleted();
                }
        }

        private void SkipFixture(ResultState resultState, string? message, string? stackTrace)
        {
            Result.SetResult(resultState.WithSite(FailureSite.SetUp), message, StackFilter.DefaultFilter.Filter(stackTrace));
            SkipChildren(this, resultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + message);
        }

        private void SkipChildren(CompositeWorkItem workItem, ResultState resultState, string message)
        {
            foreach (WorkItem child in workItem.Children)
            {
                SetChildWorkItemSkippedResult(child.Result, resultState, message);
                _suiteResult.AddResult(child.Result);

                // Some runners may depend on getting the TestFinished event
                // even for tests that have been skipped at a higher level.
                Context.Listener.TestFinished(child.Result);

                if (child is CompositeWorkItem item)
                    SkipChildren(item, resultState, message);
            }
        }

        private void SetChildWorkItemSkippedResult(TestResult result, ResultState resultState, string message)
        {
            result.SetResult(resultState, message);
            result.StartTime = Context.StartTime;
            result.EndTime = DateTime.UtcNow;
            result.Duration = Context.Duration;
        }

        private void PerformOneTimeTearDown()
        {
            // Our child tests or even unrelated tests may have
            // executed on the same thread since the time that
            // this test started, so we have to re-establish
            // the proper execution environment
            this.Context.EstablishExecutionEnvironment();

            _teardownCommand?.Execute(this.Context);
        }

        private string? GetSkipReason()
        {
            return (string?)Test.Properties.Get(PropertyNames.SkipReason);
        }

        private string? GetProviderStackTrace()
        {
            return (string?)Test.Properties.Get(PropertyNames.ProviderStackTrace);
        }

        private readonly object _childCompletionLock = new object();

        private void OnChildItemCompleted(object? sender, EventArgs e)
        {
            // Since child tests may be run on various threads, this
            // method may be called simultaneously by different children.
            // The lock is a member of the parent item and therefore
            // only blocks its own children.
            lock (_childCompletionLock)
            {
                if (sender is WorkItem childTask)
                {
                    childTask.Completed -= new EventHandler(OnChildItemCompleted);
                    _suiteResult.AddResult(childTask.Result);

                    if (Context.StopOnError && childTask.Result.ResultState.Status == TestStatus.Failed)
                        Context.ExecutionStatus = TestExecutionStatus.StopRequested;

                    // Check to see if all children completed
                    // _childTestCountdown is created before running child tasks, so is not null here.
                    _childTestCountdown!.Signal();
                    if (_childTestCountdown.CurrentCount == 0)
                        OnAllChildItemsCompleted();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void OnAllChildItemsCompleted()
        {
            if (Context.ExecutionStatus == TestExecutionStatus.AbortRequested)
                WorkItemComplete();
            else
                Context.Dispatcher.Dispatch(new OneTimeTearDownWorkItem(this));
        }

        private readonly object cancelLock = new object();

        /// <summary>
        /// Cancel (abort or stop) a CompositeWorkItem and all of its children
        /// </summary>
        /// <param name="force">true if the CompositeWorkItem and all of its children should be aborted, false if it should allow all currently running tests to complete</param>
        public override void Cancel(bool force)
        {
            lock (cancelLock)
            {
                foreach (var child in Children)
                {
                    var ctx = child.Context;
                    if (ctx is not null)
                        ctx.ExecutionStatus = force ? TestExecutionStatus.AbortRequested : TestExecutionStatus.StopRequested;

                    if (child.State == WorkItemState.Running)
                        child.Cancel(force);
                }
            }
        }

        #endregion

        #region Nested OneTimeTearDownWorkItem Class

        /// <summary>
        /// OneTimeTearDownWorkItem represents the cleanup
        /// and one-time teardown phase of a CompositeWorkItem
        /// </summary>
        public class OneTimeTearDownWorkItem : WorkItem
        {
            private readonly CompositeWorkItem _originalWorkItem;

            private readonly object _teardownLock = new object();

            /// <summary>
            /// Construct a OneTimeTearDownWOrkItem wrapping a CompositeWorkItem
            /// </summary>
            /// <param name="originalItem">The CompositeWorkItem being wrapped</param>
            public OneTimeTearDownWorkItem(CompositeWorkItem originalItem)
                : base(originalItem)
            {
                _originalWorkItem = originalItem;
            }

            /// <summary>
            /// The WorkItem name, overridden to indicate this is the teardown.
            /// </summary>
            public override string Name => $"{base.Name} OneTimeTearDown";

            /// <summary>
            /// The ExecutionStrategy for use in running this work item
            /// </summary>
            public override ParallelExecutionStrategy ExecutionStrategy => _originalWorkItem.ExecutionStrategy;

            /// <summary>
            ///
            /// </summary>
            public override void Execute()
            {
                lock (_teardownLock)
                {
                    if (Test.TestType == "Theory" && Result.ResultState == ResultState.Success && Result.PassCount == 0)
                        Result.SetResult(ResultState.Failure, "No test cases were provided");

                    if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                        _originalWorkItem.PerformOneTimeTearDown();

                    foreach (var childResult in Result.Children)
                        if (childResult.ResultState == ResultState.Cancelled)
                        {
                            this.Result.SetResult(ResultState.Cancelled, "Cancelled by user");
                            break;
                        }

                    _originalWorkItem.WorkItemComplete();
                }
            }

            /// <summary>
            /// PerformWork is not used in CompositeWorkItem
            /// </summary>
            protected override void PerformWork() { }

            /// <summary>
            /// WorkItemCancelled is called directly by the parallel dispatcher
            /// when a test suite is left hanging after a forced StopRun. We
            /// simulate WorkItemComplete() but without the ripple effect to
            /// higher level suites, since we are controlling it all directly.
            /// </summary>
            internal void WorkItemCancelled()
            {
                Result.SetResult(ResultState.Cancelled, TestResult.USER_CANCELLED_MESSAGE);
                _originalWorkItem.WorkItemComplete();
            }
        }

        #endregion
    }
}

