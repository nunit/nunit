// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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

using System.IO;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DirectoryInfo"/>s.
    /// </summary>
    internal sealed class DirectoriesComparer : IChainComparer
    {
        public bool? Equal(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
        {
            if (!(x is DirectoryInfo) || !(y is DirectoryInfo))
                return null;

            DirectoryInfo xDirectoryInfo = (DirectoryInfo)x;
            DirectoryInfo yDirectoryInfo = (DirectoryInfo)y;

            // Do quick compares first
            if (xDirectoryInfo.Attributes != yDirectoryInfo.Attributes ||
                xDirectoryInfo.CreationTime != yDirectoryInfo.CreationTime ||
                xDirectoryInfo.LastAccessTime != yDirectoryInfo.LastAccessTime)
            {
                return false;
            }

            // TODO: Find a cleaner way to do this
            return new SamePathConstraint(xDirectoryInfo.FullName).ApplyTo(yDirectoryInfo.FullName).IsSuccess;
        }
    }
}
