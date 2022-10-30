// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NaNConstraint tests that the actual value is a double or float NaN
    /// </summary>
    public class NaNConstraint : Constraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "NaN";

        /// <summary>
        /// Test that the actual value is an NaN
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, 
                actual is double d && double.IsNaN(d) ||
                actual is float f && float.IsNaN(f));
        }
    }
}
