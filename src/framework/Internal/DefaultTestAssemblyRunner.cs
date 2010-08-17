using System;
using System.Collections;
using System.Threading;
using NUnit.Framework.Api;

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
        /// <param name="options">Dictionary of option settings for loading the assembly</param>
        /// <returns>True if the load was successful</returns>
        public bool Load(string assemblyName, IDictionary options)
        {
            this.loadedTest = this.builder.Build(assemblyName, options);
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
        /// <returns></returns>
        public ITestResult Run(ITestListener listener, IDictionary runOptions)
        {
            TestExecutionContext.Save();

            try
            {
                this.runThread = Thread.CurrentThread;

                QueuingEventListener queue = new QueuingEventListener();

                TestExecutionContext.CurrentContext.Out = new EventListenerTextWriter(queue, TestOutputType.Out);
                TestExecutionContext.CurrentContext.Error = new EventListenerTextWriter(queue, TestOutputType.Error);

                using (EventPump pump = new EventPump(listener, queue.Events, true))
                {
                    pump.Start();
                    return this.loadedTest.Run(listener);
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
