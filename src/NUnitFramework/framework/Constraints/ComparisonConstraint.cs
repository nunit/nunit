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
        protected object expected;
        /// <summary>
        /// If true, less than returns success
        /// </summary>
        protected bool lessComparisonResult = false;
        /// <summary>
        /// if true, equal returns success
        /// </summary>
        protected bool equalComparisonResult = false;
        /// <summary>
        /// if true, greater than returns success
        /// </summary>
        protected bool greaterComparisonResult = false;

        /// <summary>
        /// ComparisonAdapter to be used in making the comparison
        /// </summary>
        private ComparisonAdapter comparer = ComparisonAdapter.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonConstraint"/> class.
        /// </summary>
        /// <param name="value">The value against which to make a comparison.</param>
        /// <param name="lessComparisonResult">if set to <c>true</c> less succeeds.</param>
        /// <param name="equalComparisonResult">if set to <c>true</c> equal succeeds.</param>
        /// <param name="greaterComparisonResult">if set to <c>true</c> greater succeeds.</param>
        /// <param name="predicate">String used in describing the constraint.</param>
        protected ComparisonConstraint(object value, bool lessComparisonResult, bool equalComparisonResult, bool greaterComparisonResult, string predicate)
            : base(value)
        {
            this.expected = value;
            this.lessComparisonResult = lessComparisonResult;
            this.equalComparisonResult = equalComparisonResult;
            this.greaterComparisonResult = greaterComparisonResult;
            this.Description = predicate + " " + MsgUtils.FormatValue(expected);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (expected == null)
                throw new ArgumentException("Cannot compare using a null reference", "expected");

            if (actual == null)
                throw new ArgumentException("Cannot compare to null reference", "actual");

            int icomp = comparer.Compare(expected, actual);

            bool hasSucceeded = icomp < 0 && greaterComparisonResult || icomp == 0 && equalComparisonResult || icomp > 0 && lessComparisonResult;
            return new ConstraintResult(this, actual, hasSucceeded);
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using(IComparer comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using<T>(IComparer<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self
        /// </summary>
        /// <param name="comparer">The comparer used for comparison tests</param>
        /// <returns>A constraint modified to use the given comparer</returns>
        public ComparisonConstraint Using<T>(Comparison<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }
    }
}