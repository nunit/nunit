// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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

    class SimpleEnumerableWithIEquatable : IEnumerable<object>, IEquatable<SimpleEnumerableWithIEquatable>
    {
        public List<object> Contents { get; }

        public SimpleEnumerableWithIEquatable(IEnumerable<object> source)
        {
            Contents = new List<object>(source);
        }

        public SimpleEnumerableWithIEquatable(params object[] source)
        {
            Contents = new List<object>(source);
        }

        public override bool Equals(object obj)
        {
            if (obj is IEnumerable<object>)
            {
                List<object> other = new List<object>((IEnumerable<object>)obj);

                return other[0].Equals(Contents[0]);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(SimpleEnumerableWithIEquatable other)
        {
            return Contents[0] == other.Contents[0];
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Contents.GetEnumerator();
        }
    }

    class SimpleIEquatableObj : IEquatable<SimpleIEquatableObj>
    {
        public bool Equals(SimpleIEquatableObj other)
        {
            return true;
        }
    }
}
