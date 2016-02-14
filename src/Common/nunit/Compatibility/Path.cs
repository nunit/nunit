// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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

#if PORTABLE
#if NUNIT_ENGINE
namespace NUnit.Engine.Compatibility
#elif NUNIT_FRAMEWORK
namespace NUnit.Framework.Compatibility
#else
namespace NUnit.Common.Compatibility
#endif
{
    /// <summary>
    /// Some path based methods that we need even in the Portable framework which
    /// does not have the System.IO.Path class
    /// </summary>
    public static class Path
    {
        /// <summary>
        /// Windows directory separator
        /// </summary>
        public static readonly char WindowsSeparatorChar = '\\';
        /// <summary>
        /// Alternate directory separator
        /// </summary>
        public static readonly char AltDirectorySeparatorChar = '/';
        /// <summary>
        /// A volume separator character.
        /// </summary>
        public static readonly char VolumeSeparatorChar = ':';

        /// <summary>
        /// Get the file name and extension of the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the file name and extension.</param>
        /// <returns>The filename as a <see cref="T:System.String"/>. If the last character of <paramref name="path"/> is a directory or volume separator character, this method returns <see cref="F:System.String.Empty"/>. If <paramref name="path"/> is null, this method returns null.</returns>
        public static string GetFileName(string path)
        {
            if (path != null)
            {
                int length = path.Length;
                for( int index = length - 1; index >= 0; index--)
                {
                    char ch = path[index];
                    if (ch == Path.WindowsSeparatorChar || ch == Path.AltDirectorySeparatorChar || ch == Path.VolumeSeparatorChar)
                        return path.Substring(index + 1, length - index - 1);
                }
            }
            return path;
        }
    }
}
#endif