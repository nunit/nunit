// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests
{
    internal static class ConstraintAssert
    {
        /// <summary>
        /// Validates that a constraint will fail and has the expected failure message.
        /// </summary>
        /// <typeparam name="TActual">The type of the actual value.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <param name="constraint">The constraint to validate.</param>
        /// <param name="errorConstraints">Validations to apply to the constraint.</param>
        public static void MatchesFailureMessage<TActual>(TActual actual, IConstraint constraint, params Constraint[] errorConstraints)
        {
            using var scope = Assert.EnterMultipleScope();

            var message = constraint.ApplyTo(actual).GetFailureMessage();
            foreach (var errorConstraint in errorConstraints)
            {
                Assert.That(message, errorConstraint);
            }
        }
    }
}
