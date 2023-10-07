// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    using Commands;
    using NUnit.Framework.Internal.Extensions;

    /// <summary>
    /// A WorkItem may be an individual test case, a fixture or
    /// a higher level grouping of tests. All WorkItems inherit
    /// from the abstract WorkItem class, which uses the template
    /// pattern to allow derived classes to perform work in
    /// whatever way is needed.
    ///
    /// A WorkItem is created with a particular TestExecutionContext
    /// and is responsible for re-establishing that context in the
    /// current thread before it begins or resumes execution.
    /// </summary>
    public abstract class WorkItem : IDisposable
    {
        private static readonly Logger Log = InternalTrace.GetLogger("WorkItem");

        #region Construction and Initialization

        /// <summary>
        /// Construct a WorkItem for a particular test.
        /// </summary>
        /// <param name="test">The test that the WorkItem will run</param>
        /// <param name="filter">Filter used to include or exclude child items</param>
        public WorkItem(Test test, ITestFilter filter)
        {
            Test = test;
            Filter = filter;
            Result = test.MakeTestResult();
            State = WorkItemState.Ready;

            ParallelScope = Test.Properties.TryGet(PropertyNames.ParallelScope, ParallelScope.Default);

            TargetApartment = GetTargetApartment(Test);

            State = WorkItemState.Ready;

            // Yes, this is cheating.
            // The code relies on InitializeContext being called.
            // An most code simply assumes it is not null.
            // Converting the property to nullable causes too much headache
            Context = null!;
        }

        /// <summary>
        /// Construct a work Item that wraps another work Item.
        /// Wrapper items are used to represent independently
        /// dispatched tasks, which form part of the execution
        /// of a single test, such as OneTimeTearDown.
        /// </summary>
        /// <param name="wrappedItem">The WorkItem being wrapped</param>
        public WorkItem(WorkItem wrappedItem)
        {
            // Use the same Test, Result, Filter, Actions, Context, ParallelScope
            // and TargetApartment as the item being wrapped.
            Test = wrappedItem.Test;
            Result = wrappedItem.Result;
            Filter = wrappedItem.Filter;
            Context = wrappedItem.Context;
            ParallelScope = wrappedItem.ParallelScope;
            TestWorker = wrappedItem.TestWorker;
            TargetApartment = wrappedItem.TargetApartment;

            // State is independent of the wrapped item
            State = WorkItemState.Ready;
        }

        /// <summary>
        /// Initialize the TestExecutionContext. This must be done
        /// before executing the WorkItem.
        /// </summary>
        /// <remarks>
        /// Originally, the context was provided in the constructor
        /// but delaying initialization of the context until the item
        /// is about to be dispatched allows changes in the parent
        /// context during OneTimeSetUp to be reflected in the child.
        /// </remarks>
        /// <param name="context">The TestExecutionContext to use</param>
        public void InitializeContext(TestExecutionContext context)
        {
            Guard.OperationValid(Context is null, "The context has already been initialized");

            Context = context;
        }

        #endregion

        #region Properties and Events

        /// <summary>
        /// Event triggered when the item is complete
        /// </summary>
        public event EventHandler? Completed;

        /// <summary>
        /// Gets the current state of the WorkItem
        /// </summary>
        public WorkItemState State { get; protected set; }

        /// <summary>
        /// The test being executed by the work item
        /// </summary>
        public Test Test { get; }

        /// <summary>
        /// The name of the work item - defaults to the Test name.
        /// </summary>
        public virtual string Name => Test.Name;

        /// <summary>
        /// Filter used to include or exclude child tests
        /// </summary>
        public ITestFilter Filter { get; }

        /// <summary>
        /// The execution context
        /// </summary>
        public TestExecutionContext Context { get; private set; }

        /// <summary>
        /// The worker executing this item.
        /// </summary>
        public TestWorker? TestWorker { get; internal set; }

        private ParallelExecutionStrategy? _executionStrategy;

        /// <summary>
        /// The ParallelExecutionStrategy to use for this work item
        /// </summary>
        public virtual ParallelExecutionStrategy ExecutionStrategy
        {
            get
            {
                if (!_executionStrategy.HasValue)
                    _executionStrategy = GetExecutionStrategy();

                return _executionStrategy.Value;
            }
        }

        /// <summary>
        /// Indicates whether this work item should use a separate dispatcher.
        /// </summary>
        public virtual bool IsolateChildTests { get; } = false;

        /// <summary>
        /// The test result
        /// </summary>
        public TestResult Result { get; protected set; }

        /// <summary>
        /// Gets the ParallelScope associated with the test, if any,
        /// otherwise returning ParallelScope.Default;
        /// </summary>
        public ParallelScope ParallelScope { get; }

        internal ApartmentState TargetApartment { get; set; }
        private ApartmentState CurrentApartment { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute the current work item, including any
        /// child work items.
        /// </summary>
        public virtual void Execute()
        {
            Guard.OperationValid(Context is not null, "Context must be set by InitializeContext");

            // A supplementary thread is required in two conditions...
            //
            // 1. If the test used the RequiresThreadAttribute. This
            // is at the discretion of the user.
            //
            // 2. If the test needs to run in a different apartment.
            // This should not normally occur when using the parallel
            // dispatcher because tests are dispatches to a queue that
            // matches the requested apartment. Under the SimpleDispatcher
            // (--workers=0 option) it occurs routinely whenever a
            // different apartment is requested.

            CurrentApartment = Thread.CurrentThread.GetApartmentState();
            var targetApartment = TargetApartment == ApartmentState.Unknown ? CurrentApartment : TargetApartment;
            var needsNewThreadToSetApartmentState = targetApartment != CurrentApartment;

            if (Test.RequiresThread || needsNewThreadToSetApartmentState)
            {
                // Handle error conditions in a single threaded fixture
                if (Context.IsSingleThreaded)
                {
                    string msg = Test.RequiresThread
                        ? "RequiresThreadAttribute may not be specified on a test within a single-SingleThreadedAttribute fixture."
                        : "Tests in a single-threaded fixture may not specify a different apartment";

                    Log.Error(msg);
                    Result.SetResult(ResultState.NotRunnable, msg);
                    WorkItemComplete();
                    return;
                }

                Log.Debug("Running on separate thread because {0} is specified.",
                    Test.RequiresThread ? "RequiresThread" : "different Apartment");

                RunOnSeparateThread(targetApartment);
            }
            else
            {
                RunOnCurrentThread();
            }
        }

        private readonly ManualResetEventSlim _completionEvent = new();

        /// <summary>
        /// Wait until the execution of this item is complete
        /// </summary>
        public void WaitForCompletion()
        {
            _completionEvent.Wait();
        }

        /// <summary>
        /// Marks the WorkItem as NotRunnable.
        /// </summary>
        /// <param name="reason">Reason for test being NotRunnable.</param>
        public void MarkNotRunnable(string reason)
        {
            Result.SetResult(ResultState.NotRunnable, reason);
            WorkItemComplete();
        }

#if THREAD_ABORT
        private readonly object _threadLock = new();
        private int _nativeThreadId;
#endif

        /// <summary>
        /// Cancel (abort or stop) a WorkItem
        /// </summary>
        /// <param name="force">true if the WorkItem should be aborted, false if it should run to completion</param>
        public virtual void Cancel(bool force)
        {
            if (Context is not null)
                Context.ExecutionStatus = force ? TestExecutionStatus.AbortRequested : TestExecutionStatus.StopRequested;

#if THREAD_ABORT
            if (force)
            {
                Thread tThread;
                int tNativeThreadId;

                lock (_threadLock)
                {
                    // Exit if not running on a separate thread
                    if (_thread is null)
                        return;

                    tThread = _thread;
                    tNativeThreadId = _nativeThreadId;
                    _thread = null;
                }

                if (!tThread.Join(0))
                {
                    Log.Debug("Killing thread {0} for cancel", tThread.ManagedThreadId);
                    ThreadUtility.Kill(tThread, tNativeThreadId);

                    tThread.Join();

                    ChangeResult(ResultState.Cancelled, "Cancelled by user");

                    WorkItemComplete();
                }
            }
#endif
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Standard Dispose
        /// </summary>
        public void Dispose()
        {
            _completionEvent?.Dispose();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method that performs actually performs the work. It should
        /// set the State to WorkItemState.Complete when done.
        /// </summary>
        protected abstract void PerformWork();

        /// <summary>
        /// Method called by the derived class when all work is complete
        /// </summary>
        protected void WorkItemComplete()
        {
            State = WorkItemState.Complete;

            Result.StartTime = Context.StartTime;
            Result.EndTime = DateTime.UtcNow;
            Result.Duration = Context.Duration;

            // We add in the assert count from the context. If
            // this item is for a test case, we are adding the
            // test assert count to zero. If it's a fixture, we
            // are adding in any asserts that were run in the
            // fixture setup or teardown. Each context only
            // counts the asserts taking place in that context.
            // Each result accumulates the count from child
            // results along with its own asserts.
            Result.AssertCount += Context.AssertCount;

            Context.Listener.TestFinished(Result);

            Completed?.Invoke(this, EventArgs.Empty);
            _completionEvent.Set();

            //Clear references to test objects to reduce memory usage
            Context.TestObject = null;
            Test.Fixture = null;
        }

        /// <summary>
        /// Builds the set up tear down list.
        /// </summary>
        /// <param name="setUpMethods">Unsorted array of setup MethodInfos.</param>
        /// <param name="tearDownMethods">Unsorted array of teardown MethodInfos.</param>
        /// <param name="methodValidator">Method validator used before each method execution.</param>
        /// <returns>A list of SetUpTearDownItems</returns>
        protected List<SetUpTearDownItem> BuildSetUpTearDownList(
            IMethodInfo[] setUpMethods,
            IMethodInfo[] tearDownMethods,
            IMethodValidator? methodValidator = null)
        {
            Guard.ArgumentNotNull(setUpMethods, nameof(setUpMethods));
            Guard.ArgumentNotNull(tearDownMethods, nameof(tearDownMethods));

            var list = new List<SetUpTearDownItem>();

            Type? fixtureType = Test.TypeInfo?.Type;
            if (fixtureType is null)
                return list;

            while (fixtureType is not null && fixtureType != typeof(object))
            {
                var node = BuildNode(fixtureType, setUpMethods, tearDownMethods, methodValidator);
                if (node.HasMethods)
                    list.Add(node);

                fixtureType = fixtureType.BaseType;
            }

            return list;
        }

        // This method builds a list of nodes that can be used to
        // run setup and teardown according to the NUnit specs.
        // We need to execute setup and teardown methods one level
        // at a time. However, we can't discover them by reflection
        // one level at a time, because that would cause overridden
        // methods to be called twice, once on the base class and
        // once on the derived class.
        //
        // For that reason, we start with a list of all setup and
        // teardown methods, found using a single reflection call,
        // and then descend through the inheritance hierarchy,
        // adding each method to the appropriate level as we go.
        private static SetUpTearDownItem BuildNode(
            Type fixtureType,
            IList<IMethodInfo> setUpMethods,
            IList<IMethodInfo> tearDownMethods,
            IMethodValidator? methodValidator)
        {
            // Create lists of methods for this level only.
            // Note that FindAll can't be used because it's not
            // available on all the platforms we support.
            var mySetUpMethods = SelectMethodsByDeclaringType(fixtureType, setUpMethods);
            var myTearDownMethods = SelectMethodsByDeclaringType(fixtureType, tearDownMethods);

            return new SetUpTearDownItem(mySetUpMethods, myTearDownMethods, methodValidator);
        }

        private static List<IMethodInfo> SelectMethodsByDeclaringType(Type type, IList<IMethodInfo> methods)
        {
            var list = new List<IMethodInfo>();

            foreach (var method in methods)
            {
                if (method.TypeInfo.Type == type)
                    list.Add(method);
            }

            return list;
        }

        /// <summary>
        /// Changes the result of the test, logging the old and new states
        /// </summary>
        /// <param name="resultState">The new ResultState</param>
        /// <param name="message">The new message</param>
        protected void ChangeResult(ResultState resultState, string message)
        {
            Log.Debug("Changing result from {0} to {1}", Result.ResultState, resultState);

            Result.SetResult(resultState, message);
        }

        #endregion

        #region Private Methods

        private Thread? _thread;

        private void RunOnSeparateThread(ApartmentState apartment)
        {
            _thread = new Thread(() =>
            {
                Thread.CurrentThread.CurrentCulture = Context.CurrentCulture;
                Thread.CurrentThread.CurrentUICulture = Context.CurrentUICulture;
#if THREAD_ABORT
                lock (_threadLock)
                    _nativeThreadId = ThreadUtility.GetCurrentThreadNativeId();
#endif
                RunOnCurrentThread();
            });

#if NET6_0_OR_GREATER
            if (OperatingSystem.IsWindows())
            {
                _thread.SetApartmentState(apartment);
            }
            else
            {
                const string msg = "Apartment state cannot be set on this platform.";
                Log.Error(msg);
                Result.SetResult(ResultState.Skipped, msg);
                WorkItemComplete();
                return;
            }
#else
            try
            {
                _thread.SetApartmentState(apartment);
            }
            catch (PlatformNotSupportedException)
            {
                string msg = "Apartment state cannot be set on this platform.";
                Log.Error(msg);
                Result.SetResult(ResultState.Skipped, msg);
                WorkItemComplete();
                return;
            }
#endif

            _thread.Start();
            _thread.Join();
        }

        private void RunOnCurrentThread()
        {
            Context.CurrentTest = Test;
            Context.CurrentResult = Result;
            Context.Listener.TestStarted(Test);
            Context.StartTime = DateTime.UtcNow;
            Context.StartTicks = Stopwatch.GetTimestamp();
            Context.TestWorker = TestWorker;

            Context.EstablishExecutionEnvironment();

            State = WorkItemState.Running;

            PerformWork();
        }

        private ParallelExecutionStrategy GetExecutionStrategy()
        {
            // If there is no fixture and so nothing to do but dispatch
            // grandchildren we run directly. This saves time that would
            // otherwise be spent enqueuing and dequeuing items.
            if (Test.TypeInfo is null)
                return ParallelExecutionStrategy.Direct;

            // If the context is single-threaded we are required to run
            // the tests one by one on the same thread as the fixture.
            if (Context.IsSingleThreaded)
                return ParallelExecutionStrategy.Direct;

            // Check if item is explicitly marked as non-parallel
            if (ParallelScope.HasFlag(ParallelScope.None))
                return ParallelExecutionStrategy.NonParallel;

            // Check if item is explicitly marked as parallel
            if (ParallelScope.HasFlag(ParallelScope.Self))
                return ParallelExecutionStrategy.Parallel;

            // Item is not explicitly marked, so check the inherited context
            if (Context.ParallelScope.HasFlag(ParallelScope.Children) ||
                Test is TestFixture && Context.ParallelScope.HasFlag(ParallelScope.Fixtures))
            {
                return ParallelExecutionStrategy.Parallel;
            }

            // There is no scope specified either on the item itself or in the context.
            // In that case, simple work items are test cases and just run on the same
            // thread, while composite work items and teardowns are non-parallel.
            return this is SimpleWorkItem
                ? ParallelExecutionStrategy.Direct
                : ParallelExecutionStrategy.NonParallel;
        }

        /// <summary>
        /// Recursively walks up the test hierarchy to see if the
        /// <see cref="ApartmentState"/> has been set on any of the parent tests.
        /// </summary>
        private static ApartmentState GetTargetApartment(ITest test)
        {
            var apartment = test.Properties.TryGet(PropertyNames.ApartmentState, ApartmentState.Unknown);

            if (apartment == ApartmentState.Unknown && test.Parent is not null)
                return GetTargetApartment(test.Parent);

            return apartment;
        }

        #endregion
    }
}
