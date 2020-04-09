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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            return TryFastAlgorithm(actual) ?? OriginalAlgorithm(actual);
        }

        /// <inheritdoc />
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            var matches = Matches(enumerable);
            var nonUnique = matches ? new object[0] : enumerable;
            return new UniqueItemsContstraintResult(this, actual, nonUnique);
        }

        private bool OriginalAlgorithm(IEnumerable actual)
        {
            var list = new List<object>();

            foreach (object o1 in actual)
            {
                foreach (object o2 in list)
                    if (ItemsEqual(o1, o2))
                        return false;
                list.Add(o1);
            }

            return true;
        }

        private bool? TryFastAlgorithm(IEnumerable actual)
        {
            // If the user specified any external comparer with Using, exit
            if (UsingExternalComparer)
                return null;

            // If IEnumerable<T> is not implemented exit,
            // Otherwise return value is the Type of T
            Type memberType = GetGenericTypeArgument(actual);
            if (memberType == null || !IsSealed(memberType) || IsHandledSpeciallyByNUnit(memberType))
                return null;

            // Special handling for ignore case with strings and chars
            if (IgnoringCase)
            {
                if (memberType == typeof(string))
                    return StringsUniqueIgnoringCase((IEnumerable<string>)actual);
                else if (memberType == typeof(char))
                    return CharsUniqueIgnoringCase((IEnumerable<char>)actual);
            }

            return (bool)ItemsUniqueMethod.MakeGenericMethod(memberType).Invoke(null, new object[] { actual });
        }

        private static bool IsSealed(Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }

        private static readonly MethodInfo ItemsUniqueMethod =
            typeof(UniqueItemsConstraint).GetMethod(nameof(ItemsUnique), BindingFlags.Static | BindingFlags.NonPublic);

        private static bool ItemsUnique<T>(IEnumerable<T> actual)
            => ItemsUniqueInternal(actual, EqualityComparer<T>.Default);

        private static bool StringsUniqueIgnoringCase(IEnumerable<string> actual)
            => ItemsUniqueInternal(actual, StringComparer.CurrentCultureIgnoreCase);

        private static bool CharsUniqueIgnoringCase(IEnumerable<char> actual)
            => ItemsUniqueInternal(actual, new CaseInsensitiveCharComparer());

        private static bool ItemsUniqueInternal<T>(IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            var hash = new HashSet<T>(comparer);

            foreach (T item in actual)
            {
                if (!hash.Add(item))
                    return false;
            }

            return true;
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

        private static Type GetGenericTypeArgument(IEnumerable actual)
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

        internal class CaseInsensitiveCharComparer : IEqualityComparer<char>
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

        internal class UniqueItemsContstraintResult : ConstraintResult
        {
            private ICollection<object> NonUniqueItems { get; }

            public UniqueItemsContstraintResult(IConstraint constraint, object actualValue, ICollection<object> nonUniqueItems)
            : base(constraint, actualValue, nonUniqueItems.Count > 0)
            {
                NonUniqueItems = nonUniqueItems;
            }

            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (this.Status == ConstraintStatus.Failure)
                {
                    writer.Write("non-unique: ");

                    // TODO: Expand MsgUtils so can write to StringWriter directly
                    // https://github.com/nunit/nunit/issues/3498
                    var output = MsgUtils.FormatCollection(NonUniqueItems, 0, NonUniqueItems.Count);
                    writer.Write(output);
                }
                else
                    base.WriteActualValueTo(writer);
            }
        }
    }
}
