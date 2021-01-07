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

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectToStringComparer is used in testing the <see cref="Framework.Constraints.RangeConstraint"/> when the object does not implement the <see cref="IComparer"/>  interface.
    /// Compares them as numbers when both arguments are <see cref="int"/>, else it uses <seealso cref="string.CompareTo(string)"/>.
    /// </summary>
    public class ObjectToStringComparer : IComparer
    {
        public bool WasCalled { get; private set; }

        int IComparer.Compare(object x, object y)
        {
            WasCalled = true;

            string xAsString = x.ToString();
            string yAsString = y.ToString();
            if (int.TryParse(xAsString, out int intX) && int.TryParse(yAsString, out int intY))
            {
                return intX.CompareTo(intY);
            }

            return xAsString.CompareTo(yAsString);
        }
    }

    public class ObjectToStringEqualityComparer : IEqualityComparer
    {
        private readonly IComparer _comparer = new ObjectToStringComparer();

        public bool WasCalled { get; private set; }

        public new bool Equals(object x, object y)
        {
            WasCalled = true;
            return _comparer.Compare(x, y) == 0;
        }

        public int GetHashCode(object obj)
        {
            return obj.ToString().GetHashCode();
        }
    }

    /// <summary>
    /// NoComparer is a test class without implementing the IComparer interface.
    /// Used to test the RangeConstraint.
    /// </summary>
    public class NoComparer
    {
        private readonly object _value;
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
