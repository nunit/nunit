// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using NUnit.Framework.Internal.Abstractions;

#if NETFRAMEWORK
using System.Windows.Forms;
#endif

namespace NUnit.Framework.Api
{
    /// <summary>
    /// Implementation of ITestAssemblyRunner
    /// </summary>
    public class NUnitTestAssemblyRunner : ITestAssemblyRunner
    {
        private static readonly Logger log = InternalTrace.GetLogger("DefaultTestAssemblyRunner");

        private readonly ITestAssemblyBuilder _builder;
        private readonly ManualResetEventSlim _runComplete = new ManualResetEventSlim();

        // Saved Console.Out and Console.Error
        private TextWriter _savedOut;
        private TextWriter _savedErr;

        // Event Pump
        private EventPump _pump;

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public NUnitTestAssemblyRunner(ITestAssemblyBuilder builder)
        {
            _builder = builder;
        }

#endregion

#region Properties

        /// <summary>
        /// Gets the default level of parallel execution (worker threads)
        /// </summary>
        public static int DefaultLevelOfParallelism
        {
            get { return Math.Max(Environment.ProcessorCount, 2); }
        }

        /// <summary>
        /// The tree of tests that was loaded by the builder
        /// </summary>
        public ITest LoadedTest { get; private set; }

        /// <summary>
        /// The test result, if a run has completed
        /// </summary>
        public ITestResult Result
        {
            get { return TopLevelWorkItem?.Result; }
        }

        /// <summary>
        /// Indicates whether a test is loaded
        /// </summary>
        public bool IsTestLoaded
        {
            get { return LoadedTest != null; }
        }

        /// <summary>
        /// Indicates whether a test is running
        /// </summary>
        public bool IsTestRunning
        {
            get { return TopLevelWorkItem != null && TopLevelWorkItem.State == WorkItemState.Running; }
        }

        /// <summary>
        /// Indicates whether a test run is complete
        /// </summary>
        public bool IsTestComplete
        {
            get { return TopLevelWorkItem != null && TopLevelWorkItem.State == WorkItemState.Complete; }
        }

        /// <summary>
        /// Our settings, specified when loading the assembly
        /// </summary>
        private IDictionary<string, object> Settings { get; set; }

        /// <summary>
        /// The top level WorkItem created for the assembly as a whole
        /// </summary>
        private WorkItem TopLevelWorkItem { get; set; }

        /// <summary>
        /// The TestExecutionContext for the top level WorkItem
        /// </summary>
        private TestExecutionContext Context { get; set; }

#endregion

#region Public Methods

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assemblyNameOrPath">File name or path of the assembly to load</param>
        /// <param name="settings">Dictionary of option settings for loading the assembly</param>
        /// <returns>A Test Assembly containing all loaded tests</returns>
        public ITest Load(string assemblyNameOrPath, IDictionary<string, object> settings)
        {
            Settings = settings;

            if (settings.TryGetValue(FrameworkPackageSettings.RandomSeed, out var randomSeedValue))
            {
                Randomizer.InitialSeed = (int)randomSeedValue;
            }

            WrapInNUnitCallContext(() => LoadedTest = _builder.Build(assemblyNameOrPath, settings));
            return LoadedTest;

        }

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assembly">The assembly to load</param>
        /// <param name="settings">Dictionary of option settings for loading the assembly</param>
        /// <returns>A Test Assembly containing all loaded tests</returns>
        public ITest Load(Assembly assembly, IDictionary<string, object> settings)
        {
            Settings = settings;

            if (settings.TryGetValue(FrameworkPackageSettings.RandomSeed, out var randomSeed))
            {
                Randomizer.InitialSeed = (int)randomSeed;
            }

            WrapInNUnitCallContext(() => LoadedTest = _builder.Build(assembly, settings));
            return LoadedTest;
        }

        /// <summary>
        /// Count Test Cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The number of test cases found</returns>
        public int CountTestCases(ITestFilter filter)
        {
            if (LoadedTest == null)
                throw new InvalidOperationException("Tests must be loaded before counting test cases.");

            return CountTestCases(LoadedTest, filter);
        }

