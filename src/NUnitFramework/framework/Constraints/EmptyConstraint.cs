// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EmptyConstraint tests a whether a string or collection is empty,
    /// postponing the decision about which test is applied until the
    /// type of the actual argument is known.
    /// </summary>
    public class EmptyConstraint : Constraint
    {
        private Constraint? _realConstraint;

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => _realConstraint is null ? "<empty>" : _realConstraint.Description;

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // NOTE: actual is string will fail for a null typed as string
            Type actualType = actual?.GetType() ?? typeof(TActual);

            if (actualType == typeof(string))
                _realConstraint = new EmptyStringConstraint();
            else if (actual is Guid || actualType == typeof(Guid?))
                _realConstraint = new EmptyGuidConstraint();
            else if (actual is System.IO.DirectoryInfo)
                _realConstraint = new EmptyDirectoryConstraint();
            else if (actual is System.Collections.ICollection)
                _realConstraint = new EmptyCollectionConstraint(); // Uses ICollecion.Count
            else if (actual is System.Collections.IEnumerable) // Enumerates whole collection
                _realConstraint = new EmptyCollectionConstraint();
            else if (actual is not null && CountZeroConstraint.HasCountProperty(actualType)) // For Collections that have Count but are not ICollection
                _realConstraint = new CountZeroConstraint();
            else
                throw new ArgumentException($"The actual value must be not-null, a string, Guid, have an int Count property, IEnumerable or DirectoryInfo. The value passed was of type {actualType}.", nameof(actual));

            return _realConstraint.ApplyTo(actual);
        }
    }
}
