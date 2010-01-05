using System;
using System.Collections;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// Default implementation of ITestAssemblyRunner
    /// </summary>
    public class DefaultTestAssemblyRunner : ITestAssemblyRunner
    {
        private ITestAssemblyBuilder builder;
        private TestSuite suite;
        private int runnerID;
        private Thread runThread;

        #region Constructors

        public DefaultTestAssemblyRunner(ITestAssemblyBuilder builder) : this(builder, 0) { }

        public DefaultTestAssemblyRunner(ITestAssemblyBuilder builder, int runnerID)
        {
            this.builder = builder;
            this.runnerID = runnerID;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assemblyName">File name of the assembly to load</param>
        /// <returns>True if the load was successful</returns>
        public bool Load(string assemblyName)
        {
            this.suite = this.builder.Build(assemblyName, null);
            if (suite == null) return false;

            return true;
        }

        /// <summary>
        /// Count Test Cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The number of test cases found</returns>
        public int CountTestCases(TestFilter filter)
        {
            return this.suite.CountTestCases(filter);
        }

        /// <summary>
        /// Run selected tests and return a test result. The test is run synchronously,
        /// and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">The filter to apply when running the tests</param>
        /// <returns></returns>
        public TestResult Run(ITestListener listener, TestFilter filter)
        {
            try
            {
                this.runThread = Thread.CurrentThread;

                QueuingEventListener queue = new QueuingEventListener();

                TestContext.Out = new EventListenerTextWriter(queue, TestOutputType.Out);
                TestContext.Error = new EventListenerTextWriter(queue, TestOutputType.Error);

                using (EventPump pump = new EventPump(listener, queue.Events, true))
                {
                    pump.Start();
                    return this.suite.Run(listener, filter);
                }
            }
            finally
            {
                this.runThread = null;
            }
        }

        #endregion
    }
}
