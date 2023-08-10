// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CountZeroConstraint tests whether an instance has a property .Count with value zero.
    /// </summary>
    public class CountZeroConstraint : Constraint
    {
        private const string CountPropertyName = "Count";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "<empty>";

        /// <summary>
        /// Checks if the specified <paramref name="type"/> has a int Count property.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><see langword="true"/> when <paramref name="type"/> has a 'int Count' property, <see langword="false"/> otherwise.</returns>
        public static bool HasCountProperty(Type type) => type.GetProperty(CountPropertyName)?.PropertyType == typeof(int);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            PropertyInfo? countProperty = actual?.GetType().GetProperty(CountPropertyName);
            int? countValue = (int?)countProperty?.GetValue(actual, null);
            return new ConstraintResult(this, actual, countValue == 0);
        }
    }
}
