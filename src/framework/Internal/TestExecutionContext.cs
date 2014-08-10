// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;

#if !SILVERLIGHT && !NETCF
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Enumeration indicating whether the tests are 
    /// running normally or being cancelled.
    /// </summary>
    public enum TestExecutionStatus
    {
        /// <summary>
        /// Running normally with no stop requested
        /// </summary>
        Running,

        /// <summary>
        /// A graceful stop has been requested
        /// </summary>
        StopRequested,

        /// <summary>
        /// A forced stop has been requested
        /// </summary>
        AbortRequested
    }

    /// <summary>
    /// Helper class used to save and restore certain static or
    /// singleton settings in the environment that affect tests 
    /// or which might be changed by the user tests.
    /// 
    /// An internal class is used to hold settings and a stack
    /// of these objects is pushed and popped as Save and Restore
    /// are called.
    /// </summary>
    public class TestExecutionContext
#if !SILVERLIGHT && !NETCF
        : ILogicalThreadAffinative
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
        private TestExecutionContext _priorContext;

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

        private RandomGenerator _randomGenerator;

        private IWorkItemDispatcher _dispatcher;

#if !NETCF
        /// <summary>
        /// The current culture
        /// </summary>
        private CultureInfo _currentCulture;

        /// <summary>
        /// The current UI culture
        /// </summary>
        private CultureInfo _currentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
        /// <summary>
        /// The current working directory
        /// </summary>
        private string _currentDirectory;

        /// <summary>
        /// Destination for standard output
        /// </summary>
        private TextWriter _outWriter;

        /// <summary>
        /// Destination for standard error
        /// </summary>
        private TextWriter _errorWriter;

        /// <summary>
        /// Indicates whether trace is enabled
        /// </summary>
        private bool _tracing;

        /// <summary>
        /// Destination for Trace output
        /// </summary>
        private TextWriter _traceWriter;

        /// <summary>
        /// The current Principal.
        /// </summary>
        private IPrincipal _currentPrincipal;

