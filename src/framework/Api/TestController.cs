// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.IO;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Api
{
    public class TestController : MarshalByRefObject
    {
        private ITestAssemblyBuilder builder;
        private ITestAssemblyRunner runner;
        private IDictionary options;

        #region Constructors

        /// <summary>
        /// Construct a TestController using default runner, builder and option settings.
        /// </summary>
        public TestController()
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();

            this.builder = new DefaultTestAssemblyBuilder();
            this.runner = new DefaultTestAssemblyRunner(this.builder);
        }

        /// <summary>
        /// Construct a TestController passing a dictionary of option settings.
        /// </summary>
        public TestController(IDictionary options)
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();

            this.builder = new DefaultTestAssemblyBuilder();
            this.runner = new DefaultTestAssemblyRunner(this.builder);

            this.options = options;
        }

        /// <summary>
        /// Construct a TestController, specifying the types to be used
        /// for the runner and builder.
        /// </summary>
        /// <param name="runnerType">The Type of the test runner</param>
        /// <param name="builderType">The Type of the test builder</param>
        public TestController(string runnerType, string builderType)
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            this.builder = (ITestAssemblyBuilder)myAssembly.CreateInstance(builderType);
            this.runner = (ITestAssemblyRunner)myAssembly.CreateInstance(
                runnerType, false, 0, null, new object[] { this.builder }, null, null);
        }

        #endregion

        #region Properties

        public ITestAssemblyBuilder Builder
        {
            get { return builder; }
        }

        public ITestAssemblyRunner Runner
        {
            get { return runner; }
        }

        public IDictionary Options
        {
            get { return options; }
        }

        #endregion

        #region InitializeLifetimeService

        /// <summary>
        /// InitializeLifetimeService returns null, allowing the instance to live indefinitely.
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region Nested Action Classes

        #region TestContollerAction

        /// <summary>
        /// TestControllerAction is the base class for all actions
        /// performed against a TestController.
        /// </summary>
        public abstract class TestControllerAction : MarshalByRefObject
        {
            private TestController controller;
            private ITestAssemblyRunner runner;
            private AsyncCallback callback;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestControllerAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="callback">The callback.</param>
            protected TestControllerAction(TestController controller, AsyncCallback callback)
            {
                this.controller = controller;
                this.runner = controller.Runner;
                this.callback = callback;
            }

            /// <summary>
            /// Gets the runner.
            /// </summary>
            /// <value>The runner.</value>
            protected ITestAssemblyRunner Runner
            {
                get { return this.runner; }
            }

            /// <summary>
            /// Reports the progress.
            /// </summary>
            /// <param name="progress">The progress.</param>
            public void ReportProgress(object progress)
            {
                callback(new ProgressReport(progress));
            }

            /// <summary>
            /// Reports the result.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
            public void ReportResult(object result, bool synchronous)
            {
                callback(new FinalResult(result, synchronous));
            }

            /// <summary>
            /// Initialize lifetime service to null so that the instance lives indefinitely.
            /// </summary>
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }

        #endregion

        #region LoadTestsAction

        /// <summary>
        /// LoadTestsAction loads a test into the TestController
        /// </summary>
        public class LoadTestsAction : TestControllerAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LoadTestsAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="assemblyFilename">The assembly filename.</param>
            /// <param name="callback">The callback.</param>
            public LoadTestsAction(TestController controller, string assemblyFilename, AsyncCallback callback) 
                : base(controller, callback)
            {
                ReportResult(Runner.Load(assemblyFilename), true);
            }
        }

        #endregion

        #region CountTestsAction

        /// <summary>
        /// CountTestsAction counts the number of test cases in the loaded TestSuite
        /// held by the TestController.
        /// </summary>
        public class CountTestsAction : TestControllerAction
        {
            /// <summary>
            /// Construct a CountsTestAction and perform the count of test cases.
            /// </summary>
            /// <param name="controller">A TestController holding the TestSuite whose cases are to be counted</param>
            /// <param name="callback">An AsyncCallback for reporting the count</param>
            public CountTestsAction(TestController controller, AsyncCallback callback) 
                : base(controller, callback)
            {
                ReportResult(Runner.CountTestCases(TestFilter.Empty), true);
            }
        }

        #endregion

        #region RunTestsAction

        /// <summary>
        /// RunTestsAction runs the loaded TestSuite held by the TestController.
        /// </summary>
        public class RunTestsAction : TestControllerAction, ITestListener
        {
            /// <summary>
            /// Construct a RunTestsAction and run all tests in the loaded TestSuite.
            /// </summary>
            /// <param name="controller">A TestController holding the TestSuite to run</param>
            /// <param name="callback">A callback used to report results</param>
            public RunTestsAction(TestController controller, AsyncCallback callback) 
                : base(controller, callback)
            {
                ReportResult(Runner.Run(this, TestFilter.Empty).ToXml(), true);
            }

            /// <summary>
            /// Construct a RunTestsAction and run tests in the loaded TestSuite that pass the supplied filter
            /// </summary>
            /// <param name="controller">A TestController holding the TestSuite to run</param>
            /// <param name="filter">A TestFilter used to determine which tests should be run</param>
            /// <param name="result">A callback used to report results</param>
            public RunTestsAction(TestController controller, TestFilter filter, AsyncCallback callback) 
                : base(controller, callback)
            {
                ReportResult(Runner.Run(this, filter), true);
            }

            #region ITestListener Members

            public void TestStarted(ITest test)
            {
            }

            public void TestFinished(TestResult result)
            {
            }

            public void TestOutput(TestOutput testOutput)
            {
                ReportProgress(testOutput);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested Classes for use with Callbacks

        /// <summary>
        /// AsyncResult is the abstract base for all callback classes
        /// used with the framework.
        /// </summary>
        [Serializable]
        public abstract class AsyncResult : IAsyncResult
        {
            private object state;
            private bool synchronous;
            private bool isCompleted;

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncResult"/> class.
            /// </summary>
            /// <param name="state">The state of the result.</param>
            /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
            /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
            public AsyncResult(object state, bool isCompleted, bool synchronous)
            {
                this.state = state;
                this.isCompleted = isCompleted;
                this.synchronous = synchronous;
            }

            #region IAsyncResult Members

            /// <summary>
            /// Gets the result of an operation.
            /// </summary>
            /// <value></value>
            /// <returns>The result state.</returns>
            public object AsyncState
            {
                get { return state; }
            }

            /// <summary>
            /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
            /// </summary>
            /// <returns>A WaitHandle</returns>
            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets a value that indicates whether the asynchronous operation completed synchronously.
            /// </summary>
            /// <returns>true if the operation completed synchronously; otherwise, false.
            /// </returns>
            public bool CompletedSynchronously
            {
                get { return synchronous; }
            }

            public bool IsCompleted
            {
                get { return isCompleted; }
            }

            #endregion
        }

        [Serializable]
        public class FinalResult : AsyncResult
        {
            public FinalResult(object state, bool synchronous) : base(state, true, synchronous) { }
        }

        [Serializable]
        public class ProgressReport : AsyncResult
        {
            public ProgressReport(object report) : base(report, false, false) { }
        }

        #endregion
    }
}
