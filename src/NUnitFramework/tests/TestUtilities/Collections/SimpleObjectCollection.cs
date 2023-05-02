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
    internal class SimpleObjectCollection : ICollection
    {
        private readonly List<object?> contents;

        public SimpleObjectCollection(IEnumerable<object?> source)
        {
            this.contents = new List<object?>(source);
        }

        public SimpleObjectCollection(params object?[] source)
        {
            this.contents = new List<object?>(source);
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection)contents).CopyTo(array, index);
        }

        public int Count => contents.Count;

        public bool IsSynchronized => ((ICollection)contents).IsSynchronized;

        public object SyncRoot => ((ICollection)contents).SyncRoot;

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        #endregion
    }
}
