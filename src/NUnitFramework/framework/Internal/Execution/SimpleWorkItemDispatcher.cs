// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
#if !NETSTANDARD1_6
    /// <summary>
    /// SimpleWorkItemDispatcher handles execution of WorkItems by
    /// directly executing them. It is provided so that a dispatcher
    /// is always available in the context, thereby simplifying the
    /// code needed to run child tests.
    /// </summary>
    public class SimpleWorkItemDispatcher : IWorkItemDispatcher
    {
        // The first WorkItem to be dispatched, assumed to be top-level item
        private WorkItem _topLevelWorkItem;

        // Thread used to run and cancel tests
        private Thread _runnerThread;

        #region IWorkItemDispatcher Members

        /// <summary>
        ///  The level of parallelism supported
        /// </summary>
        public int LevelOfParallelism { get { return 0; } }

        /// <summary>
        /// Start execution, creating the execution thread,
        /// setting the top level work and dispatching it.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            _topLevelWorkItem = topLevelWorkItem;
            _runnerThread = new Thread(RunnerThreadProc);

#if APARTMENT_STATE
            if (topLevelWorkItem.TargetApartment == ApartmentState.STA)
                _runnerThread.SetApartmentState(ApartmentState.STA);
#endif

            _runnerThread.Start();
        }

        /// <summary>
        /// Dispatch a single work item for execution by
        /// executing it directly.
        /// <param name="work">The item to dispatch</param>
        /// </summary>
        public void Dispatch(WorkItem work)
        {
            if (work != null)
                work.Execute();
        }


        private void RunnerThreadProc()
        {
            _topLevelWorkItem.Execute();
        }



        private readonly object cancelLock = new object();


        /// <summary>
        /// Cancel (abort or stop) the ongoing run.
        /// If no run is in process, the call has no effect.
        /// </summary>
        /// <param name="force">true if the run should be aborted, false if it should allow its currently running test to complete</param>
        public void CancelRun(bool force)
        {
            lock (cancelLock)
            {
                if (_topLevelWorkItem != null)
                {
                    _topLevelWorkItem.Cancel(force);
                    if (force)
                        _topLevelWorkItem = null;
                }
            }
        }
        #endregion
    }
#endif
}
