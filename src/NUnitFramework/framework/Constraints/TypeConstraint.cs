// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// TypeConstraint is the abstract base for constraints
    /// that take a Type as their expected value.
    /// </summary>
    /// <typeparam name="TExpected">The expected Type used by the constraint</typeparam>
    public abstract class TypeConstraint<TExpected> : Constraint
    {
        /// <summary>
        /// Construct a TypeConstraint for a given Type
        /// </summary>
        /// <param name="descriptionPrefix">Prefix used in forming the constraint description</param>
        protected TypeConstraint(string descriptionPrefix)
        {
            Description = descriptionPrefix + MsgUtils.FormatValue(typeof(TExpected));
        }

        /// <inheritdoc/>
        public override string Description { get; }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Type? actualType;
            bool success;
            if (UnionHelper.TryGetUnionValue(actual, out var unionValue))
            {
                actualType = unionValue?.GetType();
                success = Matches(unionValue);
            }
            else
            {
                actualType = GetActualType(actual);
                success = Matches(actual);
            }

            if (actual is Exception)
                return new ConstraintResult(this, actual, success);

            return new ConstraintResult(this, actualType, success);
        }

        private protected static Type? GetActualType<TActual>(TActual? actual)
        {
            return UnionHelper.TryGetUnionValue(actual, out var unionValue) ? unionValue?.GetType() : actual?.GetType();
        }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected abstract bool Matches<TActual>(TActual actual);

        /// <inheritdoc />
        protected override string GetStringRepresentation()
        {
            object[] args = [typeof(TExpected)];
            return base.GetStringRepresentation(args);
        }
    }

    /// <summary>
    /// TypeConstraint is the abstract base for constraints
    /// that take a Type as their expected value.
    /// </summary>
    public abstract class TypeConstraint : Constraint
    {
#pragma warning disable IDE1006
        /// <summary>
        /// The expected Type used by the constraint
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected Type? expectedType;

        /// <summary>
        /// The type of the actual argument to which the constraint was applied
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected Type? actualType;
#pragma warning restore IDE1006

        /// <summary>
        /// Construct a TypeConstraint for a given Type
        /// </summary>
        /// <param name="type">The expected type for the constraint</param>
        /// <param name="descriptionPrefix">Prefix used in forming the constraint description</param>
        protected TypeConstraint(Type? type, string descriptionPrefix)
            : base(type)
        {
            expectedType = type;
            Description = descriptionPrefix + MsgUtils.FormatValue(expectedType);
        }

        /// <inheritdoc/>
        public override string Description { get; }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            object? actualValue;
            if (UnionHelper.TryGetUnionValue(actual, out var unionValue))
            {
                actualType = unionValue?.GetType();
                actualValue = unionValue;
            }
            else
            {
                actualType = actual?.GetType();
                actualValue = actual;
            }

            if (actualValue is Exception)
                return new ConstraintResult(this, actual, Matches(actualValue));

            return new ConstraintResult(this, actualType, Matches(actualValue));
        }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected abstract bool Matches(object? actual);
    }
}
