namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Provides a <see cref="ConstraintResult"/> for the constraints 
    /// that are applied to each item in the collection
    /// </summary>
    public class EachItemConstraintResult : ConstraintResult
    {
        private readonly object _nonMatchingItem;
        private readonly int _nonMatchingItemIndex;

        /// <summary>
        /// Constructs a <see cref="EachItemConstraintResult" /> for a particular <see cref="Constraint" />.
        /// Only used for Failure
        /// </summary>
        /// <param name="constraint">The Constraint to which this result applies.</param>
        /// <param name="actualValue">The actual value to which the Constraint was applied.</param>
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
        public override void WriteMessageTo(MessageWriter writer)
        {
            base.WriteMessageTo(writer);

            var nonMatchingStr = $"  Non-matching item at index [{_nonMatchingItemIndex}]:  "
                + MsgUtils.FormatValue(_nonMatchingItem);
            writer.WriteLine(nonMatchingStr);
        }
    }
}
