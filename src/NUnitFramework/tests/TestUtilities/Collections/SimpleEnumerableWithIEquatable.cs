// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.TestUtilities.Collections
{
    public class SimpleEnumerableWithIEquatable : IEnumerable<object>, IEquatable<SimpleEnumerableWithIEquatable>
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

        public override bool Equals(object? obj)
        {
            if (obj is IEnumerable<object> enumerable)
            {
                List<object> other = new List<object>(enumerable);

                return other[0].Equals(Contents[0]);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(SimpleEnumerableWithIEquatable? other)
        {
            return other is not null && Contents[0] == other.Contents[0];
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
}
