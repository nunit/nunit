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

#if !NUNITLITE
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Web.UI;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// TestController provides a facade for use in loading, browsing 
    /// and running tests without requiringa reference to the NUnit 
    /// framework. All calls are encapsulated in constructors for
    /// this class and its nested classes, which only require the
    /// types of the Common Type System as arguments.
    /// </summary>
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

            // TODO: This should be taken from constructor options
            InternalTrace.Level = InternalTrace.TraceLevel.Debug;
            InternalTrace.Open("InternalTrace.txt");

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

        /// <summary>
        /// Gets the ITestAssemblyBuilder used by this controller instance.
        /// </summary>
        /// <value>The builder.</value>
        public ITestAssemblyBuilder Builder
        {
            get { return builder; }
        }

        /// <summary>
        /// Gets the ITestAssemblyRunner used by this controller instance.
        /// </summary>
        /// <value>The runner.</value>
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
            private ICallbackEventHandler handler;

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="TestControllerAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="_handler">The callback handler.</param>
            protected TestControllerAction(TestController controller, object _handler)
            {
                this.controller = controller;
                this.handler = _handler as ICallbackEventHandler;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the callback event handler for this controller action.
            /// </summary>
            protected ICallbackEventHandler Handler
            {
                get { return handler; }
            }

            #endregion

            #region Methods for Reporting Results

            /// <summary>
            /// Format and send an error report
            /// </summary>
            /// <param name="message">The error message</param>
            protected void ReportError(string message)
            {
                ReportResult(string.Format("<error message=\"{0}\"/>", message));
            }

            /// <summary>
            /// Format and send an error report for an exception
            /// </summary>
            /// <param name="ex">The exception to be reported</param>
            protected void ReportError(Exception ex)
            {
                if (ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;

                string msg = ex is System.IO.FileNotFoundException || ex is System.BadImageFormatException
                    ? string.Format("<error message=\"{0}\"/>", ex.Message)
                    : string.Format("<error message=\"{0}\" stackTrace=\"{1}\"/>", ex.Message, ex.StackTrace);

                ReportResult(msg);
            }

            /// <summary>
            /// Report the result of an operation
            /// </summary>
            /// <param name="resultString">A string representing the result</param>
            protected void ReportResult(string resultString)
            {
                handler.RaiseCallbackEvent(resultString);
            }


            /// <summary>
            /// Report the result of an operation
            /// </summary>
            /// <param name="result">A result object implementing IXmlNodeBuilder</param>
            protected void ReportResult(IXmlNodeBuilder result)
            {
                handler.RaiseCallbackEvent(result.ToXml(true).OuterXml);
            }

            #endregion

            #region Method Overrides

            /// <summary>
            /// Initialize lifetime service to null so that the instance lives indefinitely.
            /// </summary>
            public override object InitializeLifetimeService()
            {
                return null;
            }

            #endregion
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
            /// <param name="settings">Options controlling how the tests are loaded</param>
            /// <param name="_handler">The callback handler.</param>
            public LoadTestsAction(TestController controller, string assemblyFilename, IDictionary settings, object _handler)
                : base(controller, _handler)
            {
                try
                {
                    int count = controller.Runner.Load(assemblyFilename, settings)
                        ? controller.Runner.LoadedTest.TestCaseCount
                        : 0;

                    ReportResult(string.Format("<loaded assembly=\"{0}\" testcases=\"{1}\"/>", assemblyFilename, count));
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            }
        }

        #endregion

        #region ExploreTestsAction

        /// <summary>
        /// ExploreTestsAction returns info about the tests in an assembly
        /// </summary>
        public class ExploreTestsAction : TestControllerAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ExploreTestsAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="assemblyFilename">The assembly filename.</param>
            /// <param name="loadOptions">Options controlling how the tests are loaded</param>
            /// <param name="filterText">Filter used to control which tests are included</param>
            /// <param name="_handler">The callback handler.</param>
            public ExploreTestsAction(TestController controller, string assemblyFilename, IDictionary loadOptions, string filterText, object _handler)
                : base(controller, _handler)
            {
                try
                {
                    if (controller.Runner.Load(assemblyFilename, loadOptions))
                        ReportResult(controller.Runner.LoadedTest);
                    else
                        ReportError("No tests were found");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            }
        }

        #endregion

#if false
        #region GetLoadedTestsAction

        ///// <summary>
        ///// GetLoadedTestsAction returns the XML representation
        ///// of a suite of tests, which must have been loaded already.
        ///// </summary>
        //public class GetLoadedTestsAction : TestControllerAction
        //{
        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="GetLoadedTestsAction"/> class.
        //    /// </summary>
        //    /// <param name="controller">The controller.</param>
        //    /// <param name="callback">An AsynchCallback to receive the result.</param>
        //    public GetLoadedTestsAction(TestController controller, AsyncCallback callback)
        //        : base(controller, callback)
        //    {
        //        try
        //        {
        //            ITest loadedTest = controller.Runner.LoadedTest;

        //            if (loadedTest != null)
        //                callback(new FinalResult(loadedTest.ToXml(true), true));
        //        }
        //        catch (Exception ex)
        //        {
        //            callback(new ErrorReport(ex));
        //        }
        //    }
        //}

        #endregion

        #region CountTestsAction

        ///// <summary>
        ///// CountTestsAction counts the number of test cases in the loaded TestSuite
        ///// held by the TestController.
        ///// </summary>
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
#endif

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
            /// <param name="filterText">A string containing the XML representation of the filter to use</param>
            /// <param name="_handler">A callback handler used to report results</param>
            public RunTestsAction(TestController controller, string filterText, object _handler) 
                : base(controller, _handler)
            {
                try
                {
                    ITestResult result = controller.Runner.Run(new TestProgressReporter(Handler), TestFilter.FromXml(filterText));

                    // Ensure that the CallContext of the thread is not polluted
                    // by our TestExecutionContext, which is not serializable.
                    TestExecutionContext.ClearCurrentContext();

                    ReportResult(result);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
                finally
                {
                    InternalTrace.Flush();
                }
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
#endif
