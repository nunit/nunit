// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Assertions;
using NUnit.Framework.Constraints;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class FileOrDirectoryExistsConstraintTests
    {
        private FileOrDirectoryExistsConstraint _constraint;
        private TestDirectory _goodDir;
        private static readonly string BAD_DIRECTORY = Path.Combine(Path.GetTempPath(), @"\I\hope\this\is\garbage");
        private static readonly string BAD_FILE = Path.Combine(Path.GetTempPath(), "garbage.txt");
        private const string RESOURCE_FILE = "TestText1.txt";

        [SetUp]
        public void SetUp()
        {
            _constraint = new FileOrDirectoryExistsConstraint();
            _goodDir = new TestDirectory();
            Assume.That(BAD_DIRECTORY, Does.Not.Exist, BAD_DIRECTORY + " exists");
        }

        [TearDown]
        public void TearDown()
        {
            if (_goodDir != null) _goodDir.Dispose();
        }

        [Test]
        public void PassesWhenFileInfoExists()
        {
            using (var actualTestFile = new TestFile(RESOURCE_FILE))
            {
                Assert.That(_constraint.ApplyTo(actualTestFile.File).IsSuccess);
                Assert.That(actualTestFile.File, Does.Exist);
            }
        }

        [Test]
        public void PassesWhenDirectoryInfoExists()
        {
            var actual = _goodDir.Directory;
            Assert.That(_constraint.ApplyTo(actual).IsSuccess);
            Assert.That(actual, Does.Exist);
        }

        [Test]
        public void PassesWhenFileStringExists()
        {
            using (var tf = new TestFile(RESOURCE_FILE))
            {
                Assert.That(_constraint.ApplyTo(tf.File.FullName).IsSuccess);
                Assert.That(tf.File.FullName, Does.Exist);
            }
        }

        [Test]
        public void PassesWhenDirectoryStringExists()
        {
            Assert.That(_constraint.ApplyTo(_goodDir.ToString()).IsSuccess);
            Assert.That(_goodDir.ToString(), Does.Exist);
        }

        [Test]
        public void FailsWhenIgnoreFilesIsTrueWithFileString()
        {
            using (var tf = new TestFile(RESOURCE_FILE))
            {
                var constraint = new FileOrDirectoryExistsConstraint().IgnoreFiles;
                Assert.That(constraint.ApplyTo(tf.File.FullName).Status == ConstraintStatus.Failure);
            }
        }

        [Test]
        public void FailsWhenIgnoreFilesIsTrueWithFileInfo()
        {
            using (var tf = new TestFile(RESOURCE_FILE))
            {
                var constraint = new FileOrDirectoryExistsConstraint().IgnoreFiles;
                var ex = Assert.Throws<ArgumentException>(() => constraint.ApplyTo(tf.File));
                Assert.That(ex.Message, Does.StartWith("The actual value must be a string or DirectoryInfo"));
            }
        }

        [Test]
        public void FailsWhenIgnoreDirectoriesIsTrueWithDirectoryString()
        {
            var constraint = new FileOrDirectoryExistsConstraint().IgnoreDirectories;
            Assert.That(constraint.ApplyTo(_goodDir.ToString()).Status == ConstraintStatus.Failure);
        }

        [Test]
        public void FailsWhenIgnoreDirectoriesIsTrueWithDirectoryInfo()
        {
            var constraint = new FileOrDirectoryExistsConstraint().IgnoreDirectories;
            var ex = Assert.Throws<ArgumentException>(() => constraint.ApplyTo(_goodDir.Directory));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a string or FileInfo"));
        }

        [Test]
        public void FailsWhenDirectoryInfoDoesNotExist()
        {
            var actual = new DirectoryInfo(BAD_DIRECTORY);
            Assert.That(_constraint.ApplyTo(actual).Status == ConstraintStatus.Failure);
            Assert.That(actual, Does.Not.Exist);
        }

        [Test]
        public void FailsWhenFileInfoDoesNotExist()
        {
            var actual = new FileInfo(BAD_FILE);
            Assert.That(_constraint.ApplyTo(actual).Status == ConstraintStatus.Failure);
            Assert.That(actual, Does.Not.Exist);
        }

        [Test]
        public void FailsWhenFileStringDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo(BAD_FILE).Status == ConstraintStatus.Failure);
            Assert.That(BAD_FILE, Does.Not.Exist);
        }

        [Test]
        public void FailsWhenDirectoryStringDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo(BAD_DIRECTORY).Status == ConstraintStatus.Failure);
            Assert.That(BAD_DIRECTORY, Does.Not.Exist);
        }

        [Test]
        public void FailsWhenNotStringOrDirectoryInfo()
        {
            var ex = Assert.Throws<ArgumentException>(() => _constraint.ApplyTo(42));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a string, FileInfo or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((FileInfo)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string, FileInfo or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenDirectoryInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((DirectoryInfo)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string, FileInfo or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((string)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string, FileInfo or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => _constraint.ApplyTo(string.Empty));
            Assert.That(ex.Message, Does.StartWith("The actual value cannot be an empty string"));
        }
    }
}
