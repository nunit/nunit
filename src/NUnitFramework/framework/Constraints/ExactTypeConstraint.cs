// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExactTypeConstraint is used to test that an object
    /// is of the exact type provided in the constructor
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class ExactTypeConstraint<TExpected> : ExactTypeConstraint
    {
        /// <summary>
        /// Construct an ExactTypeConstraint for a given Type
        /// </summary>
        public ExactTypeConstraint()
            : base(typeof(TExpected))
        {
        }
    }

    /// <summary>
    /// ExactTypeConstraint is used to test that an object
    /// is of the exact type provided in the constructor
    /// </summary>
    public class ExactTypeConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an ExactTypeConstraint for a given Type
        /// </summary>
        /// <param name="type">The expected Type.</param>
        public ExactTypeConstraint(Type type)
            : base(type, string.Empty)
        {
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "TypeOf";

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object? actual)
        {
            return actual is not null && actual.GetType() == expectedType;
        }
    }
}
