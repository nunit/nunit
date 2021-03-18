// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NullConstraint tests that the actual value is null
    /// </summary>
    public class NullConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullConstraint"/> class.
        /// </summary>
        public NullConstraint()
        {
            this.Description = "null";
        }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, actual == null);
        }
    }
}