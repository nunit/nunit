// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework.Internal;
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
        private readonly ItemsStrategy _removeItemsStrategy;

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                var missingItems = new List<T>(_missingItems);
                var extraItems = new List<T>(_extraItems);
                return new CollectionTallyResult(missingItems, extraItems);
            }
        }

        private readonly List<T> _missingItems = new();
        private readonly List<T> _extraItems = new();

        /// <summary>Construct a CollectionTally object from a collection and a comparer.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons, which may be optimized to <see cref="EqualityComparer{T}.Default"/> when no comparer modifications are active.</param>
        public CollectionTally(IEnumerable<T> c, NUnitEqualityComparer comparer)
        {
            bool contentsArePrimitive = typeof(T).IsPrimitive;
            bool contentsAreSortable = typeof(T) != typeof(object) && (contentsArePrimitive || c.IsSortable());
            bool fuzzyCompare = comparer.IsModified || !(contentsArePrimitive || typeof(T) == typeof(string));

            _comparer = fuzzyCompare ? new NUnitEqualityComparerAdapter<T>(comparer) : EqualityComparer<T>.Default;
            _missingItems = ToList(c);

            _removeItemsStrategy = InferItemsStrategy(contentsArePrimitive, contentsAreSortable, fuzzyCompare);
            _removeItemsStrategy.Initialize(this);
        }
        /// <summary>Construct a CollectionTally object from a collection and a comparer.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons, which may be optimized to <see cref="EqualityComparer{T}.Default"/> when no comparer modifications are active.</param>
        public CollectionTally(System.Collections.IEnumerable c, NUnitEqualityComparer comparer)
        {
            bool contentsArePrimitive = false;
            // When T is object, we can't rely on sorting because the runtime types may vary and produce unpredictable sort orders
            bool contentsAreSortable = typeof(T) != typeof(object) && (contentsArePrimitive || c.IsSortable());
            bool fuzzyCompare = comparer.IsModified;
            if (!fuzzyCompare)
            {
                var underlyingType = c.GetType().FindPrimaryEnumerableInterfaceGenericTypeArgument();
                fuzzyCompare = underlyingType is null || !(underlyingType.IsPrimitive || underlyingType == typeof(string));
            }

            _comparer = fuzzyCompare ? new NUnitEqualityComparerAdapter<T>(comparer) : EqualityComparer<T>.Default;
            _missingItems = c.Cast<T>().ToList();

            _removeItemsStrategy = InferItemsStrategy(contentsArePrimitive, contentsAreSortable, fuzzyCompare);
            _removeItemsStrategy.Initialize(this);
        }

        private static ItemsStrategy InferItemsStrategy(bool contentsArePrimitive, bool contentsAreSortable, bool fuzzyCompare)
        {
            if (!fuzzyCompare && contentsArePrimitive)
            {
                return new MergeSortableItemsStrategy();
            }
            else if (!fuzzyCompare)
            {
                return new HashableItemsStrategy();
            }
            else if (contentsAreSortable)
            {
                return new QuadraticSortableItemsStrategy();
            }
            else
            {
                return new QuadraticItemsStrategy();
            }
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
            _removeItemsStrategy.RemoveItems(this, c);
        }

        /// <summary>Try to remove a set of items from the tally.</summary>
        /// <param name="c">The items to remove.</param>
        public void TryRemove(IEnumerable c)
        {
            _removeItemsStrategy.RemoveItems(this, c.Cast<T>());
        }

        private static List<T> ToList(IEnumerable<T> items)
        {
            var list = items is ICollection<T> ic ? new List<T>(ic.Count) : new List<T>();

            foreach (T item in items)
                list.Add(item);

            return list;
        }

        private abstract class ItemsStrategy
        {
            public abstract void RemoveItems(CollectionTally<T> tally, IEnumerable<T> items);
            public virtual void Initialize(CollectionTally<T> tally)
            {
            }
        }

        private sealed class MergeSortableItemsStrategy : ItemsStrategy
        {
            public override void Initialize(CollectionTally<T> tally)
            {
                tally._missingItems.Sort();
            }

            public override void RemoveItems(CollectionTally<T> tally, IEnumerable<T> items)
            {
                var remove = ToList(items);
                remove.Sort();
                int missingIndex = tally._missingItems.Count - 1;
                int removeIndex = remove.Count - 1;

                var stillMissingDescending = new List<T>(tally._missingItems.Count);
                var orderComparer = Comparer<T>.Default;
                int extrasStart = tally._extraItems.Count;

                while (missingIndex >= 0 && removeIndex >= 0)
                {
                    T missingItem = tally._missingItems[missingIndex];
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
                        tally._extraItems.Add(removeItem);
                        removeIndex--;
                    }
                }

                while (missingIndex >= 0)
                    stillMissingDescending.Add(tally._missingItems[missingIndex--]);

                while (removeIndex >= 0)
                    tally._extraItems.Add(remove[removeIndex--]);

                int extrasAddedCount = tally._extraItems.Count - extrasStart;
                if (extrasAddedCount > 1)
                    tally._extraItems.Reverse(extrasStart, extrasAddedCount);

                tally._missingItems.Clear();
                for (int index = stillMissingDescending.Count - 1; index >= 0; index--)
                    tally._missingItems.Add(stillMissingDescending[index]);
            }
        }

        private sealed class HashableItemsStrategy : ItemsStrategy
        {
            public override void RemoveItems(CollectionTally<T> tally, IEnumerable<T> items)
            {
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
                // We can suppress this since we track nulls separately and will not try to store null keys in the dictionary.
                var missingCounts = new Dictionary<T, int>(tally._comparer);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
                int missingNullCount = 0;

                foreach (T item in tally._missingItems)
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

                foreach (T item in items)
                {
                    if (item is null)
                    {
                        if (missingNullCount > 0)
                            missingNullCount--;
                        else
                            tally._extraItems.Add(item);

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
                        tally._extraItems.Add(item);
                    }
                }

                var remainingMissingItems = new List<T>(tally._missingItems.Count);
                foreach (T item in tally._missingItems)
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

                tally._missingItems.Clear();
                tally._missingItems.AddRange(remainingMissingItems);
            }
        }

        private sealed class QuadraticSortableItemsStrategy : ItemsStrategy
        {
            public override void Initialize(CollectionTally<T> tally)
            {
                tally._missingItems.Sort();
            }

            public override void RemoveItems(CollectionTally<T> tally, IEnumerable<T> items)
            {
                var remove = ToList(items);
                remove.Sort();

                int extrasStart = tally._extraItems.Count;

                // Reverse so that we match removing from the end,
                // see issue #2598 - Is.Not.EquivalentTo is extremely slow
                for (int index = remove.Count - 1; index >= 0; index--)
                    tally.TryRemove(remove[index]);

                int extrasAddedCount = tally._extraItems.Count - extrasStart;
                if (extrasAddedCount > 1)
                    tally._extraItems.Reverse(extrasStart, extrasAddedCount);
            }
        }

        private sealed class QuadraticItemsStrategy : ItemsStrategy
        {
            public override void RemoveItems(CollectionTally<T> tally, IEnumerable<T> items)
            {
                foreach (T item in items)
                    tally.TryRemove(item);
            }
        }
    }
}
