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
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using NUnit.Engine.Interfaces;

namespace NUnit.Engine.Runners
{
	/// <summary>
	/// Summary description for ProcessRunner.
	/// </summary>
	public class ProcessRunner : ITestRunner//: ProxyTestRunner
	{
        //static Logger log = InternalTrace.GetLogger(typeof(ProcessRunner));

        private ServiceContext services;
        private ITestAgent agent;
        private ITestRunner remoteRunner;

        private RuntimeFramework runtimeFramework;

        #region Constructor

        public ProcessRunner(ServiceContext services)
        {
            this.services = services;
        }

        #endregion

        #region Properties

        public ServiceContext Services
        {
            get { return services; }
        }

        public RuntimeFramework RuntimeFramework
        {
            get { return runtimeFramework; }
        }

        #endregion

        public bool Load(TestPackage package)
		{
            //log.Info("Loading " + package.Name);
			Unload();

            runtimeFramework = package.Settings["RuntimeFramework"] as RuntimeFramework;
            if ( runtimeFramework == null )
                 runtimeFramework = RuntimeFramework.CurrentFramework;

            bool enableDebug = package.GetSetting("AgentDebug", false);
            //bool enableDebug = true;

            bool loaded = false;

			try
			{
                if (this.agent == null)
                {
                    this.agent = Services.TestAgency.GetAgent(
                        runtimeFramework,
                        30000,
                        enableDebug);

                    if (this.agent == null)
                        return false;
                }
	
				if (this.remoteRunner == null)
					this.remoteRunner = agent.CreateRunner();

                loaded = this.remoteRunner.Load(package);
                return loaded;
			}
			finally
			{
                // Clean up if the load failed
				if ( !loaded ) Unload();
			}
		}

        public void Unload()
        {
            if (this.remoteRunner != null)
            {
                //log.Info("Unloading " + Path.GetFileName(Test.TestName.Name));
                this.remoteRunner.Unload();
                this.remoteRunner = null;
            }
		}

        public TestResult Run(ITestFilter filter)
        {
            return this.remoteRunner.Run(filter);
        }

		#region IDisposable Members

		public void Dispose()
		{
            if (this.agent != null)
            {
                //log.Info("Stopping remote agent");
                agent.Stop();
                this.agent = null;
            }
        }

		#endregion
	}
}
