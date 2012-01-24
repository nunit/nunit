// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

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
        //static Logger log = InternalTrace.GetLogger(typeof(NUnitTestAgent));

        static Guid AgentId;
        static string AgencyUrl;
        static ITestAgency Agency;

        /// <summary>
        /// Channel used for communications with the agency
        /// and with clients
        /// </summary>
        static TcpChannel Channel;

        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static int Main(string[] args)
		{
            AgentId = new Guid(args[0]);
            AgencyUrl = args[1];

#if DEBUG
            if (args.Length > 2 && args[2] == "--pause")
                System.Windows.Forms.MessageBox.Show("Attach debugger if desired, then press OK", "NUnit-Agent");
#endif

            // Create SettingsService early so we know the trace level right at the start
            SettingsService settingsService = new SettingsService(false);
            //InternalTrace.Initialize("nunit-agent_%p.log", (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default));

            //log.Info("Agent process {0} starting", Process.GetCurrentProcess().Id);
            //log.Info("Running under version {0}, {1}", 
            //    Environment.Version, 
            //    RuntimeFramework.CurrentFramework.DisplayName);
            Console.WriteLine("Agent process {0} starting", Process.GetCurrentProcess().Id);
            Console.WriteLine("Running under version {0}, {1}",
                Environment.Version,
                RuntimeFramework.CurrentFramework.DisplayName);

            // Create TestEngine - this program is
            // conceptually part of  the engine and
            // can access it's internals as needed.
            TestEngine engine = new TestEngine();
            
            // Custom Service Initialization
            //log.Info("Adding Services");
            Console.WriteLine("Adding Services");
            engine.Services.Add(settingsService);
            engine.Services.Add(new ProjectService());
            engine.Services.Add(new DomainManager());
            engine.Services.Add(new InProcessTestRunnerFactory());
            engine.Services.Add(new DriverFactory());
            //engine.Services.Add( new TestLoader() );

            // Initialize Services
            //log.Info("Initializing Services");
            Console.WriteLine("Initializing Services");
            engine.Services.ServiceManager.InitializeServices();

            Channel = ServerUtilities.GetTcpChannel();

            Console.WriteLine("Connecting to TestAgency at {0}", AgencyUrl);
            //log.Info("Connecting to TestAgency at {0}", AgencyUrl);
            try
            {
                Agency = Activator.GetObject(typeof(ITestAgency), AgencyUrl) as ITestAgency;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to connect", ex);
                //log.Error("Unable to connect", ex);
            }

            if (Channel != null)
            {
                Console.WriteLine("Starting RemoteTestAgent");
                //log.Info("Starting RemoteTestAgent");
                RemoteTestAgent agent = new RemoteTestAgent(AgentId, Agency, engine.Services);

                try
                {
                    if (agent.Start())
                    {
                        Console.WriteLine("Waiting for stopSignal");
                        //log.Debug("Waiting for stopSignal");
                        agent.WaitForStop();
                        //log.Debug("Stop signal received");
                        Console.WriteLine("Stop signal received");
                    }
                    else
                        Console.WriteLine("Failed to start RemoteTestAgent");
                        //log.Error("Failed to start RemoteTestAgent");
                }
                catch (Exception ex)
                {
                    //log.Error("Exception in RemoteTestAgent", ex);
                    Console.WriteLine("Exception in RemoteTestAgent", ex);
                }

                //log.Info("Unregistering Channel");
                Console.WriteLine("Unregistering Channel");
                try
                {
                    ChannelServices.UnregisterChannel(Channel);
                }
                catch (Exception ex)
                {
                    //log.Error("ChannelServices.UnregisterChannel threw an exception", ex);
                    Console.WriteLine("ChannelServices.UnregisterChannel threw an exception", ex);
                }
            }

            Console.WriteLine("Agent process {0} exiting", Process.GetCurrentProcess().Id);
            //log.Info("Agent process {0} exiting", Process.GetCurrentProcess().Id);
            //InternalTrace.Close();

			return 0;
		}
	}
}
