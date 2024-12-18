// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualNumericConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public class EqualNumericConstraint<T> : EqualNumericWithoutUsingConstraint<T>, IEqualWithUsingConstraint<T>
        where T : unmanaged, IConvertible, IEquatable<T>
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <remarks>
        /// Marked internal to prevent external instantiation with non-supported types.
        /// </remarks>
        /// <param name="expected">The expected value.</param>
        internal EqualNumericConstraint(T expected)
            : base(expected)
        {
        }

        #endregion
    }
}
