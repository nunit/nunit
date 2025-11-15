// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EndsWithConstraint can test whether a string ends
    /// with an expected substring.
    /// </summary>
    public class EndsWithConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndsWithConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected string</param>
        public EndsWithConstraint(string expected) : base(expected)
        {
            descriptionText = "String ending with";
        }

        /// <summary>
        /// Determines whether the actual string value ends with the expected substring,
        /// using string comparison specified by the constraint.
        /// </summary>
        /// <param name="actual">The string value to test.</param>
        /// <returns></returns>
        protected override bool Matches(string? actual)
        {
            return actual is not null && actual.EndsWith(expected, comparisonType ?? StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Determines whether the actual string value ends with the expected substring,
        /// using the specified <see cref="CultureInfo"/> and case sensitivity specified by the constraint.
        /// If <paramref name="cultureInfo"/> is not null, the comparison uses culture-specific rules;
        /// otherwise, it falls back to the default string comparison logic.
        /// </summary>
        /// <param name="actual">The string value to test.</param>
        /// <param name="cultureInfo">The culture information to use for the comparison.</param>
        /// <returns></returns>
        protected override bool Matches(string? actual, CultureInfo cultureInfo)
        {
            if (cultureInfo is not null)
            {
                return actual is not null && actual.EndsWith(expected, caseInsensitive, cultureInfo);
            }
            return Matches(actual);
        }
    }
}
