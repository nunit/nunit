// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EvenConstraint tests that the actual value is an even integer
    /// </summary>
    public class MultipleOfConstraint : Constraint
    {
        private readonly uint _multiple;

        /// <summary>
        /// Initializes an instance of the <see cref="MultipleOfConstraint"/>.
        /// </summary>
        /// <param name="multiple">An integer value greater than zero</param>
        public MultipleOfConstraint(int multiple)
        {
            Guard.ArgumentInRange(multiple > 0, "Multiple must be greater than zero", nameof(multiple));
            _multiple = (uint)multiple;
        }

        /// <inheritdoc/>
        public override string Description => $"MultipleOf({_multiple})";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool result;

            if (actual is int || actual is long || actual is short || actual is byte)
            {
                long value = Convert.ToInt64(actual);
                result = value % _multiple == 0;
            }
            else if (actual is uint || actual is ulong || actual is ushort || actual is byte)
            {
                ulong value = Convert.ToUInt64(actual);
                result = value % _multiple == 0;
            }
            else
            {
                // Should this throw an exception instead?
                result = false;
            }

            return new ConstraintResult(this, actual, result);
        }
    }
}
