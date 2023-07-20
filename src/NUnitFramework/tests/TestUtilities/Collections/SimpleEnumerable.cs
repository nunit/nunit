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
    public class SimpleEnumerable : IEnumerable
    {
        private readonly List<object> _contents = new();

        public SimpleEnumerable(IEnumerable<object> source)
        {
            _contents = new List<object>(source);
        }

        public SimpleEnumerable(params object[] source)
        {
            _contents = new List<object>(source);
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        #endregion
    }

    public class SimpleIEquatableObj : IEquatable<SimpleIEquatableObj>
    {
        public bool Equals(SimpleIEquatableObj? other)
        {
            return true;
        }
    }
}
