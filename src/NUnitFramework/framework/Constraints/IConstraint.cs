// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for all constraints
    /// </summary>
    public interface IConstraint : IResolveConstraint
    {
        #region Properties

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Arguments provided to this Constraint, for use in
        /// formatting the description.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        ConstraintBuilder Builder { get; set; }

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

        #pragma warning disable 3006
        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyTo<TActual>(ref TActual actual);
        #pragma warning restore 3006

        #endregion
    }
}
