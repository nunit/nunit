// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Globalization;
using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// StringConstraint is the abstract base for constraints
    /// that operate on strings. It supports the IgnoreCase
    /// modifier for string operations.
    /// </summary>
    public abstract class StringConstraint : Constraint
    {
        /// <summary>
        /// The expected value
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected readonly string expected;
#pragma warning restore IDE1006

        /// <summary>
        /// Indicates whether tests should be case-insensitive
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected bool caseInsensitive;
#pragma warning restore IDE1006

        /// <summary>
        /// Description of this constraint
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected string descriptionText = string.Empty;
#pragma warning restore IDE1006

        /// <summary>
        /// The comparison type to use for string comparisons
        /// </summary>
        private StringComparison? _comparisonType;

        /// <summary>
        /// The culture info to use for string comparisons
        /// </summary>
        private CultureInfo? _cultureInfo;

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                string desc = $"{descriptionText} {MsgUtils.FormatValue(expected)}";
                if (caseInsensitive)
                    desc += ", ignoring case";
                if (_comparisonType is not null)
                    desc += $", with comparison type {_comparisonType}";
                if (_cultureInfo is not null)
                    desc += $", with culture: {_cultureInfo.Name}";
                return desc;
            }
        }

        /// <summary>
        /// Constructs a StringConstraint without an expected value
        /// </summary>
        protected StringConstraint()
        {
            expected = string.Empty;
        }

        /// <summary>
        /// Constructs a StringConstraint given an expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        protected StringConstraint(string expected)
            : base(expected)
        {
            this.expected = expected;
        }

        /// <summary>
        /// Modify the constraint to ignore case in matching.
        /// This will call Using(StringComparison.CurrentCultureIgnoreCase).
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <see cref="StringComparison.CurrentCultureIgnoreCase"/> was already set.</exception>
        public virtual StringConstraint IgnoreCase
        {
            get
            {
                Using(StringComparison.CurrentCultureIgnoreCase);
                caseInsensitive = true;
                return this;
            }
        }

        /// <summary>
        /// Modify the constraint to use the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type was already set
        /// or when a culture info was already set.</exception>
        public virtual StringConstraint Using(StringComparison comparisonType)
        {
            if (_cultureInfo is not null)
                throw new InvalidOperationException("Cannot set comparison type when culture has already been set.");

            if (_comparisonType is not null && _comparisonType != comparisonType)
                throw new InvalidOperationException("A different comparison type was already set.");

            _comparisonType = comparisonType;
            return this;
        }

        /// <summary>
        /// Modify the constraint to use the specified culture info.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a culture info was already set
        /// or when a comparison type was already set.</exception>
        public virtual StringConstraint Using(CultureInfo culture)
        {
            if (_comparisonType is not null)
                throw new InvalidOperationException("Cannot set culture when comparison type has already been set.");

            if (_cultureInfo is not null && !_cultureInfo.Equals(culture))
                throw new InvalidOperationException("A different culture was already set.");

            _cultureInfo = culture;
            return this;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var stringValue = ConstraintUtils.RequireActual<string>(actual, nameof(actual), allowNull: true);

            bool result;
            if (_cultureInfo is not null)
                result = Matches(stringValue, _cultureInfo);
            else if (_comparisonType is not null)
                result = Matches(stringValue, _comparisonType.Value);
            else
                result = Matches(stringValue);

            return new ConstraintResult(this, actual, result);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given string
        /// </summary>
        /// <param name="actual">The string to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected abstract bool Matches(string? actual);

        /// <summary>
        /// Test whether the constraint is satisfied by a given string with a specific string comparison
        /// </summary>
        /// <param name="actual">The string to be tested</param>
        /// <param name="stringComparison">The string comparison type to be used</param>
        /// <returns>True for success, false for failure</returns>
        protected virtual bool Matches(string? actual, StringComparison stringComparison)
        {
            return Matches(actual);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given string with a specific culture
        /// </summary>
        /// <param name="actual">The string to be tested</param>
        /// <param name="cultureInfo">The culture info to use for comparison</param>
        /// <returns>True for success, false for failure</returns>
        protected virtual bool Matches(string? actual, CultureInfo cultureInfo)
        {
            return Matches(actual);
        }
    }
}
