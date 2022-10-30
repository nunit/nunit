// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Execution;

#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Helper class used to save and restore certain static or
    /// singleton settings in the environment that affect tests
    /// or which might be changed by the user tests.
    /// </summary>
    public class TestExecutionContext : LongLivedMarshalByRefObject
#if NETFRAMEWORK
        , ILogicalThreadAffinative
#endif
    {
        // NOTE: Be very careful when modifying this class. It uses
        // conditional compilation extensively and you must give
        // thought to whether any new features will be supported
        // on each platform. In particular, instance fields,
        // properties, initialization and restoration must all
        // use the same conditions for each feature.

        #region Instance Fields

        /// <summary>
        /// Link to a prior saved context
        /// </summary>
        private readonly TestExecutionContext _priorContext;

        /// <summary>
        /// Indicates that a stop has been requested
        /// </summary>
        private TestExecutionStatus _executionStatus;

        /// <summary>
        /// The event listener currently receiving notifications
        /// </summary>
        private ITestListener _listener = TestListener.NULL;

        /// <summary>
        /// The number of assertions for the current test
        /// </summary>
        private int _assertCount;

        private Randomizer _randomGenerator;

        /// <summary>
        /// The current test result
        /// </summary>
        private TestResult _currentResult;

        private SandboxedThreadState _sandboxedThreadState;

#endregion

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        public TestExecutionContext()
        {
            _priorContext = null;
            TestCaseTimeout = 0;
            UpstreamActions = new List<ITestAction>();

            UpdateContextFromEnvironment();

            CurrentValueFormatter = (val) => MsgUtils.DefaultValueFormatter(val);
            IsSingleThreaded = false;
            DefaultFloatingPointTolerance = Tolerance.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        /// <param name="other">An existing instance of TestExecutionContext.</param>
        public TestExecutionContext(TestExecutionContext other)
        {
            _priorContext = other;

            CurrentTest = other.CurrentTest;
            CurrentResult = other.CurrentResult;
            TestObject = other.TestObject;
            _listener = other._listener;
            StopOnError = other.StopOnError;
            TestCaseTimeout = other.TestCaseTimeout;
            UpstreamActions = new List<ITestAction>(other.UpstreamActions);

            _sandboxedThreadState = other._sandboxedThreadState;

            DefaultFloatingPointTolerance = other.DefaultFloatingPointTolerance;

            CurrentValueFormatter = other.CurrentValueFormatter;

            Dispatcher = other.Dispatcher;
            ParallelScope = other.ParallelScope;
            IsSingleThreaded = other.IsSingleThreaded;
        }

#endregion

#region CurrentContext Instance

        // NOTE: We use different implementations for various platforms.

#if !NETFRAMEWORK
        private static readonly AsyncLocal<TestExecutionContext> _currentContext = new AsyncLocal<TestExecutionContext>();
        /// <summary>
        /// Gets and sets the current context.
        /// </summary>
        public static TestExecutionContext CurrentContext
        {
            get
            {
                return _currentContext.Value ?? (_currentContext.Value = new AdhocContext());
            }
            internal set // internal so that AdhocTestExecutionTests can get at it
            {
                _currentContext.Value = value;
            }
        }
#else
        // In all other builds, we use the CallContext
        /// <summary>
        /// Gets and sets the current context.
        /// </summary>
        public static TestExecutionContext CurrentContext
        {
            // This method invokes security critical members on the 'System.Runtime.Remoting.Messaging.CallContext' class.
            // Callers of this method have no influence on how these methods are used so we define a 'SecuritySafeCriticalAttribute'
            // rather than a 'SecurityCriticalAttribute' to enable use by security transparent callers.
            [SecuritySafeCritical]
            get
            {
                var context = CallContext.GetData(NUnitCallContext.TestExecutionContextKey) as TestExecutionContext;

                if (context == null)
                {
                    context = new AdhocContext();
                    CallContext.SetData(NUnitCallContext.TestExecutionContextKey, context);
                }

                return context;
            }
            // This method invokes security critical members on the 'System.Runtime.Remoting.Messaging.CallContext' class.
            // Callers of this method have no influence on how these methods are used so we define a 'SecuritySafeCriticalAttribute'
            // rather than a 'SecurityCriticalAttribute' to enable use by security transparent callers.
            [SecuritySafeCritical]
            private set
            {
                if (value == null)
                    CallContext.FreeNamedDataSlot(NUnitCallContext.TestExecutionContextKey);
                else
                    CallContext.SetData(NUnitCallContext.TestExecutionContextKey, value);
            }
        }
#endif

#endregion

#region Properties

        /// <summary>
        /// Gets or sets the current test
        /// </summary>
        public Test CurrentTest { get; set; }

        /// <summary>
        /// The time the current test started execution
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The time the current test started in Ticks
        /// </summary>
        public long StartTicks { get; set; }

        /// <summary>
        /// Gets the elapsed time for running the test in seconds
        /// </summary>
        public double Duration
        {
            get
            {
                var tickCount = Stopwatch.GetTimestamp() - StartTicks;
                return (double)tickCount / Stopwatch.Frequency;
            }
        }

        /// <summary>
        /// Gets or sets the current test result
        /// </summary>
        public TestResult CurrentResult
        {
            get { return _currentResult; }
            set
            {
                _currentResult = value;
                if (value != null)
                    OutWriter = value.OutWriter;
            }
        }

        /// <summary>
        /// Gets a TextWriter that will send output to the current test result.
        /// </summary>
        public TextWriter OutWriter { get; private set; }

        /// <summary>
        /// The current test object - that is the user fixture
        /// object on which tests are being executed.
        /// </summary>
        public object TestObject { get; set; }

        /// <summary>
        /// Get or set indicator that run should stop on the first error
        /// </summary>
        public bool StopOnError { get; set; }

        /// <summary>
        /// Gets an enum indicating whether a stop has been requested.
        /// </summary>
        public TestExecutionStatus ExecutionStatus
        {
            get
            {
                // ExecutionStatus may have been set to StopRequested or AbortRequested
                // in a prior context. If so, reflect the same setting in this context.
                if (_executionStatus == TestExecutionStatus.Running && _priorContext != null)
                    _executionStatus = _priorContext.ExecutionStatus;

                return _executionStatus;
            }
            set
            {
                _executionStatus = value;

                // Push the same setting up to all prior contexts
                if (_priorContext != null)
                    _priorContext.ExecutionStatus = value;
            }
        }

        /// <summary>
        /// The current test event listener
        /// </summary>
        internal ITestListener Listener
        {
            get { return _listener; }
            set { _listener = value; }
        }

        /// <summary>
        /// The current WorkItemDispatcher. Made public for
        /// use by nunitlite.tests
        /// </summary>
        public IWorkItemDispatcher Dispatcher { get; set; }

        /// <summary>
        /// The ParallelScope to be used by tests running in this context.
        /// For builds with out the parallel feature, it has no effect.
        /// </summary>
        public ParallelScope ParallelScope { get; set; }

        /// <summary>
        /// Default tolerance value used for floating point equality
        /// when no other tolerance is specified.
        /// </summary>
        public Tolerance DefaultFloatingPointTolerance { get; set; }

        /// <summary>
        /// The worker that spawned the context.
        /// For builds without the parallel feature, it is null.
        /// </summary>
        public TestWorker TestWorker {get; internal set;}

        /// <summary>
        /// Gets the RandomGenerator specific to this Test
        /// </summary>
        public Randomizer RandomGenerator
        {
            get
            {
                if (_randomGenerator == null)
                    _randomGenerator = new Randomizer(CurrentTest.Seed);
                return _randomGenerator;
            }
        }

        /// <summary>
        /// Gets the assert count.
        /// </summary>
        /// <value>The assert count.</value>
        internal int AssertCount
        {
            get { return _assertCount; }
        }

        /// <summary>
        /// The current nesting level of multiple assert blocks
        /// </summary>
        internal int MultipleAssertLevel { get; set; }

        /// <summary>
        /// Gets or sets the test case timeout value
        /// </summary>
        public int TestCaseTimeout { get; set; }

        /// <summary>
        /// Gets a list of ITestActions set by upstream tests
        /// </summary>
        public List<ITestAction> UpstreamActions { get; }

        // TODO: Put in checks on all of these settings
        // with side effects so we only change them
        // if the value is different

        /// <summary>
        /// Saves or restores the CurrentCulture
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get { return _sandboxedThreadState.Culture; }
            set
            {
                _sandboxedThreadState = _sandboxedThreadState.WithCulture(value);
                Thread.CurrentThread.CurrentCulture = value;
            }
        }

        /// <summary>
        /// Saves or restores the CurrentUICulture
        /// </summary>
        public CultureInfo CurrentUICulture
        {
            get { return _sandboxedThreadState.UICulture; }
            set
            {
                _sandboxedThreadState = _sandboxedThreadState.WithUICulture(value);
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="IPrincipal"/> for the Thread.
        /// </summary>
        public IPrincipal CurrentPrincipal
        {
            get { return _sandboxedThreadState.Principal; }
            set
            {
                _sandboxedThreadState = _sandboxedThreadState.WithPrincipal(value);
                ThreadUtility.SetCurrentThreadPrincipal(value);
            }
        }

        /// <summary>
        /// The current head of the ValueFormatter chain, copied from MsgUtils.ValueFormatter
        /// </summary>
        public ValueFormatter CurrentValueFormatter { get; private set; }

        /// <summary>
        /// If true, all tests must run on the same thread. No new thread may be spawned.
        /// </summary>
        public bool IsSingleThreaded { get; set; }

        /// <summary>
        /// The number of times the current test has been scheduled for execution.
        /// Currently only being executed in a test using the <see cref="RetryAttribute"/>
        /// </summary>
        public int CurrentRepeatCount { get; set; }

#endregion

#region Instance Methods

        /// <summary>
        /// Record any changes in the environment made by
        /// the test code in the execution context so it
        /// will be passed on to lower level tests.
        /// </summary>
        public void UpdateContextFromEnvironment()
        {
            _sandboxedThreadState = SandboxedThreadState.Capture();
        }

        /// <summary>
        /// Set up the execution environment to match a context.
        /// Note that we may be running on the same thread where the
        /// context was initially created or on a different thread.
        /// </summary>
        [SecuritySafeCritical] // This gives partial trust code the ability to capture an existing
                               // SynchronizationContext.Current and restore it at any time.
                               // This simply unblocks us on .NET Framework and is not in the spirit
                               // of partial trust. If we choose to make partial trust a design priority,
                               // we’ll need to thoroughly review more than just this instance.
        public void EstablishExecutionEnvironment()
        {
            _sandboxedThreadState.Restore();
            CurrentContext = this;
        }

        /// <summary>
        /// Increments the assert count by one.
        /// </summary>
        public void IncrementAssertCount()
        {
            Interlocked.Increment(ref _assertCount);
        }

        /// <summary>
        /// Increments the assert count by a specified amount.
        /// </summary>
        public void IncrementAssertCount(int count)
        {
            // TODO: Temporary implementation
            while (count-- > 0)
                Interlocked.Increment(ref _assertCount);
        }

        /// <summary>
        /// Adds a new ValueFormatterFactory to the chain of formatters
        /// </summary>
        /// <param name="formatterFactory">The new factory</param>
        public void AddFormatter(ValueFormatterFactory formatterFactory)
        {
            CurrentValueFormatter = formatterFactory(CurrentValueFormatter);
        }

        private TestExecutionContext CreateIsolatedContext()
        {
            var context = new TestExecutionContext(this);

            if (context.CurrentTest != null)
                context.CurrentResult = context.CurrentTest.MakeTestResult();

            context.TestWorker = TestWorker;

            return context;
        }

        /// <summary>
        /// Sends a message from test to listeners. This message is not kind of test output and doesn't go to test result.
        /// </summary>
        /// <param name="destination">A name recognized by the intended listeners.</param>
        /// <param name="message">A message to be sent</param>
        public void SendMessage(string destination, string message)
        {
            Listener?.SendMessage(new TestMessage(destination, message, CurrentTest.Id));
        }

        #endregion

        #region InitializeLifetimeService

        /// <summary>
        /// Obtain lifetime service object
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]  // Override of security critical method must be security critical itself
        public override object InitializeLifetimeService()
        {
            return null;
        }

#endregion

#region Nested IsolatedContext Class

        /// <summary>
        /// An IsolatedContext is used when running code
        /// that may effect the current result in ways that
        /// should not impact the final result of the test.
        /// A new TestExecutionContext is created with an
        /// initially clear result, which is discarded on
        /// exiting the context.
        /// </summary>
        /// <example>
        ///     using (new TestExecutionContext.IsolatedContext())
        ///     {
        ///         // Code that should not impact the result
        ///     }
        /// </example>
        public class IsolatedContext : IDisposable
        {
            private readonly TestExecutionContext _originalContext;

            /// <summary>
            /// Save the original current TestExecutionContext and
            /// make a new isolated context current.
            /// </summary>
            public IsolatedContext()
            {
                _originalContext = CurrentContext;
                CurrentContext = _originalContext.CreateIsolatedContext();
            }

            /// <summary>
            /// Restore the original TestExecutionContext.
            /// </summary>
            public void Dispose()
            {
                _originalContext.OutWriter.Write(CurrentContext.CurrentResult.Output);
                CurrentContext = _originalContext;
            }
        }

        #endregion

        #region Nested AdhocTestExecutionContext

        /// <summary>
        /// An AdhocTestExecutionContext is created whenever a context is needed
        /// but not available in CurrentContext. This happens when tests are run
        /// on an ad-hoc basis or Asserts are used outside of tests.
        /// </summary>
        public class AdhocContext : TestExecutionContext
        {
            /// <summary>
            /// Construct an AdhocTestExecutionContext, which is used
            /// whenever the current TestExecutionContext is found to be null.
            /// </summary>
            public AdhocContext()
            {
                var type = GetType();
                var method = type.GetMethod(nameof(AdhocTestMethod), BindingFlags.NonPublic | BindingFlags.Instance);

                CurrentTest = new TestMethod(new MethodWrapper(type, method));
                CurrentResult = CurrentTest.MakeTestResult();
            }

            private void AdhocTestMethod() { }
        }

#endregion
    }
}
