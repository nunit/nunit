// ***********************************************************************
// Copyright (c) 2009-2014 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Web.UI;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

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
    public class FrameworkController : LongLivedMarshalByRefObject
    {
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";

        // Preloaded test assembly, if passed in constructor
        private readonly Assembly _testAssembly;

        #region Constructors

        /// <summary>
        /// Construct a FrameworkController using the default builder and runner.
        /// </summary>
        /// <param name="assemblyNameOrPath">The AssemblyName or path to the test assembly</param>
        /// <param name="idPrefix">A prefix used for all test ids created under this controller.</param>
        /// <param name="settings">A Dictionary of settings to use in loading and running the tests</param>
        public FrameworkController(string assemblyNameOrPath, string idPrefix, IDictionary settings)
        {
            Initialize(assemblyNameOrPath, settings);

            this.Builder = new DefaultTestAssemblyBuilder();
            this.Runner = new NUnitTestAssemblyRunner(this.Builder);

            Test.IdPrefix = idPrefix;
        }

        /// <summary>
        /// Construct a FrameworkController using the default builder and runner.
        /// </summary>
        /// <param name="assembly">The test assembly</param>
        /// <param name="idPrefix">A prefix used for all test ids created under this controller.</param>
        /// <param name="settings">A Dictionary of settings to use in loading and running the tests</param>
        public FrameworkController(Assembly assembly, string idPrefix, IDictionary settings)
            : this(assembly.FullName, idPrefix, settings)
        {
            _testAssembly = assembly;
        }

        /// <summary>
        /// Construct a FrameworkController, specifying the types to be used
        /// for the runner and builder. This constructor is provided for
        /// purposes of development.
        /// </summary>
        /// <param name="assemblyNameOrPath">The full AssemblyName or the path to the test assembly</param>
        /// <param name="idPrefix">A prefix used for all test ids created under this controller.</param>
        /// <param name="settings">A Dictionary of settings to use in loading and running the tests</param>
        /// <param name="runnerType">The Type of the test runner</param>
        /// <param name="builderType">The Type of the test builder</param>
        public FrameworkController(string assemblyNameOrPath, string idPrefix, IDictionary settings, string runnerType, string builderType)
        {
            Initialize(assemblyNameOrPath, settings);

            Builder = (ITestAssemblyBuilder)Reflect.Construct(Type.GetType(builderType));
            Runner = (ITestAssemblyRunner)Reflect.Construct(Type.GetType(runnerType), new object[] { Builder });

            Test.IdPrefix = idPrefix ?? "";
        }

        /// <summary>
        /// Construct a FrameworkController, specifying the types to be used
        /// for the runner and builder. This constructor is provided for
        /// purposes of development.
        /// </summary>
        /// <param name="assembly">The test assembly</param>
        /// <param name="idPrefix">A prefix used for all test ids created under this controller.</param>
        /// <param name="settings">A Dictionary of settings to use in loading and running the tests</param>
        /// <param name="runnerType">The Type of the test runner</param>
        /// <param name="builderType">The Type of the test builder</param>
        public FrameworkController(Assembly assembly, string idPrefix, IDictionary settings, string runnerType, string builderType)
            : this(assembly.FullName, idPrefix, settings, runnerType, builderType)
        {
            _testAssembly = assembly;
        }

        // This method invokes members on the 'System.Diagnostics.Process' class and must satisfy the link demand of
        // the full-trust 'PermissionSetAttribute' on this class. Callers of this method have no influence on how the
        // Process class is used, so we can safely satisfy the link demand with a 'SecuritySafeCriticalAttribute' rather
        // than a 'SecurityCriticalAttribute' and allow use by security transparent callers.
        [SecuritySafeCritical]
        private void Initialize(string assemblyPath, IDictionary settings)
        {
            AssemblyNameOrPath = assemblyPath;

            var newSettings = settings as IDictionary<string, object>;
            Settings = newSettings ?? settings.Cast<DictionaryEntry>().ToDictionary(de => (string)de.Key, de => de.Value);

            if (Settings.ContainsKey(FrameworkPackageSettings.InternalTraceLevel))
            {
                var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), (string)Settings[FrameworkPackageSettings.InternalTraceLevel], true);

                if (Settings.ContainsKey(FrameworkPackageSettings.InternalTraceWriter))
                    InternalTrace.Initialize((TextWriter)Settings[FrameworkPackageSettings.InternalTraceWriter], traceLevel);
                else
                {
                    var workDirectory = Settings.ContainsKey(FrameworkPackageSettings.WorkDirectory)
                        ? (string)Settings[FrameworkPackageSettings.WorkDirectory]
                        : Directory.GetCurrentDirectory();
#if NETSTANDARD1_6
                    var id = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
#else
                    var id = Process.GetCurrentProcess().Id;
#endif
                    var logName = string.Format(LOG_FILE_FORMAT, id, Path.GetFileName(assemblyPath));
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
        public ITestAssemblyBuilder Builder { get; }

        /// <summary>
        /// Gets the ITestAssemblyRunner used by this controller instance.
        /// </summary>
        /// <value>The runner.</value>
        public ITestAssemblyRunner Runner { get; }

        /// <summary>
        /// Gets the AssemblyName or the path for which this FrameworkController was created
        /// </summary>
        public string AssemblyNameOrPath { get; private set; }

        /// <summary>
        /// Gets the Assembly for which this
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets a dictionary of settings for the FrameworkController
        /// </summary>
        internal IDictionary<string, object> Settings { get; private set; }

        #endregion

        #region Public Action methods Used by nunit.driver for running tests

        /// <summary>
        /// Loads the tests in the assembly
        /// </summary>
        /// <returns></returns>
        public string LoadTests()
        {
            if (_testAssembly != null)
                Runner.Load(_testAssembly, Settings);
            else
                Runner.Load(AssemblyNameOrPath, Settings);

            return Runner.LoadedTest.ToXml(false).OuterXml;
        }

        /// <summary>
        /// Returns info about the tests in an assembly
        /// </summary>
        /// <param name="filter">A string containing the XML representation of the filter to use</param>
        /// <returns>The XML result of exploring the tests</returns>
        public string ExploreTests(string filter)
        {
            if (Runner.LoadedTest == null)
                throw new InvalidOperationException("The Explore method was called but no test has been loaded");

            return Runner.ExploreTests(TestFilter.FromXml(filter)).ToXml(true).OuterXml;
        }

        /// <summary>
        /// Runs the tests in an assembly
        /// </summary>
        /// <param name="filter">A string containing the XML representation of the filter to use</param>
        /// <returns>The XML result of the test run</returns>
        public string RunTests(string filter)
        {
            TNode result = Runner.Run(new TestProgressReporter(null), TestFilter.FromXml(filter)).ToXml(true);

            // Insert elements as first child in reverse order
            if (Settings != null) // Some platforms don't have settings
                InsertSettingsElement(result, Settings);
            InsertEnvironmentElement(result);

            return result.OuterXml;
        }

#if !NET20

        class ActionCallback : ICallbackEventHandler
        {
            Action<string> _callback;

            public ActionCallback(Action<string> callback)
            {
                _callback = callback;
            }

            public string GetCallbackResult()
            {
                throw new NotImplementedException();
            }

            public void RaiseCallbackEvent(string report)
            {
                if (_callback != null)
                    _callback.Invoke(report);
            }
        }

        /// <summary>
        /// Runs the tests in an assembly synchronously reporting back the test results through the callback
        /// or through the return value
        /// </summary>
        /// <param name="callback">The callback that receives the test results</param>
        /// <param name="filter">A string containing the XML representation of the filter to use</param>
        /// <returns>The XML result of the test run</returns>
        public string RunTests(Action<string> callback, string filter)
        {
            var handler = new ActionCallback(callback);

            TNode result = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter)).ToXml(true);

            // Insert elements as first child in reverse order
            if (Settings != null) // Some platforms don't have settings
                InsertSettingsElement(result, Settings);
            InsertEnvironmentElement(result);

            return result.OuterXml;
        }

        /// <summary>
        /// Runs the tests in an assembly asynchronously reporting back the test results through the callback
        /// </summary>
        /// <param name="callback">The callback that receives the test results</param>
        /// <param name="filter">A string containing the XML representation of the filter to use</param>
        private void RunAsync(Action<string> callback, string filter)
        {
            var handler = new ActionCallback(callback);

            Runner.RunAsync(new TestProgressReporter(handler), TestFilter.FromXml(filter));
        }
