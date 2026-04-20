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

        //private readonly IEqualityComparer<T> _comparer;
        private readonly ItemsStrategy _removeItemsStrategy;

        /// <summary>The result of the comparison between the two collections.</summary>
        public CollectionTallyResult Result
        {
            get
            {
                return _removeItemsStrategy.Result;
            }
        }

        private record struct StrategyKey(bool ContentsArePrimitive, bool ContentsAreSortable, bool FuzzyCompare);

        /// <summary>Construct a CollectionTally object from a collection and a comparer.</summary>
        /// <param name="c">The expected collection to compare against.</param>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to use for equality comparisons, which may be optimized to <see cref="EqualityComparer{T}.Default"/> when no comparer modifications are active.</param>
        public CollectionTally(IEnumerable<T> c, NUnitEqualityComparer comparer)
        {
            bool contentsArePrimitive = typeof(T).IsPrimitive;
            bool contentsAreSortable = typeof(T) != typeof(object) && (contentsArePrimitive || c.IsSortable());
            bool fuzzyCompare = comparer.IsModified || !(contentsArePrimitive || typeof(T).CanUseDefaultEquality());

            IEqualityComparer<T> equalityComparer = fuzzyCompare ? new NUnitEqualityComparerAdapter<T>(comparer) : EqualityComparer<T>.Default;
            StrategyKey key = new(contentsArePrimitive, contentsAreSortable, fuzzyCompare);

            _removeItemsStrategy = InferItemsStrategy(key, c, equalityComparer);
            _removeItemsStrategy.Initialize();
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
                fuzzyCompare = underlyingType is null || !(underlyingType.IsPrimitive || underlyingType.CanUseDefaultEquality());
            }

            IEqualityComparer<T> equalityComparer = fuzzyCompare ? new NUnitEqualityComparerAdapter<T>(comparer) : EqualityComparer<T>.Default;
            StrategyKey key = new(contentsArePrimitive, contentsAreSortable, fuzzyCompare);

            _removeItemsStrategy = InferItemsStrategy(key, c.Cast<T>(), equalityComparer);
            _removeItemsStrategy.Initialize();
        }

        private static ItemsStrategy InferItemsStrategy(StrategyKey k, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (!k.FuzzyCompare && k.ContentsArePrimitive)
            {
                return new LinearSortAndScanItemsStrategy(items, comparer);
            }
            else if (!k.FuzzyCompare)
            {
                return new HashableItemsStrategy(items, comparer);
            }
            else if (k.ContentsAreSortable)
            {
                return new QuadraticSortableItemsStrategy(items, comparer);
            }
            else
            {
                return new QuadraticItemsStrategy(items, comparer);
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

        private static List<T> ToList(IEnumerable<T> items)
        {
            var list = items is ICollection<T> ic ? new List<T>(ic.Count) : new List<T>();

            foreach (T item in items)
                list.Add(item);

            return list;
        }

        private abstract class ItemsStrategy
        {
            protected IEqualityComparer<T> Comparer { get; }
            protected List<T> MissingItems { get; }
            protected List<T> ExtraItems { get; }

            protected ItemsStrategy(IEnumerable<T> items, IEqualityComparer<T> comparer)
            {
                Comparer = comparer;
                MissingItems = ToList(items);
                ExtraItems = [];
            }

            public abstract void RemoveItems(IEnumerable<T> items);
            public virtual void Initialize()
            {
            }

            /// <summary>Try to remove an item from the tally.</summary>
            /// <param name="item">The item to remove.</param>
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

            public CollectionTallyResult Result
            {
                get
                {
                    return new CollectionTallyResult(new(MissingItems), new(ExtraItems));
                }
            }
        }

        private sealed class LinearSortAndScanItemsStrategy : ItemsStrategy
        {
            public LinearSortAndScanItemsStrategy(IEnumerable<T> items, IEqualityComparer<T> comparer)
                : base(items, comparer)
            {
            }

            public override void Initialize()
            {
                MissingItems.Sort();
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
            public HashableItemsStrategy(IEnumerable<T> items, IEqualityComparer<T> comparer)
                : base(items, comparer)
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

        private sealed class QuadraticSortableItemsStrategy : ItemsStrategy
        {
            public QuadraticSortableItemsStrategy(IEnumerable<T> items, IEqualityComparer<T> comparer)
                : base(items, comparer)
            {
            }

            public override void Initialize()
            {
                MissingItems.Sort();
            }

            public override void RemoveItems(IEnumerable<T> items)
            {
                var remove = ToList(items);
                remove.Sort();

                int extrasStart = ExtraItems.Count;

                // Reverse so that we match removing from the end,
                // see issue #2598 - Is.Not.EquivalentTo is extremely slow
                for (int index = remove.Count - 1; index >= 0; index--)
                    TryRemove(remove[index]);

                int extrasAddedCount = ExtraItems.Count - extrasStart;
                if (extrasAddedCount > 1)
                    ExtraItems.Reverse(extrasStart, extrasAddedCount);
            }
        }

        private sealed class QuadraticItemsStrategy : ItemsStrategy
        {
            public QuadraticItemsStrategy(IEnumerable<T> items, IEqualityComparer<T> comparer)
                : base(items, comparer)
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
