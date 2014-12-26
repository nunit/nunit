// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Helper class with properties and methods that supply
    /// a number of constraints used in Asserts.
    /// </summary>
    public static class Does
    {
        #region Not

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public static ConstraintExpression Not
        {
            get { return new ConstraintExpression().Not; }
        }

        #endregion

        #region Exist

#if !SILVERLIGHT && !PORTABLE
        /// <summary>
        /// Returns a constraint that succeeds if the value
        /// is a file or directory and it exists.
        /// </summary>
        public static FileOrDirectoryExistsConstraint Exist
        {
            get { return new FileOrDirectoryExistsConstraint(); }
        }
#endif

        #endregion

        #region Contain

        /// <summary>
        /// Returns a new CollectionContainsConstraint checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public static CollectionContainsConstraint Contain(object expected)
        {
            return new CollectionContainsConstraint(expected);
        }

        /// <summary>
        /// Returns a new ContainsConstraint. This constraint
        /// will, in turn, make use of the appropriate second-level
        /// constraint, depending on the type of the actual argument. 
        /// This overload is only used if the item sought is a string,
        /// since any other type implies that we are looking for a 
        /// collection member.
        /// </summary>
        public static ContainsConstraint Contain(string expected)
        {
            return new ContainsConstraint(expected);
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
        public static RegexConstraint Match(string pattern)
        {
            return new RegexConstraint(pattern);
        }

        #endregion
    }
}