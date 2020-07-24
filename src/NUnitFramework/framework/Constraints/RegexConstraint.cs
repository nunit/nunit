// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
                var description = $"String matching pattern \"{_regex}\"";
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
                actual != null && actual is string && _regex.Match(actual.ToString()).Success);
        }
    }
}
