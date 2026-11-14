// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// InstanceOfTypeConstraint is used to test that an object
    /// is of the same type provided or derived from it.
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class InstanceOfTypeConstraint<TExpected> : TypeConstraint<TExpected>
    {
        /// <summary>
        /// Construct an InstanceOfTypeConstraint for the type provided
        /// </summary>
        public InstanceOfTypeConstraint()
            : base("instance of ")
        {
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "InstanceOf";

        /// <inheritdoc />
        protected override bool Matches<TActual>(TActual actual)
        {
            return typeof(TExpected).IsInstanceOfType(actual)
                || (UnionHelper.TryGetUnionValue(actual, out var unionValue) && typeof(TExpected).IsInstanceOfType(unionValue));
        }
    }

    /// <summary>
    /// InstanceOfTypeConstraint is used to test that an object
    /// is of the same type provided or derived from it.
    /// </summary>
    public class InstanceOfTypeConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an InstanceOfTypeConstraint for the type provided
        /// </summary>
        /// <param name="type">The expected Type</param>
        public InstanceOfTypeConstraint(Type type)
            : base(type, "instance of ")
        {
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "InstanceOf";

        /// <inheritdoc />
        protected override bool Matches(object? actual)
        {
            if (expectedType is null)
                return actual is null;

            return expectedType.IsInstanceOfType(actual)
                || (UnionHelper.TryGetUnionValue(actual, out var unionValue) && expectedType.IsInstanceOfType(unionValue));
        }
    }
}
