// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private static readonly Logger Log = InternalTrace.GetLogger(typeof(DefaultTestAssemblyBuilder));

        #region Instance Fields

        /// <summary>
        /// The default suite builder used by the test assembly builder.
        /// </summary>
        private readonly ISuiteBuilder _defaultSuiteBuilder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTestAssemblyBuilder"/> class.
        /// </summary>
        public DefaultTestAssemblyBuilder()
        {
            _defaultSuiteBuilder = new DefaultSuiteBuilder();
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
        public ITest Build(Assembly assembly, IDictionary<string, object> options)
        {
            Log.Debug("Loading {0} in AppDomain {1}", assembly.FullName!, AppDomain.CurrentDomain.FriendlyName);

            string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
            string suiteName = assemblyPath.Equals("<Unknown>")
                ? AssemblyHelper.GetAssemblyName(assembly).FullName
                : assemblyPath;

            return Build(assembly, suiteName, options);
        }

        /// <summary>
        /// Build a suite of tests given the name or the location of an assembly
        /// </summary>
        /// <param name="assemblyNameOrPath">The name or the location of the assembly.</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>
        /// A TestSuite containing the tests found in the assembly
        /// </returns>
        public ITest Build(string assemblyNameOrPath, IDictionary<string, object> options)
        {
            Log.Debug("Loading {0} in AppDomain {1}", assemblyNameOrPath, AppDomain.CurrentDomain.FriendlyName);

            TestSuite testAssembly;

            try
            {
                var assembly = AssemblyHelper.Load(assemblyNameOrPath);
                testAssembly = Build(assembly, assemblyNameOrPath, options);
            }
            catch (Exception ex)
            {
                testAssembly = new TestAssembly(assemblyNameOrPath);
                testAssembly.MakeInvalid(ExceptionHelper.BuildMessage(ex, true));
            }

            return testAssembly;
        }

        private TestSuite Build(Assembly assembly, string assemblyNameOrPath, IDictionary<string, object> options)
        {
            TestSuite testAssembly;

            try
            {
                if (options.TryGetValue(FrameworkPackageSettings.DefaultTestNamePattern, out object? defaultTestNamePattern))
                    TestNameGenerator.DefaultTestNamePattern = (string)defaultTestNamePattern;

                if (options.TryGetValue(FrameworkPackageSettings.WorkDirectory, out object? workDirectory))
                    TestContext.DefaultWorkDirectory = workDirectory as string;
                else
                    TestContext.DefaultWorkDirectory = Directory.GetCurrentDirectory();

                if (options.TryGetValue(FrameworkPackageSettings.TestParametersDictionary, out object? testParametersObject) &&
                    testParametersObject is Dictionary<string, string> testParametersDictionary)
                {
                    foreach (var parameter in testParametersDictionary)
                        TestContext.Parameters.Add(parameter.Key, parameter.Value);
                }
                else
                {
                    // This cannot be changed without breaking backwards compatibility with old runners.
                    // Deserializes the way old runners understand.

                    if (options.TryGetValue(FrameworkPackageSettings.TestParameters, out var testParameters))
                    {
                        var parametersString = (string?)testParameters;
                        if (!string.IsNullOrEmpty(parametersString))
                        {
                            foreach (var param in parametersString!.Tokenize(';'))
                            {
                                var eq = param.IndexOf('=');

                                if (eq > 0 && eq < param.Length - 1)
                                {
                                    var name = param.Substring(0, eq);
                                    var val = param.Substring(eq + 1);

                                    TestContext.Parameters.Add(name, val);
                                }
                            }
                        }
                    }
                }

                var filter = new PreFilter();
                if (options.TryGetValue(FrameworkPackageSettings.LOAD, out object? load))
                {
                    foreach (string filterText in (IList)load)
                        filter.Add(filterText);
                }

                var fixtures = GetFixtures(assembly, filter);

                testAssembly = BuildTestAssembly(assembly, assemblyNameOrPath, fixtures);
            }
            catch (Exception ex)
            {
                testAssembly = new TestAssembly(assemblyNameOrPath);
                testAssembly.MakeInvalid(ExceptionHelper.BuildMessage(ex, true));
            }

            return testAssembly;
        }

        #endregion

        #region Helper Methods

        private IList<Test> GetFixtures(Assembly assembly, PreFilter filter)
        {
            var fixtures = new List<Test>();
            Log.Debug("Examining assembly for test fixtures");

            var testTypes = GetCandidateFixtureTypes(assembly, filter);

            Log.Debug("Found {0} classes to examine", testTypes.Count);
            var timer = Stopwatch.StartNew();
            var testcases = 0;
            foreach (Type testType in testTypes)
            {
                var typeInfo = new TypeWrapper(testType);

                // Any exceptions from this call are fatal problems in NUnit itself,
                // since this is always DefaultSuiteBuilder and the current implementation
                // of DefaultSuiteBuilder.CanBuildFrom cannot invoke any user code.
                if (_defaultSuiteBuilder.CanBuildFrom(typeInfo))
                {
                    // We pass the filter for use in selecting methods of the type.
                    // Any exceptions from this call are fatal problems in NUnit itself,
                    // since this is always DefaultSuiteBuilder and the current implementation
                    // of DefaultSuiteBuilder.BuildFrom handles all exceptions from user code.
                    Test fixture = _defaultSuiteBuilder.BuildFrom(typeInfo, filter);
                    fixtures.Add(fixture);
                    testcases += fixture.TestCaseCount;
                }
            }

            Log.Debug("Found {0} fixtures with {1} test cases in {2}ms", fixtures.Count, testcases, timer.ElapsedMilliseconds);

            return fixtures;
        }

        private IList<Type> GetCandidateFixtureTypes(Assembly assembly, PreFilter filter)
        {
            var result = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (filter.IsMatch(type))
                    result.Add(type);
            }

            return result;
        }

        private TestSuite BuildTestAssembly(Assembly assembly, string assemblyNameOrPath, IList<Test> fixtures)
        {
            TestSuite testAssembly = new TestAssembly(assembly, assemblyNameOrPath);

            if (fixtures.Count == 0)
            {
                testAssembly.MakeInvalid("No test fixtures were found.");
            }
            else
            {
                NamespaceTreeBuilder treeBuilder =
                    new NamespaceTreeBuilder(testAssembly);
                treeBuilder.Add(fixtures);
                testAssembly = treeBuilder.RootSuite;
            }

            testAssembly.ApplyAttributesToTest(assembly);

            try
            {
                using var process = System.Diagnostics.Process.GetCurrentProcess();
                testAssembly.Properties.Set(PropertyNames.ProcessId, process.Id);
            }
            catch (PlatformNotSupportedException)
            {
            }
            testAssembly.Properties.Set(PropertyNames.AppDomain, AppDomain.CurrentDomain.FriendlyName);

            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }
        #endregion
    }
}
