// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Helper class with properties and methods that supply
    /// a number of constraints used in Asserts.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class Contains
    {
        #region Item

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public static SomeItemsConstraint Item(object? expected) =>
            new(new EqualConstraint(expected));

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public static SomeItemsConstraint<T> Item<T>(T? expected) =>
            new(new EqualConstraint<T>(expected));

        #endregion

        #region Key

        /// <summary>
        /// Returns a new DictionaryContainsKeyConstraint checking for the
        /// presence of a particular key in the dictionary.
        /// </summary>
        public static DictionaryContainsKeyConstraint Key(object expected)
        {
            return new DictionaryContainsKeyConstraint(expected);
        }

        #endregion

        #region Value

        /// <summary>
        /// Returns a new DictionaryContainsValueConstraint checking for the
        /// presence of a particular value in the dictionary.
        /// </summary>
        public static DictionaryContainsValueConstraint Value(object? expected)
        {
            return new DictionaryContainsValueConstraint(expected);
        }

        #endregion

        #region Substring

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value contains the substring supplied as an argument.
        /// </summary>
        public static SubstringConstraint Substring(string expected)
        {
            return new SubstringConstraint(expected);
        }

        #endregion
    }
}
