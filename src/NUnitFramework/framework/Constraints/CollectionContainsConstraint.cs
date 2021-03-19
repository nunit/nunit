// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionContainsConstraint is used to test whether a collection
    /// contains an expected object as a member.
    /// </summary>
    public class CollectionContainsConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// Construct a CollectionContainsConstraint
        /// </summary>
        /// <param name="expected"></param>
        [Obsolete("This constructor has been deprecated and will be removed in a future release. "
            + "Please use 'new SomeItemsConstraint(new EqualConstraint(expected))' or 'Has.Some.EqualTo(expected)' instead.")]
        public CollectionContainsConstraint(object expected)
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
        public override string DisplayName { get { return "Contains"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "collection containing " + MsgUtils.FormatValue(Expected); }
        }

        /// <summary>
        /// Gets the expected object
        /// </summary>
        protected object Expected { get; }

        /// <summary>
        /// Test whether the expected item is contained in the collection
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            foreach (object obj in actual)
                if (ItemsEqual(obj, Expected))
                    return true;

            return false;
        }


        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionContainsConstraint Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
        {
            // reverse the order of the arguments to match expectations of PredicateEqualityComparer
            Func<TMemberType, TCollectionType, bool> invertedComparison = (actual, expected) => comparison.Invoke(expected, actual);

            base.Using(EqualityAdapter.For(invertedComparison));
            return this;
        }
    }
}