#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        public TestExecutionContext()
        {
            _priorContext = null;
            this.TestCaseTimeout = 0;

#if !NETCF
            _currentCulture = CultureInfo.CurrentCulture;
            _currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            _outWriter = Console.Out;
            _errorWriter = Console.Error;
            _traceWriter = null;
            _tracing = false;
            _currentDirectory = Environment.CurrentDirectory;
            _currentPrincipal = Thread.CurrentPrincipal;
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        /// <param name="other">An existing instance of TestExecutionContext.</param>
        public TestExecutionContext( TestExecutionContext other )
        {
            _priorContext = other;

            this.CurrentTest = other.CurrentTest;
            this.CurrentResult = other.CurrentResult;
            this.TestObject = other.TestObject;
            this.WorkDirectory = other.WorkDirectory;
            _listener = other._listener;
            this.StopOnError = other.StopOnError;
            this.TestCaseTimeout = other.TestCaseTimeout;

#if !NETCF
            _currentCulture = CultureInfo.CurrentCulture;
            _currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            _outWriter = other._outWriter;
            _errorWriter = other._errorWriter;
            _traceWriter = other._traceWriter;
            _tracing = other._tracing;
            _currentDirectory = Environment.CurrentDirectory;
            _currentPrincipal = Thread.CurrentPrincipal;
#endif

            this.Dispatcher = other.Dispatcher;
#if !NUNITLITE
            this.ParallelScope = other.ParallelScope;
#endif
        }

        #endregion

        #region Static Singleton Instance

        /// <summary>
        /// The current context, head of the list of saved contexts.
        /// </summary>
#if SILVERLIGHT
        [ThreadStatic]
        private static TestExecutionContext current;
#elif NETCF
        private static TestExecutionContext current;
#else
        private static readonly string CONTEXT_KEY = "NUnit.Framework.TestContext";
#endif

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>The current context.</value>
        public static TestExecutionContext CurrentContext
        {
            get 
            {
                // If a user creates a thread then the current context
                // will be null. This also happens when the compiler
                // automatically creates threads for async methods.
                // We create a new context, which is automatically
                // populated with _values taken from the current thread.
#if SILVERLIGHT || NETCF
                if (current == null)
                    current = new TestExecutionContext();

                return current; 
#else
                var context = CallContext.GetData(CONTEXT_KEY) as TestExecutionContext;
                if (context == null) // This can happen on Mono
                {
                    context = new TestExecutionContext();
                    CallContext.SetData(CONTEXT_KEY, context);
                }

                return context;
#endif
            }
            private set 
            { 
#if SILVERLIGHT || NETCF
                current = value;
#else
                if (value == null)
                    CallContext.FreeNamedDataSlot(CONTEXT_KEY);
                else
                    CallContext.SetData(CONTEXT_KEY, value);
#endif
            }
        }

        /// <summary>
        /// Clear the current context. This is provided to
        /// prevent "leakage" of the CallContext containing
        /// the current context back to any runners.
        /// </summary>
        public static void ClearCurrentContext()
        {
            CurrentContext = null;
        }

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
        /// Gets or sets the current test result
        /// </summary>
        public TestResult CurrentResult { get; set; }

        /// <summary>
        /// The current test object - that is the user fixture
        /// object on which tests are being executed.
        /// </summary>
        public object TestObject { get; set; }
        
        /// <summary>
        /// Get or set the working directory
        /// </summary>
        public string WorkDirectory { get; set; }

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
        /// The current WorkItemDispatcher
        /// </summary>
        internal IWorkItemDispatcher Dispatcher 
        {
            get
            {
                if (_dispatcher == null)
                    _dispatcher = new SimpleWorkItemDispatcher();

                return _dispatcher;
            }
            set { _dispatcher = value;  }
        }

#if !NUNITLITE
        /// <summary>
        /// The ParallelScope to be used by tests running in this context
        /// </summary>
        public ParallelScope ParallelScope { get; set; }
#endif

        /// <summary>
        /// Gets the RandomGenerator specific to this Test
        /// </summary>
        public RandomGenerator RandomGenerator
        {
            get
            {
                if (_randomGenerator == null)
                {
                    _randomGenerator = new RandomGenerator(CurrentTest.Seed);
                }
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
        /// Gets or sets the test case timeout value
        /// </summary>
        public int TestCaseTimeout { get; set; }

#if !NETCF
        // TODO: Put in checks on all of these settings
        // with side effects so we only change them
        // if the value is different

        /// <summary>
        /// Saves or restores the CurrentCulture
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                _currentCulture = value;
                Thread.CurrentThread.CurrentCulture = _currentCulture;
            }
        }

        /// <summary>
        /// Saves or restores the CurrentUICulture
        /// </summary>
        public CultureInfo CurrentUICulture
        {
            get { return _currentUICulture; }
            set
            {
                _currentUICulture = value;
                Thread.CurrentThread.CurrentUICulture = _currentUICulture;
            }
        }
#endif

#if !NETCF && !SILVERLIGHT
        /// <summary>
        /// Saves and restores the CurrentDirectory
        /// </summary>
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                Environment.CurrentDirectory = _currentDirectory;
            }
        }

        /// <summary>
        /// Controls where Console.Out is directed
        /// </summary>
        internal TextWriter Out
        {
            get { return _outWriter; }
            set 
            {
                if ( _outWriter != value )
                {
                    _outWriter = value; 
                    Console.Out.Flush();
                    Console.SetOut( _outWriter );
                }
            }
        }

        /// <summary>
        /// Controls where Console.Error is directed
        /// </summary>
        internal TextWriter Error
        {
            get { return _errorWriter; }
            set 
            {
                if ( _errorWriter != value )
                {
                    _errorWriter = value; 
                    Console.Error.Flush();
                    Console.SetError( _errorWriter );
                }
            }
        }

        /// <summary>
        /// Controls whether trace and debug output are written
        /// to the standard output.
        /// </summary>
        internal bool Tracing
        {
            get { return _tracing; }
            set
            {
                if (_tracing != value)
                {
                    if (_traceWriter != null && _tracing)
                        StopTracing();

                    _tracing = value;

                    if (_traceWriter != null && _tracing)
                        StartTracing();
                }
            }
        }

        /// <summary>
        /// Controls where Trace output is directed
        /// </summary>
        internal TextWriter TraceWriter
        {
            get { return _traceWriter; }
            set
            {
                if ( _traceWriter != value )
                {
                    if ( _traceWriter != null  && _tracing )
                        StopTracing();

                    _traceWriter = value;

                    if ( _traceWriter != null && _tracing )
                        StartTracing();
                }
            }
        }

        private void StopTracing()
        {
            _traceWriter.Close();
            System.Diagnostics.Trace.Listeners.Remove( "NUnit" );
        }

        private void StartTracing()
        {
            System.Diagnostics.Trace.Listeners.Add( new TextWriterTraceListener( _traceWriter, "NUnit" ) );
        }

        /// <summary>
        /// Gets or sets the current <see cref="IPrincipal"/> for the Thread.
        /// </summary>
        public IPrincipal CurrentPrincipal
        {
            get { return _currentPrincipal; }
            set
            {
                _currentPrincipal = value;
                Thread.CurrentPrincipal = _currentPrincipal;
            }
        }

#endif

        #endregion

        #region Instance Methods

        /// <summary>
        /// Record any changes in the environment made by
        /// the test code in the execution context so it
        /// will be passed on to lower level tests.
        /// </summary>
        public void UpdateContextFromEnvironment()
        {
#if !NETCF
            _currentCulture = CultureInfo.CurrentCulture;
            _currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            _currentDirectory = Environment.CurrentDirectory;
            _currentPrincipal = Thread.CurrentPrincipal;
#endif
        }

        /// <summary>
        /// Set up the execution environment to match a context.
        /// Note that we may be running on the same thread where the
        /// context was initially created or on a different thread.
        /// </summary>
        public void EstablishExecutionEnvironment()
        {
#if !NETCF
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            // TODO: We should probably remove this feature, since
            // it potentially impacts all threads.
            Environment.CurrentDirectory = _currentDirectory;
            Thread.CurrentPrincipal = _currentPrincipal;
            Console.SetOut(this.Out);
            Console.SetError(this.Error);
#endif

            CurrentContext = this;
        }

        /// <summary>
        /// Increments the assert count by one.
        /// </summary>
        public void IncrementAssertCount()
        {
            System.Threading.Interlocked.Increment(ref _assertCount);
        }

        /// <summary>
        /// Increments the assert count by a specified amount.
        /// </summary>
        public void IncrementAssertCount(int count)
        {
            // TODO: Temporary implementation
            while(count-- > 0)
                System.Threading.Interlocked.Increment(ref _assertCount);
        }

        #endregion
    }
}
