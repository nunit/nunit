// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Compatibility;
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
#if !NET35
            var hasAllComparable = true;
            var comparables = new List<IComparable>();

            foreach(var item in actual)
            {
                if (item is IComparable comparable)
                {
                    comparables.Add(comparable);
                }
                else
                {
                    hasAllComparable = false;
                    break;
                }
            }
                

            if (hasAllComparable)
                return (ICollection)NonUniqueItemsInternal(comparables, new NUnitSortingComparer(Comparer));
#endif
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
                    nonUniques.Add(o1);
            }

            return nonUniques;
      
        }

        private ICollection GetNonUniqueItems(IEnumerable actual)
        {
            // If the user specified any external comparer with Using, exit
            if (UsingExternalComparer)
                return OriginalAlgorithm(actual);

            // If IEnumerable<T> is not implemented exit,
            // Otherwise return value is the Type of T
            Type? memberType = GetGenericTypeArgument(actual);
            if (memberType == null || !IsSealed(memberType) || IsHandledSpeciallyByNUnit(memberType))
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

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsSealed(Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }

        private static readonly MethodInfo ItemsUniqueMethod =
            typeof(UniqueItemsConstraint).GetMethod(nameof(ItemsUnique), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly CaseInsensitiveCharComparer InsensitiveCharComparer =
            new CaseInsensitiveCharComparer();

        private static ICollection<T> ItemsUnique<T>(IEnumerable<T> actual)
            => NonUniqueItemsInternal(actual, EqualityComparer<T>.Default);

        private static ICollection<string> StringsUniqueIgnoringCase(IEnumerable<string> actual)
            => NonUniqueItemsInternal(actual, (IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);

        private static ICollection<char> CharsUniqueIgnoringCase(IEnumerable<char> actual)
            => NonUniqueItemsInternal(actual, InsensitiveCharComparer);

#if !NET35
        private static ICollection<T> NonUniqueItemsInternal<T>(IEnumerable<T> actual, IComparer<T> comparer)
        {
            var processedItems = new SortedSet<T>(comparer);
            var knownNonUniques = new SortedSet<T>(comparer);
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

                        if (nonUniques.Count > MsgUtils.DefaultMaxItems)
                            break;
                    }
                }
            }

            return nonUniques;
        }
#endif

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

                        if (nonUniques.Count > MsgUtils.DefaultMaxItems)
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
                if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                {
#if NET35 || NET40
                    return type.GetGenericArguments()[0];
#else
                    return type.GenericTypeArguments[0];
#endif
                }
            }

            return null;
        }

        private sealed class NUnitSortingComparer : IComparer<object>
        {
            public NUnitEqualityComparer Comparer { get; }

            public NUnitSortingComparer(NUnitEqualityComparer comparer)
            {
                Comparer = comparer;
            }

            public int Compare(object x, object y)
            {
                var tolerance = Tolerance.Default;
                if (Comparer.AreEqual(x, y, ref tolerance))
                    return 0;
                else
                    return Comparer<object>.Default.Compare(x, y);
            }
        }

        private sealed class CaseInsensitiveCharComparer : IEqualityComparer<char>
        {
            public bool Equals(char x, char y)
            {
                return char.ToLower(x) == char.ToLower(y);
            }

            public int GetHashCode(char obj)
            {
                return char.ToLower(obj).GetHashCode();
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
                    var output = MsgUtils.FormatCollection(NonUniqueItems);
                    writer.WriteLine(output);
                }
            }
        }
    }
}
