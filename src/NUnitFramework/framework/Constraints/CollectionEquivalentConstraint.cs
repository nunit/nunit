// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionEquivalentConstraint is used to determine whether two
    /// collections are equivalent.
    /// </summary>
    public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;

        /// <summary>Construct a CollectionEquivalentConstraint</summary>
        /// <param name="expected">Expected collection.</param>
        public CollectionEquivalentConstraint(IEnumerable expected)
            : base(expected)
        {
            _expected = expected;
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "Equivalent";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "equivalent to " + MsgUtils.FormatValue(_expected);

        /// <summary>
        /// Test whether two collections are equivalent
        /// </summary>
        private bool Matches(
            IEnumerable actual,
            out CollectionTally.CollectionTallyResult tallyResult)
        {
            // Early return if both collections are reference-equal or
            // both collections are in their default or uninitialized state.
            if (actual.Equals(_expected))
            {
                tallyResult = new CollectionTally.CollectionTallyResult([], []);
                return true;
            }

            CollectionTally ct = Tally(_expected);
            ct.TryRemove(actual);

            // Store the CollectionTallyResult so the comparison between the two collections
            // is only performed once.
            tallyResult = ct.Result;

            return ((tallyResult.ExtraItems.Count == 0) && (tallyResult.MissingItems.Count == 0));
        }

        /// <inheritdoc/>
        protected override bool Matches(IEnumerable actual) => Matches(actual, out _);

        /// <summary>
        /// Test whether the collection is equivalent to the expected.
        /// </summary>
        /// <typeparam name="TActual">
        /// Actual collection type.
        /// </typeparam>
        /// <param name="actual">
        /// Actual collection to compare.
        /// </param>
        /// <returns>
        /// A <see cref="CollectionEquivalentConstraintResult"/> indicating whether or not
        /// the two collections are equivalent.
        /// </returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            bool matchesResult = Matches(enumerable, out var tallyResult);

            return new CollectionEquivalentConstraintResult(
                this, tallyResult, actual, matchesResult);
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <typeparam name="TActualElement">The type of the elements in the actual value.</typeparam>
        /// <typeparam name="TExpectedElement">The type of the elements in the expected value.</typeparam>
        /// <returns>Self.</returns>
        public CollectionEquivalentConstraint Using<TActualElement, TExpectedElement>(Func<TActualElement, TExpectedElement, bool> comparison)
        {
            base.Using(EqualityAdapter.For(comparison));
            return this;
        }
    }
}
