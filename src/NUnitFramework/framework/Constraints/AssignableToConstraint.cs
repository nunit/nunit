// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AssignableToConstraint is used to test that an object
    /// can be assigned to an instance of a given Type.
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class AssignableToConstraint<TExpected> : TypeConstraint<TExpected>
    {
        /// <summary>
        /// Construct an AssignableToConstraint for the type provided
        /// </summary>
        public AssignableToConstraint() : base("assignable to ")
        {
        }

        /// <inheritdoc />
        protected override bool Matches<TActual>(TActual actual)
        {
            return typeof(TActual).CanImplicitlyConvertTo(GetActualType(actual)!);
        }
    }

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
        public AssignableToConstraint(Type type) : base(type, "assignable to ")
        {
        }

        /// <inheritdoc />
        protected override bool Matches(object? actual)
        {
            return expectedType is null ? actualType is null : actualType.CanImplicitlyConvertTo(expectedType);
        }
    }
}
