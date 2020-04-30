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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionSupersetConstraint is used to determine whether
    /// one collection is a superset of another
    /// </summary>
    public class CollectionSupersetConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;
        private List<object>? _missingItems;

        /// <summary>
        /// Construct a CollectionSupersetConstraint
        /// </summary>
        /// <param name="expected">The collection that the actual value is expected to be a superset of</param>
        public CollectionSupersetConstraint(IEnumerable expected)
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
        public override string DisplayName { get { return "SupersetOf"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "superset of " + MsgUtils.FormatValue(_expected); }
        }

        /// <summary>
        /// Test whether the actual collection is a superset of 
        /// the expected collection provided.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            // Create tally from 'actual' collection, and remove '_expected'.
            // ExtraItems from tally would be missing items for '_expected' collection. 
            CollectionTally tally = Tally(actual);
            tally.TryRemove(_expected);

            _missingItems = tally.Result.ExtraItems;

            return _missingItems.Count == 0;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IEnumerable enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            bool matches = Matches(enumerable);
            return new CollectionSupersetConstraintResult(this, actual, matches, _missingItems);
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionSupersetConstraint Using<TSupersetType, TSubsetType>(Func<TSupersetType, TSubsetType, bool> comparison)
        {
            // internal code reverses the expected order of the arguments.
            Func<TSubsetType, TSupersetType, bool> invertedComparison = (actual, expected) => comparison.Invoke(expected, actual);
            base.Using(EqualityAdapter.For(invertedComparison));
            return this;
        }

        #region Private CollectionSupersetConstraintResult Class

        private sealed class CollectionSupersetConstraintResult : ConstraintResult
        {
            private readonly List<object>? _missingItems;

            public CollectionSupersetConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, List<object>? missingItems)
                : base(constraint, actualValue, isSuccess)
            {
                _missingItems = missingItems;
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (_missingItems?.Count > 0)
                {
                    string missingItemsMessage = "Missing items: ";
                    missingItemsMessage += MsgUtils.FormatCollection(_missingItems);

                    writer.WriteMessageLine(missingItemsMessage);
                }
            }
        }

        #endregion
    }
}
