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
using System.Diagnostics;
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Execution
{
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
    public abstract class WorkItem
    {
        // The current state of the WorkItem
        private WorkItemState _state;

        // The test this WorkItem represents
        private Test _test;

        /// <summary>
        /// The result of running the test
        /// </summary>
        protected TestResult _testResult;

        // The execution context used by this work item
        private TestExecutionContext _context;

        #region Static Factory Method

        static public WorkItem CreateWorkItem(Test test, TestExecutionContext context, ITestFilter filter)
        {
            TestSuite suite = test as TestSuite;
            if (suite != null)
                return new CompositeWorkItem(suite, context, filter);
            else
                return new SimpleWorkItem((TestMethod)test, context);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a WorkItem for a particular test.
        /// </summary>
        /// <param name="test">The test that the WorkItem will run</param>
        /// <param name="context">The TestExecutionContext in which it will run</param>
        public WorkItem(Test test, TestExecutionContext context)
        {
            _test = test;
            _testResult = test.MakeTestResult();
            _state = WorkItemState.Ready;
            _context = context;
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
        public WorkItemState State
        {
            get { return _state; }
        }

        /// <summary>
        /// The test being executed by the work item
        /// </summary>
        public Test Test
        {
            get { return _test; }
        }

        /// <summary>
        /// The execution context
        /// </summary>
        protected TestExecutionContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// The test result
        /// </summary>
        public TestResult Result
        {
            get { return _testResult; }
        }

#if !SILVERLIGHT && !NETCF
        internal ApartmentState TargetApartment
        {
            get 
            {
                return Test.Properties.ContainsKey(PropertyNames.ApartmentState)
                    ? (ApartmentState)_test.Properties.Get(PropertyNames.ApartmentState)
                    : ApartmentState.Unknown;
            }
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute the current work item, including any
        /// child work items.
        /// </summary>
        public virtual void Execute()
        {
#if !SILVERLIGHT && !NETCF
            // Timeout set at a higher level
            int timeout = _context.TestCaseTimeout;

            // Timeout set on this test
            if (Test.Properties.ContainsKey(PropertyNames.Timeout))
                timeout = (int)Test.Properties.Get(PropertyNames.Timeout);

            ApartmentState currentApartment = Thread.CurrentThread.GetApartmentState();

            if (Test.RequiresThread || Test is TestMethod && timeout > 0 || currentApartment != TargetApartment && TargetApartment != ApartmentState.Unknown)
                RunTestOnOwnThread(timeout, TargetApartment);
            else
                RunTest();
#else
            RunTest();
#endif
        }


#if !SILVERLIGHT && !NETCF
        private void RunTestOnOwnThread(int timeout, ApartmentState apartment)
        {
            string reason = Test.RequiresThread
                ? "has RequiresThreadAttribute."
                : timeout > 0
                    ? "has Timeout value set."
                    : "requires a different apartment.";
            InternalTrace.Debug("Running test on own thread because it " + reason);

            Thread thread = new Thread(new ThreadStart(RunTest));

            thread.SetApartmentState(apartment);
            thread.CurrentCulture = Context.CurrentCulture;
            thread.CurrentUICulture = Context.CurrentUICulture;

            thread.Start();

            if (!Test.IsAsynchronous || timeout > 0)
            {
                if (timeout <= 0)
                    timeout = Timeout.Infinite;

                thread.Join(timeout);

                if (thread.IsAlive)
                {
                    ThreadUtility.Kill(thread);

                    // NOTE: Without the use of Join, there is a race condition here.
                    // The thread sets the result to Cancelled and our code below sets
                    // it to Failure. In order for the result to be shown as a failure,
                    // we need to ensure that the following code executes after the
                    // thread has terminated. There is a risk here: the test code might
                    // refuse to terminate. However, it's more important to deal with
                    // the normal rather than a pathological case.
                    thread.Join();

                    Result.SetResult(ResultState.Failure,
                        string.Format("Test exceeded Timeout value of {0}ms", timeout));

                    WorkItemComplete();
                }
            }
        }
#endif

        private void RunTest()
        {
            _context.CurrentTest = this.Test;
            _context.CurrentResult = this.Result;
            _context.Listener.TestStarted(this.Test);
            _context.StartTime = DateTime.Now;

            _context.EstablishExecutionEnvironment();

            PerformWork();
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
            _state = WorkItemState.Complete;

            //long tickCount = Stopwatch.GetTimestamp() - Context.StartTicks;
            //double seconds = (double)tickCount / Stopwatch.Frequency;
            //Result.Duration = TimeSpan.FromSeconds(seconds);

            Result.Duration = DateTime.Now - Context.StartTime;

            // We add in the assert count from the context. If
            // this item is for a test case, we are adding the
            // test assert count to zero. If it's a fixture, we
            // are adding in any asserts that were run in the
            // fixture setup or teardown. Each context only
            // counts the asserts taking place in that context.
            // Each result accumulates the count from child
            // results along with it's own asserts.
            Result.AssertCount += Context.AssertCount;

            _context.Listener.TestFinished(Result);

            if (Completed != null)
                Completed(this, EventArgs.Empty);
        }

        #endregion
    }
}
