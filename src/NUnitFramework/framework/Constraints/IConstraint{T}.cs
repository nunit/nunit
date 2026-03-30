// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for all constraints
    /// </summary>
    public interface IConstraint<T> : IConstraintInfo
    {
        #region Methods

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyTo(T actual);

        /// <summary>
        /// Applies the constraint to a Func that returns
        /// the value to be tested. The default implementation simply evaluates
        /// the delegate but derived classes may override it to provide for
        /// delayed processing.
        /// </summary>
        /// <param name="del">A Func</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyTo(Func<T> del);

        #endregion
    }
}
