// ***********************************************************************
// Copyright (c) 2011-2014 Charlie Poole
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
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// Summary description for ProcessRunner.
    /// </summary>
    public class ProcessRunner : AbstractTestRunner
    {
        private static readonly Logger log = InternalTrace.GetLogger(typeof(ProcessRunner));

        private ITestAgent _agent;
        private ITestEngineRunner _remoteRunner;

        public ProcessRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region Properties

        public RuntimeFramework RuntimeFramework { get; private set; }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            return _remoteRunner.Explore(filter);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            log.Info("Loading " + TestPackage.Name);
            Unload();

            string frameworkSetting = TestPackage.GetSetting(PackageSettings.RuntimeFramework, "");
            RuntimeFramework = RuntimeFramework.Parse(
                frameworkSetting != ""
                    ? frameworkSetting
                    : Services.RuntimeFrameworkSelector.SelectRuntimeFramework(TestPackage));

            bool useX86Agent = TestPackage.GetSetting(PackageSettings.RunAsX86, false);
            bool enableDebug = TestPackage.GetSetting("AgentDebug", false);
            bool verbose = TestPackage.GetSetting("Verbose", false);
            string agentArgs = string.Empty;
            if (enableDebug) agentArgs += " --pause";
            if (verbose) agentArgs += " --verbose";

            try
            {
                CreateAgentAndRunner(enableDebug, agentArgs, useX86Agent);

                return _remoteRunner.Load();
            }
            catch(Exception)
            {
                // TODO: Check if this is really needed
                // Clean up if the load failed
                Unload();
                throw;
            }
        }

        /// <summary>
        /// Unload any loaded TestPackage and clear
        /// the reference to the remote runner.
        /// </summary>
        public override void UnloadPackage()
        {
            if (_remoteRunner != null)
            {
                log.Info("Unloading remote runner");
                _remoteRunner.Unload();
                _remoteRunner = null;
            }
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        protected override int CountTests(TestFilter filter)
        {
            return _remoteRunner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestResult giving the result of the test execution</returns>
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            return _remoteRunner.Run(listener, filter);
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public override void StopRun(bool force)
        {
            _remoteRunner.StopRun(force);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _agent != null)
            {
                log.Info("Stopping remote agent");
                _agent.Stop();
                _agent = null;
            }
        }

        #endregion

        #region Helper Methods

        private void CreateAgentAndRunner(bool enableDebug, string agentArgs, bool useX86Agent)
        {
            if (_agent == null)
            {
                _agent = Services.TestAgency.GetAgent(
                    RuntimeFramework,
                    30000,
                    enableDebug,
                    agentArgs,
                    useX86Agent);

                if (_agent == null)
                    throw new Exception("Unable to acquire remote process agent");
            }

            if (_remoteRunner == null)
                _remoteRunner = _agent.CreateRunner(TestPackage);
        }

        #endregion
    }
}
