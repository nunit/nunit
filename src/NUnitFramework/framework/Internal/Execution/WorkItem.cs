// ***********************************************************************
// Copyright (c) 2012-2017 Charlie Poole, Rob Prouse
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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    using Commands;

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
        static readonly Logger log = InternalTrace.GetLogger("WorkItem");

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

            ParallelScope = Test.Properties.ContainsKey(PropertyNames.ParallelScope)
                ? (ParallelScope)Test.Properties.Get(PropertyNames.ParallelScope)
                : ParallelScope.Default;

#if APARTMENT_STATE
            TargetApartment = GetTargetApartment(Test);
#endif

            State = WorkItemState.Ready;
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
            // Use the same Test, Result, Actions, Context, ParallelScope
            // and TargetApartment as the item being wrapped.
            Test = wrappedItem.Test;
            Result = wrappedItem.Result;
            Context = wrappedItem.Context;
            ParallelScope = wrappedItem.ParallelScope;
#if PARALLEL
            TestWorker = wrappedItem.TestWorker;
#endif
#if APARTMENT_STATE
            TargetApartment = wrappedItem.TargetApartment;
#endif

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
            Guard.OperationValid(Context == null, "The context has already been initialized");

            Context = context;
        }

        #endregion

        #region Properties and Events

        /// <summary>
        /// Event triggered when the item is complete
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Gets the current state of the WorkItem
        /// </summary>
        public WorkItemState State { get; private set; }

        /// <summary>
        /// The test being executed by the work item
        /// </summary>
        public Test Test { get; }

        /// <summary>
        /// The name of the work item - defaults to the Test name.
        /// </summary>
        public virtual string Name
        {
            get { return Test.Name; }
        }

        /// <summary>
        /// Filter used to include or exclude child tests
        /// </summary>
        public ITestFilter Filter { get; }

        /// <summary>
        /// The execution context
        /// </summary>
        public TestExecutionContext Context { get; private set; }

#if PARALLEL
        /// <summary>
        /// The worker executing this item.
        /// </summary>
        public TestWorker TestWorker { get; internal set; }

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
#endif

        /// <summary>
        /// The test result
        /// </summary>
        public TestResult Result { get; protected set; }

        /// <summary>
        /// Gets the ParallelScope associated with the test, if any,
        /// otherwise returning ParallelScope.Default;
        /// </summary>
        public ParallelScope ParallelScope { get; }

#if APARTMENT_STATE
        internal ApartmentState TargetApartment { get; set; }
        private ApartmentState CurrentApartment { get; set; }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute the current work item, including any
        /// child work items.
        /// </summary>
        public virtual void Execute()
        {
#if !NETSTANDARD1_6
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

#if APARTMENT_STATE
            CurrentApartment = Thread.CurrentThread.GetApartmentState();
            var targetApartment = TargetApartment == ApartmentState.Unknown ? CurrentApartment : TargetApartment;

            if (Test.RequiresThread || targetApartment != CurrentApartment)
#else
            if (Test.RequiresThread)
#endif
            {
                // Handle error conditions in a single threaded fixture
                if (Context.IsSingleThreaded)
                {
                    string msg = Test.RequiresThread
                        ? "RequiresThreadAttribute may not be specified on a test within a single-SingleThreadedAttribute fixture."
                        : "Tests in a single-threaded fixture may not specify a different apartment";

                    log.Error(msg);
                    Result.SetResult(ResultState.NotRunnable, msg);
                    WorkItemComplete();
                    return;
                }

                log.Debug("Running on separate thread because {0} is specified.",
                    Test.RequiresThread ? "RequiresThread" : "different Apartment");

#if APARTMENT_STATE
                RunOnSeparateThread(targetApartment);
#else
                RunOnSeparateThread();
#endif
            }
            else
                RunOnCurrentThread();
#else
                RunOnCurrentThread();
#endif
        }

        private readonly ManualResetEventSlim _completionEvent = new ManualResetEventSlim();

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
        private readonly object threadLock = new object();
        private int nativeThreadId;
