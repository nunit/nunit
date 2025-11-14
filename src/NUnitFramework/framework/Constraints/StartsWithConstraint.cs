// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// StartsWithConstraint can test whether a string starts
    /// with an expected substring.
    /// </summary>
    public class StartsWithConstraint : StringConstraint
    {
        private StringComparison? _comparisonType;
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

            return actual is not null && actual.StartsWith(expected, _comparisonType ?? StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Modify the constraint to use the specified comparison.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a comparison type different
        /// than <paramref name="comparisonType"/> was already set.</exception>
        public StartsWithConstraint Using(StringComparison comparisonType)
        {
            if (_comparisonType is null)
                _comparisonType = comparisonType;
            else if (_comparisonType != comparisonType)
                throw new InvalidOperationException("A different comparison type was already set.");

            return this;
        }

        /// <summary>
        /// Modify the constraint to use the specified culture info.
        /// </summary>
        public StartsWithConstraint Using(CultureInfo cultureInfo)
        {
            if (_cultureInfo is null)
                _cultureInfo = cultureInfo;

            return this;
        }
    }
}
