// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for all constraints
    /// </summary>
    public interface IConstraint : IConstraintInfo, IResolveConstraint
    {
        #region Properties

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        ConstraintBuilder? Builder { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyTo<TActual>(TActual actual);

        /// <summary>
        /// Applies the constraint to an ActualValueDelegate that returns
        /// the value to be tested. The default implementation simply evaluates
        /// the delegate but derived classes may override it to provide for
        /// delayed processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);

        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        [Obsolete("This was never implemented and will be removed.")]
        ConstraintResult ApplyTo<TActual>(ref TActual actual);

        #endregion
    }
}
