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
        private Constraint realConstraint;

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return realConstraint == null ? "<empty>" : realConstraint.Description; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // NOTE: actual is string will fail for a null typed as string
            Type actualType = typeof(TActual);
            
            if (actualType == typeof(string))
                realConstraint = new EmptyStringConstraint();
            else if (actual is Guid || actualType == typeof(Guid?))
                realConstraint = new EmptyGuidConstraint();
            else if (actual == null)
                throw new System.ArgumentException($"The actual value must be a string, Guid, non-null IEnumerable or DirectoryInfo. The value passed was of type {actualType}.", nameof(actual));
            else if (actual is System.IO.DirectoryInfo)
                realConstraint = new EmptyDirectoryConstraint();
            else
                realConstraint = new EmptyCollectionConstraint();

            return realConstraint.ApplyTo(actual);
        }
    }
}
