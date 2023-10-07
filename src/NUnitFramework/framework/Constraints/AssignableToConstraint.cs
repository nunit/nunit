// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AssignableToConstraint is used to test that an object
    /// can be assigned to a given Type.
    /// </summary>
    public class AssignableToConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an AssignableToConstraint for the type provided
        /// </summary>
        /// <param name="type"></param>
        public AssignableToConstraint(Type type) : base(type, "assignable to ") { }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object? actual)
        {
            return expectedType is not null && actual is not null && expectedType.IsInstanceOfType(actual);
        }
    }
}
