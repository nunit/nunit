// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
    /// Interface implemented by objects representing a runtime framework.
    /// </summary>
    public interface IRuntimeFramework
    {
        /// <summary>
        /// Gets the inique Id for this runtime, such as "net-4.5"
        /// </summary>
        string Id { get;  }

        /// <summary>
        /// Gets the display name of the framework, such as ".NET 4.5"
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the framework version: usually contains two components, Major
        /// and Minor, which match the corresponding CLR components, but not always.
        /// </summary>
        Version FrameworkVersion { get; }

        /// <summary>
        /// Gets the Version of the CLR for this framework
        /// </summary>
        Version ClrVersion { get; }

        /// <summary>
        /// Gets a string representing the particular profile installed,
        /// or null if there is no profile. Currently. the only defined 
        /// values are Full and Client.
        /// </summary>
        string Profile { get; }
    }
}
