// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// FalseConstraint tests that the actual value is false
    /// </summary>
    public class FalseConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FalseConstraint"/> class.
        /// </summary>
        public FalseConstraint()
        {
            this.Description = "False";
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, false.Equals(actual));
        }
    }
}