// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
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
            var dictionary = ConstraintUtils.RequireActual<IDictionary>(actual, nameof(actual));
            foreach (var entry in dictionary)
                if (ItemsEqual(entry, _expected))
                    return true;

            return false;
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
