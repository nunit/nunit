// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Abstract base class for constraints that compare _values to
    /// determine if one is greater than, equal to or less than
    /// the other.
    /// </summary>
    public abstract class ComparisonConstraint : Constraint
    {
        /// <summary>
        /// The value against which a comparison is to be made
        /// </summary>
        private object _expected;

        /// <summary>
        /// ComparisonAdapter to be used in making the comparison
        /// </summary>
        private ComparisonAdapter _comparer = ComparisonAdapter.Default;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonConstraint"/> class.
        /// </summary>
        /// <param name="value">The value against which to make a comparison.</param>
        protected ComparisonConstraint(object value) : base(value)
        {
            _expected = value;
        }

        #endregion

        #region Apply Constraint

        /// <summary>
        /// Test whether the constraint is satisfied by a given value   
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentValid(_expected != null, "Cannot compare using a null reference.", nameof(_expected));
            Guard.ArgumentValid(actual != null, "Cannot compare to a null reference.", nameof(actual));
            Guard.OperationValid(IsSuccess != null, "Internal Error: Derived class didn't set IsSuccess.");

            return new ConstraintResult(this, actual, IsSuccess(_comparer.Compare(actual, _expected)));
        }

        /// <summary>
        /// Protected function assigned by derived class to evaluate comparison result
        /// </summary>
        protected Func<int, bool> IsSuccess;

        #endregion

        #region Constraint Modifiers

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using(IComparer comparer)
        {
            this._comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using<T>(IComparer<T> comparer)
        {
            this._comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using<T>(Comparison<T> comparer)
        {
            this._comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        #endregion
    }
}