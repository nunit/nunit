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

namespace NUnit.Engine
{
    /// <summary>
    /// Represents the manner in which test assemblies use
    /// AppDomains to provide isolation
    /// </summary>
    public enum DomainUsage
    {
        /// <summary>
        /// Use the default setting, depending on the runner
        /// and the nature of the tests to be loaded.
        /// </summary>
        Default,
        /// <summary>
        /// Don't create a test domain - run in the primary AppDomain.
        /// Note that this requires the tests to be available in the
        /// NUnit appbase or probing path.
        /// </summary>
        None,
        /// <summary>
        /// Run tests in a single separate test domain
        /// </summary>
        Single,
        /// <summary>
        /// Run tests in a separate domain per assembly
        /// </summary>
        Multiple
    }
}