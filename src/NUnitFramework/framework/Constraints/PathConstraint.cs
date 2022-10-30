// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
    #region PathConstraint
    /// <summary>
    /// PathConstraint serves as the abstract base of constraints
    /// that operate on paths and provides several helper methods.
    /// </summary>
    public abstract class PathConstraint : StringConstraint
    {
        private const char WindowsDirectorySeparatorChar = '\\';
        private const char NonWindowsDirectorySeparatorChar = '/';
        private static readonly char[] DirectorySeparatorChars = new char[] { WindowsDirectorySeparatorChar, NonWindowsDirectorySeparatorChar };

        /// <summary>
        /// Construct a PathConstraint for a give expected path
        /// </summary>
        /// <param name="expected">The expected path</param>
        protected PathConstraint(string expected)
            : base(expected)
        {
            this.expected = expected;
            this.caseInsensitive = Path.DirectorySeparatorChar == WindowsDirectorySeparatorChar;
        }

        /// <summary>
        /// Modifies the current instance to be case-sensitive
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
            return $"<{DisplayName.ToLower()} \"{expected}\" {(caseInsensitive ? "ignorecase" : "respectcase")}>";
        }

        #region Helper Methods
        /// <summary>
        /// Canonicalize the provided path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The path in standardized form</returns>
        protected string Canonicalize(string path)
        {
            if (Path.DirectorySeparatorChar != Path.AltDirectorySeparatorChar)
                path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            string leadingSeparators = "";

            foreach (char c in path)
            {
                if (c == WindowsDirectorySeparatorChar || c == NonWindowsDirectorySeparatorChar)
                {
                    leadingSeparators += Path.DirectorySeparatorChar;
                }
                else break;
            }

            string[] parts = path.Split(DirectorySeparatorChars, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            bool shifting = false;
            foreach (string part in parts)
            {
                switch (part)
                {
                    case ".":
                    case "":
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

            return leadingSeparators + String.Join(Path.DirectorySeparatorChar.ToString(), parts, 0, count);
        }

        /// <summary>
        /// Test whether one path in canonical form is a subpath of another path
        /// </summary>
        /// <param name="path1">The first path - supposed to be the parent path</param>
        /// <param name="path2">The second path - supposed to be the child path</param>
        /// <returns></returns>
        protected bool IsSubPath(string path1, string path2)
        {
            int length1 = path1.Length;
            int length2 = path2.Length;

            // if path1 is longer or equal, then path2 can't be a subpath
            if (length1 >= length2)
                return false;

            // path 2 is longer than path 1: see if initial parts match
            if (!StringUtil.StringsEqual(path1, path2.Substring(0, length1), caseInsensitive))
                return false;

            // must match through or up to a directory separator boundary
            return path2[length1 - 1] == Path.DirectorySeparatorChar ||
                path2[length1] == Path.DirectorySeparatorChar;
        }

        #endregion
    }
    #endregion
}
