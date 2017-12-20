// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// MainThreadWorkItemDispatcher handles execution of WorkItems by
    /// directly executing them on the main thread. This is different
    /// from the SimpleWorkItemDispatcher where the work item is dispatched
    /// onto its own thread.
    /// </summary>
    public class MainThreadWorkItemDispatcher : IWorkItemDispatcher
    {

        #region IWorkItemDispatcher Members

        /// <summary>
        ///  The level of parallelism supported
        /// </summary>
        public int LevelOfParallelism { get { return 0; } }

        /// <summary>
        /// Start execution, dispatching the top level
        /// work into the main thread.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            Dispatch(topLevelWorkItem);
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

        /// <summary>
        /// This method is not supported for 
        /// this dispatcher. Using it will throw a
        /// NotSupportedException.
        /// </summary>
        /// <param name="force">Not used</param>
        /// <exception cref="System.NotSupportedException">If used, it will always throw this.</exception>
        public void CancelRun(bool force)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
