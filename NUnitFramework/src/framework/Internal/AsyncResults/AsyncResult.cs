// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
    /// used within the framework. It is used to provide both
    /// final results and progress reports to the caller. Both
    /// types of results are represented by an XML fragment.
    /// </summary>
    [Serializable]
    public abstract class AsyncResult : IAsyncResult
    {
        protected string xmlResult;
        private bool synchronous;
        private bool isCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="xmlResult">The state of the result, represented by an XML string.</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
        public AsyncResult(string xmlResult, bool isCompleted, bool synchronous)
        {
            this.xmlResult = xmlResult;
            this.isCompleted = isCompleted;
            this.synchronous = synchronous;
        }

        #region IAsyncResult Members

        /// <summary>
        /// Gets the result of an operation.
        /// </summary>
        /// <value></value>
        /// <returns>The result state.</returns>
        object IAsyncResult.AsyncState
        {
            get { return xmlResult; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"/> that
        /// is used to wait for an asynchronous operation to complete.
        /// Not currently used in the NUnit framework.
        /// </summary>
        /// <returns>A WaitHandle</returns>
        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>true if the operation completed synchronously; otherwise, false.
        /// </returns>
        bool IAsyncResult.CompletedSynchronously
        {
            get { return synchronous; }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        bool IAsyncResult.IsCompleted
        {
            get { return isCompleted; }
        }

        #endregion
    }
}
