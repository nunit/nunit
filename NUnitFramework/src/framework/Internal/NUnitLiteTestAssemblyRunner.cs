using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Default implementation of ITestAssemblyRunner
    /// </summary>
    public class NUnitLiteTestAssemblyRunner : ITestAssemblyRunner
    {
        private IDictionary settings;
        private ITestAssemblyBuilder builder;
        private TestSuite loadedTest;
        //private Thread runThread;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLiteTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public NUnitLiteTestAssemblyRunner(ITestAssemblyBuilder builder)
        {
            this.builder = builder;
        }

        #endregion

        #region Properties

        /// <summary>
        /// TODO: Documentation needed for property
        /// </summary>
        public ITest LoadedTest
        {
            get
            {
                return this.loadedTest;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assemblyName">File name of the assembly to load</param>
        /// <param name="settings">Dictionary of option settings for loading the assembly</param>
        /// <returns>True if the load was successful</returns>
        public bool Load(string assemblyName, IDictionary settings)
        {
            this.settings = settings;
            this.loadedTest = this.builder.Build(assemblyName, settings);
            if (loadedTest == null) return false;

            return true;
        }

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assembly">The assembly to load</param>
        /// <param name="settings">Dictionary of option settings for loading the assembly</param>
        /// <returns>True if the load was successful</returns>
        public bool Load(Assembly assembly, IDictionary settings)
        {
            this.settings = settings;
            this.loadedTest = this.builder.Build(assembly, settings);
            if (loadedTest == null) return false;

            return true;
        }

        ///// <summary>
        ///// Count Test Cases using a filter
        ///// </summary>
        ///// <param name="filter">The filter to apply</param>
        ///// <returns>The number of test cases found</returns>
        //public int CountTestCases(TestFilter filter)
        //{
        //    return this.suite.CountTestCases(filter);
        //}

        /// <summary>
        /// Run selected tests and return a test result. The test is run synchronously,
        /// and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run</param>
        /// <returns></returns>
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            if (this.settings.Contains("WorkDirectory"))
                TestExecutionContext.CurrentContext.WorkDirectory = (string)this.settings["WorkDirectory"];
            else
#if NETCF
                TestExecutionContext.CurrentContext.WorkDirectory = Env.DocumentFolder;
#else
                TestExecutionContext.CurrentContext.WorkDirectory = Environment.CurrentDirectory;
#endif

            TestCommand command = this.loadedTest.GetTestCommand(filter);

            return CommandRunner.Execute(command);
        }

        #endregion
    }
}
