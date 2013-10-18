using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// DefaultTestAssemblyBuilder loads a single assembly and builds a TestSuite
    /// containing test fixtures present in the assembly.
    /// </summary>
    public class DefaultTestAssemblyBuilder : ITestAssemblyBuilder
    {
        static Logger log = InternalTrace.GetLogger(typeof(DefaultTestAssemblyBuilder));

        #region Instance Fields
        /// <summary>
        /// The loaded assembly
        /// </summary>
        Assembly assembly;

#if !NUNITLITE
        /// <summary>
        /// Our LegacySuite builder, which is only used when a 
        /// fixture has been passed by name on the command line.
        /// </summary>
        ISuiteBuilder legacySuiteBuilder;
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTestAssemblyBuilder"/> class.
        /// </summary>
        public DefaultTestAssemblyBuilder()
        {
#if !NUNITLITE
            // TODO: Keeping this separate till we can make
            //it work in all situations.
            legacySuiteBuilder = new LegacySuiteBuilder();
#endif
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
        public ITest Build(Assembly assembly, IDictionary options)
        {
            this.assembly = assembly;

            IList fixtureNames = options["LOAD"] as IList;

            IList fixtures = GetFixtures(assembly, fixtureNames);

            if (fixtures.Count > 0)
            {
#if NETCF || SILVERLIGHT
                AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
                return BuildTestAssembly(assemblyName.Name, fixtures);
#else   
                string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
                return BuildTestAssembly(assemblyPath, fixtures);
#endif
            }
            
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
        public ITest Build(string assemblyName, IDictionary options)
        {
            log.Debug("Loading {0} in AppDomain {1}", assemblyName, AppDomain.CurrentDomain.FriendlyName);

            this.assembly = Load(assemblyName);
            if (assembly == null) return null;

            IList fixtureNames = options["LOAD"] as IList;

            IList fixtures = GetFixtures(assembly, fixtureNames);
            if (fixtures.Count > 0)
                return BuildTestAssembly(assemblyName, fixtures);

            return null;
        }
        #endregion

        #region Helper Methods

        private Assembly Load(string path)
        {
            Assembly assembly = null;

#if NETCF || SILVERLIGHT
            return Assembly.Load(path);
#else
            // Throws if this isn't a managed assembly or if it was built
            // with a later version of the same assembly. 
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);

            assembly = Assembly.Load(assemblyName);
#endif

            // TODO: Can this ever be null?
            if (assembly == null)
            {
                log.Error("Failed to load assembly " + path);
            }
            else
            {
                log.Info("Loaded assembly " + assembly.FullName);
#if !NUNITLITE
                CoreExtensions.Host.InstallAdhocExtensions(assembly);
#endif
            }

            return assembly;
        }

        private IList GetFixtures(Assembly assembly, IList names)
        {
            var fixtures = new List<Test>();
            log.Debug("Examining assembly for test fixtures");

            IList testTypes = GetCandidateFixtureTypes(assembly, names);

            log.Debug("Found {0} classes to examine", testTypes.Count);
#if LOAD_TIMING
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif
            int testcases = 0;
            foreach (Type testType in testTypes)
            {
                if (TestFixtureBuilder.CanBuildFrom(testType))
                {
                    Test fixture = TestFixtureBuilder.BuildFrom(testType);
                    fixtures.Add(fixture);
                    testcases += fixture.TestCaseCount;
                }
#if !NUNITLITE
                else if (names != null && legacySuiteBuilder.CanBuildFrom(testType))
                    fixtures.Add(legacySuiteBuilder.BuildFrom(testType));
#endif
            }

#if LOAD_TIMING
            log.Debug("Found {0} fixtures with {1} test cases in {2} seconds", fixtures.Count, testcases, timer.Elapsed);
#else
            log.Debug("Found {0} fixtures with {1} test cases", fixtures.Count, testcases);
#endif

            return fixtures;
        }

        private IList GetCandidateFixtureTypes(Assembly assembly, IList names)
        {
            IList types = assembly.GetTypes();

            if (names == null || names.Count == 0)
                return types;

            var result = new List<Type>();

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
#if !NUNITLITE
            if (legacySuiteBuilder.CanBuildFrom(testType))
                return (TestSuite)legacySuiteBuilder.BuildFrom(testType);
            else 
#endif
            if (TestFixtureBuilder.CanBuildFrom(testType))
                return BuildTestAssembly(assemblyName,
                    new Test[] { TestFixtureBuilder.BuildFrom(testType) });
            return null;
        }

        private TestSuite BuildTestAssembly(string assemblyName, IList fixtures)
        {
            TestSuite testAssembly = new TestAssembly(assembly, assemblyName);

            if (fixtures.Count == 0)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, "Has no TestFixtures");
            }
            else
            {
                NamespaceTreeBuilder treeBuilder =
                    new NamespaceTreeBuilder(testAssembly);
                treeBuilder.Add(fixtures);
                testAssembly = treeBuilder.RootSuite;
            }

            testAssembly.ApplyAttributesToTest(assembly);

#if !SILVERLIGHT
            testAssembly.Properties.Set(PropertyNames.ProcessID, System.Diagnostics.Process.GetCurrentProcess().Id);
#endif
            testAssembly.Properties.Set(PropertyNames.AppDomain, AppDomain.CurrentDomain.FriendlyName);


            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }
        #endregion
    }
}