#endif

        /// <summary>
        /// Cancel (abort or stop) a WorkItem
        /// </summary>
        /// <param name="force">true if the WorkItem should be aborted, false if it should run to completion</param>
        public virtual void Cancel(bool force)
        {
            if (Context != null)
                Context.ExecutionStatus = force ? TestExecutionStatus.AbortRequested : TestExecutionStatus.StopRequested;

#if THREAD_ABORT
            if (force)
            {
                Thread tThread;
                int tNativeThreadId;

                lock (threadLock)
                {
                    if (thread == null)
                        return;

                    tThread = thread;
                    tNativeThreadId = nativeThreadId;
                    thread = null;
                }

                if (!tThread.Join(0))
                {
                    log.Debug("Killing thread {0} for cancel", tThread.ManagedThreadId);
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

            long tickCount = Stopwatch.GetTimestamp() - Context.StartTicks;
            double seconds = (double)tickCount / Stopwatch.Frequency;
            Result.Duration = seconds;

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
        /// <returns>A list of SetUpTearDownItems</returns>
        protected List<SetUpTearDownItem> BuildSetUpTearDownList(MethodInfo[] setUpMethods, MethodInfo[] tearDownMethods)
        {
            Guard.ArgumentNotNull(setUpMethods, nameof(setUpMethods));
            Guard.ArgumentNotNull(tearDownMethods, nameof(tearDownMethods));

            var list = new List<SetUpTearDownItem>();

            Type fixtureType = Test.Type;
            if (fixtureType == null)
                return list;

            while (fixtureType != null && fixtureType != typeof(object))
            {
                var node = BuildNode(fixtureType, setUpMethods, tearDownMethods);
                if (node.HasMethods)
                    list.Add(node);

                fixtureType = fixtureType.GetTypeInfo().BaseType;
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
        private static SetUpTearDownItem BuildNode(Type fixtureType, IList<MethodInfo> setUpMethods, IList<MethodInfo> tearDownMethods)
        {
            // Create lists of methods for this level only.
            // Note that FindAll can't be used because it's not
            // available on all the platforms we support.
            var mySetUpMethods = SelectMethodsByDeclaringType(fixtureType, setUpMethods);
            var myTearDownMethods = SelectMethodsByDeclaringType(fixtureType, tearDownMethods);

            return new SetUpTearDownItem(mySetUpMethods, myTearDownMethods);
        }

        private static List<MethodInfo> SelectMethodsByDeclaringType(Type type, IList<MethodInfo> methods)
        {
            var list = new List<MethodInfo>();

            foreach (var method in methods)
                if (method.DeclaringType == type)
                    list.Add(method);

            return list;
        }

        /// <summary>
        /// Changes the result of the test, logging the old and new states
        /// </summary>
        /// <param name="resultState">The new ResultState</param>
        /// <param name="message">The new message</param>
        protected void ChangeResult(ResultState resultState, string message)
        {
            log.Debug("Changing result from {0} to {1}", Result.ResultState, resultState);

            Result.SetResult(resultState, message);
        }

#endregion

#region Private Methods

#if !NETSTANDARD1_6
        private Thread thread;

#if APARTMENT_STATE
        private void RunOnSeparateThread(ApartmentState apartment)
#else
        private void RunOnSeparateThread()
#endif
        {
            thread = new Thread(() =>
            {
                thread.CurrentCulture = Context.CurrentCulture;
                thread.CurrentUICulture = Context.CurrentUICulture;
#if THREAD_ABORT
                lock (threadLock)
                    nativeThreadId = ThreadUtility.GetCurrentThreadNativeId();
#endif
                RunOnCurrentThread();
            });
#if APARTMENT_STATE
            thread.SetApartmentState(apartment);
#endif
            thread.Start();
            thread.Join();
        }
#endif

                private void RunOnCurrentThread()
        {
            Context.CurrentTest = this.Test;
            Context.CurrentResult = this.Result;
            Context.Listener.TestStarted(this.Test);
            Context.StartTime = DateTime.UtcNow;
            Context.StartTicks = Stopwatch.GetTimestamp();
#if PARALLEL
            Context.TestWorker = this.TestWorker;
#endif

            Context.EstablishExecutionEnvironment();

            State = WorkItemState.Running;

            PerformWork();
        }

#if PARALLEL
        private ParallelExecutionStrategy GetExecutionStrategy()
        {
            // If there is no fixture and so nothing to do but dispatch
            // grandchildren we run directly. This saves time that would
            // otherwise be spent enqueuing and dequeuing items.
            if (Test.Type == null)
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
                return ParallelExecutionStrategy.Parallel;

            // There is no scope specified either on the item itself or in the context.
            // In that case, simple work items are test cases and just run on the same
            // thread, while composite work items and teardowns are non-parallel.
            return this is SimpleWorkItem
                ? ParallelExecutionStrategy.Direct
                : ParallelExecutionStrategy.NonParallel;
        }
#endif

#if APARTMENT_STATE
        /// <summary>
        /// Recursively walks up the test hierarchy to see if the
        /// <see cref="ApartmentState"/> has been set on any of the parent tests.
        /// </summary>
        static ApartmentState GetTargetApartment(ITest test)
        {
            var apartment = test.Properties.ContainsKey(PropertyNames.ApartmentState)
                ? (ApartmentState)test.Properties.Get(PropertyNames.ApartmentState)
                : ApartmentState.Unknown;

            if (apartment == ApartmentState.Unknown && test.Parent != null)
                return GetTargetApartment(test.Parent);

            return apartment;
        }
#endif

#endregion
    }

#if NET20 || NET35
    static class ActionTargetsExtensions
    {
        public static bool HasFlag(this ActionTargets targets, ActionTargets value)
        {
            return (targets & value) != 0;
        }
    }
#endif
}
