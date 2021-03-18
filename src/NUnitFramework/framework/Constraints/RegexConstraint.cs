// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// RegexConstraint can test whether a string matches
    /// the pattern provided.
    /// </summary>
    public class RegexConstraint : Constraint
    {
        private Regex _regex;

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var description = $"String matching \"{_regex}\"";
                var caseInsensitive = (_regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
                if (caseInsensitive)
                {
                    description += ", ignoring case";
                }
                return description;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexConstraint"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public RegexConstraint(string pattern) : base(pattern)
        {
            _regex = new Regex(pattern);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexConstraint"/> class.
        /// </summary>
        /// <param name="regex">The Regex pattern object.</param>
        public RegexConstraint(Regex regex) : base(regex.ToString())
        {
            _regex = regex;
        }

        /// <summary>
        /// Modify the constraint to ignore case in matching.
        /// </summary>
        public RegexConstraint IgnoreCase
        {
            get
            {
                _regex = new Regex(_regex.ToString(), _regex.Options | RegexOptions.IgnoreCase);
                return this;
            }
        }

        /// <summary>
        /// Applies the regex constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The string to be tested.</param>
        /// <returns>True for success, false for failure.</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual,
                actual is string actualString  && _regex.Match(actualString).Success);
        }
    }
}
