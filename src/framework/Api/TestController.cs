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

        #region Constructors

        /// <summary>
        /// Construct a TestController using the default builder and runner.
        /// </summary>
        public TestController()
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();

            this.builder = new DefaultTestAssemblyBuilder();
            this.runner = new DefaultTestAssemblyRunner(this.builder);
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
            private AsyncCallback callback;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestControllerAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="callback">The callback.</param>
            protected TestControllerAction(TestController controller, AsyncCallback callback)
            {
                this.controller = controller;
                this.callback = callback;
            }

            /// <summary>
            /// Reports the result.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
            protected void ReportResult(object result, bool synchronous)
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
            /// <param name="loadOptions">Options controlling how the tests are loaded</param>
            /// <param name="callback">The callback.</param>
            public LoadTestsAction(TestController controller, string assemblyFilename, IDictionary loadOptions, AsyncCallback callback) 
                : base(controller, callback)
            {
                ReportResult(controller.Runner.Load(assemblyFilename, loadOptions), true);
            }
        }

        #endregion

        #region CountTestsAction

        /// <summary>
        /// CountTestsAction counts the number of test cases in the loaded TestSuite
        /// held by the TestController.
        /// </summary>
        //public class CountTestsAction : TestControllerAction
        //{
        //    /// <summary>
        //    /// Construct a CountsTestAction and perform the count of test cases.
        //    /// </summary>
        //    /// <param name="controller">A TestController holding the TestSuite whose cases are to be counted</param>
        //    /// <param name="callback">An AsyncCallback for reporting the count</param>
        //    public CountTestsAction(TestController controller, AsyncCallback callback) 
        //        : base(controller, callback)
        //    {
        //        ReportResult(Runner.CountTestCases(TestFilter.Empty), true);
        //    }
        //}

        #endregion

        #region RunTestsAction

        /// <summary>
        /// RunTestsAction runs the loaded TestSuite held by the TestController.
        /// </summary>
        public class RunTestsAction : TestControllerAction
        {
            /// <summary>
            /// Construct a RunTestsAction and run all tests in the loaded TestSuite.
            /// </summary>
            /// <param name="controller">A TestController holding the TestSuite to run</param>
            /// <param name="callback">A callback used to report results</param>
            public RunTestsAction(TestController controller, AsyncCallback callback) 
                : base(controller, callback)
            {
                ITestResult result = controller.Runner.Run(new TestProgressReporter(callback));
                ReportResult(result.ToXml(true), true);
            }

            ///// <summary>
            ///// Construct a RunTestsAction and run tests in the loaded TestSuite that pass the supplied filter
            ///// </summary>
            ///// <param name="controller">A TestController holding the TestSuite to run</param>
            ///// <param name="filter">A TestFilter used to determine which tests should be run</param>
            ///// <param name="result">A callback used to report results</param>
            //public RunTestsAction(TestController controller, TestFilter filter, AsyncCallback callback) 
            //    : base(controller, callback)
            //{
            //    ReportResult(Runner.Run(this, filter), true);
            //}
        }

        #endregion

        #endregion
    }
}
