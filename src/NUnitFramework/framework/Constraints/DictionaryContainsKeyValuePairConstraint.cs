// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// DictionaryContainsKeyValuePairConstraint is used to test whether a dictionary
    /// contains an expected object as a key-value-pair.
    /// </summary>
    public sealed class DictionaryContainsKeyValuePairConstraint : CollectionItemsEqualConstraint
    {
        private readonly DictionaryEntry _expected;

        /// <summary>
        /// Construct a DictionaryContainsKeyValuePairConstraint
        /// </summary>
        public DictionaryContainsKeyValuePairConstraint(object key, object value)
        {
            _expected = new DictionaryEntry(key, value);
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "ContainsKeyValuePair"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "dictionary containing entry " + MsgUtils.FormatValue(_expected); }
        }

        private bool Matches(object actual)
        {
            if (actual == null)
                throw new ArgumentException("Expected: IDictionary But was: null", nameof(actual));

            if (TypeHelper.TryCast<IDictionary>(actual, out var dictionary))
            {
                foreach (var entry in dictionary)
                    if (ItemsEqual(entry, _expected))
                        return true;
                return false;
            }

            // If 'actual' implements IDictionary<TKey, TValue>, construct an 'expected' KeyValuePair<TKey, TValue>
            // and look it up by iterating using IEnumerable
            if (actual.GetType().GetInterfaces().Any(i => i.FullName.StartsWith("System.Collections.Generic.IDictionary`2[")))
            {
                var expected = new KeyValuePair<object, object>(_expected.Key, _expected.Value);
                var enumerable = actual as IEnumerable;

                foreach (var item in enumerable)
                {
                    if (ItemsEqual(item, expected))
                        return true;
                }

                return false;
            }

            throw new ArgumentException($"Expected: IDictionary But was: {actual.GetType()}", nameof(actual));
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, Matches(actual));
        }

        /// <summary>
        /// Test whether the expected key is contained in the dictionary
        /// </summary>
        protected override bool Matches(IEnumerable collection)
        {
            return Matches(collection);
        }
    }
}
