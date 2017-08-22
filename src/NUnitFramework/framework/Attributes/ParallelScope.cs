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

using System;
using System.ComponentModel;

namespace NUnit.Framework
{
    /// <summary>
    /// The ParallelScope enumeration permits specifying the degree to
    /// which a test and its descendants may be run in parallel.
    /// </summary>
    [Flags]
    public enum ParallelScope
    {
        /// <summary>
        /// No ParallelScope was specified on the test
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Default = 0,

        /// <summary>
        /// The test may be run in parallel with others at the same level.
        /// Valid on classes and methods but not assemblies.
        /// </summary>
        Self = 1,
        /// <summary>
        /// Test may not be run in parallel with any others. Valid on
        /// classes and methods but not assemblies.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        None = 2,
        /// <summary>
        /// Mask used to extract the flags that apply to the item on which a
        /// ParallelizableAttribute has been placed, as opposed to descendants.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ItemMask = Self + None,

        /// <summary>
        /// Descendants of the test may be run in parallel with one another.
        /// Valid on assemblies and classes but not on methods.
        /// </summary>
        Children = 256,
        /// <summary>
        /// Descendants of the test down to the level of TestFixtures may be 
        /// run in parallel with one another. Valid on assemblies and classes
        /// but not on methods.
        /// </summary>
        Fixtures = 512,
        /// <summary>
        /// Mask used to extract all the flags that impact descendants of a 
        /// test and place them in the TestExecutionContext.
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        ContextMask = Children + Fixtures,

        /// <summary>
        /// The test and its descendants may be run in parallel with others at
        /// the same level. Valid on classes and methods but not assemblies.
        /// </summary>
        All = Self + Children
    }
}
