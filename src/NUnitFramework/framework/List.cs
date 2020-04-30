// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using System.Collections;

namespace NUnit.Framework
{
    /// <summary>
    /// The List class is a helper class with properties and methods
    /// that supply a number of constraints used with lists and collections.
    /// </summary>
    [Obsolete("The List class has been deprecated and will be removed in a future release. "
        + "Please use the extension method System.Linq.Enumerable.Select instead.")]
    public class List
    {
        /// <summary>
        /// List.Map returns a ListMapper, which can be used to map
        /// the original collection to another collection.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static ListMapper Map( ICollection actual )
        {
            return new ListMapper( actual );
        }
    }
}
