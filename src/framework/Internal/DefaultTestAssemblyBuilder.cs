using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// DefaultTestAssemblyBuilder loads a single assembly and builds a TestSuite
    /// containing test fixtures present in the assembly.
    /// </summary>
    public class DefaultTestAssemblyBuilder : ITestAssemblyBuilder
    {
        #region Instance Fields
        /// <summary>
        /// The loaded assembly
        /// </summary>
        Assembly assembly;

        /// <summary>
        /// Our LegacySuite builder, which is only used when a 
        /// fixture has been passed by name on the command line.
        /// </summary>
        NUnit.Core.Extensibility.ISuiteBuilder legacySuiteBuilder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTestAssemblyBuilder"/> class.
        /// </summary>
        public DefaultTestAssemblyBuilder()
        {
            // TODO: Keeping this separate till we can make
            //it work in all situations.
            legacySuiteBuilder = new NUnit.Core.Builders.LegacySuiteBuilder();
        }

        #endregion

        #region Build Methods
        /// <summary>
        /// Build a suite of tests from a provided assembly
        /// </summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>
        /// A TestSuite containing the tests found in the assembly
        /// </returns>
        public TestSuite Build(Assembly assembly, IDictionary options)
        {
            IList fixtureNames = options["LOAD"] as IList;

            IList fixtures = GetFixtures(assembly, fixtureNames);
            if (fixtures.Count > 0)
                return BuildTestAssembly(assembly.GetName().Name, fixtures);

            return null;
        }

        /// <summary>
        /// Build a suite of tests given the filename of an assembly
        /// </summary>
        /// <param name="assemblyName">The filename of the assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>
        /// A TestSuite containing the tests found in the assembly
        /// </returns>
        public TestSuite Build(string assemblyName, IDictionary options)
        {
            // Change currentDirectory in case assembly references unmanaged dlls
            // and so that any addins are able to access the directory easily.
            using (new DirectorySwapper(Path.GetDirectoryName(assemblyName)))
            {
                this.assembly = Load(assemblyName);
                if (assembly == null) return null;

                IList fixtureNames = options["LOAD"] as IList;

                IList fixtures = GetFixtures(assembly, fixtureNames);
                if (fixtures.Count > 0)
                    return BuildTestAssembly(assemblyName, fixtures);

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
            AssemblyName.GetAssemblyName(Path.GetFileName(path));

            // TODO: Figure out why we can't load using the assembly name
            // in all cases. Might be a problem with the tests themselves.
            assembly = Assembly.Load(Path.GetFileNameWithoutExtension(path));

            if (assembly != null)
                CoreExtensions.Host.InstallAdhocExtensions(assembly);

            InternalTrace.Info("Loaded assembly " + assembly.FullName);

            return assembly;
        }

        private IList GetFixtures(Assembly assembly, IList names)
        {
            ObjectList fixtures = new ObjectList();
            InternalTrace.Debug("Examining assembly for test fixtures");

            IList testTypes = GetCandidateFixtureTypes(assembly, names);

            InternalTrace.Debug("Found {0} classes to examine", testTypes.Count);
#if LOAD_TIMING
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif
            foreach (Type testType in testTypes)
            {
                if (TestFixtureBuilder.CanBuildFrom(testType))
                    fixtures.Add(TestFixtureBuilder.BuildFrom(testType));
                else if (names != null && legacySuiteBuilder.CanBuildFrom(testType))
                    fixtures.Add(legacySuiteBuilder.BuildFrom(testType));
            }

#if LOAD_TIMING
            InternalTrace.Debug("Found {0} fixtures in {1} seconds", fixtures.Count, timer.Elapsed);
#else
            InternalTrace.Debug("Found {0} fixtures", fixtures.Count);
#endif

            return fixtures;
        }

        private IList GetCandidateFixtureTypes(Assembly assembly, IList names)
        {
            IList types = assembly.GetTypes();

            if (names == null || names.Count == 0)
                return types;

            ObjectList result = new ObjectList();

            foreach (string name in names)
            {
                Type fixtureType = assembly.GetType(name);
                if (fixtureType != null)
                    result.Add(fixtureType);
                else
                {
                    string prefix = name + ".";

                    foreach (Type type in types)
                        if (type.FullName.StartsWith(prefix))
                            result.Add(type);
                }
            }

            return result;
        }

        private TestSuite BuildFromFixtureType(string assemblyName, Type testType)
        {
            // TODO: This is the only situation in which we currently
            // recognize and load legacy suites. We need to determine 
            // whether to allow them in more places.
            if (legacySuiteBuilder.CanBuildFrom(testType))
                return (TestSuite)legacySuiteBuilder.BuildFrom(testType);
            else if (TestFixtureBuilder.CanBuildFrom(testType))
                return BuildTestAssembly(assemblyName,
                    new Test[] { TestFixtureBuilder.BuildFrom(testType) });
            return null;
        }

        private TestSuite BuildTestAssembly(string assemblyName, IList fixtures)
        {
            TestSuite testAssembly = new TestAssembly(assemblyName);

            //NamespaceTreeBuilder treeBuilder =
            //    new NamespaceTreeBuilder(testAssembly);
            //treeBuilder.Add(fixtures);
            //testAssembly = treeBuilder.RootSuite;

            foreach (Test fixture in fixtures)
                testAssembly.Add(fixture);

            if (fixtures.Count == 0)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.IgnoreReason = "Has no TestFixtures";
            }

            testAssembly.ApplyCommonAttributes(Reflect.GetAttributes(assembly, false));

            testAssembly.Properties["_PID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
            testAssembly.Properties["_APPDOMAIN"] = AppDomain.CurrentDomain.FriendlyName;


            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }
        #endregion
    }
}
