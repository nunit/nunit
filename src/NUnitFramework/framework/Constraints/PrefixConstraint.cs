// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        protected IConstraint BaseConstraint { get; set; }

        /// <summary>
        /// Prefix used in forming the constraint description
        /// </summary>
        protected string DescriptionPrefix { get; set; }

        /// <summary>
        /// Construct given a base constraint
        /// </summary>
        /// <param name="baseConstraint"></param>
        protected PrefixConstraint(IResolveConstraint baseConstraint)
            : base(baseConstraint)
        {
            Guard.ArgumentNotNull(baseConstraint, nameof(baseConstraint));

            BaseConstraint = baseConstraint.Resolve();
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
            return string.Format(
                baseConstraint is EqualConstraint ? "{0} equal to {1}" : "{0} {1}",
                descriptionPrefix,
                baseConstraint.Description);
        }
    }
}
