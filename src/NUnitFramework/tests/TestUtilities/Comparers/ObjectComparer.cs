// ***********************************************************************
// Copyright (c) 2013 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectComparer is used in testing to ensure that only
    /// methods of the IComparer interface are used.
    /// </summary>
    public class ObjectComparer : IComparer
    {
        public bool WasCalled = false;
        public static readonly IComparer Default = new ObjectComparer();

        int IComparer.Compare(object x, object y)
        {
            WasCalled = true;
#if NETCOREAPP1_1
            return Comparer<object>.Default.Compare(x, y);
#else
            return Comparer.Default.Compare(x, y);
#endif
        }
    }
}
