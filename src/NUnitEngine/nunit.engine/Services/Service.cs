// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Abstract base class for services that can use it. Some Services
    /// already inherit from a different class and can't use this, which
    /// is why we define the IService interface as well.
    /// </summary>
    public abstract class Service : IService, IDisposable
    {
        #region IService Default Implementation

        /// <summary>
        /// The ServiceContext
        /// </summary>
        public IServiceLocator ServiceContext { get; set; }

        /// <summary>
        /// Gets the ServiceStatus of this service
        /// </summary>
        public ServiceStatus Status { get; protected set;  }

        /// <summary>
        /// Initialize the Service
        /// </summary>
        public virtual void StartService()
        {
            Status = ServiceStatus.Started;
        }

        /// <summary>
        /// Do any cleanup needed before terminating the service
        /// </summary>
        public virtual void StopService()
        {
            Status = ServiceStatus.Stopped;
        }

        #endregion

        #region IDisposable Default Implementation

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected bool _disposed = false;

        protected virtual void Dispose(bool disposing) { }

        #endregion
    }
}
