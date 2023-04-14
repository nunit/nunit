// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AssignableFromConstraint is used to test that an object
    /// can be assigned from a given Type.
    /// </summary>
    public class AssignableFromConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an AssignableFromConstraint for the type provided
        /// </summary>
        /// <param name="type"></param>
        public AssignableFromConstraint(Type type) : base(type, "assignable from ") { }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object? actual)
        {
            return actual != null && actual.GetType().IsAssignableFrom(expectedType);
        }
    }
}
