// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AssignableFromConstraint is used to test that an object
    /// can be assigned from an instance of a given Type.
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class AssignableFromConstraint<TExpected> : TypeConstraint<TExpected>
    {
        /// <summary>
        /// Construct an AssignableFromConstraint for the type provided
        /// </summary>
        public AssignableFromConstraint() : base("assignable from ")
        {
        }

        /// <inheritdoc />
        protected override bool Matches<TActual>(TActual actual)
        {
            return typeof(TExpected).CanImplicitlyConvertTo(GetActualType(actual)!);
        }
    }

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
        public AssignableFromConstraint(Type type) : base(type, "assignable from ")
        {
        }

        /// <inheritdoc />
        protected override bool Matches(object? actual)
        {
            return actualType is null ? expectedType is null : expectedType.CanImplicitlyConvertTo(actualType);
        }
    }
}
