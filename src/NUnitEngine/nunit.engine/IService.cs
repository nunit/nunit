// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// Enumeration representing the status of a service
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>Service was never started or has been stopped</summary>
        Stopped,
        /// <summary>Started successfully</summary>
        Started,
        /// <summary>Service failed to start and is unavailable</summary>
        Error
    }

    /// <summary>
    /// The IService interface is implemented by all Services.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// The ServiceContext
        /// </summary>
        ServiceContext ServiceContext { get; set; }

        /// <summary>
        /// Gets the ServiceStatus of this service
        /// </summary>
        ServiceStatus Status { get;  }

        /// <summary>
        /// Initialize the Service
        /// </summary>
        void StartService();

        /// <summary>
        /// Do any cleanup needed before terminating the service
        /// </summary>
        void StopService();
    }
}
