// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
    /// OneOfConstraint is used to determine whether item is present in a collection
    /// </summary>
    public class OneOfConstraint : Constraint
    {
        private readonly IEnumerable _expected;

        /// <summary>
        /// The NUnitEqualityComparer in use for this constraint
        /// </summary>
        private readonly NUnitEqualityComparer _comparer = NUnitEqualityComparer.Default;

        /// <summary>
        /// Construct a OneOfConstraint
        /// </summary>
        /// <param name="expected">Expected collection</param>
        public OneOfConstraint(IEnumerable expected)
        {
            _expected = expected;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                return "one of " + MsgUtils.FormatValue(_expected);
            }
        }

        /// <summary>
        /// Test whether item is present in expected collection
        /// </summary>
        /// <typeparam name="TActual">Actual item type</typeparam>
        /// <param name="actual">Actual item</param>
        /// <returns>A <see cref="ConstraintResult"/> indicating whether item is present 
        /// in expected collection.</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var tolerance = Tolerance.Default;
            foreach (var item in _expected)
            {
                if (_comparer.AreEqual(actual, item, ref tolerance))
                {
                    return new ConstraintResult(this, actual, ConstraintStatus.Success);
                }
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }

        #region Modifiers

        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public OneOfConstraint IgnoreCase
        {
            get
            {
                _comparer.IgnoreCase = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public OneOfConstraint Using(IComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public OneOfConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The Comparison object to use.</param>
        /// <returns>Self.</returns>
        public OneOfConstraint Using<T>(Comparison<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IEqualityComparer object to use.</param>
        /// <returns>Self.</returns>
        public OneOfConstraint Using(IEqualityComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public OneOfConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The supplied boolean-returning delegate to use.</param>
        public OneOfConstraint Using<T>(Func<T, T, bool> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        internal OneOfConstraint Using(EqualityAdapter adapter)
        {
            _comparer.ExternalComparers.Add(adapter);
            return this;
        }

        #endregion
    }
}
