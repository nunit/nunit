// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Abstract base class used for prefixes
    /// </summary>
    public abstract class PrefixConstraint : Constraint
    {
        /// <summary>
        /// The base constraint
        /// </summary>
        protected IConstraint BaseConstraint { get; }

        /// <summary>
        /// Prefix used in forming the constraint description
        /// </summary>
        protected string DescriptionPrefix { get; }

        /// <summary>
        /// Construct given a base constraint
        /// </summary>
        /// <param name="baseConstraint"></param>
        /// <param name="descriptionPrefix">Prefix used in forming the constraint description</param>
        protected PrefixConstraint(IResolveConstraint baseConstraint, string descriptionPrefix)
            : base(baseConstraint)
        {
            Guard.ArgumentNotNull(baseConstraint, nameof(baseConstraint));

            BaseConstraint = baseConstraint.Resolve();
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
        internal static string FormatDescription(string descriptionPrefix, IConstraint baseConstraint)
        {
            bool isEqualConstraint = baseConstraint is EqualConstraint or EqualStringConstraint;

            if (!isEqualConstraint)
            {
                Type constraintType = baseConstraint.GetType();
                isEqualConstraint = constraintType.IsGenericType && constraintType.GetGenericTypeDefinition() == typeof(EqualNumericConstraint<>);
            }

            return string.Format(
                isEqualConstraint ? "{0} equal to {1}" : "{0} {1}",
                descriptionPrefix,
                baseConstraint.Description);
        }
    }
}
