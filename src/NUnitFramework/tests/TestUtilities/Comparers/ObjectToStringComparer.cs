// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectToStringComparer is used in testing the <see cref="NUnit.Framework.Constraints.RangeConstraint"/> when the object does not implement the <see cref="IComparer"/>  interface.
    /// Compares them as numbers when both arguments are <see cref="int"/>, else it uses <seealso cref="string.CompareTo(string)"/>.
    /// </summary>
    public class ObjectToStringComparer : System.Collections.IComparer
    {
        public bool WasCalled = false;
        int System.Collections.IComparer.Compare(object x, object y)
        {
            WasCalled = true;
            int intX = 0;
            int intY = 0;

            if(int.TryParse(x.ToString(), out intX) && int.TryParse(y.ToString(), out intY))
            {
                return intX.CompareTo(intY);
            }

            return x.ToString().CompareTo(y.ToString());

        }
    }

    /// <summary>
    /// NoComparer is a test class without implementing the IComparer interface.
    /// Used to test the RangeConstraint.
    /// </summary>
    public class NoComparer
    {
        public readonly object _value;
        public NoComparer(object value)
        {
            _value = value;
        }
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}