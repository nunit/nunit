// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualDateTimeOffsetConstraint : EqualTimeBaseConstraint<DateTimeOffset>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualDateTimeOffsetConstraint(DateTimeOffset expected)
            : base(expected, x => x.UtcTicks)
        {
        }

        #endregion

        #region Constraint Modifiers

        /// <summary>
        /// Flags the constraint to include <see cref="DateTimeOffset.Offset"/>
        /// property in comparison of two <see cref="DateTimeOffset"/> values.
        /// </summary>
        /// <remarks>
        /// Using this modifier does not allow to use the <see cref="EqualConstraint.Within"/>
        /// constraint modifier.
        /// </remarks>
        public EqualDateTimeOffsetConstraintWithSameOffset WithSameOffset
        {
            get
            {
                var constraint = new EqualDateTimeOffsetConstraintWithSameOffset(Expected);
                Builder?.Replace(constraint);
                return constraint;
            }
        }

        #endregion
    }
}
