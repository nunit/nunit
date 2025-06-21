// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;

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
            if (actual is null)
            {
                // Null is always considered the default value.
                return new ConstraintResult(this, actual, true);
            }

            // We ignore TActual if it is object as that type is used a lot in non-generic contexts.
            Type actualType = typeof(TActual) == typeof(object) ? actual.GetType() : typeof(TActual);
            if (!actualType.IsValueType)
            {
                // For reference types, as actual is not null, it cannot be default.
                return new ConstraintResult(this, actual, false);
            }

            object? defaultValue = typeof(TActual) == typeof(object)
                ? Activator.CreateInstance(actualType) // Create a default instance of the actual type.
                : default(TActual);

            var isDefault = RuntimeHelpers.Equals(actual, defaultValue);
            return new ConstraintResult(this, actual, isDefault);
        }
    }
}
