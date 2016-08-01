// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.Engine.Agents
{
    /// <summary>
    /// Abstract base for all types of TestAgents.
    /// A TestAgent provides services of locating,
    /// loading and running tests in a particular
    /// context such as an AppDomain or Process.
    /// </summary>
    public abstract class TestAgent : MarshalByRefObject, ITestAgent, IDisposable
    {
        #region Private Fields

        private ITestAgency agency;
        private Guid agentId;
        private IServiceLocator services;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAgent"/> class.
        /// </summary>
        /// <param name="agentId">The identifier of the agent.</param>
        /// <param name="agency">The agency that this agent is associated with.</param>
        /// <param name="services">The services available to the agent.</param>
        public TestAgent( Guid agentId, ITestAgency agency, IServiceLocator services )
        {
            this.agency = agency;
            this.agentId = agentId;
            this.services = services;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The services available to the agent
        /// </summary>
        protected IServiceLocator Services
        {
            get { return services; }
        }

        #endregion

        #region ITestAgent members

        /// <summary>
        /// Gets a reference to the TestAgency with which this agent 
        /// is associated. Returns null if the agent is not 
        /// connected to an agency.
        /// </summary>
        public ITestAgency Agency
        {
            get { return agency; }
        }

        /// <summary>
        /// Gets a Guid that uniquely identifies this agent.
        /// </summary>
        public Guid Id
        {
            get { return agentId; }
        }

        /// <summary>
        /// Starts the agent, performing any required initialization
        /// </summary>
        /// <returns><c>true</c> if the agent was started successfully.</returns>
        public abstract bool Start();

        /// <summary>
        /// Stops the agent, releasing any resources
        /// </summary>
        public abstract void Stop();

        /// <summary>
        ///  Creates a test runner
        /// </summary>
        public abstract ITestEngineRunner CreateRunner(TestPackage package);

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private bool _disposed = false;

        /// <summary>
        /// Dispose is overridden to stop the agent
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Stop();

                _disposed = true;
            }
        }
        #endregion

        #region InitializeLifeTimeService
        /// <summary>
        /// Overridden to cause object to live indefinitely
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion
    }
}
