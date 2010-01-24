// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// AsyncResult is the abstract base for all callback classes
    /// used within the framework.
    /// </summary>
    [Serializable]
    public abstract class AsyncResult : IAsyncResult
    {
        private object state;
        private bool synchronous;
        private bool isCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="state">The state of the result.</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
        public AsyncResult(object state, bool isCompleted, bool synchronous)
        {
            this.state = state;
            this.isCompleted = isCompleted;
            this.synchronous = synchronous;
        }

        #region IAsyncResult Members

        /// <summary>
        /// Gets the result of an operation.
        /// </summary>
        /// <value></value>
        /// <returns>The result state.</returns>
        public object AsyncState
        {
            get { return state; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>A WaitHandle</returns>
        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>true if the operation completed synchronously; otherwise, false.
        /// </returns>
        public bool CompletedSynchronously
        {
            get { return synchronous; }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        #endregion
    }

    /// <summary>
    /// FinalResult is an AsyncResult used the action has been completed.
    /// </summary>
    [Serializable]
    public class FinalResult : AsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalResult"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
        public FinalResult(object state, bool synchronous) : base(state, true, synchronous) { }
    }

    /// <summary>
    /// ProgressReport is an AsyncResult used when the action is still ongoing.
    /// </summary>
    [Serializable]
    public class ProgressReport : AsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        /// <param name="report">The report.</param>
        public ProgressReport(object report) : base(report, false, false) { }
    }
}
