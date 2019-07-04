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

using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// StringConstraint is the abstract base for constraints
    /// that operate on strings. It supports the IgnoreCase
    /// modifier for string operations.
    /// </summary>
    public abstract class StringConstraint : Constraint
    {
        /// <summary>
        /// The expected value
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected string expected;
#pragma warning restore IDE1006

        /// <summary>
        /// Indicates whether tests should be case-insensitive
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected bool caseInsensitive;
#pragma warning restore IDE1006

        /// <summary>
        /// Description of this constraint
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected string descriptionText;
#pragma warning restore IDE1006

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                string desc = string.Format("{0} {1}", descriptionText, MsgUtils.FormatValue(expected));
                if (caseInsensitive)
                    desc += ", ignoring case";
                return desc;
            }
        }

        /// <summary>
        /// Constructs a StringConstraint without an expected value
        /// </summary>
        protected StringConstraint() { }

        /// <summary>
        /// Constructs a StringConstraint given an expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        protected StringConstraint(string expected)
            : base(expected)
        {
            this.expected = expected;
        }

        /// <summary>
        /// Modify the constraint to ignore case in matching.
        /// </summary>
        public virtual StringConstraint IgnoreCase
        {
            get { caseInsensitive = true; return this; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var stringValue = ConstraintUtils.RequireActual<string>(actual, nameof(actual), allowNull: true);

            return new ConstraintResult(this, actual, Matches(stringValue));
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given string
        /// </summary>
        /// <param name="actual">The string to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected abstract bool Matches(string actual);
    }
}
