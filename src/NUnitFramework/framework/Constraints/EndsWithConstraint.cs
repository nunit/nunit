// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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
        /// Test whether the constraint is matched by the actual value.
        /// This is a template method, which calls the IsMatch method
        /// of the derived class.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(string? actual)
        {
            var stringComparison = caseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return actual is not null && actual.EndsWith(expected, stringComparison);
        }
    }
}
