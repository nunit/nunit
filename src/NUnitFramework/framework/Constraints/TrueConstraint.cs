// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// TrueConstraint tests that the actual value is true
    /// </summary>
    public class TrueConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrueConstraint"/> class.
        /// </summary>
        public TrueConstraint()
        {
            Description = "True";
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, true.Equals(actual));
        }
    }
}