        /// <summary>
        /// Explore the test cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>Test Assembly with test cases that matches the filter</returns>
        public ITest ExploreTests(ITestFilter filter)
        {
            if (LoadedTest == null)
                throw new InvalidOperationException("Tests must be loaded before exploring them.");

            if (filter == TestFilter.Empty)
                return LoadedTest;

            return new TestAssembly(LoadedTest as TestAssembly, filter);
        }

        /// <summary>
        /// Run selected tests and return a test result. The test is run synchronously,
        /// and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run</param>
        /// <returns>The test results from the run</returns>
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            RunAsync(listener, filter);
            WaitForCompletion(Timeout.Infinite);
            return Result;
        }

        /// <summary>
        /// Run selected tests asynchronously, notifying the listener interface as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run</param>
        /// <remarks>
        /// RunAsync is a template method, calling various abstract and
        /// virtual methods to be overridden by derived classes.
        /// </remarks>
        public void RunAsync(ITestListener listener, ITestFilter filter)
        {
            log.Info("Running tests");
            if (LoadedTest == null)
                throw new InvalidOperationException("Tests must be loaded before running them.");

            _runComplete.Reset();

            CreateTestExecutionContext(listener);

            TopLevelWorkItem = WorkItemBuilder.CreateWorkItem(LoadedTest, filter, new DebuggerProxy(), true);
            TopLevelWorkItem.InitializeContext(Context);
            TopLevelWorkItem.Completed += OnRunCompleted;

            WrapInNUnitCallContext(() => StartRun(listener));
        }

        /// <summary>
        /// Wait for the ongoing run to complete.
        /// </summary>
        /// <param name="timeout">Time to wait in milliseconds</param>
        /// <returns>True if the run completed, otherwise false</returns>
        public bool WaitForCompletion(int timeout)
        {
            return _runComplete.Wait(timeout);
        }

        /// <summary>
        /// Signal any test run that is in process to stop. Return without error if no test is running.
        /// </summary>
        /// <param name="force">If true, kill any tests that are currently running</param>
        public void StopRun(bool force)
        {
            if (IsTestRunning)
            {
                Context.ExecutionStatus = force
                    ? TestExecutionStatus.AbortRequested
                    : TestExecutionStatus.StopRequested;

                Context.Dispatcher.CancelRun(force);
            }
        }

#endregion

#region Helper Methods

        /// <summary>
        /// Initiate the test run.
        /// </summary>
        private void StartRun(ITestListener listener)
        {
            // Save Console.Out and Error for later restoration
            _savedOut = Console.Out;
            _savedErr = Console.Error;

            Console.SetOut(new TextCapture(Console.Out));
            Console.SetError(new EventListenerTextWriter("Error", Console.Error));

            // Queue and pump events, unless settings have SynchronousEvents == false
            if (!Settings.TryGetValue(FrameworkPackageSettings.SynchronousEvents, out var synchronousEvents) ||
                !(bool)synchronousEvents)
            {
                QueuingEventListener queue = new QueuingEventListener();
                Context.Listener = queue;

                _pump = new EventPump(listener, queue.Events);
                _pump.Start();
            }

            if (!Debugger.IsAttached &&
                Settings.TryGetValue(FrameworkPackageSettings.DebugTests, out var debugTests) &&
                (bool)debugTests)
            {
                try
                {
                    Debugger.Launch();
                }
                catch (SecurityException)
                {
                    TopLevelWorkItem.MarkNotRunnable("System.Security.Permissions.UIPermission must be granted in order to launch the debugger.");
                    return;
                }
                //System.Diagnostics.Debugger.Launch() not implemented on mono
                catch (NotImplementedException)
                {
                    TopLevelWorkItem.MarkNotRunnable("This platform does not support launching the debugger.");
                    return;
                }
            }

#if NETFRAMEWORK
            if (Settings.TryGetValue(FrameworkPackageSettings.PauseBeforeRun, out var pauseBeforeRun) &&
                (bool)pauseBeforeRun)
                PauseBeforeRun();
#endif

            Context.Dispatcher.Start(TopLevelWorkItem);
        }

