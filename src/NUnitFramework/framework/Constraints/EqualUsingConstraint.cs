// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualUsingConstraint where the comparison is done by a user supplied comparer.
    /// </summary>
    public class EqualUsingConstraint<T> : Constraint
    {
        #region Static and Instance Fields

        private readonly T? _expected;

        private readonly Func<T, T, bool>? _comparer;
        private readonly Func<object, object, bool>? _nonTypedComparer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, Func<T, T, bool> comparer)
            : base(expected)
        {
            _expected = expected;
            _comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, Func<object, object, bool> comparer)
            : base(expected)
        {
            _expected = expected;
            _nonTypedComparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        [OverloadResolutionPriority(1)]
        public EqualUsingConstraint(T? expected, IEqualityComparer<T> comparer)
            : this(expected, comparer.Equals)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, IComparer<T> comparer)
            : this(expected, (x, y) => comparer.Compare(x, y) == 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, Comparison<T> comparer)
            : this(expected, (x, y) => comparer.Invoke(x, y) == 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, IEqualityComparer comparer)
            : this(expected, (object x, object y) => comparer.Equals(x, y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="comparer">The comparer to use.</param>
        public EqualUsingConstraint(T? expected, IComparer comparer)
            : this(expected, (object x, object y) => comparer.Compare(x, y) == 0)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public virtual ConstraintResult ApplyTo(T? actual)
        {
            bool hasSucceeded;

            if (actual is null)
            {
                hasSucceeded = _expected is null;
            }
            else if (_expected is null)
            {
                hasSucceeded = false;
            }
            else if (_comparer is not null)
            {
                hasSucceeded = _comparer.Invoke(actual, _expected);
            }
            else if (_nonTypedComparer is not null)
            {
                hasSucceeded = _nonTypedComparer.Invoke(actual, _expected);
            }
            else
            {
                hasSucceeded = false;
            }

            return ConstraintResult(actual, hasSucceeded);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// I wish we could hide this method, but it is public in the base class.
        /// </remarks>
        public sealed override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool hasSucceeded;

            if (actual is null)
            {
                hasSucceeded = _expected is null;
            }
            else if (_expected is null)
            {
                hasSucceeded = false;
            }
            else if (_nonTypedComparer is not null)
            {
                hasSucceeded = _nonTypedComparer.Invoke(actual, _expected);
            }
            else if (_comparer is not null && actual is T t)
            {
                hasSucceeded = _comparer.Invoke(t, _expected);
            }
            else
            {
                hasSucceeded = false;
            }

            return ConstraintResult(actual, hasSucceeded);
        }

        private ConstraintResult ConstraintResult<TActual>(TActual actual, bool hasSucceeded)
        {
            return new EqualConstraintResult(this, actual, hasSucceeded);
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var sb = new StringBuilder(MsgUtils.FormatValue(_expected));

                if (_comparer is not null)
                    sb.Append(", using strongly typed comparer");

                if (_nonTypedComparer is not null)
                    sb.Append(", using untyped comparer");

                return sb.ToString();
            }
        }

        #endregion
    }
}
