// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExactCountConstraint applies another constraint to each
    /// item in a collection, succeeding only if a specified
    /// number of items succeed.
    /// </summary>
    public class ExactCountConstraint : Constraint
    {
        private readonly int _expectedCount;
        private readonly IConstraint? _itemConstraint;

        /// <summary>
        /// Construct a standalone ExactCountConstraint
        /// </summary>
        /// <param name="expectedCount"></param>
        public ExactCountConstraint(int expectedCount)
        {
            _expectedCount = expectedCount;
        }

        /// <summary>
        /// Construct an ExactCountConstraint on top of an existing constraint
        /// </summary>
        /// <param name="expectedCount"></param>
        /// <param name="itemConstraint"></param>
        public ExactCountConstraint(int expectedCount, IConstraint itemConstraint)
            : base(itemConstraint)
        {
            Guard.ArgumentNotNull(itemConstraint, nameof(itemConstraint));

            _itemConstraint = itemConstraint.Resolve();
            _expectedCount = expectedCount;
        }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// succeeding only if the expected number of items pass.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));
            var itemList = new Collection<object>();
            var matchCount = 0;

            foreach (var item in enumerable)
            {
                if (_itemConstraint is not null)
                {
                    if (_itemConstraint.ApplyTo(item).IsSuccess)
                        matchCount++;
                }
                else
                {
                    matchCount++;
                }

                // We intentionally add one item too many because we use it to trigger
                // the ellipsis when we call "MsgUtils.FormatCollection" later on.
                if (itemList.Count <= MsgUtils.DefaultMaxItems)
                    itemList.Add(item);
            }

            return new ExactCountConstraintResult(this, actual, matchCount == _expectedCount, matchCount, itemList);
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var descriptionPrefix =
                    _expectedCount == 0 ? "no item" :
                    _expectedCount == 1 ? "exactly one item" :
                    $"exactly {_expectedCount} items";

                return _itemConstraint is not null ? PrefixConstraint.FormatDescription(descriptionPrefix, _itemConstraint) : descriptionPrefix;
            }
        }
    }
}
