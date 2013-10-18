// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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

#if !NUNITLITE
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Extensibility;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// CoreExtensions is a singleton class that groups together all 
	/// the extension points that are supported in the test domain.
	/// It also provides access to the test builders and decorators
	/// by other parts of the NUnit core.
	/// </summary>
	public class CoreExtensions : ExtensionHost
	{
        static Logger log = InternalTrace.GetLogger("CoreExtensions");

		#region Instance Fields
        private static CoreExtensions host;
        
        private IAddinRegistry addinRegistry;
		private bool initialized;

		private SuiteBuilderCollection suiteBuilders;
		private TestCaseBuilderCollection testBuilders;
		private EventListenerCollection listeners;
	    private TestCaseProviders testcaseProviders;
        private ParameterDataProviders parameterDataProviders;
         
		#endregion

		#region CoreExtensions Singleton

        /// <summary>
        /// Gets the CoreExtensions host singleton.
        /// </summary>
        /// <value>The host.</value>
		public static CoreExtensions Host
		{
			get
			{
				if (host == null)
					host = new CoreExtensions();

				return host;
			}
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreExtensions"/> class.
        /// </summary>
		public CoreExtensions() 
		{
			this.suiteBuilders = new SuiteBuilderCollection(this);
			this.testBuilders = new TestCaseBuilderCollection(this);
			this.listeners = new EventListenerCollection(this);
            this.testcaseProviders = new TestCaseProviders(this);
            this.parameterDataProviders = new ParameterDataProviders(this);

		    extensions.Add(suiteBuilders);
		    extensions.Add(testBuilders);
		    extensions.Add(listeners);
		    extensions.Add(testcaseProviders);
            extensions.Add(parameterDataProviders);

			this.supportedTypes = ExtensionType.Core;

			// TODO: This should be somewhere central
//			string logfile = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
//			logfile = Path.Combine( logfile, "NUnit" );
//			logfile = Path.Combine( logfile, "NUnitTest.log" );
//
//			appender = new log4net.Appender.ConsoleAppender();
////			appender.File = logfile;
////			appender.AppendToFile = true;
////			appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
//			appender.Layout = new log4net.Layout.PatternLayout(
//				"%date{ABSOLUTE} %-5level [%4thread] %logger{1}: PID=%property{PID} %message%newline" );
//			appender.Threshold = log4net.Core.Level.All;
//			log4net.Config.BasicConfigurator.Configure(appender);
		}
		#endregion

		#region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="CoreExtensions"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		public bool Initialized
		{
			get { return initialized; }
		}

		public IAddinRegistry AddinRegistry
		{
			get 
			{
				if ( addinRegistry == null )
                    addinRegistry = new AddinRegistry();

				return addinRegistry; 
			}
			set { addinRegistry = value; }
		}
		#endregion

		#region Internal Properties
		internal ISuiteBuilder SuiteBuilders
		{
			get { return suiteBuilders; }
		}

		internal ITestCaseBuilder2 TestBuilders
		{
			get { return testBuilders; }
		}

		internal ITestListener Listeners
		{
			get { return listeners; }
		}

        internal TestCaseProviders TestCaseProviders
	    {
            get { return testcaseProviders; }
	    }

	    #endregion

		#region Public Methods	

        /// <summary>
        /// Initializes this instance of the CoreExtensions host.
        /// </summary>
        public void Initialize()
        {
            InstallBuiltins();
            InstallAddins();

            initialized = true;
        }

        /// <summary>
        /// Installs the builtin 'extensions'.
        /// </summary>
        public void InstallBuiltins()
		{
            log.Info("Installing Builtins");

			// Install builtin SuiteBuilders
			suiteBuilders.Install( new NUnitTestFixtureBuilder() );
			suiteBuilders.Install( new SetUpFixtureBuilder() );

            // Install builtin TestCaseBuilder
            testBuilders.Install( new NUnitTestCaseBuilder() );
            //testBuilders.Install(new TheoryBuilder());

            // Install builtin TestCaseProviders
            testcaseProviders.Install(new DataAttributeTestCaseProvider());
            testcaseProviders.Install(new CombinatorialTestCaseProvider());

            // Install builtin ParameterDataProviders
            parameterDataProviders.Install(new ParameterDataProvider());
            parameterDataProviders.Install(new DatapointProvider());
		}

        /// <summary>
        /// Installs the addins.
        /// </summary>
		public void InstallAddins()
		{
            log.Info("Installing Addins");

			if( AddinRegistry != null )
			{
				foreach (Addin addin in AddinRegistry.Addins)
				{
					if ( (this.ExtensionTypes & addin.ExtensionType) != 0 )
					{
						AddinStatus status = AddinStatus.Unknown;
						string message = null;

						try
						{
							Type type = Type.GetType(addin.TypeName);
							if ( type == null )
							{
								status = AddinStatus.Error;
								message = string.Format( "Unable to locate {0} Type", addin.TypeName );
							}
							else if ( !InstallAddin( type ) )
							{
								status = AddinStatus.Error;
								message = "Install method returned false";
							}
							else
							{
								status = AddinStatus.Loaded;
							}
						}
						catch( Exception ex )
						{
							status = AddinStatus.Error;
							message = ex.ToString(); 				
						}

						AddinRegistry.SetStatus( addin.Name, status, message );
						if ( status != AddinStatus.Loaded )
						{
                            log.Error("Failed to load {0}", addin.Name);
                            log.Error(message);
						}
					}
				}
			}
		}

        /// <summary>
        /// Installs any adhoc extensions.
        /// </summary>
        /// <param name="assembly">The assembly to scan for adhoc extensions.</param>
		public void InstallAdhocExtensions( Assembly assembly )
		{
			foreach ( Type type in assembly.GetExportedTypes() )
			{
				if ( type.GetCustomAttributes(typeof(NUnitAddinAttribute), false).Length == 1 )
					InstallAddin( type );
			}
		}
		#endregion

		#region Helper Methods
		private bool InstallAddin( Type type )
		{
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			object obj = ctor.Invoke( new object[0] );
			IAddin theAddin = (IAddin)obj;

			return theAddin.Install(this);
		}
		#endregion
	}
}
#endif
