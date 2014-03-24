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
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// Summary description for ProcessRunner.
    /// </summary>
    public class ProcessRunner : AbstractTestRunner
    {
        static Logger log = InternalTrace.GetLogger(typeof(ProcessRunner));

        private ITestAgent _agent;
        private ITestEngineRunner _remoteRunner;

        public ProcessRunner(ServiceContext services) : base(services) { }

        #region Properties

        public RuntimeFramework RuntimeFramework { get; private set; }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Explore(TestFilter filter)
        {
            TestEngineResult result = this._remoteRunner.Explore(filter);
            return result as TestEngineResult; // TODO: Remove need for this cast
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            log.Info("Loading " + package.Name);
            Unload();

            this.TestPackage = package;

            string frameworkSetting = package.GetSetting(RunnerSettings.RuntimeFramework, "");
            this.RuntimeFramework = frameworkSetting != ""
                ? RuntimeFramework.Parse(frameworkSetting)
                : RuntimeFramework.CurrentFramework;

            bool enableDebug = package.GetSetting("AgentDebug", false);
            bool verbose = package.GetSetting("Verbose", false);
            string agentArgs = string.Empty;
            if (enableDebug) agentArgs += " --pause";
            if (verbose) agentArgs += " --verbose";

            try
            {
                CreateAgentAndRunner(enableDebug, agentArgs);

                // TODO: Remove need for this cast
                return _remoteRunner.Load(package) as TestEngineResult;
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
        public override void Unload()
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
        public override int CountTestCases(TestFilter filter)
        {
            return _remoteRunner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestResult giving the result of the test execution</returns>
        public override TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            return (TestEngineResult)_remoteRunner.Run(listener, filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public override void BeginRun(ITestEventHandler listener, TestFilter filter)
        {
            _remoteRunner.BeginRun(listener, filter);
        }

        /// <summary>
        /// Cancel the ongoing test run. If no test is running,
        /// the call is ignored.
        /// </summary>
        public override void CancelRun()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (_agent != null)
            {
                log.Info("Stopping remote agent");
                _agent.Stop();
                _agent = null;
            }
        }

        #endregion

        #region Helper Methods

        private void CreateAgentAndRunner(bool enableDebug, string agentArgs)
        {
            if (_agent == null)
            {
                _agent = Services.TestAgency.GetAgent(
                    RuntimeFramework,
                    30000,
                    enableDebug,
                    agentArgs);

                if (_agent == null)
                    throw new Exception("Unable to acquire remote process agent");
            }

            if (_remoteRunner == null)
                _remoteRunner = _agent.CreateRunner();
        }

        #endregion
    }
}
