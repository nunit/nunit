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
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected StringComparison? comparisonType;
#pragma warning restore IDE1006

        /// <summary>
        /// The culture info to use for string comparisons
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected CultureInfo? cultureInfo;
#pragma warning restore IDE1006

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
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="strComparison"/> was already set.</exception>
        public virtual StringConstraint Using(StringComparison strComparison)
        {
            if (comparisonType is null)
                comparisonType = strComparison;
            else if (comparisonType != strComparison)
                throw new InvalidOperationException("A different comparison type was already set.");

            return this;
        }

        /// <summary>
        /// Modify the constraint to use the specified culture info.
        /// </summary>
        public virtual StringConstraint Using(CultureInfo culture)
        {
            cultureInfo ??= culture;
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

            return new ConstraintResult(this, actual, cultureInfo is not null ? Matches(stringValue, cultureInfo) : Matches(stringValue));
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given string
        /// </summary>
        /// <param name="actual">The string to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected abstract bool Matches(string? actual);

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
