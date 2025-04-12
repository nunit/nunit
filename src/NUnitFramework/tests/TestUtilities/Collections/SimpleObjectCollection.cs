// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.TestUtilities.Collections
{
    /// <summary>
    /// SimpleObjectCollection is used in testing to ensure that only
    /// methods of the ICollection interface are accessible.
    /// </summary>
    public class SimpleObjectCollection : ICollection
    {
        private readonly List<object?> _contents;

        public SimpleObjectCollection(IEnumerable<object?> source)
        {
            _contents = new List<object?>(source);
        }

        public SimpleObjectCollection(params object?[] source)
        {
            _contents = new List<object?>(source);
        }

        public override string ToString() => $"SimpleObjectCollection [{string.Join(", ", _contents)}]";

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
    }
}
