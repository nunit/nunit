// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly List<object> contents = new List<object>();

        public SimpleObjectList(IEnumerable<object> source)
        {
            this.contents = new List<object>(source);
        }

        public SimpleObjectList(params object[] source)
        {
            this.contents = new List<object>(source);
        }

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
            get { return  ((ICollection)contents).IsSynchronized; }
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

        #region IList Members

        public int Add(object value)
        {
            contents.Add(value);
            return contents.Count - 1;
        }

        public void Clear()
        {
            contents.Clear();
        }

        public bool Contains(object value)
        {
            return contents.Contains(value);
        }

        public int IndexOf(object value)
        {
            return contents.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            contents.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            contents.Remove(value);
        }

        public void RemoveAt(int index)
        {
            contents.RemoveAt(index);
        }

        public object this[int index]
        {
            get { return contents[index]; }
            set { contents[index] = value; }
        }

        #endregion
    }
}
