// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework.Constraints.Comparers;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;

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
        public override string Description => "all items unique";

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
                {
                    processedItems.Add(o1);
                }
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
                if (item is not null)
                    allTypes.Add(item.GetType());
            }

            // Partly optimization, partly makes any subsequent all()/any() calls reliable
            if (allTypes.Count == 0)
                return Array.Empty<object>();

            var distinctTypes = allTypes.Distinct().ToList();
            if (distinctTypes.Count == 1)
            {
                var itemsType = distinctTypes[0];
                if (IsTypeSafeForFastPath(itemsType))
                {
                    var itemsOfT = ItemsCastMethod.MakeGenericMethod(itemsType).Invoke(null, new[] { actual })!;

                    if (IgnoringCase || IgnoringWhiteSpace || IgnoringLineEndingFormat)
                    {
                        if (itemsType == typeof(string))
                            return (ICollection)StringsUniqueIgnoringCaseOrWhiteSpaceOrLineEndingFormat((IEnumerable<string>)itemsOfT);
                        else if (itemsType == typeof(char))
                            return (ICollection)CharsUniqueIgnoringCaseOrCarriageReturnVsNewline((IEnumerable<char>)itemsOfT);
                    }

                    return (ICollection)ItemsUniqueMethod.MakeGenericMethod(itemsType).Invoke(null, new[] { itemsOfT })!;
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
            {
                return type.FullName().StartsWith("System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal);
            }
            else if (Numerics.IsNumericType(type))
            {
                return true;
            }
            else
            {
                return
                    type == typeof(string)
                    || type == typeof(char)
                    || type == typeof(DateTimeOffset)
                    || type == typeof(DictionaryEntry);
            }
        }

        private ICollection GetNonUniqueItems(IEnumerable actual)
        {
            // If the user specified any external comparer with Using, exit
            if (UsingExternalComparer)
                return OriginalAlgorithm(actual);

            // If IEnumerable<T> is not implemented exit,
            // Otherwise return value is the Type of T
            Type? memberType = GetGenericTypeArgument(actual);
            if (memberType is null)
                return TryInferFastPath(actual) ?? OriginalAlgorithm(actual);
            else if (!IsTypeSafeForFastPath(memberType))
                return OriginalAlgorithm(actual);

            // Special handling for ignore case/white-space with strings and chars
            if (IgnoringCase || IgnoringWhiteSpace || IgnoringLineEndingFormat)
            {
                if (memberType == typeof(string))
                    return (ICollection)StringsUniqueIgnoringCaseOrWhiteSpaceOrLineEndingFormat((IEnumerable<string>)actual);
                else if (memberType == typeof(char))
                    return (ICollection)CharsUniqueIgnoringCaseOrCarriageReturnVsNewline((IEnumerable<char>)actual);
            }

            return (ICollection)ItemsUniqueMethod.MakeGenericMethod(memberType).Invoke(null, new object[] { actual })!;
        }

        private static bool IsTypeSafeForFastPath(Type? type)
        {
            return type is not null && type.IsSealed && !IsHandledSpeciallyByNUnit(type);
        }

        private static readonly MethodInfo ItemsUniqueMethod =
            typeof(UniqueItemsConstraint).GetMethod(nameof(ItemsUnique), BindingFlags.Static | BindingFlags.NonPublic)!;

        private static readonly MethodInfo ItemsCastMethod =
            typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), BindingFlags.Static | BindingFlags.Public)!;

        private static ICollection<T> ItemsUnique<T>(IEnumerable<T> actual)
        {
            NullEquivalents? nullEquivalents = TestExecutionContext.GetNullEquivalents();
            IEqualityComparer<T> comparer = nullEquivalents is not null && MayHoldNullEquivalents(Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), nullEquivalents)
                ? new NullEquivalentEqualityComparer<T>(nullEquivalents)
                : EqualityComparer<T>.Default;

            return NonUniqueItemsInternal(actual, comparer);
        }

        // Whether an item may be null-equivalent, or be a tuple, key/value pair or dictionary entry with a part that may be.
        private static bool MayHoldNullEquivalents(Type type, NullEquivalents nullEquivalents)
        {
            if (nullEquivalents.MayApplyTo(type) || type == typeof(DictionaryEntry))
                return true;

            return CompositeParts.Get(type) is CompositeParts composite
                && composite.CompositeType.GetGenericArguments().Any(argument => MayHoldNullEquivalents(Nullable.GetUnderlyingType(argument) ?? argument, nullEquivalents));
        }

        // Default equality, except that null and null-equivalent values are equal to each other, also as parts of
        // tuples, key/value pairs and dictionary entries, matching how NUnitEqualityComparer treats them.
        private sealed class NullEquivalentEqualityComparer<T> : IEqualityComparer<T>
        {
            // A sealed item type that isn't composite never has parts to look at.
            private static readonly bool ItemsMayHaveParts = !typeof(T).IsSealed || CompositeParts.Get(Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) is not null;

            private readonly NullEquivalents _nullEquivalents;

            public NullEquivalentEqualityComparer(NullEquivalents nullEquivalents) => _nullEquivalents = nullEquivalents;

            public bool Equals(T? x, T? y)
            {
                if (x is null || (ItemsMayHaveParts && CompositeParts.Get(x.GetType()) is not null))
                    return PartsEqual(x, y);

                if (IsNull(x))
                    return IsNull(y);

                return !IsNull(y) && EqualityComparer<T>.Default.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                if (obj is not null && (!ItemsMayHaveParts || CompositeParts.Get(obj.GetType()) is null))
                    return IsNull(obj) ? 0 : EqualityComparer<T>.Default.GetHashCode(obj);

                return PartsHashCode(obj);
            }

            private bool PartsEqual(object? x, object? y)
            {
                if (IsNull(x))
                    return IsNull(y);
                if (IsNull(y))
                    return false;

                CompositeParts? xParts = CompositeParts.Get(x.GetType());
                CompositeParts? yParts = CompositeParts.Get(y.GetType());
                if (xParts is null || yParts is null)
                    return xParts is null && yParts is null && x.Equals(y);

                // Hash codes of composites come from their parts, so only the same kind of composite can be equal.
                if (xParts.CompositeType != yParts.CompositeType)
                    return false;

                foreach (Func<object, object?> part in xParts.Parts)
                {
                    if (!PartsEqual(part(x), part(y)))
                        return false;
                }

                return true;
            }

            private int PartsHashCode(object? value)
            {
                if (IsNull(value))
                    return 0;

                CompositeParts? parts = CompositeParts.Get(value.GetType());
                if (parts is null)
                    return value.GetHashCode();

                int hashCode = 17;
                foreach (Func<object, object?> part in parts.Parts)
                    hashCode = unchecked((hashCode * 31) + PartsHashCode(part(value)));

                return hashCode;
            }

            private bool IsNull([NotNullWhen(false)] object? value) => value is null || _nullEquivalents.IsNullEquivalent(value);
        }

        // Reads the parts of tuples, key/value pairs and dictionary entries, or null for other types.
        private sealed class CompositeParts
        {
            private static readonly ConcurrentDictionary<Type, CompositeParts?> PartsByType = new();
            private static readonly Func<Type, CompositeParts?> CreateFunc = Create;

            private CompositeParts(Type compositeType, Func<object, object?>[] parts)
            {
                CompositeType = compositeType;
                Parts = parts;
            }

            // The tuple, key/value pair or dictionary entry type, also for a subclass of a Tuple class.
            public Type CompositeType { get; }

            public Func<object, object?>[] Parts { get; }

            public static CompositeParts? Get(Type type) => PartsByType.GetOrAdd(type, CreateFunc);

            private static CompositeParts? Create(Type type)
            {
                if (type == typeof(DictionaryEntry))
                    return new CompositeParts(type, new Func<object, object?>[] { value => ((DictionaryEntry)value).Key, value => ((DictionaryEntry)value).Value });

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    PropertyInfo key = type.GetProperty(nameof(KeyValuePair<object, object>.Key))!;
                    PropertyInfo value = type.GetProperty(nameof(KeyValuePair<object, object>.Value))!;
                    return new CompositeParts(type, new Func<object, object?>[] { key.GetValue, value.GetValue });
                }

                // Tuple classes aren't sealed, and Tuple.Equals accepts a subclass.
                for (Type? tupleType = type; tupleType is not null; tupleType = tupleType.BaseType)
                {
#if !NETFRAMEWORK
                    // ITuple reads items without reflection. It flattens Rest, so use it below 8 items only.
                    if (tupleType.IsTupleOrValueTuple() && typeof(System.Runtime.CompilerServices.ITuple).IsAssignableFrom(tupleType))
                    {
                        int length = tupleType.GetGenericArguments().Length;
                        if (length < 8)
                        {
                            var itemParts = Enumerable.Range(0, length)
                                .Select(index => (Func<object, object?>)(value => ((System.Runtime.CompilerServices.ITuple)value)[index]))
                                .ToArray();
                            return new CompositeParts(tupleType, itemParts);
                        }
                    }
#endif
                    if (TupleTypeValuesSupplier.For(tupleType) is ITupleTypeValuesSupplier tuple)
                    {
                        var parts = Enumerable.Range(0, tuple.Length).Select(index => (Func<object, object?>)(value => tuple.GetValue(value, index))).ToArray();
                        return new CompositeParts(tupleType, parts);
                    }
                }

                return null;
            }
        }

        private ICollection<string> StringsUniqueIgnoringCaseOrWhiteSpaceOrLineEndingFormat(IEnumerable<string> actual)
            => NonUniqueItemsInternal(actual, new NUnitStringEqualityComparer(IgnoringCase, IgnoringWhiteSpace, IgnoringLineEndingFormat));

        private ICollection<char> CharsUniqueIgnoringCaseOrCarriageReturnVsNewline(IEnumerable<char> actual)
        {
            var result = NonUniqueItemsInternal(
                actual.Select(x => x.ToString()),
                new NUnitStringEqualityComparer(IgnoringCase, false, IgnoringLineEndingFormat));
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
            if (type == typeof(string))
                return false; // even though it's IEnumerable

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
                if (type.FullName().StartsWith("System.Collections.Generic.IEnumerable`1", StringComparison.Ordinal))
                {
                    return type.GenericTypeArguments[0];
                }
            }

            return null;
        }

        private sealed class NUnitStringEqualityComparer : IEqualityComparer<string>
        {
            private static readonly Regex WhiteSpace = new(@"\s+", RegexOptions.Compiled);

            private readonly bool _ignoreCase;
            private readonly bool _ignoreWhiteSpace;
            private readonly bool _ignoreLineEndingFormat;

            public NUnitStringEqualityComparer(bool ignoreCase, bool ignoreWhiteSpace, bool ignoreLineEndingFormat)
            {
                _ignoreCase = ignoreCase;
                _ignoreWhiteSpace = ignoreWhiteSpace;
                _ignoreLineEndingFormat = ignoreLineEndingFormat;
            }

            public bool Equals(string? x, string? y)
            {
                return x is not null && y is not null
                    ? StringsComparer.Equals(x, y, _ignoreCase, _ignoreWhiteSpace, _ignoreLineEndingFormat)
                    : ReferenceEquals(x, y);
            }

            public int GetHashCode(string obj)
            {
                if (obj is not string s)
                {
                    return 0;
                }

                if (_ignoreWhiteSpace)
                {
                    s = WhiteSpace.Replace(s, string.Empty);
                }

                IEqualityComparer<string> comparer = (_ignoreCase, _ignoreLineEndingFormat) switch
                {
                    (true, true) => IgnoreLineEndingFormatStringComparer.CurrentCultureIgnoreCase,
                    (true, false) => StringComparer.CurrentCultureIgnoreCase,
                    (false, true) => IgnoreLineEndingFormatStringComparer.Ordinal,
                    (false, false) => StringComparer.Ordinal,
                };
                return comparer.GetHashCode(s);
            }
        }

        internal sealed class UniqueItemsConstraintResult : ConstraintResult
        {
            internal ICollection NonUniqueItems { get; }

            public UniqueItemsConstraintResult(IConstraint constraint, object? actualValue, ICollection nonUniqueItems)
                : base(constraint, actualValue, nonUniqueItems.Count == 0)
            {
                NonUniqueItems = nonUniqueItems;
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (Status == ConstraintStatus.Failure)
                {
                    writer.Write("  Not unique items: ");
                    var output = MsgUtils.FormatCollection(NonUniqueItems, 0, MsgUtils.DefaultMaxItems);
                    writer.WriteLine(output);
                }
            }
        }
    }
}
