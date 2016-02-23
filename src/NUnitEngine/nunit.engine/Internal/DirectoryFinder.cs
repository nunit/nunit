// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NUnit.Engine.Internal
{
    #region Public Methods

    /// <summary>
    /// DirectoryFinder is a utility class used for extended wildcard
    /// selection of directories and files. It's less than a full-fledged
    /// Linux-style globbing utility and more than standard wildcard use.
    /// </summary>
    public static class DirectoryFinder
    {
        /// <summary>
        /// Get a list of diretories matching and extended wildcard pattern.
        /// Each path component may have wildcard characters and a component
        /// of "**" may be used to represent all directories, recusively.
        /// </summary>
        /// <param name="baseDir">A DirectoryInfo from which the matching starts</param>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>A list of DirectoryInfos</returns>
        public static IList<DirectoryInfo> GetDirectories(DirectoryInfo baseDir, string pattern)
        {
            Guard.ArgumentNotNullOrEmpty(pattern, "pattern");

            if (Path.DirectorySeparatorChar == '\\')
                pattern = pattern.Replace(Path.DirectorySeparatorChar, '/');

            var dirList = new List<DirectoryInfo>();
            dirList.Add(baseDir);

            while (pattern.Length > 0)
            {
                string range;
                int sep = pattern.IndexOf('/');

                if (sep >= 0)
                {
                    range = pattern.Substring(0, sep);
                    pattern = pattern.Substring(sep + 1);
                }
                else
                {
                    range = pattern;
                    pattern = "";
                }

                if (range == "." || range == "")
                    continue;

                dirList = ExpandOneStep(dirList, range);
            }

            return dirList;
        }

        /// <summary>
        /// Get files using an extended pattern with the option of wildcard 
        /// characters in each path component.
        /// </summary>
        /// <param name="baseDir">A DirectoryInfo from which the matching starts</param>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>A list of FileInfos</returns>
        public static IList<FileInfo> GetFiles(DirectoryInfo baseDir, string pattern)
        {
            // If there is no directory path in pattern, delegate to DirectoryInfo
            int lastSep = pattern.LastIndexOf('/');
            if (lastSep < 0) // Simple file name entry, no path
                return baseDir.GetFiles(pattern);

            // Othersise split pattern into two parts around last separator
            var pattern1 = pattern.Substring(0, lastSep);
            var pattern2 = pattern.Substring(lastSep + 1);

            var fileList = new List<FileInfo>();

            foreach (var dir in DirectoryFinder.GetDirectories(baseDir, pattern1))
                fileList.AddRange(dir.GetFiles(pattern2));

            return fileList;
        }

        #endregion

        #region Helper Methods

        private static List<DirectoryInfo> ExpandOneStep(IList<DirectoryInfo> dirList, string pattern)
        {
            var newList = new List<DirectoryInfo>();

            foreach (var dir in dirList)
            {
                if (pattern == "." || pattern == "")
                    newList.Add(dir);
                else if (pattern == "..")
                    newList.Add(dir.Parent);
                else if (pattern == "**")
                {
                    var subDirs = dir.GetDirectories("*", SearchOption.AllDirectories);
                    if (subDirs.Length > 0) newList.AddRange(subDirs);
                }
                else
                {
                    var subDirs = dir.GetDirectories(pattern);
                    if (subDirs.Length > 0) newList.AddRange(subDirs);
                }
            }

            return newList;
        }

        #endregion
    }
}
