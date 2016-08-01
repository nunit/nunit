using System;

namespace NUnit.Framework.Constraints
{
    internal class ExceptionNotThrownConstraint : Constraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "No Exception to be thrown"; }
        }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var exception = actual as Exception;
            return new ConstraintResult(this, exception, exception == null);
        }
    }
}