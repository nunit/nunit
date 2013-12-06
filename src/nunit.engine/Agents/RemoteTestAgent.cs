// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
	public class RemoteTestAgent : TestAgent, ITestRunner
	{
        static Logger log = InternalTrace.GetLogger(typeof(RemoteTestAgent));

		#region Fields

        private ITestRunner runner;

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

		public override ITestRunner CreateRunner()
		{
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

        #region ITestRunner Members

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Explore(TestFilter filter)
        {
            return runner == null ? null : runner.Explore(filter);
        }

        public ITestEngineResult Load(TestPackage package)
        {
            //System.Diagnostics.Debug.Assert(false, "Attach debugger if desired");

            this.runner = Services.TestRunnerFactory.MakeTestRunner(package);
            return runner.Load(package);
        }

        public void Unload()
        {
            if (runner != null)
                runner.Unload();
        }

        /// <summary>
        /// Run the tests in the loaded TestPackage and return a test result. The tests
        /// are run synchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public ITestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            return runner == null ? null : runner.Run(listener, filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public void BeginRun(ITestEventHandler listener, TestFilter filter)
        {
            if (runner != null)
                runner.BeginRun(listener, filter);
        }

        #endregion
    }
}
