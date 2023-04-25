// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NotConstraint negates the effect of some other constraint
    /// </summary>
    public class NotConstraint : PrefixConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConstraint"/> class.
        /// </summary>
        /// <param name="baseConstraint">The base constraint to be negated.</param>
        public NotConstraint(IConstraint baseConstraint)
            : base(baseConstraint, "not")
        {
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for if the base constraint fails, false if it succeeds</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var baseResult = BaseConstraint.ApplyTo(actual);
            return new ConstraintResult(this, baseResult.ActualValue, !baseResult.IsSuccess);
        }

        // TODO: May need a special result type
    }
}
