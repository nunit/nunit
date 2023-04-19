// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// <see cref="AnyOfConstraint"/> is used to determine whether the value is equal to any of the expected values.
    /// </summary>
    public class AnyOfConstraint : Constraint
    {
        private readonly ICollection _expected;
        private readonly NUnitEqualityComparer _comparer = new NUnitEqualityComparer();

        /// <summary>
        /// Construct a <see cref="AnyOfConstraint"/>
        /// </summary>
        /// <param name="expected">Collection of expected values</param>
        public AnyOfConstraint(ICollection expected) : base(expected)
        {
            Guard.ArgumentNotNull(expected, nameof(expected));
            Guard.ArgumentValid(expected.Count > 0,
                $"{nameof(AnyOfConstraint)} requires non-empty expected collection!", nameof(expected));

            _expected = expected;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "any of " + MsgUtils.FormatValue(_expected);

        /// <inheritdoc/>
        protected override string GetStringRepresentation()
            => GetStringRepresentation(_expected);

        /// <summary>
        /// Test whether item is present in expected collection
        /// </summary>
        /// <typeparam name="TActual">Actual item type</typeparam>
        /// <param name="actual">Actual item</param>
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
        public AnyOfConstraint IgnoreCase
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
        public AnyOfConstraint Using(IComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public AnyOfConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The Comparison object to use.</param>
        public AnyOfConstraint Using<T>(Comparison<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IEqualityComparer object to use.</param>
        public AnyOfConstraint Using(IEqualityComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public AnyOfConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The supplied boolean-returning delegate to use.</param>
        public AnyOfConstraint Using<T>(Func<T, T, bool> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        #endregion
    }
}
