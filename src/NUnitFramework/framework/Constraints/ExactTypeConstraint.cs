// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExactTypeConstraint is used to test that an object
    /// is of the exact type provided in the constructor
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public class ExactTypeConstraint<TExpected> : TypeConstraint<TExpected>
    {
        /// <summary>
        /// Construct an ExactTypeConstraint for a given Type
        /// </summary>
        public ExactTypeConstraint()
            : base(string.Empty)
        {
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "TypeOf";

        /// <inheritdoc />
        protected override bool Matches<TActual>(TActual actual)
        {
            return actual?.GetType() == typeof(TExpected);
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

        /// <inheritdoc />
        protected override bool Matches(object? actual)
        {
            return actualType == expectedType;
        }
    }
}
