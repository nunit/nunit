// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using System.Reflection;
using System.Threading;
using NUnit.Framework.Api;
using NUnit.Framework.Internal.WorkItems;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Default implementation of ITestAssemblyRunner
    /// </summary>
    public class DefaultTestAssemblyRunner : ITestAssemblyRunner
    {
        private ITestAssemblyBuilder builder;
        private TestSuite loadedTest;
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
        /// <param name="filter">A test filter used to select tests to be run</param>
        /// <returns></returns>
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            TestExecutionContext context = new TestExecutionContext();

            if (loadedTest == null)
                throw new InvalidOperationException("Run was called but no test has been loaded.");

            if (this.settings.Contains("DefaultTimeout"))
                context.TestCaseTimeout = (int)this.settings["DefaultTimeout"];
            if (this.settings.Contains("StopOnError"))
                context.StopOnError = (bool)this.settings["StopOnError"];
	
			if (this.settings.Contains("WorkDirectory"))
				context.WorkDirectory = (string)this.settings["WorkDirectory"];
			else
				context.WorkDirectory = Environment.CurrentDirectory;

#if NUNITLITE
            context.Listener = listener;

            WorkItem workItem = loadedTest.CreateWorkItem(filter);
            workItem.Execute(context);

            while (workItem.State != WorkItemState.Complete)
                System.Threading.Thread.Sleep(5);
            return workItem.Result;
#else
            QueuingEventListener queue = new QueuingEventListener();

            context.Out = new EventListenerTextWriter(queue, TestOutputType.Out);
            context.Error = new EventListenerTextWriter(queue, TestOutputType.Error);
            context.Listener = queue;

            WorkItem workItem = loadedTest.CreateWorkItem(filter);

            using (EventPump pump = new EventPump(listener, queue.Events))
            {
                pump.Start();

                workItem.Execute(context);

                while (workItem.State != WorkItemState.Complete)
                    Thread.Sleep(5);
                return workItem.Result;
            }
#endif
        }

        #endregion
    }
}
