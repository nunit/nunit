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
    /// <summary>
    /// Provides a <see cref="ConstraintResult"/> for the <see cref="CollectionEquivalentConstraint"/>.
    /// </summary>
    /// <remarks>
    /// Assumes that a <see cref="CollectionTally"/> has been created and used to compare the expected
    /// equivalent collections.
    /// </remarks>
    public class CollectionEquivalentConstraintResult : ConstraintResult
    {
        /// <summary>
        /// Preprocessed <see cref="CollectionTally"/> of the collections to compare for equivalence.
        /// </summary>
        private CollectionTally _tally;

        /// <summary>
        /// Maximum amount of elements to write to the <see cref="MessageWriter"/> if there are
        /// extra/missing elements from the collection.
        /// </summary>
        private const int MaxDifferingElemsToWrite = 10;
        
        /// <summary>
        /// Construct a <see cref="CollectionEquivalentConstraintResult"/> with a <see cref="CollectionTally"/>
        /// already applied to the actual collection.
        /// </summary>
        /// <param name="Constraint">
        /// Source <see cref="CollectionEquivalentConstraint"/>.
        /// </param>
        /// <param name="Tally">
        /// Preprocessed <see cref="CollectionTally"/> for the collections to compare.
        /// </param>
        /// <param name="actual">
        /// Collection to compare to the expected one.
        /// </param>
        /// <param name="isSuccess">
        /// Whether or not the <see cref="CollectionEquivalentConstraint"/> succeeded.
        /// </param>
        /// <remarks>
        /// The already processed <see cref="CollectionTally"/> is provided to reduce
        /// unnecessary iterations over the collections to determine equivalence.
        /// </remarks>
        public CollectionEquivalentConstraintResult(
            CollectionEquivalentConstraint Constraint,
            CollectionTally Tally,
            object actual,
            bool isSuccess)
            : base(Constraint, actual, isSuccess)
        {
            if (Tally == null)
            {
                throw new System.ArgumentNullException("Tally was null.");
            }

            _tally = Tally;
        }

        /// <summary>
        /// Write the custom failure message for this object's 
        /// <see cref="CollectionEquivalentConstraint"/>.
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteMessageTo(MessageWriter writer)
        {
            //Write the expected/actual message first.
            base.WriteMessageTo(writer);
            
            if (_tally.Result.MissingItems.Count > 0)
            {
                int missingItemsCount = _tally.Result.MissingItems.Count;

                string missingStr = $"Missing ({missingItemsCount}): ";
                missingStr += MsgUtils.FormatCollection(_tally.Result.MissingItems, 0, MaxDifferingElemsToWrite);

                writer.WriteMessageLine(missingStr);
            }

            if (_tally.Result.ExtraItems.Count > 0)
            {
                int extraItemsCount = _tally.Result.ExtraItems.Count;

                string extraStr = $"Extra ({extraItemsCount}): ";
                extraStr += MsgUtils.FormatCollection(_tally.Result.ExtraItems, 0, MaxDifferingElemsToWrite);

                writer.WriteMessageLine(extraStr);
            }
        }
    }
}
