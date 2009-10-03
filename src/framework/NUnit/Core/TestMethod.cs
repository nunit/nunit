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

namespace NUnit.Core
{
	using System;
    using System.Threading;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Reflection;

	/// <summary>
	/// The TestMethod class represents a Test implemented as a method.
	/// 
	/// Because of how exceptions are handled internally, this class
	/// must incorporate processing of expected exceptions. A change to
	/// the Test interface might make it easier to process exceptions
	/// in an object that aggregates a TestMethod in the future.
	/// </summary>
	public abstract class TestMethod : Test
	{
        static Logger log = InternalTrace.GetLogger(typeof(TestMethod));

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

        /// <summary>
        /// The expected result of the method return value
        /// </summary>
	    internal object expectedResult;

        /// <summary>
        /// The fixture object, if it has been created
        /// </summary>
        private object fixture;

        private Exception builderException;

		#endregion

		#region Constructors
		public TestMethod( MethodInfo method ) 
			: base( method.ReflectedType.FullName, method.Name ) 
		{
            if( method.DeclaringType != method.ReflectedType)
                this.TestName.Name = method.DeclaringType.Name + "." + method.Name;

            this.method = method;
		}
		#endregion

		#region Properties
		public MethodInfo Method
		{
			get { return method; }
		}

        public override Type FixtureType
        {
            get { return method.ReflectedType; }
        }

        public ExpectedExceptionProcessor ExceptionProcessor
        {
            get { return exceptionProcessor; }
            set { exceptionProcessor = value; }
        }

		public bool ExceptionExpected
		{
            get { return exceptionProcessor != null; }
		}

        public override object Fixture
        {
            get { return fixture; }
            set { fixture = value; }
        }

        public int Timeout
        {
            get
            {
                return Properties.Contains("Timeout")
                    ? (int)Properties["Timeout"]
                    : TestContext.TestCaseTimeout;
            }
        }

        public Exception BuilderException
        {
            get { return builderException; }
            set { builderException = value; }
        }
        #endregion

		#region Run Methods
        public override TestResult Run(ITestListener listener, TestFilter filter)
        {
            using (new TestContext())
            {
                TestResult testResult = new TestResult(this);

                log.Debug("Test Starting: " + this.TestName.FullName);
                listener.TestStarted(this.TestName);
                long startTime = DateTime.Now.Ticks;

                switch (this.RunState)
                {
                    case RunState.Runnable:
                    case RunState.Explicit:
                        Run(testResult);
                        break;
                    case RunState.Skipped:
                    default:
                        testResult.Skip(IgnoreReason);
                        break;
                    case RunState.NotRunnable:
                        if (BuilderException != null)
                            testResult.Invalid(BuilderException);
                        else
                            testResult.Invalid(IgnoreReason);
                        break;
                    case RunState.Ignored:
                        testResult.Ignore(IgnoreReason);
                        break;
                }

                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                testResult.Time = time;

                listener.TestFinished(testResult);
                return testResult;
            }
        }
        
        public virtual void Run(TestResult testResult)
		{
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

                if (this.Properties["_SETCULTURE"] != null)
                    TestContext.CurrentCulture =
                        new System.Globalization.CultureInfo((string)Properties["_SETCULTURE"]);

                if (this.Properties["_SETUICULTURE"] != null)
                    TestContext.CurrentUICulture =
                        new System.Globalization.CultureInfo((string)Properties["_SETUICULTURE"]);

                int repeatCount = this.Properties.Contains("Repeat")
                    ? (int)this.Properties["Repeat"] : 1;

                while (repeatCount-- > 0)
                {
                    if (RequiresThread || Timeout > 0 || ApartmentState != GetCurrentApartment())
                        new TestMethodThread(this).Run(testResult, NullListener.NULL, TestFilter.Empty);
                    else
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
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();

                RecordException(ex, testResult);
            }
            finally
            {
                Fixture = null;
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
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();

                RecordException(ex, testResult);
			}
			finally 
			{
				doTearDown( testResult );

				DateTime stop = DateTime.Now;
				TimeSpan span = stop.Subtract(start);
				testResult.Time = (double)span.Ticks / (double)TimeSpan.TicksPerSecond;


                if (testResult.IsSuccess && this.Properties.Contains("MaxTime"))
                {
                    int elapsedTime = (int)Math.Round(testResult.Time * 1000.0);
                    int maxTime = (int)this.Properties["MaxTime"];

                    if (maxTime > 0 && elapsedTime > maxTime)
                        testResult.Failure(
                            string.Format("Elapsed time of {0}ms exceeds maximum of {1}ms",
                                elapsedTime, maxTime),
                            null);
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
				// TODO: What about ignore exceptions in teardown?
				testResult.Error( ex,FailureSite.TearDown );
			}
		}

		private void doTestCase( TestResult testResult )
		{
            try
            {
                RunTestMethod(testResult);
                if (testResult.IsSuccess && exceptionProcessor != null)
                    exceptionProcessor.ProcessNoException(testResult);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();

                if (exceptionProcessor == null)
                    RecordException(ex, testResult);
                else
                    exceptionProcessor.ProcessException(ex, testResult);
            }
		}

		public virtual void RunTestMethod(TestResult testResult)
		{
		    object fixture = this.method.IsStatic ? null : this.Fixture;

			object result = Reflect.InvokeMethod( this.method, fixture, this.arguments );

            if (this.expectedResult != null)
                NUnitFramework.Assert.AreEqual(expectedResult, result);

            testResult.Success();
        }

		#endregion

		#region Record Info About An Exception
		protected virtual void RecordException( Exception exception, TestResult testResult )
		{
            if (exception is NUnitException)
                exception = exception.InnerException;

            testResult.SetResult(NUnitFramework.GetResultState(exception), exception);
		}
		#endregion
    }
}
