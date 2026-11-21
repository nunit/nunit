// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests
{
    public static class ConstraintAssert
    {
        public static void MatchesFailureMessage<TActual>(TActual actual, IConstraint constraint, params Constraint[] errorConstraints)
        {
            var message = constraint.ApplyTo(actual).GetFailureMessage();

            foreach (var errorConstraint in errorConstraints)
            {
                Assert.That(message, errorConstraint);
            }
        }
    }
}
