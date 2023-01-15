// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Abstract base class for constraints that compare values to
    /// determine if one is greater than, equal to or less than
    /// the other.
    /// </summary>
    public abstract class ComparisonConstraint : Constraint
    {
        /// <summary>
        /// The value against which a comparison is to be made
        /// </summary>
        private readonly object _expected;

        /// <summary>
        /// Tolerance used in making the comparison
        /// </summary>
        private Tolerance _tolerance = Tolerance.Default;

        /// <summary>
        /// ComparisonAdapter to be used in making the comparison
        /// </summary>
        private ComparisonAdapter _comparer = ComparisonAdapter.Default;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonConstraint"/> class.
        /// </summary>
        /// <param name="expected">The value against which to make a comparison.</param>
        protected ComparisonConstraint(object expected) : base(expected)
        {
            Guard.ArgumentValid(expected != null, "Cannot compare using a null reference.", nameof(_expected));
            _expected = expected;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentValid(actual != null, "Cannot compare to a null reference.", nameof(actual));

            return new ConstraintResult(this, actual, PerformComparison(_comparer, actual, _expected, _tolerance));
        }

        /// <summary>
        /// Protected function overridden by derived class to actually perform the comparison
        /// </summary>
        protected abstract bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);

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

        /// <summary>
        /// Set the tolerance for use in this comparison
        /// </summary>
        public ComparisonConstraint Within(object amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            _tolerance = new Tolerance(amount);
            return this;
        }

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a percentage that the actual values is allowed to deviate from
        /// the expected value.
        /// </summary>
        /// <returns>Self</returns>
        public ComparisonConstraint Percent
        {
            get
            {
                _tolerance = _tolerance.Percent;
                return this;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Provides standard description of what the constraint tests
        /// based on comparison text.
        /// </summary>
        /// <param name="comparisonText">Describes the comparison being tested, throws <see cref="ArgumentNullException"/>
        /// if null</param>
        /// <exception cref="ArgumentNullException">Is thrown when null passed to a method</exception>
        protected string DefaultDescription(string comparisonText)
        {
            if (comparisonText == null)
                throw new ArgumentNullException(nameof(comparisonText), "Comparison text can not be null");
            
            StringBuilder sb = new StringBuilder(comparisonText);
            sb.Append(MsgUtils.FormatValue(_expected));
                
            if (_tolerance != null && !_tolerance.IsUnsetOrDefault)
            {
                sb.Append(" within ");
                sb.Append(MsgUtils.FormatValue(_tolerance.Amount));

                if (_tolerance.Mode == ToleranceMode.Percent)
                {
                    sb.Append(" percent");
                }
            }
                
            return sb.ToString();
        }

        #endregion
    }
}
