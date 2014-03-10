// ***********************************************************************
// Copyright (c) 2009-2014 Charlie Poole
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Web.UI;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// FrameworkController provides a facade for use in loading, browsing 
    /// and running tests without requiring a reference to the NUnit 
    /// framework. All calls are encapsulated in constructors for
    /// this class and its nested classes, which only require the
    /// types of the Common Type System as arguments.
    /// 
    /// Note that the controller uses the non-generic ICollection 
    /// interface by design, for maximum portability.
    /// </summary>
    public class FrameworkController : MarshalByRefObject
    {
        #region Constructors

        /// <summary>
        /// Construct a FrameworkController using the default builder and runner.
        /// </summary>
        public FrameworkController(string assemblyPath, IDictionary settings)
        {
            this.Builder = new DefaultTestAssemblyBuilder();
            this.Runner = new DefaultTestAssemblyRunner(this.Builder);

            Initialize(assemblyPath, settings);
        }

        /// <summary>
        /// Construct a FrameworkController, specifying the types to be used
        /// for the runner and builder. This constructor is provided for
        /// purposes of development.
        /// </summary>
        /// <param name="assemblyPath">The path to the test assembly</param>
        /// <param name="settings">A Dictionary of settings to use in loading and running the tests</param>
        /// <param name="runnerType">The Type of the test runner</param>
        /// <param name="builderType">The Type of the test builder</param>
        public FrameworkController(string assemblyPath, IDictionary settings, string runnerType, string builderType)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            this.Builder = (ITestAssemblyBuilder)myAssembly.CreateInstance(builderType);
            this.Runner = (ITestAssemblyRunner)myAssembly.CreateInstance(
                runnerType, false, 0, null, new object[] { this.Builder }, null, null);

            Initialize(assemblyPath, settings);
        }

        private void Initialize(string assemblyPath, IDictionary settings)
        {
            this.AssemblyPath = assemblyPath;
            this.Settings = settings;

            if (settings.Contains(DriverSettings.InternalTraceLevel))
            {
                var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), (string)settings[DriverSettings.InternalTraceLevel]);

                if (settings.Contains("InternalTraceWriter"))
                    InternalTrace.Initialize((TextWriter)settings[DriverSettings.InternalTraceWriter], traceLevel);
                else
                {
                    var workDirectory = settings.Contains("WorkDirectory") ? (string)settings[DriverSettings.WorkDirectory] : Environment.CurrentDirectory;
                    var logName = string.Format("InternalTrace.{0}.{1}.log", Process.GetCurrentProcess().Id, Path.GetFileName(assemblyPath));
                    //var logName = string.Format("InternalTrace.{0}.log", Process.GetCurrentProcess().Id);
                    InternalTrace.Initialize(Path.Combine(workDirectory, logName), traceLevel);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ITestAssemblyBuilder used by this controller instance.
        /// </summary>
        /// <value>The builder.</value>
        public ITestAssemblyBuilder Builder { get; private set; }

        /// <summary>
        /// Gets the ITestAssemblyRunner used by this controller instance.
        /// </summary>
        /// <value>The runner.</value>
        public ITestAssemblyRunner Runner { get; private set; }

        /// <summary>
        /// Gets the path to the assembly for this FrameworkController
        /// </summary>
        public string AssemblyPath { get; private set; }

        /// <summary>
        /// Gets a dictionary of settings for the FrameworkController
        /// </summary>
        public IDictionary Settings { get; private set; }

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

        #region Private Action Methods Used by Nested Classes

        private void LoadTests(ICallbackEventHandler handler)
        {
            try
            {
                int count = Runner.Load(AssemblyPath, Settings)
                    ? Runner.LoadedTest.TestCaseCount
                    : 0;

                //TestExecutionContext.ClearCurrentContext();
                handler.RaiseCallbackEvent(string.Format("<loaded assembly=\"{0}\" testcases=\"{1}\"/>", AssemblyPath, count));
            }
            catch (Exception ex)
            {
                handler.RaiseCallbackEvent(FormatErrorReport(ex));
            }
        }

        private void ExploreTests(ICallbackEventHandler handler)
        {
            try
            {
                // TODO: Make use of the filter
                if (Runner.Load(AssemblyPath, Settings))
                    handler.RaiseCallbackEvent(Runner.LoadedTest.ToXml(true).OuterXml);
                else
                    handler.RaiseCallbackEvent(FormatErrorReport("No tests were found"));
            }
            catch (Exception ex)
            {
                handler.RaiseCallbackEvent(FormatErrorReport(ex));
            }
        }

        private void RunTests(ICallbackEventHandler handler, string filter)
        {
            try
            {
                ITestResult result = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter));

                // Ensure that the CallContext of the thread is not polluted
                // by our TestExecutionContext, which is not serializable.
                TestExecutionContext.ClearCurrentContext();

                handler.RaiseCallbackEvent(result.ToXml(true).OuterXml);
            }
            catch (Exception ex)
            {
                handler.RaiseCallbackEvent(FormatErrorReport(ex));
            }
            finally
            {
                //InternalTrace.Flush();
            }
        }

        private void CountTests(ICallbackEventHandler handler, string filter)
        {
            try
            {
                var count = Runner.CountTestCases(TestFilter.FromXml(filter));
                handler.RaiseCallbackEvent(count.ToString());
            }
            catch (Exception ex)
            {
                handler.RaiseCallbackEvent(FormatErrorReport(ex));
            }
        }

        #endregion

        #region Format Error Reports

        private static string FormatErrorReport(string message)
        {
            return string.Format("<error message=\"{0}\"/>" ,message);
        }

        private static string FormatErrorReport(string message, string stackTrace)
        {
            message = System.Security.SecurityElement.Escape(message);
            stackTrace = System.Security.SecurityElement.Escape(stackTrace);
            return string.Format("<error message=\"{0}\" stackTrace=\"{1}\"/>", message, stackTrace);
        }

        private static string FormatErrorReport(Exception ex)
        {
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;

            string msg = ex is System.IO.FileNotFoundException || ex is System.BadImageFormatException
                ? FormatErrorReport(ex.Message)
                : FormatErrorReport(ex.Message, ex.StackTrace);
            return msg;
        }

        #endregion

        #region Nested Action Classes

        #region TestContollerAction

        /// <summary>
        /// FrameworkControllerAction is the base class for all actions
        /// performed against a FrameworkController.
        /// </summary>
        public abstract class FrameworkControllerAction : MarshalByRefObject
        {
            #region InitializeLifetimeService

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
        /// LoadTestsAction loads a test into the FrameworkController
        /// </summary>
        public class LoadTestsAction : FrameworkControllerAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LoadTestsAction"/> class.
            /// </summary>
            /// <param name="controller">The controller.</param>
            /// <param name="handler">The callback handler.</param>
            public LoadTestsAction(FrameworkController controller, object handler)
            {
                controller.LoadTests((ICallbackEventHandler)handler);
            }
        }

        #endregion

        #region ExploreTestsAction

        /// <summary>
        /// ExploreTestsAction returns info about the tests in an assembly
        /// </summary>
        public class ExploreTestsAction : FrameworkControllerAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ExploreTestsAction"/> class.
            /// </summary>
            /// <param name="controller">The controller for which this action is being performed.</param>
            /// <param name="filter">Filter used to control which tests are included (NYI)</param>
            /// <param name="handler">The callback handler.</param>
            public ExploreTestsAction(FrameworkController controller, string filter, object handler)
            {
                controller.ExploreTests((ICallbackEventHandler)handler);
            }
        }

        #endregion

