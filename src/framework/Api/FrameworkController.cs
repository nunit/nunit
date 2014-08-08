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

#if !NETCF && !SILVERLIGHT
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
    /// The controller supports four actions: Load, Explore, Count and Run.
    /// They are intended to be called by a driver, which should allow for
    /// proper sequencing of calls. Load must be called before any of the 
    /// other actions. The driver may support other actions, such as
    /// reload on run, by combining these calls.
    /// </summary>
    public class FrameworkController : MarshalByRefObject
    {
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";

        #region Constructors

        /// <summary>
        /// Construct a FrameworkController using the default builder and runner.
        /// </summary>
        public FrameworkController(string assemblyPath, IDictionary settings)
        {
            this.Builder = new DefaultTestAssemblyBuilder();
            this.Runner = CreateRunner(this.Builder);

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

        private ITestAssemblyRunner CreateRunner(ITestAssemblyBuilder builder)
        {
#if NUNITLITE
            return new NUnitLiteTestAssemblyRunner(builder);
#else
            return new NUnitTestAssemblyRunner(builder);
#endif
        }

        private void Initialize(string assemblyPath, IDictionary settings)
        {
            this.AssemblyPath = assemblyPath;
            this.Settings = settings;

            if (settings.Contains(DriverSettings.InternalTraceLevel))
            {
                var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), (string)settings[DriverSettings.InternalTraceLevel]);

                if (settings.Contains(DriverSettings.InternalTraceWriter))
                    InternalTrace.Initialize((TextWriter)settings[DriverSettings.InternalTraceWriter], traceLevel);
                else
                {
                    var workDirectory = settings.Contains(DriverSettings.WorkDirectory) ? (string)settings[DriverSettings.WorkDirectory] : Environment.CurrentDirectory;
                    var logName = string.Format(LOG_FILE_FORMAT, Process.GetCurrentProcess().Id, Path.GetFileName(assemblyPath));
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
            Runner.Load(AssemblyPath, Settings);
            handler.RaiseCallbackEvent(Runner.LoadedTest.ToXml(false).OuterXml);
        }

        private void ExploreTests(ICallbackEventHandler handler, string filter)
        {
            Guard.ArgumentNotNull(filter, "filter");

            if (Runner.LoadedTest == null)
                throw new InvalidOperationException("The Explore method was called but no test has been loaded");

            // TODO: Make use of the filter
            handler.RaiseCallbackEvent(Runner.LoadedTest.ToXml(true).OuterXml);
        }

        private void RunTests(ICallbackEventHandler handler, string filter)
        {
            Guard.ArgumentNotNull(filter, "filter");

            ITestResult result = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter));

            // Ensure that the CallContext of the thread is not polluted
            // by our TestExecutionContext, which is not serializable.
            TestExecutionContext.ClearCurrentContext();

            handler.RaiseCallbackEvent(result.ToXml(true).OuterXml);
        }

        private void RunAsync(ICallbackEventHandler handler, string filter)
        {
            Guard.ArgumentNotNull(filter, "filter");

            Runner.RunAsync(new TestProgressReporter(handler), TestFilter.FromXml(filter));
        }

        private void StopRun(ICallbackEventHandler handler, bool force)
        {
            Runner.StopRun(force);
        }

        private void CountTests(ICallbackEventHandler handler, string filter)
        {
            Guard.ArgumentNotNull(filter, "filter");

            var count = Runner.CountTestCases(TestFilter.FromXml(filter));
            handler.RaiseCallbackEvent(count.ToString());
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
            /// LoadTestsAction loads the tests in an assembly.
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
                controller.ExploreTests((ICallbackEventHandler)handler, filter);
            }
        }

        #endregion

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
        }

        #endregion

        #region RunAsyncAction

        /// <summary>
        /// RunAsyncAction initiates an asynchronous test run, returning immediately
        /// </summary>
        public class RunAsyncAction : FrameworkControllerAction
        {
            /// <summary>
            /// Construct a RunAsyncAction and run all tests in the loaded TestSuite.
            /// </summary>
            /// <param name="controller">A FrameworkController holding the TestSuite to run</param>
            /// <param name="filter">A string containing the XML representation of the filter to use</param>
            /// <param name="handler">A callback handler used to report results</param>
            public RunAsyncAction(FrameworkController controller, string filter, object handler) 
            {
                controller.RunAsync((ICallbackEventHandler)handler, filter);
            }
        }

        #endregion

        #region StopRunAction

        /// <summary>
        /// StopRunAction stops an ongoing run.
        /// </summary>
        public class StopRunAction : FrameworkControllerAction
        {
            /// <summary>
            /// Construct a StopRunAction and stop any ongoing run. If no
            /// run is in process, no error is raised.
            /// </summary>
            /// <param name="controller">The FrameworkController for which a run is to be stopped.</param>
            /// <param name="force">True the stop should be forced, false for a cooperative stop.</param>
            /// <param name="handler">>A callback handler used to report results</param>
            /// <remarks>A forced stop will cause threads and processes to be killed as needed.</remarks>
            public StopRunAction(FrameworkController controller, bool force, object handler)
            {
                controller.StopRun((ICallbackEventHandler)handler, force);
            }
        }

        #endregion

        #endregion
    }
}
#endif
