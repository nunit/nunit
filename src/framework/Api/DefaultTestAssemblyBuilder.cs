// ***********************************************************************
// Copyright (c) 2012-2014 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Api
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

        /// <summary>
        /// The default suite builder used by the test assembly builder.
        /// </summary>
        ISuiteBuilder _defaultSuiteBuilder;

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
            _defaultSuiteBuilder = new DefaultSuiteBuilder();

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
            log.Debug("Loading {0} in AppDomain {1}", assembly.FullName, AppDomain.CurrentDomain.FriendlyName);

            this.assembly = assembly;
            TestSuite testAssembly = null;

#if NETCF || SILVERLIGHT
            string assemblyPath = AssemblyHelper.GetAssemblyName(assembly).Name;
#else
            string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
#endif

            try
            {
                IList fixtureNames = options[DriverSettings.LOAD] as IList;
                IList fixtures = GetFixtures(assembly, fixtureNames);

                testAssembly = BuildTestAssembly(assemblyPath, fixtures);
            }
            catch (Exception ex)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);
            }

            return testAssembly;
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

            TestSuite testAssembly = new TestAssembly(assemblyName);

            try
            {
                this.assembly = Load(assemblyName);

                IList fixtureNames = options[DriverSettings.LOAD] as IList;

                IList fixtures = GetFixtures(assembly, fixtureNames);
                testAssembly = BuildTestAssembly(assemblyName, fixtures);
            }
            catch(Exception ex)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);
            }

            return testAssembly;
        }
        #endregion

        #region Helper Methods

        private Assembly Load(string path)
        {
#if NETCF || SILVERLIGHT 
            return Assembly.Load(path);
#else
            Assembly assembly = null;

            // Throws if this isn't a managed assembly or if it was built
            // with a later version of the same assembly. 
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);

            assembly = Assembly.Load(assemblyName);

            log.Info("Loaded assembly " + assembly.FullName);

            return assembly;
#endif
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
                if (_defaultSuiteBuilder.CanBuildFrom(testType))
                {
                    Test fixture = _defaultSuiteBuilder.BuildFrom(testType);
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

        private TestSuite BuildTestAssembly(string assemblyName, IList fixtures)
        {
            TestSuite testAssembly = new TestAssembly(assemblyName);

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
