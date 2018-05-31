// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
    /// <summary>Provides a <see cref="ConstraintResult"/> for the <see cref="CollectionEquivalentConstraint"/>.</summary>
    public class CollectionEquivalentConstraintResult : ConstraintResult
    {
        /// <summary>Result of a <see cref="CollectionTally"/> of the collections to compare for equivalence.</summary>
        private readonly CollectionTally.CollectionTallyResult _tallyResult;

        /// <summary>Maximum amount of elements to write to the <see cref="MessageWriter"/> if there are
        /// extra/missing elements from the collection.</summary>
        private const int MaxDifferingElemsToWrite = 10;
        
        /// <summary>Construct a <see cref="CollectionEquivalentConstraintResult"/> using a <see cref="CollectionTally.CollectionTallyResult"/>.</summary>
        /// <param name="constraint">Source <see cref="CollectionEquivalentConstraint"/>.</param>
        /// <param name="tallyResult">Result of the collection comparison.</param>
        /// <param name="actual">Actual collection to compare.</param>
        /// <param name="isSuccess">Whether or not the <see cref="CollectionEquivalentConstraint"/> succeeded.</param>
        public CollectionEquivalentConstraintResult(
            CollectionEquivalentConstraint constraint,
            CollectionTally.CollectionTallyResult tallyResult,
            object actual,
            bool isSuccess)
            : base(constraint, actual, isSuccess)
        {
            Guard.ArgumentNotNull(tallyResult, nameof(tallyResult));

            _tallyResult = tallyResult;
        }

        /// <summary>Write any additional lines (following <c>Expected:</c> and <c>But was:</c>) for a failing constraint.</summary>
        /// <param name="writer">The <see cref="MessageWriter"/> to write the failure message to.</param>
        public override void WriteAdditionalLinesTo(MessageWriter writer)
        { 
            if (_tallyResult.MissingItems.Count > 0)
            {
                int missingItemsCount = _tallyResult.MissingItems.Count;

                string missingStr = $"Missing ({missingItemsCount}): ";
                missingStr += MsgUtils.FormatCollection(_tallyResult.MissingItems, 0, MaxDifferingElemsToWrite);

                writer.WriteMessageLine(missingStr);
            }

            if (_tallyResult.ExtraItems.Count > 0)
            {
                int extraItemsCount = _tallyResult.ExtraItems.Count;

                string extraStr = $"Extra ({extraItemsCount}): ";
                extraStr += MsgUtils.FormatCollection(_tallyResult.ExtraItems, 0, MaxDifferingElemsToWrite);

                writer.WriteMessageLine(extraStr);
            }
        }
    }
}
