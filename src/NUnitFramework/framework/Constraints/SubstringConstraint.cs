// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SubstringConstraint can test whether a string contains
    /// the expected substring.
    /// </summary>
    public class SubstringConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstringConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected.</param>
        public SubstringConstraint(string expected) : base(expected)
        {
            descriptionText = "String containing";
        }

        /// <summary>
        /// Modify the constraint to the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="comparisonType"/> was already set.</exception>
        public new SubstringConstraint Using(StringComparison comparisonType)
        {
            // This method is needed because of binary backward compatibility.
            return (SubstringConstraint)base.Using(comparisonType);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual)
        {
            return actual is not null && actual.IndexOf(expected) >= 0;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// using the specified string comparison.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <param name="stringComparison">The string comparison to use</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual, StringComparison stringComparison)
        {
            return actual is not null && actual.IndexOf(expected, stringComparison) >= 0;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// using the specified culture.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <param name="cultureInfo">The culture info to use for comparison</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual, CultureInfo cultureInfo)
        {
            return actual is not null && cultureInfo.CompareInfo.IndexOf(actual, expected,
                caseInsensitive ? CompareOptions.IgnoreCase : CompareOptions.None) >= 0;
        }
    }
}
