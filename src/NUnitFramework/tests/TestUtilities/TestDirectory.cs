// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
