// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
        private readonly ICollection<object> _itemList;

        /// <summary>
        /// Constructs a <see cref="ConstraintResult"/> for a <see cref="ExactCountConstraint"/>.
        /// </summary>
        /// <param name="constraint">The Constraint to which this result applies.</param>
        /// <param name="actualValue">The actual value to which the Constraint was applied.</param>
        /// <param name="isSuccess">If true, applies a status of Success to the result, otherwise Failure.</param>
        /// <param name="matchCount">Count of matched items of the <see cref="ExactCountConstraint"/></param>
        /// <param name="itemList">A list with maximum count (+1) of items of the <see cref="ExactCountConstraint"/></param>
        internal ExactCountConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, int matchCount, ICollection<object> itemList)
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
            if (_itemList == null || _itemList.Count == 0)
            {
                writer.Write("no items");
                return;
            }

            writer.Write(_matchCount != 1 ? "{0} items " : "{0} item ", _matchCount);
            writer.Write(MsgUtils.FormatCollection(_itemList));
        }
    }
}
