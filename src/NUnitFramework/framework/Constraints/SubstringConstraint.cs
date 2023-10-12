// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SubstringConstraint can test whether a string contains
    /// the expected substring.
    /// </summary>
    public class SubstringConstraint : StringConstraint
    {
        private StringComparison? _comparisonType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstringConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected.</param>
        public SubstringConstraint(string expected) : base(expected)
        {
            descriptionText = "String containing";
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
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string actual)
        {
            if (actual is null) return false;

            var actualComparison = _comparisonType ?? StringComparison.CurrentCulture;
            return actual.IndexOf(expected, actualComparison) >= 0;
        }

        /// <summary>
        /// Modify the constraint to the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="comparisonType"/> was already set.</exception>
        public SubstringConstraint Using(StringComparison comparisonType)
        {
            if (_comparisonType == null)
                _comparisonType = comparisonType;
            else if (_comparisonType != comparisonType)
                throw new InvalidOperationException("A different comparison type was already set.");

            return this;
        }
    }
}
