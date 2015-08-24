// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
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
using System.Threading;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Agents
{
    /// <summary>
    /// RemoteTestAgent represents a remote agent executing in another process
    /// and communicating with NUnit by TCP. Although it is similar to a
    /// TestServer, it does not publish a Uri at which clients may connect 
    /// to it. Rather, it reports back to the sponsoring TestAgency upon 
    /// startup so that the agency may in turn provide it to clients for use.
    /// </summary>
    public class RemoteTestAgent : TestAgent, ITestEngineRunner
    {
        static Logger log = InternalTrace.GetLogger(typeof(RemoteTestAgent));

        #region Fields

        private ITestEngineRunner _runner;
        private TestPackage _package;

        private ManualResetEvent stopSignal = new ManualResetEvent(false);
        
        #endregion

        #region Constructor

        /// <summary>
        /// Construct a RemoteTestAgent
        /// </summary>
        public RemoteTestAgent( Guid agentId, ITestAgency agency, ServiceContext services )
            : base(agentId, agency, services) 
        {
        }

        #endregion

        #region Properties
        public int ProcessId
        {
            get { return System.Diagnostics.Process.GetCurrentProcess().Id; }
        }
        #endregion

        #region Public Methods

        public override ITestEngineRunner CreateRunner(TestPackage package)
        {
            _package = package;
            return this;
        }

        public override bool Start()
        {
            log.Info("Agent starting");

            try
            {
                this.Agency.Register( this );
                log.Debug( "Registered with TestAgency" );
            }
            catch( Exception ex )
            {
                log.Error( "RemoteTestAgent: Failed to register with TestAgency", ex );
                return false;
            }

            return true;
        }

        public override void Stop()
        {
            log.Info("Stopping");
            // This causes an error in the client because the agent 
            // database is not thread-safe.
            //if (agency != null)
            //    agency.ReportStatus(this.ProcessId, AgentStatus.Stopping);


            stopSignal.Set();
        }

        public void WaitForStop()
        {
            stopSignal.WaitOne();
        }

        #endregion

        #region ITestEngineRunner Members

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="filter">Criteria used to filter the search results</param>
        /// <returns>A TestEngineResult.</returns>
        public TestEngineResult Explore(TestFilter filter)
        {
            if (_runner == null)
                throw new InvalidOperationException("RemoteTestAgent: Explore called before Load");
            
            return _runner.Explore(filter);
        }

        public TestEngineResult Load()
        {
            //System.Diagnostics.Debug.Assert(false, "Attach debugger if desired");

            _runner = Services.GetService<ITestRunnerFactory>().MakeTestRunner(_package);
            return _runner.Load();
        }

        public void Unload()
        {
            if (_runner != null)
                _runner.Unload();
        }

        public TestEngineResult Reload()
        {
            if (_runner == null)
                throw new InvalidOperationException("RemoteTestAgent: Reload called before Load");
                
            return _runner.Reload();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        public int CountTestCases(TestFilter filter)
        {
            if (_runner == null)
                throw new InvalidOperationException("RemoteTestAgent: CountTestCases called before Load");

            return _runner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in the loaded TestPackage and return a test result. The tests
        /// are run synchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public TestEngineResult Run(ITestEventListener listener, TestFilter filter)
        {
            if (_runner == null)
                throw new InvalidOperationException("RemoteTestAgent: Run called before Load");

            return _runner.Run(listener, filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A <see cref="ITestRun"/> that will provide the result of the test execution</returns>
        public ITestRun RunAsync(ITestEventListener listener, TestFilter filter)
        {
            if (_runner == null)
                throw new InvalidOperationException("RemoteTestAgent: RunAsync called before Load");

            return _runner.RunAsync(listener, filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public void StartRun(ITestEventListener listener, TestFilter filter)
        {
            RunAsync(listener, filter);
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public void StopRun(bool force)
        {
            if (_runner != null)
                _runner.StopRun(force);
        }

        #endregion
    }
}
