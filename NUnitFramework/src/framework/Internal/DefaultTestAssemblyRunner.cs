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
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Api;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Default implementation of ITestAssemblyRunner
    /// </summary>
    public class DefaultTestAssemblyRunner : ITestAssemblyRunner
    {
        private ITestAssemblyBuilder _builder;
        private TestSuite _loadedTest;
        private IDictionary _settings;
        private AutoResetEvent _runComplete = new AutoResetEvent(false);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public DefaultTestAssemblyRunner(ITestAssemblyBuilder builder)
        {
            _builder = builder;
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
                return _loadedTest;
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
            _settings = settings;

            Randomizer.InitialSeed = settings.Contains("RandomSeed")
                ? (int)settings["RandomSeed"]
                : new Random().Next();

            _loadedTest = (TestSuite)_builder.Build(assemblyName, settings);
            if (_loadedTest == null) return false;

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
            _settings = settings;
            _loadedTest = (TestSuite)_builder.Build(assembly, settings);
            if (_loadedTest == null) return false;

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
            InternalTrace.Info("Running tests");
            if (_loadedTest == null)
                throw new InvalidOperationException("Run was called but no test has been loaded.");

            // Save Console.Out and Error for later restoration
            TextWriter savedOut = Console.Out;
            TextWriter savedErr = Console.Error;

            TestExecutionContext initialContext = CreateTestExecutionContext(_settings);

#if NUNITLITE
            initialContext.Listener = listener;

            WorkItem workItem = WorkItem.CreateWorkItem(_loadedTest, initialContext, filter);
            workItem.Completed += new EventHandler(OnRunCompleted);
            workItem.Execute();

            _runComplete.WaitOne();

            return workItem.Result;
#else
            QueuingEventListener queue = new QueuingEventListener();

            if (_settings.Contains("CaptureStandardOutput"))
                initialContext.Out = new EventListenerTextWriter(queue, TestOutputType.Out);
            if (_settings.Contains("CapureStandardError"))
                initialContext.Error = new EventListenerTextWriter(queue, TestOutputType.Error);

            initialContext.Listener = queue;

            int numWorkers = _settings.Contains("NumberOfTestWorkers")
                ? (int)_settings["NumberOfTestWorkers"]
                : 0;

            WorkItemDispatcher dispatcher = null;

            if (numWorkers > 0)
            {
                dispatcher = new WorkItemDispatcher(numWorkers);
                initialContext.Dispatcher = dispatcher;
            }

            WorkItem workItem = WorkItem.CreateWorkItem(_loadedTest, initialContext, filter);
            workItem.Completed += new EventHandler(OnRunCompleted);

            using (EventPump pump = new EventPump(listener, queue.Events))
            {
                pump.Start();

                if (dispatcher != null)
                {
                    dispatcher.Dispatch(workItem);
                    dispatcher.Start();
                }
                else
                    workItem.Execute();

                _runComplete.WaitOne();
            }

            Console.SetOut(savedOut);
            Console.SetError(savedErr);

            if (dispatcher != null)
            {
                dispatcher.Stop();
                dispatcher = null;
            }

            return workItem.Result;
#endif
        }

        private void OnRunCompleted(object sender, EventArgs e)
        {
            _runComplete.Set();
        }

        private static TestExecutionContext CreateTestExecutionContext(IDictionary settings)
        {
            TestExecutionContext context = new TestExecutionContext();

            if (settings.Contains("DefaultTimeout"))
                context.TestCaseTimeout = (int)settings["DefaultTimeout"];
            if (settings.Contains("StopOnError"))
                context.StopOnError = (bool)settings["StopOnError"];

            if (settings.Contains("WorkDirectory"))
                context.WorkDirectory = (string)settings["WorkDirectory"];
            else
#if NETCF || SILVERLIGHT
                context.WorkDirectory = Env.DocumentFolder;
#else
                context.WorkDirectory = Environment.CurrentDirectory;
#endif
            return context;
        }

        #endregion
    }
}
