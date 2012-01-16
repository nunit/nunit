// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
    #region PathConstraint
    /// <summary>
    /// PathConstraint serves as the abstract base of constraints
    /// that operate on paths and provides several helper methods.
    /// </summary>
    public abstract class PathConstraint : Constraint
    {
        private static readonly char[] DirectorySeparatorChars = new char[] { '\\', '/' };

        /// <summary>
        /// The expected path used in the constraint
        /// </summary>
        protected string expected;

        /// <summary>
        /// Flag indicating whether a caseInsensitive comparison should be made
        /// </summary>
        protected bool caseInsensitive = Path.DirectorySeparatorChar == '\\';

        /// <summary>
        /// Construct a PathConstraint for a give expected path
        /// </summary>
        /// <param name="expected">The expected path</param>
        protected PathConstraint(string expected)
            : base(expected)
        {
            this.expected = expected;
        }

        /// <summary>
        /// Modifies the current instance to be case-insensitve
        /// and returns it.
        /// </summary>
        public PathConstraint IgnoreCase
        {
            get { caseInsensitive = true; return this; }
        }

        /// <summary>
        /// Modifies the current instance to be case-sensitve
        /// and returns it.
        /// </summary>
        public PathConstraint RespectCase
        {
            get { caseInsensitive = false; return this; }
        }

        /// <summary>
        /// Returns the string representation of this constraint
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return string.Format("<{0} \"{1}\" {2}>", DisplayName, expected, caseInsensitive ? "ignorecase" : "respectcase");
        }

        #region Helper Methods
        /// <summary>
        /// Canonicalize the provided path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The path in standardized form</returns>
        protected string Canonicalize(string path)
        {
            string[] parts = path.Split(DirectorySeparatorChars, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            bool shifting = false;
            foreach (string part in parts)
            {
                switch (part)
                {
                    case ".":
                        shifting = true;
                        break;

                    case "..":
                        shifting = true;
                        if (count > 0)
                            --count;
                        break;
                    default:
                        if (shifting)
                            parts[count] = part;
                        ++count;
                        break;
                }
            }

            return String.Join(Path.DirectorySeparatorChar.ToString(), parts, 0, count);
        }

        /// <summary>
        /// Test whether two paths are the same
        /// </summary>
        /// <param name="path1">The first path</param>
        /// <param name="path2">The second path</param>
        /// <returns></returns>
        protected bool IsSamePath(string path1, string path2)
        {
            return string.Compare(Canonicalize(expected), Canonicalize((string)actual), caseInsensitive) == 0;
        }

        /// <summary>
        /// Test whether one path is the same as or under another path
        /// </summary>
        /// <param name="path1">The first path - supposed to be the parent path</param>
        /// <param name="path2">The second path - supposed to be the child path</param>
        /// <returns></returns>
        protected bool IsSamePathOrUnder(string path1, string path2)
        {
            path1 = Canonicalize(path1);
            path2 = Canonicalize(path2);

            int length1 = path1.Length;
            int length2 = path2.Length;

            // if path1 is longer, then path2 can't be under it
            if (length1 > length2)
                return false;

            // if lengths are the same, check for equality
            if (length1 == length2)
                return string.Compare(path1, path2, caseInsensitive) == 0;

            // path 2 is longer than path 1: see if initial parts match
            if (string.Compare(path1, path2.Substring(0, length1), caseInsensitive) != 0)
                return false;

            // must match through or up to a directory separator boundary
            return path2[length1 - 1] == Path.DirectorySeparatorChar ||
                path2[length1] == Path.DirectorySeparatorChar;
        }
        #endregion
    }
    #endregion
}