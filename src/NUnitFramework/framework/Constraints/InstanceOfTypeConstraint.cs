// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// InstanceOfTypeConstraint is used to test that an object
    /// is of the same type provided or derived from it.
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class InstanceOfTypeConstraint<TExpected> : InstanceOfTypeConstraint
    {
        /// <summary>
        /// Construct an InstanceOfTypeConstraint for the type provided
        /// </summary>
        public InstanceOfTypeConstraint()
            : base(typeof(TExpected))
        {
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

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object? actual)
        {
            return actual is not null && expectedType.IsInstanceOfType(actual);
        }
    }
}
