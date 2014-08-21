// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if !NUNITLITE
using System;
using System.IO;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// Default NUnit implementation of ITestAssemblyRunner
    /// </summary>
    public class NUnitTestAssemblyRunner : AbstractTestAssemblyRunner
    {
        // Saved Console.Out and Error
        TextWriter _savedOut;
        TextWriter _savedErr;

        #region Constructor

        /// <summary>
        /// Construct an NUnitTestAssemblyRunner
        /// </summary>
        /// <param name="builder"></param>
        public NUnitTestAssemblyRunner(ITestAssemblyBuilder builder) : base(builder) { }

        #endregion

        #region AbstractTestAssemblyRunner Overrides

        /// <summary>
        /// Create the initial TestExecutionContext used to run tests
        /// </summary>
        /// <param name="listener">The ITestListener specified in the RunAsync call</param>
        protected override void CreateTestExecutionContext(ITestListener listener)
        {
            base.CreateTestExecutionContext(listener);

            int levelOfParallelization = GetLevelOfParallelization();

            if (levelOfParallelization > 0)
            {
                Context.Dispatcher = new ParallelWorkItemDispatcher(levelOfParallelization);
                // Assembly does not have IApplyToContext attributes applied
                // when the test is built, so  we do it here.
                // TODO: Generalize this
                if (LoadedTest.Properties.ContainsKey(PropertyNames.ParallelScope))
                    Context.ParallelScope =
                        (ParallelScope)LoadedTest.Properties.Get(PropertyNames.ParallelScope) & ~ParallelScope.Self;
            }
            else
                Context.Dispatcher = new SimpleWorkItemDispatcher();
        }

        /// <summary>
        /// Initiate the test run.
        /// </summary>
        public override void StartRun(ITestListener listener)
        {
            // Save Console.Out and Error for later restoration
            _savedOut = Console.Out;
            _savedErr = Console.Error;

            QueuingEventListener queue = new QueuingEventListener();

            if (Settings.Contains(DriverSettings.CaptureStandardOutput))
                Console.SetOut(new EventListenerTextWriter(queue, TestOutputType.Out));
            if (Settings.Contains(DriverSettings.CaptureStandardError))
                Console.SetError(new EventListenerTextWriter(queue, TestOutputType.Error));

            Context.Listener = queue;

            using (EventPump pump = new EventPump(listener, queue.Events))
            {
                pump.Start();

                Context.Dispatcher.Dispatch(TopLevelWorkItem);
            }
        }

        /// <summary>
        /// Handle the the Completed event for the top level work item
        /// </summary>
        protected override void OnRunCompleted(object sender, EventArgs e)
        {
            Console.SetOut(_savedOut);
            Console.SetError(_savedErr);

            base.OnRunCompleted(sender, e);
        }

        #endregion

        #region Helper Methods

        private int GetLevelOfParallelization()
        {
            return Settings.Contains(DriverSettings.NumberOfTestWorkers)
                ? (int)Settings[DriverSettings.NumberOfTestWorkers]
                : LoadedTest.Properties.ContainsKey(PropertyNames.LevelOfParallelization)
                    ? (int)LoadedTest.Properties.Get(PropertyNames.LevelOfParallelization)
                    : Math.Max(Environment.ProcessorCount, 2);
        }

        #endregion
    }
}
#endif
