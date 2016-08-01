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

#if !PORTABLE
using System;
using System.IO;
using NUnit.Framework;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Creates a directory for testing purposes
    /// </summary>
    public class TestDirectory : IDisposable
    {
        private DirectoryInfo _testDir;

        public TestDirectory()
        {
            var tempPath = Path.GetTempPath();

            _testDir = new DirectoryInfo(Path.Combine(tempPath, Guid.NewGuid().ToString()));
            Assume.That(_testDir.Exists, Is.False, _testDir + " should not already exist");

            _testDir.Create();
            _testDir.Refresh();
            Assume.That(_testDir.Exists, Is.True, "Failed to create test dir " + _testDir);
        }

        /// <summary>
        /// Clean up after ourselves
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_testDir != null && _testDir.Exists)
                {
                    _testDir.Delete(true);
                }
            }
            catch { }
            _testDir = null;
        }

        /// <summary>
        /// Gets the test directory.
        /// </summary>
        public DirectoryInfo Directory { get { return _testDir; } }

        /// <summary>
        /// Returns the full path of the contained test directory
        /// </summary>
        public override string ToString()
        {
            return _testDir == null ? string.Empty : _testDir.FullName;
        }
    }
}
    #endif