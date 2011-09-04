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
    public class DefaultTestAssemblyRunner : ITestAssemblyRunner
    {
        private ITestAssemblyBuilder builder;
        private TestSuite loadedTest;
        private Thread runThread;
        private IDictionary settings;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public DefaultTestAssemblyRunner(ITestAssemblyBuilder builder)
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
        /// <param name="runOptions">A dictionary containing options for this run</param>
        /// <returns></returns>
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            if (loadedTest == null)
                throw new InvalidOperationException("Run was called but no test has been loaded.");

            TestCommand command = this.loadedTest.GetTestCommand(filter);

            TestExecutionContext.Save();

            //ITestCommand rootCommand = TestCommandFactory.MakeCommand(this.loadedTest, filter);

            try
            {
                this.runThread = Thread.CurrentThread;

                QueuingEventListener queue = new QueuingEventListener();

                TestExecutionContext.CurrentContext.Out = new EventListenerTextWriter(queue, TestOutputType.Out);
                TestExecutionContext.CurrentContext.Error = new EventListenerTextWriter(queue, TestOutputType.Error);

                if (this.settings.Contains("DefaultTimeout"))
                    TestExecutionContext.CurrentContext.TestCaseTimeout = (int)this.settings["DefaultTimeout"];

                using (EventPump pump = new EventPump(listener, queue.Events, true))
                {
                    pump.Start();

                    return command.Execute(null, queue);
                }
            }
            finally
            {
                this.runThread = null;
                TestExecutionContext.Restore();
            }
        }

        #endregion
    }
}
