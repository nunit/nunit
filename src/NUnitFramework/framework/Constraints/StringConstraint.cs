// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        protected readonly string expected;
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
        /// Indicates whether tests should normalize newlines
        /// </summary>
        protected bool NormalizingLineEndings;

        /// <summary>
        /// Description of this constraint
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected string descriptionText = string.Empty;
#pragma warning restore IDE1006

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                string desc = $"{descriptionText} {MsgUtils.FormatValue(expected)}";
                if (caseInsensitive)
                    desc += ", ignoring case";
                return desc;
            }
        }

        /// <summary>
        /// Constructs a StringConstraint without an expected value
        /// </summary>
        protected StringConstraint()
        {
            expected = string.Empty;
        }

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
            get
            {
                caseInsensitive = true;
                return this;
            }
        }

        /// <summary>
        /// Modify the constraint to normalize newlines before matching.
        /// </summary>
        public virtual StringConstraint NormalizeLineEndings
        {
            get
            {
                NormalizingLineEndings = true;
                return this;
            }
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
        protected abstract bool Matches(string? actual);
    }
}
