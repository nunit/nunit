// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

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

        #region Public Methods

        /// <summary>
        /// Sets the <see cref="StringComparer"/> to be used in the comparison.
        /// </summary>
        /// <param name="comparer">comparer to use for comparing strings.</param>
        /// <returns>
        /// Equal constraint comparing <see cref="IEqualWithUsingConstraint{TExpected}.Expected"/>
        /// with an actual value using the user supplied comparer.
        /// </returns>
        public EqualUsingConstraint<string> Using(StringComparer comparer)
        {
            var constraint = new EqualUsingConstraint<string>(Expected, (IEqualityComparer<string>)comparer)
            {
                Builder = Builder
            };
            constraint.Builder?.Replace(constraint);
            return constraint;
        }

        #endregion
    }
}
