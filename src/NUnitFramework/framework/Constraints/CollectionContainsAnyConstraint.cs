// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionContainsAnyConstraint is used to determine whether
    /// one collection contains any element from another collection
    /// </summary>
    public class CollectionContainsAnyConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;
        private List<object>? _presentItems;

        /// <summary>
        /// Construct a CollectionContainsAnyConstraint
        /// </summary>
        /// <param name="expected">The collection that any element from the actual collection is expected to be in</param>
        public CollectionContainsAnyConstraint(IEnumerable expected)
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
        public override string DisplayName { get { return "ContainsAny"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "contains any from " + MsgUtils.FormatValue(_expected); }
        }

        /// <summary>
        /// Test whether the actual collection contains an element from
        /// the expected collection provided.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            CollectionTally tally = Tally(_expected);
            tally.TryRemove(actual);

            _presentItems = tally.Result.PresentItems;

            return _presentItems.Count > 0;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            bool hasElementPresent = Matches(enumerable);
            return new CollectionContainsAnyConstraintResult(this, actual, hasElementPresent, _presentItems);
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionContainsAnyConstraint Using<TContainsAnyType, TExpectedType>(Func<TContainsAnyType, TExpectedType, bool> comparison)
        {
            // internal code reverses the expected order of the arguments.
            Func<TExpectedType, TContainsAnyType, bool> invertedComparison = (actual, expected) => comparison.Invoke(expected, actual);
            base.Using(EqualityAdapter.For(invertedComparison));
            return this;
        }

        #region Private CollectionContainsAnyConstraintResult Class

        private sealed class CollectionContainsAnyConstraintResult : ConstraintResult
        {
            private readonly List<object>? _presentItems;

            public CollectionContainsAnyConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, List<object>? extraItems)
                : base(constraint, actualValue, isSuccess)
            {
                _presentItems = extraItems;
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (_presentItems?.Count > 0)
                {
                    string presentItemsMessage = "Present items: ";
                    presentItemsMessage += MsgUtils.FormatCollection(_presentItems);

                    writer.WriteMessageLine(presentItemsMessage);
                }
            }
        }

        #endregion
    }
}
