// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;
using NUnit.Framework.Constraints.Comparers;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualStringWithoutUsingConstraint : Constraint
    {
        #region Static and Instance Fields

        private readonly string? _expected;

        private bool _caseInsensitive;
        private bool _ignoringWhiteSpace;
        private bool _normalizeLineEndings;
        private bool _clipStrings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualStringWithoutUsingConstraint(string? expected)
            : base(expected)
        {
            _expected = expected;
            _clipStrings = true;
        }

        #endregion

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        public string? Expected => _expected;

        #region Constraint Modifiers

        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public EqualStringWithoutUsingConstraint IgnoreCase
        {
            get
            {
                _caseInsensitive = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to ignore white space and return self.
        /// </summary>
        public EqualStringWithoutUsingConstraint IgnoreWhiteSpace
        {
            get
            {
                _ignoringWhiteSpace = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to normalize newlines and return self.
        /// </summary>
        public EqualStringWithoutUsingConstraint NormalizeLineEndings
        {
            get
            {
                _normalizeLineEndings = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to suppress string clipping
        /// and return self.
        /// </summary>
        public EqualStringWithoutUsingConstraint NoClip
        {
            get
            {
                _clipStrings = false;
                return this;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public ConstraintResult ApplyTo(string? actual)
        {
            bool hasSucceeded;

            if (actual is null)
            {
                hasSucceeded = _expected is null;
            }
            else if (_expected is null)
            {
                hasSucceeded = false;
            }
            else
            {
                hasSucceeded = StringsComparer.Equals(_expected, actual, _caseInsensitive, _ignoringWhiteSpace, _normalizeLineEndings);
            }

            return ConstraintResult(actual, hasSucceeded);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// I wish we could hide this method, but it is public in the base class.
        /// </remarks>
        public sealed override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool hasSucceeded;

            if (actual is null)
            {
                hasSucceeded = _expected is null;
            }
            else if (actual is string actualString)
            {
                return ApplyTo(actualString);
            }
            else if (actual is IEquatable<string> equatableString)
            {
                if (_caseInsensitive || _ignoringWhiteSpace || _normalizeLineEndings)
                {
                    throw new InvalidOperationException("Cannot use IgnoreCase or IgnoreWhiteSpace or NormalizeLineEndings with IEquatable<string>.");
                }

                hasSucceeded = equatableString.Equals(_expected);
            }
            else if (_expected is null)
            {
                hasSucceeded = false;
            }
            else
            {
                // We fall back to pre 4.3 EqualConstraint behavior
                // But if the actual value cannot be convert to a string nor can be compared to one
                // not sure if that makes any difference.
                return new EqualConstraint(_expected).ApplyTo(actual);
            }

            return ConstraintResult(actual, hasSucceeded);
        }

        private ConstraintResult ConstraintResult<T>(T actual, bool hasSucceeded)
        {
            return new EqualConstraintResult(this, actual, _caseInsensitive, _ignoringWhiteSpace, _clipStrings, hasSucceeded);
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var sb = new StringBuilder(MsgUtils.FormatValue(_expected));

                if (_caseInsensitive)
                    sb.Append(", ignoring case");

                if (_ignoringWhiteSpace)
                    sb.Append(", ignoring white-space");

                return sb.ToString();
            }
        }

#endregion
    }
}
