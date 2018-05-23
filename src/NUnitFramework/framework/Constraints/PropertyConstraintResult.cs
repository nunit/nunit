namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Contains the result of matching a <see cref="PropertyConstraint"/> against an actual value.
    /// </summary>
    internal sealed class PropertyConstraintResult : ConstraintResult
    {
        private readonly ConstraintResult _baseResult;

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="PropertyConstraintResult"/> for a particular <see cref="PropertyConstraint"/>.
        /// </summary>
        /// <param name="constraint">The Constraint to which this result applies.</param>
        /// <param name="baseResult">The base result with actual value to which the Constraint was applied.</param>       
        public PropertyConstraintResult(IConstraint constraint, ConstraintResult baseResult) : base(constraint, baseResult.ActualValue, baseResult.Status)
        {
            _baseResult = baseResult;
        }

        #endregion

        #region Write Methods


        /// <summary>
        /// Write the additional failure message for a failing constraint to a
        /// MessageWriter.
        /// </summary>
        /// <param name="writer">The writer on which the actual value is displayed</param>
        public override void WriteAdditionalLinesTo(MessageWriter writer)
        {
            _baseResult.WriteAdditionalLinesTo(writer);
        }

        #endregion
    }

}
