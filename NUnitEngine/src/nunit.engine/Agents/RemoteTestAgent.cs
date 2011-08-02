// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

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
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Explore(TestPackage package)
        {
            this.runner = Services.TestRunnerFactory.MakeTestRunner(package);
            return runner.Explore(package);
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

        public ITestEngineResult Run(ITestEventHandler listener, ITestFilter filter)
        {
            return runner == null ? null : runner.Run(listener, filter);
        }

        #endregion
    }
}