        /// <summary>
        /// Create the initial TestExecutionContext used to run tests
        /// </summary>
        /// <param name="listener">The ITestListener specified in the RunAsync call</param>
        private void CreateTestExecutionContext(ITestListener listener)
        {
            Context = new TestExecutionContext();

            // Apply package settings to the context
            if (Settings.TryGetValue(FrameworkPackageSettings.DefaultTimeout, out var timeout))
                Context.TestCaseTimeout = (int)timeout;
            if (Settings.TryGetValue(FrameworkPackageSettings.DefaultCulture, out var culture))
                Context.CurrentCulture = new((string)culture, false);
            if (Settings.TryGetValue(FrameworkPackageSettings.DefaultUICulture, out var uiCulture))
                Context.CurrentUICulture = new((string)uiCulture, false);
            if (Settings.TryGetValue(FrameworkPackageSettings.StopOnError, out var stopOnError))
                Context.StopOnError = (bool)stopOnError;

            // Apply attributes to the context

            // Set the listener - overriding runners may replace this
            Context.Listener = listener;

            int levelOfParallelism = GetLevelOfParallelism();

            if (Settings.TryGetValue(FrameworkPackageSettings.RunOnMainThread, out var runOnMainThread) &&
                (bool)runOnMainThread)
                Context.Dispatcher = new MainThreadWorkItemDispatcher();
            else if (levelOfParallelism > 0)
                Context.Dispatcher = new ParallelWorkItemDispatcher(levelOfParallelism);
            else
                Context.Dispatcher = new SimpleWorkItemDispatcher();
        }

        /// <summary>
        /// Handle the Completed event for the top level work item
        /// </summary>
        private void OnRunCompleted(object sender, EventArgs e)
        {
            if (_pump != null)
                _pump.Dispose();

            Console.SetOut(_savedOut);
            Console.SetError(_savedErr);

            _runComplete.Set();
        }

        private int CountTestCases(ITest test, ITestFilter filter)
        {
            if (!test.IsSuite)
                return filter.Pass(test) ? 1: 0;

            int count = 0;
            foreach (ITest child in test.Tests)
            {
                count += CountTestCases(child, filter);
            }

            return count;
        }

        private int GetLevelOfParallelism()
        {
            return Settings.TryGetValue(FrameworkPackageSettings.NumberOfTestWorkers, out var numberOfTestWorkers)
                ? (int)numberOfTestWorkers
                : LoadedTest.Properties.ContainsKey(PropertyNames.LevelOfParallelism)
                   ? (int)LoadedTest.Properties.Get(PropertyNames.LevelOfParallelism)
                   : NUnitTestAssemblyRunner.DefaultLevelOfParallelism;
        }

#if NETFRAMEWORK
        // This method invokes members on the 'System.Diagnostics.Process' class and must satisfy the link demand of
        // the full-trust 'PermissionSetAttribute' on this class. Callers of this method have no influence on how the
        // Process class is used, so we can safely satisfy the link demand with a 'SecuritySafeCriticalAttribute' rather
        // than a 'SecurityCriticalAttribute' and allow use by security transparent callers.
        [SecuritySafeCritical]
        private static void PauseBeforeRun()
        {
            using var process = Process.GetCurrentProcess();

            MessageBox.Show(
                $"Pausing as requested. If you would like to attach a debugger, the process name and ID are {process.ProcessName}.exe and {process.Id}." + Environment.NewLine
                + Environment.NewLine
                + "Click OK when you are ready to continue.",
                $"{process.ProcessName} – paused",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
#endif

#if NETFRAMEWORK
        /// <summary>
        /// Executes the action within an <see cref="NUnitCallContext" />
        /// which ensures the <see cref="System.Runtime.Remoting.Messaging.CallContext"/> is cleaned up
        /// suitably at the end of the test run. This method only has an effect running
        /// the full .NET Framework.
        /// </summary>
#else
        /// <summary>
        /// This method is a no-op in .NET Standard builds.
        /// </summary>
#endif
        protected void WrapInNUnitCallContext(Action action)
        {
#if NETFRAMEWORK
            using (new NUnitCallContext())
                action();
#else
            action();
#endif
        }
    }

#endregion
}
