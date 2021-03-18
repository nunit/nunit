// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ReusableConstraint wraps a constraint expression after
    /// resolving it so that it can be reused consistently.
    /// </summary>
    public class ReusableConstraint : IResolveConstraint
    {
        private readonly IConstraint constraint;

        /// <summary>
        /// Construct a ReusableConstraint from a constraint expression
        /// </summary>
        /// <param name="c">The expression to be resolved and reused</param>
        public ReusableConstraint(IResolveConstraint c)
        {
            this.constraint = c.Resolve();
        }

        /// <summary>
        /// Converts a constraint to a ReusableConstraint
        /// </summary>
        /// <param name="c">The constraint to be converted</param>
        /// <returns>A ReusableConstraint</returns>
        public static implicit operator ReusableConstraint(Constraint c)
        {
            return new ReusableConstraint(c);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return constraint.ToString();
        }

        #region IResolveConstraint Members

        /// <summary>
        /// Return the top-level constraint for this expression
        /// </summary>
        /// <returns></returns>
        public IConstraint Resolve()
        {
            return constraint;
        }

        #endregion
    }
}
