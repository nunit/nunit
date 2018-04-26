// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary><see cref="CollectionTally"/> counts (tallies) the number of occurrences 
    /// of each object in one or more enumerations.</summary>
    public sealed class CollectionTally
    {
        private static readonly object NullKey = new object();

        /// <summary>The result of a <see cref="CollectionTally"/>.</summary>
        public sealed class CollectionTallyResult
        {
            /// <summary>Items that were not in the expected collection.</summary>
            public List<object> ExtraItems { get; }

            /// <summary>Items that were not accounted for in the expected collection.</summary>
            public List<object> MissingItems { get; }

            /// <summary>Initializes a new instance of the <see cref="CollectionTallyResult"/> class with the given fields.</summary>
            public CollectionTallyResult(List<object> missingItems, List<object> extraItems)
            {
                MissingItems = missingItems;
                ExtraItems = extraItems;
            }
        }

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                return new CollectionTallyResult(
                    new List<object>(_missingItems),
                    new List<object>(_extraItems));
            }
        }

        private readonly MissingItemCollection _missingItems;

        private readonly List<object> _extraItems = new List<object>();

        /// <summary>Construct a CollectionTally object from a comparer and a collection.</summary>
        /// <param name="comparer">The comparer to use for equality.</param>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(NUnitEqualityComparer comparer, IEnumerable c)
        {
            _missingItems = new MissingItemCollection(comparer, c);
        }

        /// <summary>Try to remove an object from the tally.</summary>
        /// <param name="o">The object to remove.</param>
        public void TryRemove(object o)
        {
            if (_missingItems.Remove(o)) return;
            _extraItems.Add(o);
        }

        /// <summary>Try to remove a set of objects from the tally.</summary>
        /// <param name="c">The objects to remove.</param>
        public void TryRemove(IEnumerable c)
        {
            foreach (object o in c)
                TryRemove(o);
        }

        private class MissingItemCollection : IEnumerable<object>
        {
            private readonly Dictionary<object, int> _items;
            private readonly int _count;

            public MissingItemCollection(NUnitEqualityComparer comparer, IEnumerable items)
            {
                _items = new Dictionary<object, int>(new TallyEqualityComparer(comparer));

                foreach (object item in items)
                {
                    var key = item ?? NullKey;

                    int occurrenceCount;
                    if (_items.TryGetValue(key, out occurrenceCount))
                    {
                        _items[key] = occurrenceCount + 1;
                    }
                    else
                    {
                        _items.Add(key, 1);
                    }

                    _count++;
                }
            }

            public bool Remove(object item)
            {
                var key = item ?? NullKey;

                int occurrenceCount;
                if (_items.TryGetValue(key, out occurrenceCount))
                {
                    if (occurrenceCount > 1)
                    {
                        _items[key] = occurrenceCount - 1;
                    }
                    else
                    {
                        _items.Remove(key);
                    }
                    return true;
                }

                return false;
            }

            public IEnumerator<object> GetEnumerator()
            {
                foreach (var pair in _items)
                {
                    var item = pair.Key == NullKey ? null : pair.Key;
                    for (var i = 0; i < pair.Value; i++)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class TallyEqualityComparer : EqualityComparer<object>
        {
            private readonly NUnitEqualityComparer _comparer;
            private Tolerance _tolerance = Tolerance.Default;

            public TallyEqualityComparer(NUnitEqualityComparer comparer)
            {
                _comparer = comparer;
            }

            public override bool Equals(object x, object y)
            {
                return _comparer.AreEqual(x, y, ref _tolerance);
            }

            public override int GetHashCode(object obj)
            {
                return _comparer.GetHashCode(obj);
            }
        }
    }
}
