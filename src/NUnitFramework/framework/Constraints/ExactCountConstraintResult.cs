// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Contain the result of matching a <see cref="ExactCountConstraint"/> against an actual value.
    /// </summary>
    internal sealed class ExactCountConstraintResult : ConstraintResult
    {
        /// <summary>
        /// The count of matched items of the <see cref="ExactCountConstraint"/>
        /// </summary>
        private readonly int _matchCount;

        /// <summary>
        /// A list with maximum count (+1) of items of the <see cref="ExactCountConstraint"/>
        /// </summary>
        private readonly ICollection<object?> _itemList;

        /// <summary>
        /// Constructs a <see cref="ConstraintResult"/> for a <see cref="ExactCountConstraint"/>.
        /// </summary>
        /// <param name="constraint">The Constraint to which this result applies.</param>
        /// <param name="actualValue">The actual value to which the Constraint was applied.</param>
        /// <param name="isSuccess">If true, applies a status of Success to the result, otherwise Failure.</param>
        /// <param name="matchCount">Count of matched items of the <see cref="ExactCountConstraint"/></param>
        /// <param name="itemList">A list with maximum count (+1) of items of the <see cref="ExactCountConstraint"/></param>
        internal ExactCountConstraintResult(IConstraint constraint, object? actualValue, bool isSuccess, int matchCount, ICollection<object?> itemList)
            : base(constraint, actualValue, isSuccess)
        {
            _matchCount = matchCount;
            _itemList = itemList;
        }

        /// <summary>
        /// Write the actual value for a failing constraint test to a MessageWriter.
        /// </summary>
        /// <param name="writer">The writer on which the actual value is displayed</param>
        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (_itemList is null || _itemList.Count == 0)
            {
                writer.Write("no matching items");
                return;
            }

            writer.Write(_matchCount != 1 ? "{0} matching items " : "{0} matching item ", _matchCount);
            writer.Write(MsgUtils.FormatCollection(_itemList));
        }
    }
}
