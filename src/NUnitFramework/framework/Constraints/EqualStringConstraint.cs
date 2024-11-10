// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualStringConstraint : EqualStringWithoutUsingConstraint, IEqualWithUsingConstraint<string?>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualStringConstraint(string? expected)
            : base(expected)
        {
        }

        #endregion
    }
}
