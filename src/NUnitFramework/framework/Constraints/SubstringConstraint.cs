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
        /// Test whether the constraint is satisfied by a given value.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual)
        {
            if (actual is null)
                return false;

            return actual.IndexOf(expected) >= 0;
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
            if (actual is null)
                return false;

            return actual.IndexOf(expected, stringComparison) >= 0;
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
            if (actual is null)
                return false;

            return cultureInfo.CompareInfo.IndexOf(actual, expected, caseInsensitive ? CompareOptions.IgnoreCase : CompareOptions.None) >= 0;
        }
    }
}
