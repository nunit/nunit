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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using NUnit.Engine;
using NUnit.Engine.Agents;
using NUnit.Engine.Internal;
using NUnit.Engine.Services;

namespace NUnit.Agent
{
    /// <summary>
    /// Summary description for Program.
    /// </summary>
    public class NUnitTestAgent
    {
        static Logger log = InternalTrace.GetLogger(typeof(NUnitTestAgent));

        static Guid AgentId;
        static string AgencyUrl;
        static ITestAgency Agency;

        /// <summary>
        /// Channel used for communications with the agency
        /// and with clients
        /// </summary>
        static TcpChannel Channel;

        private const string LOG_FILE_FORMAT = "nunit-agent_{0}.log";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            AgentId = new Guid(args[0]);
            AgencyUrl = args[1];

            InternalTraceLevel traceLevel = InternalTraceLevel.Off;

            for (int i = 2; i < args.Length; i++)
            {
                string arg = args[i];
                
                // NOTE: we can test these strings exactly since
                // they originate from the engine itself.
                if (arg == "--debug-agent")
                {
                    if (!Debugger.IsAttached)
                        Debugger.Launch();
                }
                else if (arg.StartsWith("--trace:"))
                {
                    traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), arg.Substring(8));
                }
            }

            // Initialize trace so we can see what's happening
            int pid = Process.GetCurrentProcess().Id;
            string logname = string.Format(LOG_FILE_FORMAT, pid);
            InternalTrace.Initialize(logname, traceLevel);

            log.Info("Agent process {0} starting", pid);
            log.Info("Running under version {0}, {1}",
                Environment.Version,
                RuntimeFramework.CurrentFramework.DisplayName);

            // Restore the COMPLUS_Version env variable if it's been overridden by TestAgency::LaunchAgentProcess
            try
            {
              string cpvOriginal = Environment.GetEnvironmentVariable("TestAgency_COMPLUS_Version_Original");
              if(!string.IsNullOrEmpty(cpvOriginal))
              {
                log.Debug("Agent process has the COMPLUS_Version environment variable value \"{0}\" overridden with \"{1}\", restoring the original value.", cpvOriginal, Environment.GetEnvironmentVariable("COMPLUS_Version"));
                Environment.SetEnvironmentVariable("TestAgency_COMPLUS_Version_Original", null, EnvironmentVariableTarget.Process); // Erase marker
                Environment.SetEnvironmentVariable("COMPLUS_Version", (cpvOriginal == "NULL" ? null : cpvOriginal), EnvironmentVariableTarget.Process); // Restore original (which might be n/a)
              }
            }
            catch(Exception ex)
            {
              log.Warning("Failed to restore the COMPLUS_Version variable. " + ex.Message); // Proceed with running tests anyway
            }

            // Create TestEngine - this program is
            // conceptually part of  the engine and
            // can access it's internals as needed.
            TestEngine engine = new TestEngine();

            // TODO: We need to get this from somewhere. Argument?
            engine.InternalTraceLevel = InternalTraceLevel.Debug;
            
            // Custom Service Initialization
            //log.Info("Adding Services");
            engine.Services.Add(new SettingsService(false));
            engine.Services.Add(new ExtensionService());
            engine.Services.Add(new ProjectService());
            engine.Services.Add(new DomainManager());
            engine.Services.Add(new InProcessTestRunnerFactory());
            engine.Services.Add(new DriverService());
            //engine.Services.Add( new TestLoader() );

            // Initialize Services
            log.Info("Initializing Services");
            engine.Initialize();

            Channel = ServerUtilities.GetTcpChannel();

            log.Info("Connecting to TestAgency at {0}", AgencyUrl);
            try
            {
                Agency = Activator.GetObject(typeof(ITestAgency), AgencyUrl) as ITestAgency;
            }
            catch (Exception ex)
            {
                log.Error("Unable to connect", ex);
            }

            if (Channel != null)
            {
                log.Info("Starting RemoteTestAgent");
                RemoteTestAgent agent = new RemoteTestAgent(AgentId, Agency, engine.Services);

                try
                {
                    if (agent.Start())
                    {
                        log.Debug("Waiting for stopSignal");
                        agent.WaitForStop();
                        log.Debug("Stop signal received");
                    }
                    else
                        log.Error("Failed to start RemoteTestAgent");
                }
                catch (Exception ex)
                {
                    log.Error("Exception in RemoteTestAgent", ex);
                }

                //log.Info("Unregistering Channel");
                try
                {
                    ChannelServices.UnregisterChannel(Channel);
                }
                catch (Exception ex)
                {
                    log.Error("ChannelServices.UnregisterChannel threw an exception", ex);
                }
            }

            log.Info("Agent process {0} exiting", Process.GetCurrentProcess().Id);

            return 0;
        }
    }
}
