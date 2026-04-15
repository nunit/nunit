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
        private readonly bool _useMergeOptimization;

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
            _useMergeOptimization = ReferenceEquals(comparer, EqualityComparer<T>.Default);

            _missingItems = ToList(c);

            if (false && c.IsSortable())
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

                if (_useMergeOptimization)
                {
                    TryRemoveMergeSorted(remove);
                    return;
                }

                // Reverse so that we match removing from the end,
                // see issue #2598 - Is.Not.EquivalentTo is extremely slow
                for (int index = remove.Count - 1; index >= 0; index--)
                    TryRemove(remove[index]);
            }
            else if (_useMergeOptimization)
            {
                TryRemoveUnsortedHashed(c);
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

        private void TryRemoveUnsortedHashed(IEnumerable<T> removeItems)
        {
            var missingCounts = new Dictionary<T, int>();
            int missingNullCount = 0;

            foreach (T item in _missingItems)
            {
                if (item is null)
                {
                    missingNullCount++;
                    continue;
                }

                if (missingCounts.TryGetValue(item, out int count))
                    missingCounts[item] = count + 1;
                else
                    missingCounts[item] = 1;
            }

            foreach (T item in removeItems)
            {
                if (item is null)
                {
                    if (missingNullCount > 0)
                        missingNullCount--;
                    else
                        _extraItems.Add(item);

                    continue;
                }

                if (missingCounts.TryGetValue(item, out int count))
                {
                    if (count == 1)
                        missingCounts.Remove(item);
                    else
                        missingCounts[item] = count - 1;
                }
                else
                {
                    _extraItems.Add(item);
                }
            }

            var remainingMissingItems = new List<T>(_missingItems.Count);
            foreach (T item in _missingItems)
            {
                if (item is null)
                {
                    if (missingNullCount > 0)
                    {
                        remainingMissingItems.Add(item);
                        missingNullCount--;
                    }

                    continue;
                }

                if (missingCounts.TryGetValue(item, out int count) && count > 0)
                {
                    remainingMissingItems.Add(item);

                    if (count == 1)
                        missingCounts.Remove(item);
                    else
                        missingCounts[item] = count - 1;
                }
            }

            _missingItems.Clear();
            _missingItems.AddRange(remainingMissingItems);
        }

        private void TryRemoveMergeSorted(List<T> remove)
        {
            int missingIndex = _missingItems.Count - 1;
            int removeIndex = remove.Count - 1;

            var stillMissingDescending = new List<T>(_missingItems.Count);
            var orderComparer = Comparer<T>.Default;

            while (missingIndex >= 0 && removeIndex >= 0)
            {
                T missingItem = _missingItems[missingIndex];
                T removeItem = remove[removeIndex];

                int comparison = orderComparer.Compare(missingItem, removeItem);
                if (comparison == 0)
                {
                    missingIndex--;
                    removeIndex--;
                }
                else if (comparison > 0)
                {
                    stillMissingDescending.Add(missingItem);
                    missingIndex--;
                }
                else
                {
                    _extraItems.Add(removeItem);
                    removeIndex--;
                }
            }

            while (missingIndex >= 0)
                stillMissingDescending.Add(_missingItems[missingIndex--]);

            while (removeIndex >= 0)
                _extraItems.Add(remove[removeIndex--]);

            _missingItems.Clear();
            for (int index = stillMissingDescending.Count - 1; index >= 0; index--)
                _missingItems.Add(stillMissingDescending[index]);
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
