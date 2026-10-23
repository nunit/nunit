// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly TestSuite _suite;
        private readonly TestSuiteResult _suiteResult;
        private TestCommand? _setupCommand;
        private TestCommand? _teardownCommand;

        /// <summary>
        /// List of Child WorkItems
        /// </summary>
        public List<WorkItem> Children { get; } = [];

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
            if (Test.RunState == RunState.Explicit && !Filter.IsExplicitMatch(Test))
            {
                SkipFixture(ResultState.Explicit, GetSkipReason(), null);
            }
            else
            {
                switch (Test.RunState)
                {
                    default:
                    case RunState.Runnable:
                    case RunState.Explicit:
                        if (Children.Count > 0)
                        {
                            InitializeSetUpAndTearDownCommands();

                            PerformOneTimeSetUp();

                            if (!CheckForCancellation())
                            {
                                if ((Result.ResultState.Status is not TestStatus.Failed and not TestStatus.Skipped) &&
                                    (Result.ResultState.Site != FailureSite.SetUp))
                                {
                                    RunChildren();
                                    return;
                                    // Just return: completion event will take care
                                    // of OneTimeTearDown when all tests are done.
                                }
                                SkipChildren(this, Result.ResultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + Result.Message, Result.StackTrace);
                            }

                            // Directly execute the OneTimeFixtureTearDown for tests that
                            // were skipped, failed or set to inconclusive in one time setup
                            // unless we are aborting.
                            if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                                PerformOneTimeTearDown();
                        }
                        else if (Test.TestType == "Theory")
                        {
                            Result.SetResult(ResultState.Failure, "No test cases were provided");
                        }
                        else
                        {
                            var status = Test.GetEffectiveProperty(PropertyNames.NoTests, TestStatus.Inconclusive);
                            Result.SetResult(new ResultState(status), "No test cases were provided");
                        }

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
            }

            // Fall through in case nothing was run.
            // Otherwise, this is done in the completion event.
            WorkItemComplete();
        }

        #region Helper Methods

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
                bool applyToSuite = action.Targets.HasFlag(ActionTargets.Suite)
                    || (action.Targets == ActionTargets.Default && Test is not ParameterizedMethodSuite);

                bool applyToTest = action.Targets.HasFlag(ActionTargets.Test)
                    && Test is not ParameterizedMethodSuite;

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

            // Add Action Commands in reverse order so that they are executed in the order they were added
            command = command.WrapWith(Enumerable.Reverse(actions), (c, action) => new BeforeTestActionCommand(c, action));

            if (Test.IsFixture)
            {
                // Build the chain of OneTimeSetUpCommands
                command = command.WrapWith(setUpTearDown, (c, item) => new OneTimeSetUpCommand(c, item));

                // Construct the fixture if necessary
                if (!Test.TypeInfo!.IsStaticClass && !Test.HasLifeCycle(LifeCycle.InstancePerTestCase))
                    command = new ConstructFixtureCommand(command);
            }

            // Prefix with any IApplyToContext items from attributes
            command = command.WrapWith(Test.GetCustomAttributes<IApplyToContext>(true), (c, attribute) => new ApplyChangesToContextCommand(c, attribute));

            return command;
        }

        private TestCommand MakeOneTimeTearDownCommand(List<SetUpTearDownItem> setUpTearDownItems, List<TestActionItem> actions)
        {
            TestCommand command = new EmptyTestCommand(Test);

            // For Theories, follow with TheoryResultCommand to adjust result as needed
            if (Test.TestType == "Theory")
                command = new TheoryResultCommand(command);

            // Create the chain of AfterTestAction commands
            command = command.WrapWith(Enumerable.Reverse(actions), (c, action) => new AfterTestActionCommand(c, action));

            // Create the chain of OneTimeTearDown commands
            command = command.WrapWith(setUpTearDownItems, (c, item) => new OneTimeTearDownCommand(c, item));

            // Dispose of fixture if necessary
            if (Test is IDisposableFixture && Test.IsFixture && DisposeHelper.IsDisposable(Test.TypeInfo!.Type) && !Test.HasLifeCycle(LifeCycle.InstancePerTestCase))
                command = new DisposeFixtureCommand(command);

            return command;
        }

        private void PerformOneTimeSetUp()
        {
            Context.SetActiveTests(() => CollectActiveTests(Children));
            try
            {
                _setupCommand?.Execute(Context);

                // SetUp may have changed some things in the environment
                Context.UpdateContextFromEnvironment();
            }
            catch (Exception ex)
            {
                Result.RecordException(ex.Unwrap(), FailureSite.SetUp);
            }
        }

        private static IReadOnlyList<ITest> CollectActiveTests(List<WorkItem> children)
        {
            var result = new List<ITest>();
            Collect(children, result);
            return result.AsReadOnly();

            static void Collect(List<WorkItem> items, List<ITest> accumulator)
            {
                foreach (var item in items)
                {
                    if (item is CompositeWorkItem composite)
                        Collect(composite.Children, accumulator);
                    else
                        accumulator.Add(item.Test);
                }
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

                // A child whose dependency has not completed yet is started
                // from OnChildItemCompleted when the dependency completes.
                if (!WaitForDependency(child))
                    StartChild(child, TestWorker);

                childCount--;
            }

            // If run was cancelled, reduce countdown by number of
            // child items not yet staged and check if we are done.
            if (childCount > 0)
            {
                lock (_childCompletionLock)
                {
                    _childTestCountdown.Signal(childCount);
                    if (_childTestCountdown.CurrentCount == 0)
                        OnAllChildItemsCompleted();
                }
            }
        }

        private void SkipFixture(ResultState resultState, string message, string? stackTrace)
        {
            Result.SetResult(resultState.WithSite(FailureSite.SetUp), message, StackFilter.DefaultFilter.Filter(stackTrace));
            SkipChildren(this, resultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + message, stackTrace);
        }

        private void SkipChildren(CompositeWorkItem workItem, ResultState resultState, string message, string? stackTrace)
        {
            foreach (WorkItem child in workItem.Children)
            {
                SetChildWorkItemSkippedResult(child.Result, resultState, message, stackTrace);
                _suiteResult.AddResult(child.Result);

                // Some runners may depend on getting the TestFinished event
                // even for tests that have been skipped at a higher level.
                Context.Listener.TestFinished(child.Result);

                if (child is CompositeWorkItem item)
                    SkipChildren(item, resultState, message, stackTrace);
            }
        }

        private void SetChildWorkItemSkippedResult(TestResult result, ResultState resultState, string message, string? stackTrace)
        {
            result.SetResult(resultState, message, stackTrace);
            result.StartTime = Context.StartTime;
            result.EndTime = DateTime.UtcNow;
            result.Duration = Context.Duration;
        }

        /// <summary>
        /// Gets the status of a child's dependency, if any.
        /// Returns null if the child has no dependency, does not require a passing dependency,
        /// or the dependency hasn't been run yet.
        /// </summary>
        private TestStatus? GetDependencyStatus(WorkItem child) =>
            child.Test.Properties.TryGet(PropertyNames.DependsOnAllowFailure, false)
                ? null
                : FindDependency(child)?.Result.ResultState.Status; // A missing dependency would indicate an error in the framework, when the test is runnable.

        /// <summary>
        /// Finds the work item of a child's dependency among our children, if any.
        /// </summary>
        private WorkItem? FindDependency(WorkItem child)
        {
            ITest? dependantTest = child.Test.DependantTest;
            return dependantTest is null
                ? null
                : Children.Find(c => c.Test == dependantTest);
        }

        /// <summary>
        /// Registers the child to be started when its dependency completes.
        /// Returns false if the child has no dependency, or the dependency has already completed.
        /// </summary>
        /// <remarks>
        /// The dispatcher may only queue the dependency, so it can still be running here.
        /// This also applies with AllowFailure set, since the dependency must still finish first.
        /// A child that is not runnable never waits: invalid dependencies, such as cycles,
        /// would otherwise wait for each other forever.
        /// </remarks>
        private bool WaitForDependency(WorkItem child)
        {
            if (child.Test.RunState is not (RunState.Runnable or RunState.Explicit))
                return false;

            WorkItem? dependency = FindDependency(child);
            if (dependency is null)
                return false;

            lock (_childCompletionLock)
            {
                if (_completedChildren.Contains(dependency))
                    return false;

                if (!_waitingForDependency.TryGetValue(dependency, out var dependents))
                    _waitingForDependency[dependency] = dependents = [];

                dependents.Add(child);
                return true;
            }
        }

        /// <summary>
        /// Dispatches a child, or skips it if its dependency did not pass.
        /// </summary>
        /// <param name="child">The child to start.</param>
        /// <param name="worker">The worker running the current thread. A directly executed child runs on it.
        /// It can differ from our own worker when a dependency completed on another worker.</param>
        private void StartChild(WorkItem child, TestWorker? worker)
        {
            // In case we run directly, on same thread
            child.TestWorker = worker;

            if (child.Test.RunState is RunState.Runnable or RunState.Explicit)
            {
                TestStatus? dependencyStatus = GetDependencyStatus(child);
                if (dependencyStatus.HasValue && dependencyStatus is not (TestStatus.Passed or TestStatus.Warning))
                {
                    var dependant = child.Test.DependantTest!;
                    var message = $"Dependency on {dependant.Name} did not pass.";

                    SetChildWorkItemSkippedResult(child.Result, ResultState.Skipped, message, null);
                    OnChildItemCompleted(child, EventArgs.Empty);
                    return;
                }
            }

            child.Completed += OnChildItemCompleted;
            child.InitializeContext(new TestExecutionContext(Context));
            Context.Dispatcher.Dispatch(child);
        }

        private void PerformOneTimeTearDown()
        {
            // Our child tests or even unrelated tests may have
            // executed on the same thread since the time that
            // this test started, so we have to re-establish
            // the proper execution environment
            Context.EstablishExecutionEnvironment();

            _teardownCommand?.Execute(Context);
        }

        private string GetSkipReason() => (string?)Test.Properties.Get(PropertyNames.SkipReason) ?? string.Empty;

        private string? GetProviderStackTrace() => (string?)Test.Properties.Get(PropertyNames.ProviderStackTrace);

        private readonly Lock _childCompletionLock = new();
        private readonly HashSet<WorkItem> _completedChildren = [];
        private readonly Dictionary<WorkItem, List<WorkItem>> _waitingForDependency = new();
        private readonly Queue<WorkItem> _readyDependents = new();
        private bool _startingDependents;

        private void OnChildItemCompleted(object? sender, EventArgs e)
        {
            // Since child tests may be run on various threads, this
            // method may be called simultaneously by different children.
            // The lock is a member of the parent item and therefore
            // only blocks its own children.
            if (sender is not WorkItem childTask)
                return;

            lock (_childCompletionLock)
            {
                childTask.Completed -= OnChildItemCompleted;
                _suiteResult.AddResult(childTask.Result);

                if (Context.StopOnError && childTask.Result.ResultState.Status == TestStatus.Failed)
                    Context.ExecutionStatus = TestExecutionStatus.StopRequested;

                _completedChildren.Add(childTask);
                if (_waitingForDependency.TryGetValue(childTask, out var dependents)) // TryGetValue needed for .net framework.
                {
                    _waitingForDependency.Remove(childTask);
                    foreach (WorkItem dependent in dependents)
                        _readyDependents.Enqueue(dependent);
                }

                // Check to see if all children completed
                // _childTestCountdown is created before running child tasks, so is not null here.
                _childTestCountdown!.Signal();
                if (_childTestCountdown.CurrentCount == 0)
                    OnAllChildItemsCompleted();

                // A call that is already starting dependents picks up the new ones.
                if (_startingDependents || _readyDependents.Count == 0)
                    return;

                _startingDependents = true;
            }

            // The dependents start on the thread that ran the completed child.
            StartReadyDependents(childTask.TestWorker);
        }

        /// <summary>
        /// Starts ready dependents one by one until none are left.
        /// </summary>
        /// <remarks>
        /// A skipped or directly executed child completes inside <see cref="StartChild"/> and calls
        /// <see cref="OnChildItemCompleted"/> again. That call only queues its dependents, so the stack
        /// does not grow with the length of a dependency chain. Children are started outside the lock.
        /// </remarks>
        private void StartReadyDependents(TestWorker? worker)
        {
            while (true)
            {
                WorkItem dependent;

                lock (_childCompletionLock)
                {
                    if (_readyDependents.Count == 0)
                    {
                        _startingDependents = false;
                        return;
                    }

                    dependent = _readyDependents.Dequeue();
                }

                StartChild(dependent, worker);
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

        private readonly Lock _cancelLock = new();

        /// <summary>
        /// Cancel a CompositeWorkItem and all of its children
        /// </summary>
        public override void Cancel()
        {
            lock (_cancelLock)
            {
                foreach (var child in Children)
                {
                    var ctx = child.Context;
                    if (ctx is not null)
                        ctx.ExecutionStatus = TestExecutionStatus.StopRequested;

                    if (child.State == WorkItemState.Running)
                        child.Cancel();
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

            private readonly Lock _teardownLock = new();

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
                    {
                        if (childResult.ResultState == ResultState.Cancelled)
                        {
                            Result.SetResult(ResultState.Cancelled, TestResult.USER_CANCELLED_MESSAGE);
                            break;
                        }
                    }

                    _originalWorkItem.WorkItemComplete();
                }
            }

            /// <summary>
            /// PerformWork is not used in CompositeWorkItem
            /// </summary>
            protected override void PerformWork()
            {
            }

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
