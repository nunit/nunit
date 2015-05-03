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
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Enumeration used to report AgentStatus
    /// </summary>
    public enum AgentStatus
    {
        Unknown,
        Starting,
        Ready,
        Busy,
        Stopping
    }

    /// <summary>
    /// The TestAgency class provides RemoteTestAgents
    /// on request and tracks their status. Agents
    /// are wrapped in an instance of the TestAgent
    /// class. Multiple agent types are supported
    /// but only one, ProcessAgent is implemented
    /// at this time.
    /// </summary>
    public class TestAgency : ServerBase, ITestAgency, IService
    {
        static Logger log = InternalTrace.GetLogger(typeof(TestAgency));

        #region Private Fields

        private AgentDataBase _agentData = new AgentDataBase();

        private IRuntimeFrameworkService _runtimeService;

        #endregion

        #region Constructors
        public TestAgency() : this( "TestAgency", 0 ) { }

        public TestAgency( string uri, int port ) : base( uri, port ) { }
        #endregion

        #region ServerBase Overrides
        //public override void Stop()
        //{
        //    foreach( KeyValuePair<Guid,AgentRecord> pair in agentData )
        //    {
        //        AgentRecord r = pair.Value;

        //        if ( !r.Process.HasExited )
        //        {
        //            if ( r.Agent != null )
        //            {
        //                r.Agent.Stop();
        //                r.Process.WaitForExit(10000);
        //            }

        //            if ( !r.Process.HasExited )
        //                r.Process.Kill();
        //        }
        //    }

        //    agentData.Clear();

        //    base.Stop ();
        //}
        #endregion

        #region Public Methods - Called by Agents
        public void Register( ITestAgent agent )
        {
            AgentRecord r = _agentData[agent.Id];
            if ( r == null )
                throw new ArgumentException(
                    string.Format("Agent {0} is not in the agency database", agent.Id),
                    "agentId");
            r.Agent = agent;
        }

        public void ReportStatus( Guid agentId, AgentStatus status )
        {
            AgentRecord r = _agentData[agentId];

            if ( r == null )
                throw new ArgumentException(
                    string.Format("Agent {0} is not in the agency database", agentId),
                    "agentId" );

            r.Status = status;
        }
        #endregion

        #region Public Methods - Called by Clients

        public ITestAgent GetAgent(TestPackage package, int waitTime)
        {
            // TODO: Decide if we should reuse agents
            //AgentRecord r = FindAvailableRemoteAgent(type);
            //if ( r == null )
            //    r = CreateRemoteAgent(type, framework, waitTime);
            return CreateRemoteAgent(package, waitTime);
        }

        public void ReleaseAgent( ITestAgent agent )
        {
            AgentRecord r = _agentData[agent.Id];
            if (r == null)
                log.Error(string.Format("Unable to release agent {0} - not in database", agent.Id));
            else
            {
                r.Status = AgentStatus.Ready;
                log.Debug("Releasing agent " + agent.Id.ToString());
            }
        }

        //public void DestroyAgent( ITestAgent agent )
        //{
        //    AgentRecord r = agentData[agent.Id];
        //    if ( r != null )
        //    {
        //        if( !r.Process.HasExited )
        //            r.Agent.Stop();
        //        agentData[r.Id] = null;
        //    }
        //}
        #endregion

        #region Helper Methods
        private Guid LaunchAgentProcess(TestPackage package)
        {
            string runtimeSetting = package.GetSetting(PackageSettings.RuntimeFramework, "");
            RuntimeFramework targetRuntime = RuntimeFramework.Parse(
                runtimeSetting != ""
                    ? runtimeSetting
                    : _runtimeService.SelectRuntimeFramework(package));

            bool useX86Agent = package.GetSetting(PackageSettings.RunAsX86, false);
            bool enableDebug = package.GetSetting("AgentDebug", false);
            bool verbose = package.GetSetting("Verbose", false);
            string agentArgs = string.Empty;
            if (enableDebug) agentArgs += " --pause";
            if (verbose) agentArgs += " --verbose";

            log.Info("Getting {0} agent for use under {1}", useX86Agent ? "x86" : "standard", targetRuntime);

            if (!targetRuntime.IsAvailable)
                throw new ArgumentException(
                    string.Format("The {0} framework is not available", targetRuntime),
                    "framework");

            string agentExePath = GetTestAgentExePath(targetRuntime.ClrVersion, useX86Agent);

            if (agentExePath == null)
                throw new ArgumentException(
                    string.Format("NUnit components for version {0} of the CLR are not installed",
                    targetRuntime.ClrVersion.ToString()), "targetRuntime");

            log.Debug("Using nunit-agent at " + agentExePath);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            Guid agentId = Guid.NewGuid();
            string arglist = agentId.ToString() + " " + ServerUrl + " " + agentArgs;

            switch( targetRuntime.Runtime )
            {
                case RuntimeType.Mono:
                    p.StartInfo.FileName = NUnitConfiguration.MonoExePath;
                    string monoOptions = "--runtime=v" + targetRuntime.ClrVersion.ToString(3);
                    if (enableDebug) monoOptions += " --debug";
                    p.StartInfo.Arguments = string.Format("{0} \"{1}\" {2}", monoOptions, agentExePath, arglist);
                    break;
                case RuntimeType.Net:
                    p.StartInfo.FileName = agentExePath;

                    if (targetRuntime.ClrVersion.Build < 0)
                        targetRuntime = RuntimeFramework.GetBestAvailableFramework(targetRuntime);

                    string envVar = "v" + targetRuntime.ClrVersion.ToString(3);
                    p.StartInfo.EnvironmentVariables["COMPLUS_Version"] = envVar;

                    p.StartInfo.Arguments = arglist;
                    break;
                default:
                    p.StartInfo.FileName = agentExePath;
                    p.StartInfo.Arguments = arglist;
                    break;
            }
            
            //p.Exited += new EventHandler(OnProcessExit);
            p.Start();
            log.Info("Launched Agent process {0} - see nunit-agent_{0}.log", p.Id);
            log.Info("Command line: \"{0}\" {1}", p.StartInfo.FileName, p.StartInfo.Arguments);

            _agentData.Add( new AgentRecord( agentId, p, null, AgentStatus.Starting ) );
            return agentId;
        }

        //private void OnProcessExit(object sender, EventArgs e)
        //{
        //    Process p = sender as Process;
        //    if (p != null)
        //        agentData.Remove(p.Id);
        //}

        //private AgentRecord FindAvailableAgent()
        //{
        //    foreach( AgentRecord r in agentData )
        //        if ( r.Status == AgentStatus.Ready)
        //        {
        //            log.Debug( "Reusing agent {0}", r.Id );
        //            r.Status = AgentStatus.Busy;
        //            return r;
        //        }

        //    return null;
        //}

        private ITestAgent CreateRemoteAgent(TestPackage package, int waitTime)
        {
            Guid agentId = LaunchAgentProcess(package);

            log.Debug( "Waiting for agent {0} to register", agentId.ToString("B") );

            int pollTime = 200;
            bool infinite = waitTime == Timeout.Infinite;

            while( infinite || waitTime > 0 )
            {
                Thread.Sleep( pollTime );
                if ( !infinite ) waitTime -= pollTime;
                ITestAgent agent = _agentData[agentId].Agent;
                if ( agent != null )
                {
                    log.Debug( "Returning new agent {0}", agentId.ToString("B") );
                    return agent;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the NUnit Bin Directory for a particular
        /// runtime version, or null if it's not installed.
        /// For normal installations, there are only 1.1 and
        /// 2.0 directories. However, this method accommodates
        /// 3.5 and 4.0 directories for the benefit of NUnit
        /// developers using those runtimes.
        /// </summary>
        private static string GetNUnitBinDirectory(Version v)
        {
            // Get current bin directory
            string dir = NUnitConfiguration.NUnitBinDirectory;

            // Return current directory if current and requested
            // versions are both >= 2 or both 1
            if ((Environment.Version.Major >= 2) == (v.Major >= 2))
                return dir;

            // Check whether special support for version 1 is installed
            if (v.Major == 1)
            {
                string altDir = Path.Combine(dir, "net-1.1");
                if (Directory.Exists(altDir))
                    return altDir;

                // The following is only applicable to the dev environment,
                // which uses parallel build directories. We try to substitute
                // one version number for another in the path.
                string[] search = new string[] { "2.0", "3.0", "3.5", "4.0" };
                string[] replace = v.Minor == 0
                    ? new string[] { "1.0", "1.1" }
                    : new string[] { "1.1", "1.0" };

                // Look for current value in path so it can be replaced
                string current = null;
                foreach (string s in search)
                    if (dir.IndexOf(s) >= 0)
                    {
                        current = s;
                        break;
                    }

                // Try the substitution
                if (current != null)
                {
                    foreach (string target in replace)
                    {
                        altDir = dir.Replace(current, target);
                        if (Directory.Exists(altDir))
                            return altDir;
                    }
                }
            }

            return null;
        }

        private static string GetTestAgentExePath(Version v, bool requires32Bit)
        {
            string binDir = NUnitConfiguration.NUnitBinDirectory;
            if (binDir == null) return null;

            string agentName = v.Major > 1 && requires32Bit
                ? "nunit-agent-x86.exe"
                : "nunit-agent.exe";

            string agentExePath = Path.Combine(binDir, agentName);
            return File.Exists(agentExePath) ? agentExePath : null;
        }

        #endregion

        #region IService Members

        public ServiceContext ServiceContext { get; set; }

        public ServiceStatus Status { get; private set; }

        public void StopService()
        {
            try
            {
                Stop();
            }
            finally
            {
                Status = ServiceStatus.Stopped;
            }
        }

        public void StartService()
        {
            try
            {
                // TestAgency depends on the RuntimeFrameworkService.
                // If it isn't available it will not start.
                _runtimeService = ServiceContext.GetService<IRuntimeFrameworkService>();
                if (_runtimeService != null && ((IService)_runtimeService).Status == ServiceStatus.Started)
                {
                    try
                    {
                        Start();
                        Status = ServiceStatus.Started;
                    }
                    catch (Exception ex)
                    {
                        Status = ServiceStatus.Error;
                        throw;
                    }
                }
                else
                {
                    Status = ServiceStatus.Error;
                }
            }
            catch
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        #endregion

        #region Nested Class - AgentRecord
        private class AgentRecord
        {
            public Guid Id;
            public Process Process;
            public ITestAgent Agent;
            public AgentStatus Status;

            public AgentRecord( Guid id, Process p, ITestAgent a, AgentStatus s )
            {
                this.Id = id;
                this.Process = p;
                this.Agent = a;
                this.Status = s;
            }

        }
        #endregion

        #region Nested Class - AgentDataBase
        /// <summary>
        ///  A simple class that tracks data about this
        ///  agencies active and available agents
        /// </summary>
        private class AgentDataBase
        {
            private Dictionary<Guid, AgentRecord> agentData = new Dictionary<Guid, AgentRecord>();

            public AgentRecord this[Guid id]
            {
                get { return agentData[id]; }
                set
                {
                    if ( value == null )
                        agentData.Remove( id );
                    else
                        agentData[id] = value;
                }
            }

            public AgentRecord this[ITestAgent agent]
            {
                get
                {
                    foreach( KeyValuePair<Guid, AgentRecord> entry in agentData)
                    {
                        AgentRecord r = entry.Value;
                        if ( r.Agent == agent )
                            return r;
                    }

                    return null;
                }
            }

            public void Add( AgentRecord r )
            {
                agentData[r.Id] = r;
            }

            public void Remove(Guid agentId)
            {
                agentData.Remove(agentId);
            }

            public void Clear()
            {
                agentData.Clear();
            }

            //#region IEnumerable Members
            //public IEnumerator<KeyValuePair<Guid,AgentRecord>> GetEnumerator()
            //{
            //    return agentData.GetEnumerator();
            //}
            //#endregion
        }

        #endregion
    }
}
