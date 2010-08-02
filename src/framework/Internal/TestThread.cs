// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Represents a thread of test execution and runs a test
    /// on a thread, implementing timeout and setting the 
    /// apartment state appropriately.
    /// </summary>
    public abstract class TestThread
    {
        #region Private Fields
        /// <summary>
        /// The TestMethod or TestSuite to be run on the thread
        /// </summary>
        private Test test;

        /// <summary>
        /// The Thread object used to run tests
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// The result of the test being run
        /// </summary>
        protected TestResult testResult;

        /// <summary>
        /// The ITestListener to receive progress event calls
        /// </summary>
        protected ITestListener listener;

        /// <summary>
        /// Unexpected exception thrown by test thread
        /// </summary>
        protected Exception thrownException;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestThread"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        protected TestThread(Test test)
        {
            this.test = test;

            this.thread = new Thread(new ThreadStart(RunTestProc));
            thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;

            // Setting to Unknown causes an error under the Mono 1.0 profile
            if ( test.ApartmentState != ApartmentState.Unknown )
                this.ApartmentState = test.ApartmentState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the desired ApartmentState.
        /// </summary>
        /// <value>The ApartmentState to use for the test.</value>
        public ApartmentState ApartmentState
        {
#if CLR_2_0 || CLR_4_0
            get { return thread.GetApartmentState(); }
            set { thread.SetApartmentState(value); }
#else
            get { return thread.ApartmentState; }
            set { thread.ApartmentState = value; }
#endif
        }

        #endregion

        /// <summary>
        /// Run the test, honoring any timeout value provided. If the
        /// timeout is exceeded, set the testresult as a failure. As
        /// currently implemented, the thread proc calls test.doRun,
        /// which handles all exceptions itself. However, for safety,
        /// any exception thrown is rethrown upwards.
        /// 
        /// TODO: It would be cleaner to call test.Run, since that's
        /// part of the pubic interface, but it would require some
        /// restructuring of the Test hierarchy.
        /// </summary>
        public void Run(TestResult testResult, ITestListener listener)
        {
            this.thrownException = null;
            this.testResult = testResult;
            this.listener = listener;

            InternalTrace.Debug("Starting TestThread");
            thread.Start();
            thread.Join(this.Timeout);
            InternalTrace.Debug("Join Complete");

            // Timeout?
            if (thread.IsAlive)
            {
                thread.Abort();
                //thread.Join();
                testResult.SetResult(ResultState.Failure,
                    string.Format("Test exceeded Timeout value of {0}ms", Timeout));
            }

            // Handle any exception communicated back from the thread, ie. from proc!
            if (thrownException != null && thrownException.GetType() != typeof(ThreadAbortException))
                throw thrownException;
        }

        /// <summary>
        /// This is the engine of this class; the actual call to test.doRun!
        /// Note that any thrown exception is saved for later use!
        /// </summary>
        private void RunTestProc()
        {
            try
            {
                RunTest();
            }
            catch (Exception e)
            {
                thrownException = e;
            }
        }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>

        protected abstract int Timeout { get; }

        /// <summary>
        /// Runs the test.
        /// </summary>
        protected abstract void RunTest();
    }

    /// <summary>
    /// TestMethodThread is a specialized version of TestThread
    /// used for running methods.
    /// </summary>
    public class TestMethodThread : TestThread
    {
        private TestMethod testMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodThread"/> class.
        /// </summary>
        /// <param name="testMethod">The test method to be run.</param>
        public TestMethodThread(TestMethod testMethod)
            : base(testMethod)
        {
            this.testMethod = testMethod;
        }

        /// <summary>
        /// Gets the timeout to be used for this test.
        /// </summary>
        /// <value>The timeout.</value>
        protected override int Timeout
        {
            get 
            { 
                return testMethod.Timeout == 0
                    ? System.Threading.Timeout.Infinite
                    : testMethod.Timeout;
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        protected override void RunTest()
        {
            testMethod.doRun(testResult);
        }
    }

    /// <summary>
    /// TestSuiteThread specializes TestThread for running a test suite.
    /// </summary>
    public class TestSuiteThread : TestThread
    {
        private TestSuite suite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuiteThread"/> class.
        /// </summary>
        /// <param name="suite">The suite.</param>
        public TestSuiteThread(TestSuite suite)
            : base(suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Gets the timeout value to be used in running the test.
        /// </summary>
        /// <value>The timeout.</value>
        protected override int Timeout
        {
            get { return System.Threading.Timeout.Infinite; }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        protected override void RunTest()
        {
            suite.Run((TestSuiteResult)testResult, listener);
        }
    }
}
