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
        public TestExecutionContext prior;

        /// <summary>
        /// The event listener currently receiving notifications
        /// </summary>
        private ITestListener listener = TestListener.NULL;

        /// <summary>
        /// The number of assertions for the current test
        /// </summary>
        private int assertCount;

        private RandomGenerator randomGenerator;

#if !NETCF
        /// <summary>
        /// The current culture
        /// </summary>
        private CultureInfo currentCulture;

        /// <summary>
        /// The current UI culture
        /// </summary>
        private CultureInfo currentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
        /// <summary>
        /// The current working directory
        /// </summary>
        private string currentDirectory;

        /// <summary>
        /// Destination for standard output
        /// </summary>
        private TextWriter outWriter;

        /// <summary>
        /// Destination for standard error
        /// </summary>
        private TextWriter errorWriter;

        /// <summary>
        /// Indicates whether trace is enabled
        /// </summary>
        private bool tracing;

        /// <summary>
        /// Destination for Trace output
        /// </summary>
        private TextWriter traceWriter;

        /// <summary>
        /// The current Principal.
        /// </summary>
        private IPrincipal currentPrincipal;

        /// <summary>
        /// Our LogCapture object
        /// </summary>
        private LogCapture logCapture;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        public TestExecutionContext()
        {
            this.prior = null;
            this.TestCaseTimeout = 0;

#if !NETCF
            this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            this.outWriter = Console.Out;
            this.errorWriter = Console.Error;
            this.traceWriter = null;
            this.tracing = false;
            this.currentDirectory = Environment.CurrentDirectory;
            this.currentPrincipal = Thread.CurrentPrincipal;
            this.logCapture = new Log4NetCapture();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        /// <param name="other">An existing instance of TestExecutionContext.</param>
        public TestExecutionContext( TestExecutionContext other )
        {
            this.prior = other;

            this.CurrentTest = other.CurrentTest;
            this.CurrentResult = other.CurrentResult;
            this.TestObject = other.TestObject;
            this.WorkDirectory = other.WorkDirectory;
            this.listener = other.listener;
            this.StopOnError = other.StopOnError;
            this.TestCaseTimeout = other.TestCaseTimeout;

#if !NETCF
            this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            this.outWriter = other.outWriter;
            this.errorWriter = other.errorWriter;
            this.traceWriter = other.traceWriter;
            this.tracing = other.tracing;
            this.currentDirectory = Environment.CurrentDirectory;
            this.currentPrincipal = Thread.CurrentPrincipal;
            this.logCapture = other.logCapture;
#endif

#if !NUNITLITE
            this.Dispatcher = other.Dispatcher;
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
        /// The current test event listener
        /// </summary>
        internal ITestListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

#if !NUNITLITE
        /// <summary>
        /// The current WorkItemDispatcher
        /// </summary>
        internal WorkItemDispatcher Dispatcher { get; set; }

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
                if (randomGenerator == null)
                {
                    randomGenerator = new RandomGenerator(CurrentTest.Seed);
                }
                return randomGenerator;
            }
        }

        /// <summary>
        /// Gets the assert count.
        /// </summary>
        /// <value>The assert count.</value>
        internal int AssertCount
        {
            get { return assertCount; }
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
            get { return currentCulture; }
            set
            {
                currentCulture = value;
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Saves or restores the CurrentUICulture
        /// </summary>
        public CultureInfo CurrentUICulture
        {
            get { return currentUICulture; }
            set
            {
                currentUICulture = value;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
            }
        }
#endif

#if !NETCF && !SILVERLIGHT
        /// <summary>
        /// Saves and restores the CurrentDirectory
        /// </summary>
        public string CurrentDirectory
        {
            get { return currentDirectory; }
            set
            {
                currentDirectory = value;
                Environment.CurrentDirectory = currentDirectory;
            }
        }

        /// <summary>
        /// Controls where Console.Out is directed
        /// </summary>
        internal TextWriter Out
        {
            get { return outWriter; }
            set 
            {
                if ( outWriter != value )
                {
                    outWriter = value; 
                    Console.Out.Flush();
                    Console.SetOut( outWriter );
                }
            }
        }

        /// <summary>
        /// Controls where Console.Error is directed
        /// </summary>
        internal TextWriter Error
        {
            get { return errorWriter; }
            set 
            {
                if ( errorWriter != value )
                {
                    errorWriter = value; 
                    Console.Error.Flush();
                    Console.SetError( errorWriter );
                }
            }
        }

        /// <summary>
        /// Controls whether trace and debug output are written
        /// to the standard output.
        /// </summary>
        internal bool Tracing
        {
            get { return tracing; }
            set
            {
                if (tracing != value)
                {
                    if (traceWriter != null && tracing)
                        StopTracing();

                    tracing = value;

                    if (traceWriter != null && tracing)
                        StartTracing();
                }
            }
        }

        /// <summary>
        /// Controls where Trace output is directed
        /// </summary>
        internal TextWriter TraceWriter
        {
            get { return traceWriter; }
            set
            {
                if ( traceWriter != value )
                {
                    if ( traceWriter != null  && tracing )
                        StopTracing();

                    traceWriter = value;

                    if ( traceWriter != null && tracing )
                        StartTracing();
                }
            }
        }

        private void StopTracing()
        {
            traceWriter.Close();
            System.Diagnostics.Trace.Listeners.Remove( "NUnit" );
        }

        private void StartTracing()
        {
            System.Diagnostics.Trace.Listeners.Add( new TextWriterTraceListener( traceWriter, "NUnit" ) );
        }

        /// <summary>
        /// Gets or sets the current <see cref="IPrincipal"/> for the Thread.
        /// </summary>
        public IPrincipal CurrentPrincipal
        {
            get { return this.currentPrincipal; }
            set
            {
                this.currentPrincipal = value;
                Thread.CurrentPrincipal = this.currentPrincipal;
            }
        }

        /// <summary>
        /// Controls whether log output is captured
        /// </summary>
        public bool Logging
        {
            get { return logCapture.Enabled; }
            set { logCapture.Enabled = value; }
        }

        /// <summary>
        ///  Gets or sets the Log writer, which is actually held by a log4net 
        ///  TextWriterAppender. When first set, the appender will be created
        ///  and will thereafter send any log events to the writer.
        ///  
        ///  In normal operation, LogWriter is set to an EventListenerTextWriter
        ///  connected to the EventQueue in the test domain. The events are
        ///  subsequently captured in the Gui an the output displayed in
        ///  the Log tab. The application under test does not need to define
        ///  any additional appenders.
        /// </summary>
        public TextWriter LogWriter
        {
            get { return logCapture.Writer; }
            set { logCapture.Writer = value; }
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
            this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            this.currentDirectory = Environment.CurrentDirectory;
            this.currentPrincipal = Thread.CurrentPrincipal;
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
            Thread.CurrentThread.CurrentCulture = this.currentCulture;
            Thread.CurrentThread.CurrentUICulture = this.currentUICulture;
#endif

#if !NETCF && !SILVERLIGHT
            // TODO: We should probably remove this feature, since
            // it potentially impacts all threads.
            Environment.CurrentDirectory = this.currentDirectory;
            Thread.CurrentPrincipal = this.currentPrincipal;
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
            System.Threading.Interlocked.Increment(ref assertCount);
        }

        /// <summary>
        /// Increments the assert count by a specified amount.
        /// </summary>
        public void IncrementAssertCount(int count)
        {
            // TODO: Temporary implementation
            while(count-- > 0)
                System.Threading.Interlocked.Increment(ref assertCount);
        }

        #endregion
    }
}
