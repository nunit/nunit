// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// TypeConstraint is the abstract base for constraints
    /// that take a Type as their expected value.
    /// </summary>
    public abstract class TypeConstraint : Constraint
    {
        /// <summary>
        /// The expected Type used by the constraint
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected Type expectedType;
#pragma warning restore IDE1006

        /// <summary>
        /// The type of the actual argument to which the constraint was applied
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected Type actualType;
#pragma warning restore IDE1006

        /// <summary>
        /// Construct a TypeConstraint for a given Type
        /// </summary>
        /// <param name="type">The expected type for the constraint</param>
        /// <param name="descriptionPrefix">Prefix used in forming the constraint description</param>
        protected TypeConstraint(Type type, string descriptionPrefix)
            : base(type)
        {
            this.expectedType = type;
            this.Description = descriptionPrefix + MsgUtils.FormatValue(expectedType);
        }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            actualType = actual == null ? null : actual.GetType();

            if (actual is Exception)
                return new ConstraintResult(this, actual, this.Matches(actual));

            return new ConstraintResult(this, actualType, this.Matches(actual));
        }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected abstract bool Matches(object actual);
    }
}
