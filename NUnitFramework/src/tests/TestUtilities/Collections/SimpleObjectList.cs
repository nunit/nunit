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
using System.Collections;
using System.Collections.Generic;

namespace NUnit.TestUtilities.Collections
{
    /// <summary>
    /// SimpleObjectCollection is used in testing to ensure that only
    /// methods of the ICollection interface are accessible.
    /// </summary>
    class SimpleObjectList : IList
    {
        private readonly IList contents = new List<object>();

        public SimpleObjectList(IEnumerable source)
        {
            foreach (object o in source)
                this.contents.Add(o);
        }

        public SimpleObjectList(params object[] source)
        {
            foreach (object o in source)
                this.contents.Add(o);
        }

        #region IList Members

        int IList.Add(object value)
        {
            return contents.Add(value);
        }

        void IList.Clear()
        {
            contents.Clear();
        }

        bool IList.Contains(object value)
        {
            return contents.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return contents.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            contents.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return contents.IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return contents.IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            contents.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            contents.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return contents[index];
            }
            set
            {
                contents[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection)contents).CopyTo(array, index);
        }

        public int Count
        {
            get { return contents.Count; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)contents).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)contents).SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        #endregion
    }
}
