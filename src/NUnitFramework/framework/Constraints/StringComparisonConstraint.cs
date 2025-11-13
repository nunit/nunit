using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Abstract base class that provides string comparison modifier functionality.
    /// </summary>
    public abstract class StringComparisonConstraint : StringConstraint
    {
        private StringComparison? _comparisonType;

        /// <summary>
        /// Constructs a StringComparisonConstraint given an expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        protected StringComparisonConstraint(string expected) : base(expected)
        {
        }

        /// <summary>
        /// Modify the constraint to ignore case in matching.
        /// This will call Using(StringComparison.CurrentCultureIgnoreCase).
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <see cref="StringComparison.CurrentCultureIgnoreCase"/> was already set.</exception>
        public override StringConstraint IgnoreCase
        {
            get
            {
                Using(StringComparison.CurrentCultureIgnoreCase);
                return base.IgnoreCase;
            }
        }

        /// <summary>
        /// Determines the <see cref="StringComparison"/> value based on the
        /// <see cref="StringConstraint.caseInsensitive"/> field.
        /// If <c>caseInsensitive</c> is true, it returns <see cref="StringComparison.OrdinalIgnoreCase"/>;
        /// otherwise, it returns <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        protected StringComparison DetermineComparisonType()
        {
            return _comparisonType ?? (caseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Modify the constraint to the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="stringComparison"/> was already set.</exception>
        public StringComparisonConstraint Using(StringComparison stringComparison)
        {
            if (_comparisonType is null)
                _comparisonType = stringComparison;
            else if (_comparisonType != stringComparison || caseInsensitive)
                throw new InvalidOperationException("A different comparison type was already set.");

            return this;
        }
    }
}