#endif

        /// <summary>
        /// Stops the test run
        /// </summary>
        /// <param name="force">True to force the stop, false for a cooperative stop</param>
        public void StopRun(bool force)
        {
            Runner.StopRun(force);
        }

        /// <summary>
        /// Counts the number of test cases in the loaded TestSuite
        /// </summary>
        /// <param name="filter">A string containing the XML representation of the filter to use</param>
        /// <returns>The number of tests</returns>
        public int CountTests(string filter)
        {
            return Runner.CountTestCases(TestFilter.FromXml(filter));
        }

        #endregion

        #region Private Action Methods Used by Nested Classes

        private void LoadTests(ICallbackEventHandler handler)
        {
            handler.RaiseCallbackEvent(LoadTests());
        }

        private void ExploreTests(ICallbackEventHandler handler, string filter)
        {
            if (Runner.LoadedTest == null)
                throw new InvalidOperationException("The Explore method was called but no test has been loaded");

            handler.RaiseCallbackEvent(Runner.ExploreTests(TestFilter.FromXml(filter)).ToXml(true).OuterXml);
        }

        private void RunTests(ICallbackEventHandler handler, string filter)
        {
            TNode result = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter)).ToXml(true);

            // Insert elements as first child in reverse order
            if (Settings != null) // Some platforms don't have settings
                InsertSettingsElement(result, Settings);
            InsertEnvironmentElement(result);

            handler.RaiseCallbackEvent(result.OuterXml);
        }

        private void RunAsync(ICallbackEventHandler handler, string filter)
        {
            Runner.RunAsync(new TestProgressReporter(handler), TestFilter.FromXml(filter));
        }

        private void StopRun(ICallbackEventHandler handler, bool force)
        {
            StopRun(force);
        }

        private void CountTests(ICallbackEventHandler handler, string filter)
        {
            handler.RaiseCallbackEvent(CountTests(filter).ToString());
        }

        /// <summary>
        /// Inserts environment element
        /// </summary>
        /// <param name="targetNode">Target node</param>
        /// <returns>The new node</returns>
        public static TNode InsertEnvironmentElement(TNode targetNode)
        {
            TNode env = new TNode("environment");
            targetNode.ChildNodes.Insert(0, env);

            env.AddAttribute("framework-version", typeof(FrameworkController).GetTypeInfo().Assembly.GetName().Version.ToString());
#if NETSTANDARD1_6
            env.AddAttribute("clr-version", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
#else
            env.AddAttribute("clr-version", Environment.Version.ToString());
#endif
#if !PLATFORM_DETECTION
            env.AddAttribute("os-version", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
#else
            env.AddAttribute("os-version", OSPlatform.CurrentPlatform.ToString());
#endif
#if !NETSTANDARD1_6
            env.AddAttribute("platform", Environment.OSVersion.Platform.ToString());
#endif
            env.AddAttribute("cwd", Directory.GetCurrentDirectory());
            env.AddAttribute("machine-name", Environment.MachineName);
#if !NETSTANDARD1_6
            env.AddAttribute("user", Environment.UserName);
            env.AddAttribute("user-domain", Environment.UserDomainName);
#endif
            env.AddAttribute("culture", CultureInfo.CurrentCulture.ToString());
            env.AddAttribute("uiculture", CultureInfo.CurrentUICulture.ToString());
            env.AddAttribute("os-architecture", GetProcessorArchitecture());

            return env;
        }

        private static string GetProcessorArchitecture()
        {
            return IntPtr.Size == 8 ? "x64" : "x86";
        }

        /// <summary>
        /// Inserts settings element
        /// </summary>
        /// <param name="targetNode">Target node</param>
        /// <param name="settings">Settings dictionary</param>
        /// <returns>The new node</returns>
        public static TNode InsertSettingsElement(TNode targetNode, IDictionary<string, object> settings)
        {
            TNode settingsNode = new TNode("settings");
            targetNode.ChildNodes.Insert(0, settingsNode);

            foreach (string key in settings.Keys)
                AddSetting(settingsNode, key, settings[key]);

#if PARALLEL
            // Add default values for display
            if (!settings.ContainsKey(FrameworkPackageSettings.NumberOfTestWorkers))
                AddSetting(settingsNode, FrameworkPackageSettings.NumberOfTestWorkers, NUnitTestAssemblyRunner.DefaultLevelOfParallelism);
#endif

            return settingsNode;
        }

        private static void AddSetting(TNode settingsNode, string name, object value)
        {
            TNode setting = new TNode("setting");
            setting.AddAttribute("name", name);

            if (value != null)
            {
                var dict = value as IDictionary;
                if (dict != null)
                {
                    AddDictionaryEntries(setting, dict);
                    AddBackwardsCompatibleDictionaryEntries(setting, dict);
                }
                else
                {
                    setting.AddAttribute("value", value.ToString());
                }
            }
            else
            {
                setting.AddAttribute("value", null);
            }

            settingsNode.ChildNodes.Add(setting);
        }

        private static void AddBackwardsCompatibleDictionaryEntries(TNode settingsNode, IDictionary entries)
        {
            var pairs = new List<string>(entries.Count);
            foreach (var key in entries.Keys)
            {
                pairs.Add($"[{key}, {entries[key]}]");
            }
            settingsNode.AddAttribute("value", string.Join(", ", pairs.ToArray()));
        }

        private static void AddDictionaryEntries(TNode settingNode, IDictionary entries)
        {
            foreach(var key in entries.Keys)
            {
                var value = entries[key];
                var entryNode = new TNode("item");
                entryNode.AddAttribute("key", key.ToString());
                entryNode.AddAttribute("value", value?.ToString() ?? "");
                settingNode.ChildNodes.Add(entryNode);
            }
        }

#endregion

#region Nested Action Classes

#region TestContollerAction

        /// <summary>
        /// FrameworkControllerAction is the base class for all actions
        /// performed against a FrameworkController.
        /// </summary>
        public abstract class FrameworkControllerAction : LongLivedMarshalByRefObject
        {
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
