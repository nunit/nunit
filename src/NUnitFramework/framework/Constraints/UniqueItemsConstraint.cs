// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// UniqueItemsConstraint tests whether all the items in a
    /// collection are unique.
    /// </summary>
    public class UniqueItemsConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "all items unique"; }
        }

        /// <summary>
        /// Check that all items are unique.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            var nonUniqueItems = GetNonUniqueItems(actual);
            return nonUniqueItems.Count == 0;
        }

        /// <inheritdoc />
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));

            var nonUniqueItems = GetNonUniqueItems(enumerable);

            return new UniqueItemsConstraintResult(this, actual, nonUniqueItems);
        }

        private ICollection OriginalAlgorithm(IEnumerable actual)
        {
            var nonUniques = new List<object>();
            var processedItems = new List<object>();

            foreach (var o1 in actual)
            {
                var isUnique = true;
                var unknownNonUnique = false;

                foreach (var o2 in processedItems)
                {
                    if (ItemsEqual(o1, o2))
                    {
                        isUnique = false;
                        unknownNonUnique = !nonUniques.Any(o2 => ItemsEqual(o1, o2));
                        break;
                    }
                }

                if (isUnique)
                    processedItems.Add(o1);
                else if (unknownNonUnique)
                {
                    nonUniques.Add(o1);
                    if (nonUniques.Count == MsgUtils.DefaultMaxItems)
                        break;
                }
            }

            return nonUniques;
        }

        private ICollection? TryInferFastPath(IEnumerable actual)
        {
            var allTypes = new List<Type>();
            var allItems = new List<object>();
            foreach (var item in actual)
            {
                allItems.Add(item);
                if (item != null)
                    allTypes.Add(item.GetType());
            }

            // Partly optimization, partly makes any subsequent all()/any() calls reliable
            if (allTypes.Count == 0)
                return Array.Empty<object>();

            var distinctTypes = allTypes.Distinct().ToList();
            if (distinctTypes.Count == 1)
            {
                var itemsType = distinctTypes.FirstOrDefault();
                if (IsTypeSafeForFastPath(itemsType))
                {
                    var itemsOfT = ItemsCastMethod.MakeGenericMethod(itemsType).Invoke(null, new[] { actual });

                    if (IgnoringCase)
                    {
                        if (itemsType == typeof(string))
                            return (ICollection)StringsUniqueIgnoringCase((IEnumerable<string>)itemsOfT);
                        else if (itemsType == typeof(char))
                            return (ICollection)CharsUniqueIgnoringCase((IEnumerable<char>)itemsOfT);
                    }

                    return (ICollection)ItemsUniqueMethod.MakeGenericMethod(itemsType).Invoke(null, new[] { itemsOfT });
                }
            }
            else
            {
                if (distinctTypes.All(o => IsTypeSafeForFastPath(o) && !IsSpecialComparisonType(o)))
                {
                    return (ICollection)ItemsUnique(allItems);
                }
            }

            return null;
        }

        private static bool IsSpecialComparisonType(Type type)
        {
            if (type.IsGenericType)
                return type.FullName.StartsWith("System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal);
            else if (Numerics.IsNumericType(type))
                return true;
            else
                return
                    type == typeof(string)
                    || type == typeof(char)
                    || type == typeof(DateTimeOffset)
                    || type == typeof(DictionaryEntry);
        }

        private ICollection GetNonUniqueItems(IEnumerable actual)
        {
            // If the user specified any external comparer with Using, exit
            if (UsingExternalComparer)
                return OriginalAlgorithm(actual);

            // If IEnumerable<T> is not implemented exit,
            // Otherwise return value is the Type of T
            Type? memberType = GetGenericTypeArgument(actual);
            if (memberType == null)
                return TryInferFastPath(actual) ?? OriginalAlgorithm(actual);
            else if (!IsTypeSafeForFastPath(memberType))
                return OriginalAlgorithm(actual);


            // Special handling for ignore case with strings and chars
            if (IgnoringCase)
            {
                if (memberType == typeof(string))
                    return (ICollection)StringsUniqueIgnoringCase((IEnumerable<string>)actual);
                else if (memberType == typeof(char))
                    return (ICollection)CharsUniqueIgnoringCase((IEnumerable<char>)actual);
            }

            return (ICollection)ItemsUniqueMethod.MakeGenericMethod(memberType).Invoke(null, new object[] { actual });
        }

        private static bool IsTypeSafeForFastPath(Type? type)
        {
            return type != null && type.IsSealed && !IsHandledSpeciallyByNUnit(type);
        }

        private static readonly MethodInfo ItemsUniqueMethod =
            typeof(UniqueItemsConstraint).GetMethod(nameof(ItemsUnique), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo ItemsCastMethod =
            typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), BindingFlags.Static | BindingFlags.Public);

        private static ICollection<T> ItemsUnique<T>(IEnumerable<T> actual)
            => NonUniqueItemsInternal(actual, EqualityComparer<T>.Default);

        private ICollection<string> StringsUniqueIgnoringCase(IEnumerable<string> actual)
            => NonUniqueItemsInternal(actual, new NUnitStringEqualityComparer(IgnoringCase));

        private ICollection<char> CharsUniqueIgnoringCase(IEnumerable<char> actual)
        {
            var result = NonUniqueItemsInternal(
                actual.Select(x => x.ToString()),
                new NUnitStringEqualityComparer(IgnoringCase)
            );
            return result.Select(x => x[0]).ToList();
        }

        private static ICollection<T> NonUniqueItemsInternal<T>(IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            var processedItems = new HashSet<T>(comparer);
            var knownNonUniques = new HashSet<T>(comparer);
            var nonUniques = new List<T>();

            foreach (T item in actual)
            {
                // Check if 'item' is a duplicate of a previously-processed item
                if (!processedItems.Add(item))
                {
                    // Check if 'item' has previously been flagged as a duplicate
                    if (knownNonUniques.Add(item))
                    {
                        nonUniques.Add(item);

                        if (nonUniques.Count == MsgUtils.DefaultMaxItems)
                            break;
                    }
                }
            }

            return nonUniques;
        }

        // Return true if NUnitEqualityHandler has special logic for Type
        private static bool IsHandledSpeciallyByNUnit(Type type)
        {
            if (type == typeof(string)) return false; // even though it's IEnumerable

            return type.IsArray
                || typeof(IEnumerable).IsAssignableFrom(type) // Covers lists, collections, dictionaries as well
                || typeof(System.IO.Stream).IsAssignableFrom(type) // Covers all streams
                || typeof(System.IO.DirectoryInfo).IsAssignableFrom(type) // Unlikely to be derived, but just in case
                || type.FullName == "System.Tuple"
                || type.FullName == "System.ValueTuple";
        }

        private static Type? GetGenericTypeArgument(IEnumerable actual)
        {
            foreach (var type in actual.GetType().GetInterfaces())
            {
                if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1", StringComparison.Ordinal))
                {
                    return type.GenericTypeArguments[0];
                }
            }

            return null;
        }

        private sealed class NUnitStringEqualityComparer : IEqualityComparer<string>
        {
            private readonly bool _ignoreCase;

            public NUnitStringEqualityComparer(bool ignoreCase)
            {
                _ignoreCase = ignoreCase;
            }

            public bool Equals(string x, string y)
            {
                var stringComparison = _ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.Ordinal;
                return x.Equals(y, stringComparison);
            }

            public int GetHashCode(string obj)
            {
                if (obj is null)
                    return 0;
                else if (_ignoreCase)
                    return StringComparer.CurrentCultureIgnoreCase.GetHashCode(obj);
                else
                    return obj.GetHashCode();
            }
        }

        internal sealed class UniqueItemsConstraintResult : ConstraintResult
        {
            internal ICollection NonUniqueItems { get; }

            public UniqueItemsConstraintResult(IConstraint constraint, object actualValue, ICollection nonUniqueItems)
                : base(constraint, actualValue, nonUniqueItems.Count == 0)
            {
                NonUniqueItems = nonUniqueItems;
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (this.Status == ConstraintStatus.Failure)
                {
                    writer.Write("  Not unique items: ");
                    var output = MsgUtils.FormatCollection(NonUniqueItems, 0, MsgUtils.DefaultMaxItems);
                    writer.WriteLine(output);
                }
            }
        }
    }
}