#if false
        #region GetLoadedTestsAction

        ///// <summary>
        ///// GetLoadedTestsAction returns the XML representation
        ///// of a suite of tests, which must have been loaded already.
        ///// </summary>
        //public class GetLoadedTestsAction : FrameworkControllerAction
        //{
        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="GetLoadedTestsAction"/> class.
        //    /// </summary>
        //    /// <param name="controller">The controller.</param>
        //    /// <param name="callback">An AsynchCallback to receive the result.</param>
        //    public GetLoadedTestsAction(FrameworkController controller, AsyncCallback callback)
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
#endif

        #region CountTestsAction

        /// <summary>
        /// CountTestsAction counts the number of test cases in the loaded TestSuite
        /// held by the FrameworkController.
        /// </summary>
        public class CountTestsAction : FrameworkControllerAction
        {
            /// <summary>
            /// Construct a CountsTestAction and perform the count of test cases.
            /// </summary>
            /// <param name="controller">A FrameworkController holding the TestSuite whose cases are to be counted</param>
            /// <param name="filter">A string containing the XML representation of the filter to use</param>
            /// <param name="handler">A callback handler used to report results</param>
            public CountTestsAction(FrameworkController controller, string filter, object handler) 
            {
                controller.CountTests((ICallbackEventHandler)handler, filter);
            }
        }

        #endregion

        #region RunTestsAction

        /// <summary>
        /// RunTestsAction runs the loaded TestSuite held by the FrameworkController.
        /// </summary>
        public class RunTestsAction : FrameworkControllerAction
        {
            /// <summary>
            /// Construct a RunTestsAction and run all tests in the loaded TestSuite.
            /// </summary>
            /// <param name="controller">A FrameworkController holding the TestSuite to run</param>
            /// <param name="filter">A string containing the XML representation of the filter to use</param>
            /// <param name="handler">A callback handler used to report results</param>
            public RunTestsAction(FrameworkController controller, string filter, object handler) 
            {
                controller.RunTests((ICallbackEventHandler)handler, filter);
            }

            ///// <summary>
            ///// Construct a RunTestsAction and run tests in the loaded TestSuite that pass the supplied filter
            ///// </summary>
            ///// <param name="controller">A FrameworkController holding the TestSuite to run</param>
            ///// <param name="filter">A TestFilter used to determine which tests should be run</param>
            ///// <param name="result">A callback used to report results</param>
            //public RunTestsAction(FrameworkController controller, TestFilter filter, AsyncCallback callback) 
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
