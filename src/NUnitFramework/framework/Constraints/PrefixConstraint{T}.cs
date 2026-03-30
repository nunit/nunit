// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Abstract base class used for prefixes
    /// </summary>
    public abstract class PrefixConstraint<T> : Constraint<T>
    {
        /// <summary>
        /// The base constraint
        /// </summary>
        protected IConstraint<T> BaseConstraint { get; }

        /// <summary>
        /// Prefix used in forming the constraint description
        /// </summary>
        protected string DescriptionPrefix { get; }

        /// <summary>
        /// Construct given a base constraint
        /// </summary>
        /// <param name="baseConstraint"></param>
        /// <param name="descriptionPrefix">Prefix used in forming the constraint description</param>
        protected PrefixConstraint(IConstraint<T> baseConstraint, string descriptionPrefix)
            : base(baseConstraint)
        {
            ArgumentNullException.ThrowIfNull(baseConstraint);

            BaseConstraint = baseConstraint;
            DescriptionPrefix = descriptionPrefix;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => FormatDescription(DescriptionPrefix, BaseConstraint);

        /// <summary>
        /// Formats a prefix constraint's description.
        /// </summary>
        internal static string FormatDescription(string descriptionPrefix, IConstraintInfo baseConstraint)
        {
            return string.Format(
                baseConstraint is EqualConstraint ? "{0} equal to {1}" : "{0} {1}",
                descriptionPrefix,
                baseConstraint.Description);
        }
    }
}
