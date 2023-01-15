// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// DictionaryContainsValueConstraint is used to test whether a dictionary
    /// contains an expected object as a value.
    /// </summary>
    public class DictionaryContainsValueConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// Construct a DictionaryContainsValueConstraint
        /// </summary>
        /// <param name="expected"></param>
        public DictionaryContainsValueConstraint(object expected)
            : base(expected)
        {
            Expected = expected;
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "ContainsValue";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "dictionary containing value " + MsgUtils.FormatValue(Expected);

        /// <summary>
        /// Gets the expected object
        /// </summary>
        protected object Expected { get; }

        /// <summary>
        /// Test whether the expected value is contained in the dictionary
        /// </summary>
        protected override bool Matches(IEnumerable actual)
        {
            var dictionary = ConstraintUtils.RequireActual<IDictionary>(actual, nameof(actual));

            foreach (object obj in dictionary.Values)
                if (ItemsEqual(obj, Expected))
                    return true;

            return false;
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public DictionaryContainsValueConstraint Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
        {
            // reverse the order of the arguments to match expectations of PredicateEqualityComparer
            Func<TMemberType, TCollectionType, bool> invertedComparison = (actual, expected) => comparison.Invoke(expected, actual);

            base.Using(EqualityAdapter.For(invertedComparison));
            return this;
        }
    }
}
