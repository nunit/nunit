// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// The ITestAssemblyRunner interface is implemented by classes
    /// that are able to execute a suite of tests loaded
    /// from an assembly.
    /// </summary>
    public interface ITestAssemblyRunner
    {
        #region Properties

        /// <summary>
        /// Gets the tree of loaded tests, or null if
        /// no tests have been loaded.
        /// </summary>
        ITest? LoadedTest { get; }

        /// <summary>
        /// Gets the tree of test results, if the test
        /// run is completed, otherwise null.
        /// </summary>
        ITestResult? Result { get; }

        /// <summary>
        /// Indicates whether a test has been loaded
        /// </summary>
        bool IsTestLoaded { get; }

        /// <summary>
        /// Indicates whether a test is currently running
        /// </summary>
        bool IsTestRunning { get; }

        /// <summary>
        /// Indicates whether a test run is complete
        /// </summary>
        bool IsTestComplete { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the tests found in an Assembly, returning an
        /// indication of whether or not the load succeeded.
        /// </summary>
        /// <param name="assemblyName">File name of the assembly to load</param>
        /// <param name="settings">Dictionary of options to use in loading the test</param>
        /// <returns>An ITest representing the loaded tests</returns>
        ITest Load(string assemblyName, IDictionary<string, object> settings);

        /// <summary>
        /// Loads the tests found in an Assembly, returning an
        /// indication of whether or not the load succeeded.
        /// </summary>
        /// <param name="assembly">The assembly to load</param>
        /// <param name="settings">Dictionary of options to use in loading the test</param>
        /// <returns>An ITest representing the loaded tests</returns>
        ITest Load(Assembly assembly, IDictionary<string, object> settings);

        /// <summary>
        /// Count Test Cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The number of test cases found</returns>
        int CountTestCases(ITestFilter filter);

        /// <summary>
        /// Explore the test cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>Test Assembly with test cases that matches the filter</returns>
        ITest ExploreTests(ITestFilter filter);

        /// <summary>
        /// Run selected tests and return a test result. The test is run synchronously,
        /// and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive ITestListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run</param>
        ITestResult Run(ITestListener listener, ITestFilter filter);

        /// <summary>
        /// Run selected tests asynchronously, notifying the listener interface as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run</param>
        void RunAsync(ITestListener listener, ITestFilter filter);

        /// <summary>
        /// Wait for the ongoing run to complete.
        /// </summary>
        /// <param name="timeout">Time to wait in milliseconds</param>
        /// <returns>True if the run completed, otherwise false</returns>
        bool WaitForCompletion(int timeout);

        /// <summary>
        /// Signal any test run that is in process to stop. Return without error if no test is running.
        /// </summary>
        /// <param name="force">If true, kill any test-running threads</param>
        void StopRun(bool force);

        #endregion
    }
}
