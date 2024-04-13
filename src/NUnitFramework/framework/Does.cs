// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Helper class with properties and methods that supply
    /// a number of constraints used in Asserts.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class Does
    {
        #region Not

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public static ConstraintExpression Not => new ConstraintExpression().Not;

        #endregion

        #region Exist

        /// <summary>
        /// Returns a constraint that succeeds if the value
        /// is a file or directory and it exists.
        /// </summary>
        public static FileOrDirectoryExistsConstraint Exist => new();

        #endregion

        #region Contain

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public static SomeItemsConstraint Contain(object? expected) =>
            new(new EqualConstraint(expected));

        /// <summary>
        /// Returns a new <see cref="ContainsConstraint"/>. This constraint
        /// will, in turn, make use of the appropriate second-level
        /// constraint, depending on the type of the actual argument.
        /// This overload is only used if the item sought is a string,
        /// since any other type implies that we are looking for a
        /// collection member.
        /// </summary>
        public static ContainsConstraint Contain(string? expected) =>
            new(expected);

        #endregion

        #region DictionaryContain
        /// <summary>
        /// Returns a new DictionaryContainsKeyConstraint checking for the
        /// presence of a particular key in the Dictionary key collection.
        /// </summary>
        /// <param name="expected">The key to be matched in the Dictionary key collection</param>
        public static DictionaryContainsKeyConstraint ContainKey(object expected)
        {
            return Contains.Key(expected);
        }

        /// <summary>
        /// Returns a new DictionaryContainsValueConstraint checking for the
        /// presence of a particular value in the Dictionary value collection.
        /// </summary>
        /// <param name="expected">The value to be matched in the Dictionary value collection</param>
        public static DictionaryContainsValueConstraint ContainValue(object? expected)
        {
            return Contains.Value(expected);
        }

        #endregion

        #region StartWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        public static StartsWithConstraint StartWith(string expected)
        {
            return new StartsWithConstraint(expected);
        }

        #endregion

        #region EndWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        public static EndsWithConstraint EndWith(string expected)
        {
            return new EndsWithConstraint(expected);
        }

        #endregion

        #region Match

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public static RegexConstraint Match([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        {
            return new RegexConstraint(pattern);
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public static RegexConstraint Match(Regex regex)
        {
            return new RegexConstraint(regex);
        }

        #endregion
    }
}
