// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Extension methods for <see cref="EqualNumericConstraint{T}"/>.
    /// </summary>
    public static class EqualNumericConstraintExtensions
    {
        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="constraint">The original constraint.</param>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Original constraint promoted to <see cref="double"/>.</returns>
        public static EqualNumericConstraint<double> Within(this EqualNumericConstraint<int> constraint, double amount)
        {
            return new EqualNumericConstraint<double>(constraint.Expected).Within(amount);
        }

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="constraint">The original constraint.</param>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Original constraint promoted to <see cref="double"/>.</returns>
        public static EqualNumericConstraint<double> Within(this EqualNumericConstraint<float> constraint, double amount)
        {
            return new EqualNumericConstraint<double>(constraint.Expected).Within(amount);
        }
    }
}
