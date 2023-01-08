// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionSubsetConstraint is used to determine whether
    /// one collection is a subset of another
    /// </summary>
    public class CollectionSubsetConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;
        private List<object>? _extraItems;

        /// <summary>
        /// Construct a CollectionSubsetConstraint
        /// </summary>
        /// <param name="expected">The collection that the actual value is expected to be a subset of</param>
        public CollectionSubsetConstraint(IEnumerable expected) : base(expected)
        {
            _expected = expected;
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "SubsetOf";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "subset of " + MsgUtils.FormatValue(_expected);

        /// <summary>
        /// Test whether the actual collection is a subset of 
        /// the expected collection provided.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            CollectionTally tally = Tally(_expected);
            tally.TryRemove(actual);

            _extraItems = tally.Result.ExtraItems;

            return _extraItems.Count == 0;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            bool matches = Matches(enumerable);
            return new CollectionSubsetConstraintResult(this, actual, matches, _extraItems);
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionSubsetConstraint Using<TSubsetType, TSupersetType>(Func<TSubsetType, TSupersetType, bool> comparison)
        {
            base.Using(EqualityAdapter.For(comparison));
            return this;
        }

        #region Private CollectionSubsetConstraintResult Class

        private sealed class CollectionSubsetConstraintResult : ConstraintResult
        {
            private readonly List<object>? _extraItems;

            public CollectionSubsetConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, List<object>? extraItems)
                : base(constraint, actualValue, isSuccess)
            {
                _extraItems = extraItems;
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (_extraItems?.Count > 0)
                {
                    string extraItemsMessage = "Extra items: ";
                    extraItemsMessage += MsgUtils.FormatCollection(_extraItems);

                    writer.WriteMessageLine(extraItemsMessage);
                }
            }
        }

        #endregion
    }
}
