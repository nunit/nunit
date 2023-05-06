// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Predicate constraint wraps a Predicate in a constraint,
    /// returning success if the predicate is true.
    /// </summary>
    public class PredicateConstraint<T> : Constraint
    {
        private readonly Predicate<T> _predicate;

        /// <summary>
        /// Construct a PredicateConstraint from a predicate
        /// </summary>
        public PredicateConstraint(Predicate<T> predicate)
        {
            this._predicate = predicate;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get
            {
                var name = _predicate.GetMethodInfo().Name;
                return name.StartsWith("<", StringComparison.Ordinal)
                    ? "value matching lambda expression"
                    : "value matching " + name;
            }
        }

        /// <summary>
        /// Determines whether the predicate succeeds when applied
        /// to the actual value.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var argument = ConstraintUtils.RequireActual<T>(actual, nameof(actual), allowNull: true);

            return new ConstraintResult(this, actual, _predicate(argument));
        }
    }
}
