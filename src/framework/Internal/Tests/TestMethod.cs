// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Reflection;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestMethod class represents a Test implemented as a method.
    /// Because of how exceptions are handled internally, this class
    /// must incorporate processing of expected exceptions. A change to
    /// the Test interface might make it easier to process exceptions
    /// in an object that aggregates a TestMethod in the future.
    /// </summary>
	public class TestMethod : Test
	{
		#region Fields
		/// <summary>
		/// The test method
		/// </summary>
		internal MethodInfo method;

		/// <summary>
		/// The SetUp method.
		/// </summary>
		protected MethodInfo[] setUpMethods;

		/// <summary>
		/// The teardown method
		/// </summary>
		protected MethodInfo[] tearDownMethods;

        /// <summary>
        /// The ExpectedExceptionProcessor for this test, if any
        /// </summary>
        internal ExpectedExceptionProcessor exceptionProcessor;

        /// <summary>
        /// Arguments to be used in invoking the method
        /// </summary>
	    internal object[] arguments;

#if !NUNITLITE
        /// <summary>
        /// The expected result of the method return value
        /// </summary>
	    internal object expectedResult;

        /// <summary>
        /// Indicated whether the method has an expected result.
        /// </summary>
	    internal bool hasExpectedResult;
#endif

        /// <summary>
        /// The fixture object, if it has been created
        /// </summary>
        private object fixture;

        private Exception builderException;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
		public TestMethod( MethodInfo method ) 
			: base( method.ReflectedType.FullName, method.Name ) 
		{
            // Disambiguate call to base class methods
            // TODO: This should not be here - it's a presentation issue
            if( method.DeclaringType != method.ReflectedType)
                this.Name = method.DeclaringType.Name + "." + method.Name;

            this.method = method;
		}
		#endregion

        #region Properties
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method that performs the test.</value>
		public MethodInfo Method
		{
			get { return method; }
		}

        /// <summary>
        /// Gets the Type of the fixture used in running this test
        /// </summary>
        /// <value></value>
        public override Type FixtureType
        {
            get { return method.ReflectedType; }
        }

        /// <summary>
        /// Gets or sets the exception processor.
        /// </summary>
        /// <value>The exception processor.</value>
        public ExpectedExceptionProcessor ExceptionProcessor
        {
            get { return exceptionProcessor; }
            set { exceptionProcessor = value; }
        }

        /// <summary>
        /// Gets a value indicating whether an exception is expected.
        /// </summary>
        /// <value><c>true</c> if an exception is expected; otherwise, <c>false</c>.</value>
		public bool ExceptionExpected
		{
            get { return exceptionProcessor != null; }
		}

        /// <summary>
        /// Gets or sets a fixture object for running this test
        /// </summary>
        /// <value></value>
        public override object Fixture
        {
            get { return fixture; }
            set { fixture = value; }
        }

#if !NUNITLITE
        /// <summary>
        /// Gets the timeout value to be used for this test.
        /// </summary>
        /// <value>The timeout in milliseconds.</value>
        public int Timeout
        {
            get
            {
                return Properties.ContainsKey(PropertyNames.Timeout)
                    ? (int)Properties.Get(PropertyNames.Timeout)
                    : TestExecutionContext.CurrentContext.TestCaseTimeout;
            }
        }
#endif

        /// <summary>
        /// Gets or sets a builder exception, which was thrown
        /// when attempting to construct the test.
        /// </summary>
        /// <value>The builder exception.</value>
        public Exception BuilderException
        {
            get { return builderException; }
            set { builderException = value; }
        }

        /// <summary>
        /// Indicates whether this test is a test case
        /// </summary>
        public override bool IsTestCase
        {
            get { return true; }
        }
        #endregion

		#region Run Methods

        /// <summary>
        /// Creates a TestCaseResult.
        /// </summary>
        /// <returns>The new TestCaseResult.</returns>
        public override TestResult MakeTestResult()
        {
            return new TestCaseResult(this);
        }

        /// <summary>
        /// Runs the test under a particular filter, sending
        /// notifications to a listener.
        /// </summary>
        /// <param name="listener">An event listener to receive notifications</param>
        /// <returns>A TestResult.</returns>
        public override TestResult Run(ITestListener listener)
        {
            TestResult testResult = this.MakeTestResult();

            listener.TestStarted(this);
            
            long startTime = DateTime.Now.Ticks;

            switch (this.RunState)
            {
                case RunState.Runnable:
                case RunState.Explicit:
                    Run(testResult);
                    break;
                case RunState.Skipped:
                default:
                    testResult.SetResult(ResultState.Skipped, IgnoreReason);
                    break;
                case RunState.NotRunnable:
                    if (BuilderException != null)
#if !NETCF_1_0
                        testResult.SetResult(ResultState.NotRunnable, 
                            ExceptionHelper.BuildMessage( BuilderException ), 
                            ExceptionHelper.BuildStackTrace(BuilderException));
#else
                        testResult.SetResult(ResultState.NotRunnable, 
                            ExceptionHelper.BuildMessage( BuilderException ));
#endif
                    else
                        testResult.SetResult(ResultState.NotRunnable, IgnoreReason);
                    break;
                case RunState.Ignored:
                    testResult.SetResult(ResultState.Ignored, IgnoreReason);
                    break;
            }

            long stopTime = DateTime.Now.Ticks;
            double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
            testResult.Time = time;

            testResult.AssertCount = NUnit.Framework.Assert.Counter;

            listener.TestFinished(testResult);
            return testResult;
        }

        /// <summary>
        /// Runs the test, recoding information in the specified TestResult.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        public virtual void Run(TestResult testResult)
		{
#if !NUNITLITE
            TestExecutionContext.Save();
            
            TestExecutionContext.CurrentContext.CurrentTest = this;
            TestExecutionContext.CurrentContext.CurrentResult = testResult;
#endif

            try
            {
                if (this.Parent != null)
                {
                    this.Fixture = this.Parent.Fixture;
                    TestSuite suite = this.Parent as TestSuite;
                    if (suite != null)
                    {
                        this.setUpMethods = suite.GetSetUpMethods();
                        this.tearDownMethods = suite.GetTearDownMethods();
                    }
                }

                // Temporary... to allow for tests that directly execute a test case
                if (Fixture == null && !method.IsStatic)
                    Fixture = Reflect.Construct(this.FixtureType);

#if !NUNITLITE
                string setCulture = (string)this.Properties.Get(PropertyNames.SetCulture);
                if (setCulture != null)
                    TestExecutionContext.CurrentContext.CurrentCulture =
                        new System.Globalization.CultureInfo(setCulture);

                string setUICulture = (string)this.Properties.Get(PropertyNames.SetUICulture);
                if (setUICulture != null)
                    TestExecutionContext.CurrentContext.CurrentUICulture =
                        new System.Globalization.CultureInfo(setUICulture);
#endif

                int repeatCount = this.Properties.ContainsKey(PropertyNames.RepeatCount)
                    ? (int)this.Properties.Get(PropertyNames.RepeatCount) : 1;

                while (repeatCount-- > 0)
                {
#if !NUNITLITE
                    if (RequiresThread || Timeout > 0 || ApartmentState != GetCurrentApartment())
                        new TestMethodThread(this).Run(testResult, TestListener.NULL);
                    else
#endif
                        doRun(testResult);

                    if (testResult.ResultState == ResultState.Failure ||
                        testResult.ResultState == ResultState.Error ||
                        testResult.ResultState == ResultState.Cancelled)
                    {
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif

                testResult.RecordException(ex);
            }
            finally
            {
                Fixture = null;
#if !NUNITLITE
                TestExecutionContext.Restore();
#endif
            }
		}

		/// <summary>
		/// The doRun method is used to run a test internally.
		/// It assumes that the caller is taking care of any 
		/// TestFixtureSetUp and TestFixtureTearDown needed.
		/// </summary>
		/// <param name="testResult">The result in which to record success or failure</param>
		public virtual void doRun( TestResult testResult )
		{
			DateTime start = DateTime.Now;

			try 
			{
                doSetUp();

				doTestCase( testResult );
			}
			catch(Exception ex)
			{
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif

                testResult.RecordException(ex);
			}
			finally 
			{
				doTearDown( testResult );

				DateTime stop = DateTime.Now;
				TimeSpan span = stop.Subtract(start);
				testResult.Time = (double)span.Ticks / (double)TimeSpan.TicksPerSecond;


                if (testResult.ResultState == ResultState.Success && this.Properties.ContainsKey(PropertyNames.MaxTime))
                {
                    int elapsedTime = (int)Math.Round(testResult.Time * 1000.0);
                    int maxTime = (int)this.Properties.Get(PropertyNames.MaxTime);

                    if (maxTime > 0 && elapsedTime > maxTime)
                        testResult.SetResult(ResultState.Failure,
                            string.Format("Elapsed time of {0}ms exceeds maximum of {1}ms",
                                elapsedTime, maxTime));
                }
			}
		}
		#endregion

		#region Invoke Methods by Reflection, Recording Errors

        private void doSetUp()
        {
            if (setUpMethods != null)
                foreach( MethodInfo setUpMethod in setUpMethods )
                    Reflect.InvokeMethod(setUpMethod, setUpMethod.IsStatic ? null : this.Fixture);
        }

		private void doTearDown( TestResult testResult )
		{
			try
			{
                if (tearDownMethods != null)
                {
                    int index = tearDownMethods.Length;
                    while (--index >= 0)
                        Reflect.InvokeMethod(tearDownMethods[index], tearDownMethods[index].IsStatic ? null : this.Fixture);
                }
			}
			catch(Exception ex)
			{
				if ( ex is NUnitException )
					ex = ex.InnerException;

                // TODO: Can we move this logic into TestResult itself?
                string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
                if (testResult.Message != null)
                    message = testResult.Message + NUnit.Env.NewLine + message;

#if !NETCF_1_0
                string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
                if (testResult.StackTrace != null)
                    stackTrace = testResult.StackTrace + NUnit.Env.NewLine + stackTrace;

                // TODO: What about ignore exceptions in teardown?
                testResult.SetResult(ResultState.Error, message, stackTrace);
#else
                testResult.SetResult(ResultState.Error, message);
#endif
            }
		}

		private void doTestCase( TestResult testResult )
		{
            try
            {
                RunTestMethod(testResult);
                if (testResult.ResultState == ResultState.Success && exceptionProcessor != null)
                    exceptionProcessor.ProcessNoException(testResult);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif

                if (exceptionProcessor == null)
                    testResult.RecordException(ex);
                else
                    exceptionProcessor.ProcessException(ex, testResult);
            }
		}

        /// <summary>
        /// Runs the test method setting the TestResult.
        /// </summary>
        /// <param name="testResult">The test result.</param>
		public virtual void RunTestMethod(TestResult testResult)
		{
		    object fixture = this.method.IsStatic ? null : this.Fixture;

			object result = Reflect.InvokeMethod( this.method, fixture, this.arguments );

#if !NUNITLITE
            if (this.hasExpectedResult)
                NUnit.Framework.Assert.AreEqual(expectedResult, result);
#endif

            testResult.SetResult(ResultState.Success);
        }

		#endregion
    }
}
