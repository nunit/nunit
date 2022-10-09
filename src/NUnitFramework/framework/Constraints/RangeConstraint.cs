// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// RangeConstraint tests whether two values are within a 
    /// specified range.
    /// </summary>
    public class RangeConstraint : Constraint
    {
        private readonly object from;
        private readonly object to;

        private ComparisonAdapter comparer = ComparisonAdapter.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeConstraint"/> class.
        /// </summary>
        /// <param name="from">Inclusive beginning of the range.</param>
        /// <param name="to">Inclusive end of the range.</param>
        public RangeConstraint(object from, object to) : base(from, to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return $"in range ({from},{to})"; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if ( from == null || to == null || actual == null)
                throw new ArgumentException( "Cannot compare using a null reference", nameof(actual) );
            CompareFromAndTo();
            bool isInsideRange = comparer.Compare(from, actual) <= 0 && comparer.Compare(to, actual) >= 0;
            return new ConstraintResult(this, actual, isInsideRange);
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public RangeConstraint Using(IComparer comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(IComparer<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(Comparison<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        private void CompareFromAndTo()
        {
            if (comparer.Compare(from, to) > 0)
                throw new ArgumentException("The from value must be less than or equal to the to value.");
        }
    }
}
