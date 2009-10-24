// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Reflection;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
	/// <summary>
	/// TestAssemblyBuilder loads a single assembly and builds a TestSuite
    /// containing test fixtures present in the assembly.
	/// </summary>
	public class TestAssemblyBuilder
	{
		static Logger log = InternalTrace.GetLogger("TestAssemblyBuilder");

		#region Instance Fields
		/// <summary>
		/// The loaded assembly
		/// </summary>
		Assembly assembly;

		/// <summary>
		/// Our LegacySuite builder, which is only used when a 
		/// fixture has been passed by name on the command line.
		/// </summary>
		ISuiteBuilder legacySuiteBuilder;

		private TestAssemblyInfo assemblyInfo = null;

		#endregion

		#region Properties
        /// <summary>
        /// Gets information about the loaded assembly
        /// </summary>
		public TestAssemblyInfo AssemblyInfo
		{
			get 
			{ 
				if ( assemblyInfo == null && assembly != null )
				{
					string path = AssemblyHelper.GetAssemblyPath( assembly );
					AssemblyReader rdr = new AssemblyReader( path );
					Version imageRuntimeVersion = new Version( rdr.ImageRuntimeVersion.Substring( 1 ) );
					IList frameworks = CoreExtensions.Host.TestFrameworks.GetReferencedFrameworks( assembly );
					assemblyInfo = new TestAssemblyInfo( path, imageRuntimeVersion, RuntimeFramework.CurrentFramework, frameworks );
				}

				return assemblyInfo;
			}
		}
		#endregion

		#region Constructor

		public TestAssemblyBuilder()
		{
			// TODO: Keeping this separate till we can make
			//it work in all situations.
			legacySuiteBuilder = new NUnit.Core.Builders.LegacySuiteBuilder();
		}

		#endregion

		#region Build Method
        /// <summary>
        /// Loads the specified assembly and returns a TestSuite containing
        /// test fixtures found in the assembly. If fixtureName is null, then
        /// all fixtures are loaded. Otherwise only the specified fixture or
        /// namespace is loaded. 
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to load</param>
        /// <param name="fixtureName">Name of the fixture or of a namespace to load</param>
        /// <param name="autoSuites">If true, automatic namespace suites are created</param>
        /// <returns></returns>
		public TestSuite Build( string assemblyName, string fixtureName, bool autoSuites )
		{
            // Change currentDirectory in case assembly references unmanaged dlls
            // and so that any addins are able to access the directory easily.
            using (new DirectorySwapper(Path.GetDirectoryName(assemblyName)))
            {
                this.assembly = Load(assemblyName);
                if (assembly == null) return null;

                // If provided test name is actually the name of
                // a type, we handle it specially
                if (fixtureName != null && fixtureName != string.Empty)
                {
                    Type testType = assembly.GetType(fixtureName);
                    if (testType != null)
                        return BuildFromFixtureType(assemblyName, testType, autoSuites);
                }

                IList fixtures = GetFixtures(assembly, fixtureName);
                if (fixtures.Count > 0)
                    return BuildTestAssembly(assemblyName, fixtures, autoSuites);

                return null;
            }
		}
		#endregion

		#region Helper Methods

		private Assembly Load(string path)
		{
            Assembly assembly = null;

            // Throws if this isn't a managed assembly or if it was built
			// with a later version of the same assembly. 
			AssemblyName.GetAssemblyName( Path.GetFileName( path ) );
			
			// TODO: Figure out why we can't load using the assembly name
			// in all cases. Might be a problem with the tests themselves.
            assembly = Assembly.Load(Path.GetFileNameWithoutExtension(path));
			
            if ( assembly != null )
                CoreExtensions.Host.InstallAdhocExtensions( assembly );

			log.Info( "Loaded assembly " + assembly.FullName );

			return assembly;
		}

		private IList GetFixtures( Assembly assembly, string ns )
		{
			ArrayList fixtures = new ArrayList();
            log.Debug("Examining assembly for test fixtures");

			IList testTypes = GetCandidateFixtureTypes( assembly, ns );

            log.Debug("Found {0} classes to examine", testTypes.Count);
#if LOAD_TIMING
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif

			foreach(Type testType in testTypes)
			{
				if( TestFixtureBuilder.CanBuildFrom( testType ) )
					fixtures.Add( TestFixtureBuilder.BuildFrom( testType ) );
			}

#if LOAD_TIMING
            log.Debug("Found {0} fixtures in {1} seconds", fixtures.Count, timer.Elapsed);
#else
            log.Debug("Found {0} fixtures", fixtures.Count);
#endif

			return fixtures;
		}
	
		private IList GetCandidateFixtureTypes( Assembly assembly, string ns )
		{
			IList types = assembly.GetTypes();
				
			if ( ns == null || ns == string.Empty || types.Count == 0 ) 
				return types;

			string prefix = ns + "." ;
			
			ArrayList result = new ArrayList();
			foreach( Type type in types )
				if ( type.FullName.StartsWith( prefix ) )
					result.Add( type );

			return result;
		}

        private TestSuite BuildFromFixtureType(string assemblyName, Type testType, bool autoSuites)
        {
            // TODO: This is the only situation in which we currently
            // recognize and load legacy suites. We need to determine 
            // whether to allow them in more places.
            if (legacySuiteBuilder.CanBuildFrom(testType))
                return (TestSuite)legacySuiteBuilder.BuildFrom(testType);
            else if (TestFixtureBuilder.CanBuildFrom(testType))
                return BuildTestAssembly(assemblyName,
                    new Test[] { TestFixtureBuilder.BuildFrom(testType) }, autoSuites);
            return null;
        }

        private TestSuite BuildTestAssembly(string assemblyName, IList fixtures, bool autoSuites)
        {
            TestSuite testAssembly = new TestSuite(assemblyName);

            if (autoSuites)
            {
                NamespaceTreeBuilder treeBuilder =
                    new NamespaceTreeBuilder(testAssembly);
                treeBuilder.Add(fixtures);
                testAssembly = treeBuilder.RootSuite;
            }
            else
                foreach (TestSuite fixture in fixtures)
                {
                    if (fixture is SetUpFixture)
                    {
                        fixture.RunState = RunState.NotRunnable;
                        fixture.IgnoreReason = "SetUpFixture cannot be used when loading tests as a flat list of fixtures";
                    }

                    testAssembly.Add(fixture);
                }

            if (fixtures.Count == 0)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.IgnoreReason = "Has no TestFixtures";
            }

            NUnitFramework.ApplyCommonAttributes(assembly, testAssembly);

            testAssembly.Properties["_PID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
            testAssembly.Properties["_APPDOMAIN"] = AppDomain.CurrentDomain.FriendlyName;


            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }
        #endregion
    }
}
