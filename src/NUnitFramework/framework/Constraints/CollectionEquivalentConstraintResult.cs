// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt


namespace NUnit.Framework.Constraints
{
    /// <summary>Provides a <see cref="ConstraintResult"/> for the <see cref="CollectionEquivalentConstraint"/>.</summary>
    public class CollectionEquivalentConstraintResult : ConstraintResult
    {
        /// <summary>Result of a <see cref="CollectionTally"/> of the collections to compare for equivalence.</summary>
        private readonly CollectionTally.CollectionTallyResult _tallyResult;

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
                missingStr += MsgUtils.FormatCollection(_tallyResult.MissingItems);

                writer.WriteMessageLine(missingStr);
            }

            if (_tallyResult.ExtraItems.Count > 0)
            {
                int extraItemsCount = _tallyResult.ExtraItems.Count;

                string extraStr = $"Extra ({extraItemsCount}): ";
                extraStr += MsgUtils.FormatCollection(_tallyResult.ExtraItems);

                writer.WriteMessageLine(extraStr);
            }
        }
    }
}
