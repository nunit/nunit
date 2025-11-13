// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;

namespace NUnit.Framework.Constraints
{ /// <summary>
    /// StartsWithConstraint can test whether a string starts
    /// with an expected substring.
    /// </summary>
    public class StartsWithConstraint : StringComparisonConstraint
    {
        private CultureInfo? _cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartsWithConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected string</param>
        public StartsWithConstraint(string expected) : base(expected)
        {
            descriptionText = "String starting with";
        }

        /// <summary>
        /// Test whether the constraint is matched by the actual value.
        /// This is a template method, which calls the IsMatch method
        /// of the derived class.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(string? actual)
        {
            if (_cultureInfo is not null)
            {
                return actual is not null && actual.StartsWith(expected, caseInsensitive, _cultureInfo);
            }

            return actual is not null && actual.StartsWith(expected, DetermineComparisonType());
        }

        /// <summary>
        /// Modify the constraint to the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="cultureInfo"/> was already set.</exception>
        public StringComparisonConstraint Using(CultureInfo cultureInfo)
        {
            if (_cultureInfo is null)
                _cultureInfo = cultureInfo;
            else if (_cultureInfo != cultureInfo)
                throw new InvalidOperationException("A different culture info was already set.");

            return this;
        }
    }
}
