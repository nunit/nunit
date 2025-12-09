// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AssignableFromConstraint is used to test that an object
    /// can be assigned from an instance of a given Type.
    /// </summary>
    public class AssignableFromConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an AssignableFromConstraint for the type provided
        /// </summary>
        /// <param name="type"></param>
        public AssignableFromConstraint(Type type) : base(type, "assignable from ")
        {
        }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object? actual)
        {
            if (expectedType is null || actual is null)
            {
                return false;
            }

            if (expectedType == actualType)
            {
                return true;
            }

            if (actual.GetType().IsAssignableFrom(expectedType))
            {
                return true;
            }

            return Reflect.CanImplicitlyConvertTo(expectedType, actual.GetType());
        }
    }
}
