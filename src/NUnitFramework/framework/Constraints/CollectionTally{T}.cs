// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
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
    internal sealed class CollectionTally<T>
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

        private readonly ItemsStrategy _removeItemsStrategy;

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result => _removeItemsStrategy.Result;

        /// <summary>Construct a CollectionTally object from a collection and a comparer.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons, which may be optimized to <see cref="EqualityComparer{T}.Default"/> when no comparer modifications are active.</param>
        public CollectionTally(IEnumerable<T> c, NUnitEqualityComparer comparer)
        {
            bool contentsArePrimitive = typeof(T).IsPrimitive;
            bool contentsAreSortable = typeof(T) != typeof(object) && (contentsArePrimitive || c.IsSortable());
            bool fuzzyCompare = comparer.IsModified || !(contentsArePrimitive || typeof(T).CanUseDefaultEquality());

            IEqualityComparer<T> comparerToUse = fuzzyCompare ? new NUnitEqualityComparerAdapter<T>(comparer) : EqualityComparer<T>.Default;

            _removeItemsStrategy = InferItemsStrategy(contentsArePrimitive, contentsAreSortable, fuzzyCompare, comparerToUse, c);
        }
        /// <summary>Construct a CollectionTally object from a collection and a comparer.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons, which may be optimized to <see cref="EqualityComparer{T}.Default"/> when no comparer modifications are active.</param>
        public CollectionTally(IEnumerable c, NUnitEqualityComparer comparer)
        {
            if (typeof(T) != typeof(object))
                throw new ArgumentException($"When using the non-generic constructor of {nameof(CollectionTally<T>)}, the type parameter T must be object.", nameof(c));

            bool contentsArePrimitive = false;
            bool contentsAreSortable = false;
            bool fuzzyCompare = comparer.IsModified;

            IEqualityComparer<T> comparerToUse = new NUnitEqualityComparerAdapter<T>(comparer);

            _removeItemsStrategy = InferItemsStrategy(contentsArePrimitive, contentsAreSortable, fuzzyCompare, comparerToUse, c.Cast<T>());
        }

        private static ItemsStrategy InferItemsStrategy(
            bool contentsArePrimitive, bool contentsAreSortable, bool fuzzyCompare,
            IEqualityComparer<T> comparerToUse, IEnumerable<T> items)
        {
            if (!fuzzyCompare && contentsArePrimitive)
            {
                return new MergeSortableItemsStrategy(comparerToUse, items);
            }
            else if (!fuzzyCompare)
            {
                return new HashableItemsStrategy(comparerToUse, items);
            }
            else if (contentsAreSortable)
            {
                return new QuadraticSortableItemsStrategy(comparerToUse, items);
            }
            else
            {
                return new QuadraticItemsStrategy(comparerToUse, items);
            }
        }

        /// <summary>Try to remove an item from the tally.</summary>
        /// <param name="item">The item to remove.</param>
        public void TryRemove(T item)
        {
            _removeItemsStrategy.TryRemove(item);
        }

        /// <summary>Try to remove a set of items from the tally.</summary>
        /// <param name="c">The items to remove.</param>
        public void TryRemove(IEnumerable<T> c)
        {
            _removeItemsStrategy.RemoveItems(c);
        }

        /// <summary>Try to remove a set of items from the tally.</summary>
        /// <param name="c">The items to remove.</param>
        public void TryRemove(IEnumerable c)
        {
            _removeItemsStrategy.RemoveItems(c.Cast<T>());
        }

        private abstract class ItemsStrategy
        {
            protected IEqualityComparer<T> Comparer { get; }
            protected List<T> MissingItems { get; }
            protected List<T> ExtraItems { get; }

            public CollectionTallyResult Result => new CollectionTallyResult(MissingItems, ExtraItems);

            protected ItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
            {
                Comparer = comparer;
                MissingItems = ToList(items);
                ExtraItems = new List<T>();
            }

            public void TryRemove(T item)
            {
                for (int index = MissingItems.Count - 1; index >= 0; index--)
                {
                    if (Comparer.Equals(MissingItems[index], item))
                    {
                        MissingItems.RemoveAt(index);
                        return;
                    }
                }

                ExtraItems.Add(item);
            }

            public abstract void RemoveItems(IEnumerable<T> items);

            protected static List<T> ToList(IEnumerable<T> items)
            {
                var list = items is ICollection<T> ic ? new List<T>(ic.Count) : new List<T>();

                foreach (T item in items)
                    list.Add(item);

                return list;
            }
        }

        private abstract class SortableItemsStrategy : ItemsStrategy
        {
            protected SortableItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
                : base(comparer, items)
            {
                MissingItems.Sort();
            }
        }

        private sealed class MergeSortableItemsStrategy : SortableItemsStrategy
        {
            public MergeSortableItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
                : base(comparer, items)
            {
            }

            public override void RemoveItems(IEnumerable<T> items)
            {
                var remove = ToList(items);
                remove.Sort();
                int missingIndex = MissingItems.Count - 1;
                int removeIndex = remove.Count - 1;

                var stillMissingDescending = new List<T>(MissingItems.Count);
                var orderComparer = Comparer<T>.Default;
                int extrasStart = ExtraItems.Count;

                while (missingIndex >= 0 && removeIndex >= 0)
                {
                    T missingItem = MissingItems[missingIndex];
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
                        ExtraItems.Add(removeItem);
                        removeIndex--;
                    }
                }

                while (missingIndex >= 0)
                    stillMissingDescending.Add(MissingItems[missingIndex--]);

                while (removeIndex >= 0)
                    ExtraItems.Add(remove[removeIndex--]);

                int extrasAddedCount = ExtraItems.Count - extrasStart;
                if (extrasAddedCount > 1)
                    ExtraItems.Reverse(extrasStart, extrasAddedCount);

                MissingItems.Clear();
                for (int index = stillMissingDescending.Count - 1; index >= 0; index--)
                    MissingItems.Add(stillMissingDescending[index]);
            }
        }

        private sealed class HashableItemsStrategy : ItemsStrategy
        {
            public HashableItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
                : base(comparer, items)
            {
            }

            public override void RemoveItems(IEnumerable<T> items)
            {
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
                // We can suppress this since we track nulls separately and will not try to store null keys in the dictionary.
                var missingCounts = new Dictionary<T, int>(Comparer);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
                int missingNullCount = 0;

                foreach (T item in MissingItems)
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
                            ExtraItems.Add(item);

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
                        ExtraItems.Add(item);
                    }
                }

                var remainingMissingItems = new List<T>(MissingItems.Count);
                foreach (T item in MissingItems)
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

                MissingItems.Clear();
                MissingItems.AddRange(remainingMissingItems);
            }
        }

        private sealed class QuadraticSortableItemsStrategy : SortableItemsStrategy
        {
            public QuadraticSortableItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
                : base(comparer, items)
            {
            }

            public override void RemoveItems(IEnumerable<T> items)
            {
                var remove = ToList(items);
                remove.Sort();

                int extrasAtStart = ExtraItems.Count;

                // Reverse so that we match removing from the end,
                // see issue #2598 - Is.Not.EquivalentTo is extremely slow
                for (int index = remove.Count - 1; index >= 0; index--)
                    TryRemove(remove[index]);

                int extrasAddedCount = ExtraItems.Count - extrasAtStart;
                if (extrasAddedCount > 1)
                    ExtraItems.Reverse(extrasAtStart, extrasAddedCount);
            }
        }

        private sealed class QuadraticItemsStrategy : ItemsStrategy
        {
            public QuadraticItemsStrategy(IEqualityComparer<T> comparer, IEnumerable<T> items)
                : base(comparer, items)
            {
            }

            public override void RemoveItems(IEnumerable<T> items)
            {
                foreach (T item in items)
                    TryRemove(item);
            }
        }
    }
}
