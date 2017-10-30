// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

namespace NUnit.TestUtilities.Collections
{
    /// <summary>
    /// SimpleObjectCollection is used in testing to ensure that only
    /// methods of the ICollection interface are accessible.
    /// </summary>
    class SimpleEnumerable : IEnumerable
    {
        private readonly List<object> contents = new List<object>();

        public SimpleEnumerable(IEnumerable<object> source)
        {
            this.contents = new List<object>(source);
        }

        public SimpleEnumerable(params object[] source)
        {
            this.contents = new List<object>(source);
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        #endregion
    }

    class SimpleEnumerableWithIEquatable : IEnumerable, IEquatable<IEnumerable<object>>
    {
        private readonly List<object> _contents = new List<object>();

        public SimpleEnumerableWithIEquatable(IEnumerable<object> source)
        {
            _contents = new List<object>(source);
        }

        public SimpleEnumerableWithIEquatable(params object[] source)
        {
            _contents = new List<object>(source);
        }

        public IEnumerator GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj is IEnumerable<object>)
            {
                List<object> other = new List<object>((IEnumerable<object>)obj);

                return other[0].Equals(_contents[0]);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(IEnumerable<object> other)
        {
            List<object> otherList = new List<object>(other);

            return _contents[0] == otherList[0];
        }
    }

    class SimpleIEquatableObj : IEquatable<SimpleIEquatableObj>
    {
        public bool Equals(SimpleIEquatableObj other)
        {
            return true;
        }
    }

    class SimpleOverridenEqualsObj
    {
        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
