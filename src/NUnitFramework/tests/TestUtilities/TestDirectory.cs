// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Creates a directory for testing purposes
    /// </summary>
    public class TestDirectory : IDisposable
    {
        private readonly DirectoryInfo _testDir;

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
                if (_testDir.Exists)
                {
                    _testDir.Delete(true);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the test directory.
        /// </summary>
        public DirectoryInfo Directory => _testDir;

        /// <summary>
        /// Returns the full path of the contained test directory
        /// </summary>
        public override string ToString()
        {
            return _testDir is null ? string.Empty : _testDir.FullName;
        }
    }
}
