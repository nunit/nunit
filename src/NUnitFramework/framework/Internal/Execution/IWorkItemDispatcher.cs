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

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// An IWorkItemDispatcher handles execution of work items.
    /// </summary>
    public interface IWorkItemDispatcher
    {
        /// <summary>
        /// Dispatch a single work item for execution. The first
        /// work item dispatched is saved as the top-level
        /// work item and used when stopping the run.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        void Dispatch(WorkItem work);

        /// <summary>
        /// Cancel the ongoing run completely.
        /// If no run is in process, the call has no effect.
        /// </summary>
        /// <param name="force">true if the IWorkItemDispatcher should abort all currently running WorkItems, false if it should allow all currently running WorkItems to complete</param>
        void CancelRun(bool force);
    }
}
