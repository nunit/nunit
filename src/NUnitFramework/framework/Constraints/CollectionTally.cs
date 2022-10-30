// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary><see cref="CollectionTally"/> counts (tallies) the number of occurrences 
    /// of each object in one or more enumerations.</summary>
    public sealed class CollectionTally
    {
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

        private readonly NUnitEqualityComparer _comparer;

        private readonly bool _isSortable;
        private bool _sorted = false;

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                var missingItems = new List<object>(_missingItems.Count);
                foreach (var o in _missingItems)
                    missingItems.Add(o);

                List<object> extraItems = new List<object>(_extraItems.Count);
                if (_sorted)
                {
                    for (int index = _extraItems.Count - 1; index >= 0; index--)
                        extraItems.Add(_extraItems[index]);
                }
                else
                {
                    extraItems.AddRange(_extraItems);
                }

                return new CollectionTallyResult(missingItems, extraItems);
            }
        }

        private readonly ArrayList _missingItems = new ArrayList();

        private readonly List<object> _extraItems = new List<object>();

        /// <summary>Construct a CollectionTally object from a comparer and a collection.</summary>
        /// <param name="comparer">The comparer to use for equality.</param>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(NUnitEqualityComparer comparer, IEnumerable c)
        {
            this._comparer = comparer;

            _missingItems = ToArrayList(c);

            if (c.IsSortable())
            {
                _missingItems.Sort();
                _isSortable = true;
            }
        }

        private bool ItemsEqual(object expected, object actual)
        {
            Tolerance tolerance = Tolerance.Default;
            return _comparer.AreEqual(expected, actual, ref tolerance);
        }

        /// <summary>Try to remove an object from the tally.</summary>
        /// <param name="o">The object to remove.</param>
        public void TryRemove(object o)
        {
            for (int index = _missingItems.Count - 1; index >= 0; index--)
            {
                if (ItemsEqual(_missingItems[index], o))
                {
                    _missingItems.RemoveAt(index);
                    return;
                }
            }

            _extraItems.Add(o);
        }

        /// <summary>Try to remove a set of objects from the tally.</summary>
        /// <param name="c">The objects to remove.</param>
        public void TryRemove(IEnumerable c)
        {
            if (_isSortable && c.IsSortable())
            {
                var remove = ToArrayList(c);
                remove.Sort();

                _sorted = true;

                // Reverse so that we match removing from the end,
                // see issue #2598 - Is.Not.EquivalentTo is extremely slow
                for (int index = remove.Count - 1; index >= 0; index--)
                    TryRemove(remove[index]);
            }
            else
            {
                TryRemoveSlow(c);
            }

            void TryRemoveSlow(IEnumerable c)
            {
                foreach (object o in c)
                    TryRemove(o);
            }
        }

        private static ArrayList ToArrayList(IEnumerable items)
        {
            var list = items is ICollection ic ? new ArrayList(ic.Count) : new ArrayList();

            foreach (object o in items)
                list.Add(o);

            return list;
        }
    }
}
