// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionEquivalentConstraint is used to determine whether two
    /// collections are equivalent.
    /// </summary>
    public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;

        /// <summary>The result of the <see cref="CollectionTally"/> from the collections
        /// under comparison.</summary>
        private CollectionTally.CollectionTallyResult _tallyResult;

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
        public override string DisplayName { get { return "Equivalent"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "equivalent to " + MsgUtils.FormatValue(_expected); }
        }

        /// <summary>
        /// Test whether two collections are equivalent
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            CollectionTally ct = Tally(_expected);
            ct.TryRemove(actual);

            //Store the CollectionTallyResult so the comparison between the two collections
            //is only performed once.
            _tallyResult = ct.Result;

            return ((_tallyResult.ExtraItems.Count == 0) && (_tallyResult.MissingItems.Count == 0));
        }

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
            bool matchesResult = Matches(enumerable);

            return new CollectionEquivalentConstraintResult(
                this, _tallyResult, actual, matchesResult);
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionEquivalentConstraint Using<TActual, TExpected>(Func<TActual, TExpected, bool> comparison)
        {
            base.Using(EqualityAdapter.For(comparison));
            return this;
        }
    }
}
