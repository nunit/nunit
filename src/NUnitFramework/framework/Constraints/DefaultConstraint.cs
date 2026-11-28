// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Tests that the actual value is default value for type.
    /// </summary>
    public class DefaultConstraint : Constraint
    {
        /// <inheritdoc/>
        public override string Description => "default";

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool isDefault = UnionHelper.TryGetUnionValue(actual, out var unionValue)
                ? unionValue is null
                : RuntimeHelpers.Equals(actual, default(TActual));

            return new ConstraintResult(this, actual, isDefault);
        }
    }
}
