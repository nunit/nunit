// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// RangeConstraint tests whether two values are within a
    /// specified range.
    /// </summary>
    public class RangeConstraint : Constraint
    {
        private readonly object _from;
        private readonly object _to;

        private ComparisonAdapter _comparer = ComparisonAdapter.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeConstraint"/> class.
        /// </summary>
        /// <param name="from">Inclusive beginning of the range.</param>
        /// <param name="to">Inclusive end of the range.</param>
        public RangeConstraint(object from, object to) : base(from, to)
        {
            _from = from;
            _to = to;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => $"in range ({_from},{_to})";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if ( _from is null || _to is null || actual is null)
                throw new ArgumentException( "Cannot compare using a null reference", nameof(actual) );
            CompareFromAndTo();
            bool isInsideRange = _comparer.Compare(_from, actual) <= 0 && _comparer.Compare(_to, actual) >= 0;
            return new ConstraintResult(this, actual, isInsideRange);
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public RangeConstraint Using(IComparer comparer)
        {
            _comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(Comparison<T> comparer)
        {
            _comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        private void CompareFromAndTo()
        {
            if (_comparer.Compare(_from, _to) > 0)
                throw new ArgumentException("The from value must be less than or equal to the to value.");
        }
    }
}
