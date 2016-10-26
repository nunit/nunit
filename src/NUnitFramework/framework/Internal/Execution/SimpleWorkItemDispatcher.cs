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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// SimpleWorkItemDispatcher handles execution of WorkItems by
    /// directly executing them. It is provided so that a dispatcher
    /// is always available in the context, thereby simplifying the
    /// code needed to run child tests.
    /// </summary>
    public class SimpleWorkItemDispatcher : IWorkItemDispatcher
    {
#if !PORTABLE
        // The first WorkItem to be dispatched, assumed to be top-level item
        private WorkItem _topLevelWorkItem;

        // Thread used to run and cancel tests
        private Thread _runnerThread;
#endif

        #region IWorkItemDispatcher Members

        /// <summary>
        /// Dispatch a single work item for execution. The first
        /// work item dispatched is saved as the top-level
        /// work item and a thread is created on which to
        /// run it. Subsequent calls come from the top level
        /// item or its descendants on the proper thread.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
#if PORTABLE
            if (work != null)
                work.Execute();
#else
            if (_topLevelWorkItem != null)
                work.Execute();
            else
            {
                _topLevelWorkItem = work;
                _runnerThread = new Thread(RunnerThreadProc);

                if (work.TargetApartment == ApartmentState.STA)
                    _runnerThread.SetApartmentState(ApartmentState.STA);

                _runnerThread.Start();
            }
#endif
        }

#if !PORTABLE
        private void RunnerThreadProc()
        {
            _topLevelWorkItem.Execute();
        }
#endif

#if !PORTABLE
        private object cancelLock = new object();
#endif

        /// <summary>
        /// Cancel (abort or stop) the ongoing run.
        /// If no run is in process, the call has no effect.
        /// </summary>
        /// <param name="force">true if the run should be aborted, false if it should allow its currently running test to complete</param>
        public void CancelRun(bool force)
        {
#if !PORTABLE
            lock (cancelLock)
            {
                if (_topLevelWorkItem != null)
                {
                    _topLevelWorkItem.Cancel(force);
                    if (force)
                        _topLevelWorkItem = null;
                }
            }
#endif
        }
        #endregion
    }
}
