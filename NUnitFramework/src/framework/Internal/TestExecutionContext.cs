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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

#if !NUNITLITE
using System.Security.Principal;
#endif

using NUnit.Framework.Api;

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
	/// 
	/// Static methods for each setting forward to the internal 
	/// object on the top of the stack.
	/// </summary>
	public class TestExecutionContext
	{
        #region Instance Fields

        /// <summary>
        /// Link to a prior saved context
        /// </summary>
        public TestExecutionContext prior;

        /// <summary>
        /// The currently executing test
        /// </summary>
        private Test currentTest;

        /// <summary>
        /// The active TestResult for the current test
        /// </summary>
        private TestResult currentResult;
		
		/// <summary>
		/// The work directory to receive test output
		/// </summary>
		private string workDirectory;
		
        /// <summary>
        /// The object on which tests are currently being executed - i.e. the user fixture object
        /// </summary>
        private object testObject;

        /// <summary>
        /// The event listener currently receiving notifications
        /// </summary>
        private ITestListener listener = TestListener.NULL;

        /// <summary>
        /// The number of assertions for the current test
        /// </summary>
        private int assertCount;

        /// <summary>
        /// Indicates whether execution should terminate after the first error
        /// </summary>
        private bool stopOnError;

#if !NETCF_1_0
        /// <summary>
        /// Destination for standard output
        /// </summary>
        private TextWriter outWriter;

        /// <summary>
        /// Destination for standard error
        /// </summary>
        private TextWriter errorWriter;
#endif

#if !NETCF
        /// <summary>
		/// Indicates whether trace is enabled
		/// </summary>
		private bool tracing;

        /// <summary>
        /// Destination for Trace output
        /// </summary>
        private TextWriter traceWriter;

        /// <summary>
        /// The current culture
        /// </summary>
        private CultureInfo currentCulture;

        /// <summary>
        /// The current UI culture
        /// </summary>
        private CultureInfo currentUICulture;
#endif

#if !NUNITLITE
        /// <summary>
        /// Default timeout for test cases
        /// </summary>
        private int testCaseTimeout;

        /// <summary>
        /// Indicates whether logging is enabled
        /// </summary>
        private bool logging;

		/// <summary>
		/// The current working directory
		/// </summary>
		private string currentDirectory;

		private Log4NetCapture logCapture;

        /// <summary>
        /// The current Principal.
        /// </summary>
		private IPrincipal currentPrincipal;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        public TestExecutionContext()
		{
			this.prior = null;

#if !NETCF_1_0
			this.outWriter = Console.Out;
			this.errorWriter = Console.Error;
#endif

#if !NETCF
            this.traceWriter = null;
            this.tracing = false;
			this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NUNITLITE
            this.testCaseTimeout = 0;
			this.logging = false;
			this.currentDirectory = Environment.CurrentDirectory;
            this.logCapture = new Log4NetCapture();
            this.currentPrincipal = Thread.CurrentPrincipal;
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionContext"/> class.
        /// </summary>
        /// <param name="other">An existing instance of TestExecutionContext.</param>
		public TestExecutionContext( TestExecutionContext other )
		{
			this.prior = other;

            this.currentTest = other.currentTest;
            this.currentResult = other.currentResult;
            this.testObject = other.testObject;
			this.workDirectory = other.workDirectory;
            this.listener = other.listener;
            this.stopOnError = other.stopOnError;

#if !NETCF_1_0
			this.outWriter = other.outWriter;
			this.errorWriter = other.errorWriter;
#endif

#if !NETCF
            this.traceWriter = other.traceWriter;
            this.tracing = other.tracing;
			this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif

#if !NUNITLITE
            this.testCaseTimeout = other.testCaseTimeout;
			this.logging = other.logging;
			this.currentDirectory = Environment.CurrentDirectory;
            this.logCapture = other.logCapture;
            this.currentPrincipal = Thread.CurrentPrincipal;
#endif
        }

        #endregion

        #region Static Singleton Instance

        /// <summary>
        /// The current context, head of the list of saved contexts.
        /// </summary>
        private static TestExecutionContext current = new TestExecutionContext();

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>The current context.</value>
        public static TestExecutionContext CurrentContext
        {
            get { return current; }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Saves the old context and makes a fresh one 
        /// current without changing any settings.
        /// </summary>
        public static void Save()
        {
            TestExecutionContext.current = new TestExecutionContext(current);
        }

        /// <summary>
        /// Restores the last saved context and puts
        /// any saved settings back into effect.
        /// </summary>
        public static void Restore()
        {
            current.ReverseChanges();

            int latestAsserts = current.AssertCount;
            current = current.prior;
            current.assertCount += latestAsserts;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current test
        /// </summary>
        public Test CurrentTest
        {
            get { return currentTest; }
            set { currentTest = value; }
        }

        /// <summary>
        /// Gets or sets the current test result
        /// </summary>
        public TestResult CurrentResult
        {
            get { return currentResult; }
            set { currentResult = value; }
        }

        /// <summary>
        /// The current test object - that is the user fixture
        /// object on which tests are being executed.
        /// </summary>
        public object TestObject
        {
            get { return testObject; }
            set { testObject = value; }
        }
		
        /// <summary>
        /// Get or set the working directory
        /// </summary>
		public string WorkDirectory
		{
			get { return workDirectory; }
			set { workDirectory = value; }
		}

        /// <summary>
        /// Get or set indicator that run should stop on the first error
        /// </summary>
        public bool StopOnError
        {
            get { return stopOnError; }
            set { stopOnError = value; }
        }
		
        /// <summary>
        /// The current test event listener
        /// </summary>
        public ITestListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        /// <summary>
        /// Gets the assert count.
        /// </summary>
        /// <value>The assert count.</value>
        public int AssertCount
        {
            get { return assertCount; }
        }

#if !NETCF_1_0
        /// <summary>
		/// Controls where Console.Out is directed
		/// </summary>
		public TextWriter Out
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
		public TextWriter Error
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
#endif

#if !NETCF
        /// <summary>
        /// Controls whether trace and debug output are written
        /// to the standard output.
        /// </summary>
        public bool Tracing
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
		public TextWriter TraceWriter
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

#if !NUNITLITE
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
        /// Gets or sets the test case timeout vaue
        /// </summary>
        public int TestCaseTimeout
        {
            get { return testCaseTimeout; }
            set { testCaseTimeout = value; }
        }
#endif

        #endregion

        #region Instance Methods

        /// <summary>
		/// Used to restore settings to their prior
		/// values before reverting to a prior context.
		/// </summary>
		public void ReverseChanges()
		{ 
			if ( prior == null )
				throw new InvalidOperationException( "TestContext: too many Restores" );

#if !NETCF_1_0
			this.Out = prior.Out;
			this.Error = prior.Error;
#endif

#if !NETCF
            this.Tracing = prior.Tracing;
			this.CurrentCulture = prior.CurrentCulture;
            this.CurrentUICulture = prior.CurrentUICulture;
#endif

#if !NUNITLITE
            this.CurrentDirectory = prior.CurrentDirectory;
            this.TestCaseTimeout = prior.TestCaseTimeout;
			this.CurrentPrincipal = prior.CurrentPrincipal;
#endif
		}

        /// <summary>
        /// Record any changed values in the current context
        /// </summary>
        public void Update()
        {
#if !NETCF
            this.currentCulture = CultureInfo.CurrentCulture;
            this.currentUICulture = CultureInfo.CurrentUICulture;
#endif
#if !NUNITLITE
            this.currentDirectory = Environment.CurrentDirectory;
            this.currentPrincipal = System.Threading.Thread.CurrentPrincipal;
#endif
        }

        /// <summary>
        /// Increments the assert count.
        /// </summary>
        public void IncrementAssertCount()
        {
            System.Threading.Interlocked.Increment(ref assertCount);
        }

        #endregion
	}
}
