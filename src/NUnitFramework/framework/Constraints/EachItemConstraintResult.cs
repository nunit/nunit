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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Provides a <see cref="ConstraintResult"/> for the constraints 
    /// that are applied to each item in the collection
    /// </summary>
    internal sealed class EachItemConstraintResult : ConstraintResult
    {
        private readonly object _nonMatchingItem;
        private readonly int _nonMatchingItemIndex;

        /// <summary>
        /// Constructs a <see cref="EachItemConstraintResult" /> for a particular <see cref="Constraint" />
        /// Only used for Failure
        /// </summary>
        /// <param name="constraint">The Constraint to which this result applies</param>
        /// <param name="actualValue">The actual value to which the Constraint was applied</param>
        /// <param name="nonMatchingItem">Actual item that does not match expected condition</param>
        /// <param name="nonMatchingIndex">Non matching item index</param>
        public EachItemConstraintResult(IConstraint constraint, object actualValue, object nonMatchingItem, int nonMatchingIndex)
            : base(constraint, actualValue, false)
        {
            _nonMatchingItem = nonMatchingItem;
            _nonMatchingItemIndex = nonMatchingIndex;
        }

        /// <summary>
        /// Write constraint description, actual items, and non-matching item
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display the message</param>
        public override void WriteAdditionalLinesTo(MessageWriter writer)
        {
            var nonMatchingStr = $"  First non-matching item at index [{_nonMatchingItemIndex}]:  "
                + MsgUtils.FormatValue(_nonMatchingItem);
            writer.WriteLine(nonMatchingStr);
        }
    }
}
