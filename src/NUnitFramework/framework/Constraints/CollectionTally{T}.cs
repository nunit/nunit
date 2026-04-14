// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Generic version of <see cref="CollectionTally"/> that counts (tallies).
    /// the number of occurrences of each typed item in one or more enumerations.
    /// </summary>
    /// <typeparam name="T">The type of items in the collections being compared.</typeparam>
    public sealed class CollectionTally<T>
    {
        /// <summary>The result of a <see cref="CollectionTally{T}"/>.</summary>
        [DebuggerDisplay("Missing = {MissingItems.Count}, Extra = {ExtraItems.Count}")]
        public sealed class CollectionTallyResult
        {
            /// <summary>Items that were not in the expected collection.</summary>
            public List<T> ExtraItems { get; }

            /// <summary>Items that were not accounted for in the expected collection.</summary>
            public List<T> MissingItems { get; }

            /// <summary>Initializes a new instance of the <see cref="CollectionTallyResult"/> class with the given fields.</summary>
            public CollectionTallyResult(List<T> missingItems, List<T> extraItems)
            {
                MissingItems = missingItems;
                ExtraItems = extraItems;
            }
        }

        private readonly IEqualityComparer<T> _comparer;

        private readonly bool _isSortable;
        private bool _sorted = false;

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                var missingItems = new List<T>(_missingItems);

                var extraItems = _sorted
                    ? new List<T>(_extraItems.Reverse<T>())
                    : new List<T>(_extraItems);

                return new CollectionTallyResult(missingItems, extraItems);
            }
        }

        private readonly List<T> _missingItems = new();
        private readonly List<T> _extraItems = new();

        /// <summary>Construct a CollectionTally object from a comparer and a collection.</summary>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons.</param>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(NUnitEqualityComparer comparer, IEnumerable<T> c)
            : this(new NUnitEqualityComparerAdapter<T>(comparer), c)
        {
        }

        /// <summary>Construct a CollectionTally object from a comparer and a collection.</summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use for equality comparisons.</param>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(IEqualityComparer<T> comparer, IEnumerable<T> c)
        {
            _comparer = comparer;

            _missingItems = ToList(c);

            if (c.IsSortable())
            {
                _missingItems.Sort();
                _isSortable = true;
            }
        }

        /// <summary>Construct a CollectionTally object from a collection using a new <see cref="NUnitEqualityComparer"/>.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        public CollectionTally(IEnumerable<T> c)
            : this(new NUnitEqualityComparer(), c)
        {
        }

        /// <summary>Try to remove an item from the tally.</summary>
        /// <param name="item">The item to remove.</param>
        public void TryRemove(T item)
        {
            for (int index = _missingItems.Count - 1; index >= 0; index--)
            {
                if (_comparer.Equals(_missingItems[index], item))
                {
                    _missingItems.RemoveAt(index);
                    return;
                }
            }

            _extraItems.Add(item);
        }

        /// <summary>Try to remove a set of items from the tally.</summary>
        /// <param name="c">The items to remove.</param>
        public void TryRemove(IEnumerable<T> c)
        {
            if (_isSortable && c.IsSortable())
            {
                var remove = ToList(c);
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

            void TryRemoveSlow(IEnumerable<T> c)
            {
                foreach (T item in c)
                    TryRemove(item);
            }
        }

        private static List<T> ToList(IEnumerable<T> items)
        {
            var list = items is ICollection<T> ic ? new List<T>(ic.Count) : new List<T>();

            foreach (T item in items)
                list.Add(item);

            return list;
        }
    }
}
