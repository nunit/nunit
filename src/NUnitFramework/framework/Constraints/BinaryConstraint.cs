// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// BinaryConstraint is the abstract base of all constraints
    /// that combine two other constraints in some fashion.
    /// </summary>
    public abstract class BinaryConstraint : Constraint
    {
        /// <summary>
        /// The first constraint being combined
        /// </summary>
        protected IConstraint Left;
        /// <summary>
        /// The second constraint being combined
        /// </summary>
        protected IConstraint Right;

        /// <summary>
        /// Construct a BinaryConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        protected BinaryConstraint(IConstraint left, IConstraint right)
            : base(left, right)
        {
            Guard.ArgumentNotNull(left, nameof(left));
            this.Left = left;

            Guard.ArgumentNotNull(right, nameof(right));
            this.Right = right;
        }
    }
}
