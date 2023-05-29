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
    internal class SimpleObjectList : IList
    {
        private readonly List<object?> _contents;

        public SimpleObjectList(IEnumerable<object?> source)
        {
            _contents = new List<object?>(source);
        }

        public SimpleObjectList(params object?[] source)
        {
            _contents = new List<object?>(source);
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_contents).CopyTo(array, index);
        }

        public int Count => _contents.Count;

        public bool IsSynchronized => ((ICollection)_contents).IsSynchronized;

        public object SyncRoot => ((ICollection)_contents).SyncRoot;

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object? value)
        {
            _contents.Add(value);
            return _contents.Count - 1;
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public bool Contains(object? value)
        {
            return _contents.Contains(value);
        }

        public int IndexOf(object? value)
        {
            return _contents.IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            _contents.Insert(index, value);
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public void Remove(object? value)
        {
            _contents.Remove(value);
        }

        public void RemoveAt(int index)
        {
            _contents.RemoveAt(index);
        }

        public object? this[int index]
        {
            get => _contents[index];
            set => _contents[index] = value;
        }

        #endregion
    }
}
