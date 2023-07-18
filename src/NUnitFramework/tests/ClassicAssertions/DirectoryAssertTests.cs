// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using NUnit.Framework.Classic;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class DirectoryAssertTests
    {
        private TestDirectory _goodDir1;
        private TestDirectory _goodDir2;
        private const string BAD_DIRECTORY = @"\I\hope\this\is\garbage";

        [SetUp]
        public void SetUp()
        {
            _goodDir1 = new TestDirectory();
            _goodDir2 = new TestDirectory();
            Assume.That(_goodDir1.Directory, Is.Not.EqualTo(_goodDir2.Directory), "The two good directories are the same");
            Assume.That(BAD_DIRECTORY, Does.Not.Exist, BAD_DIRECTORY + " exists");
        }

        [TearDown]
        public void TearDown()
        {
            _goodDir1?.Dispose();
            _goodDir2?.Dispose();
        }

        #region AreEqual

        #region Success Tests

        [Test]
        public void AreEqualPassesWithDirectoryInfos()
        {
            var expected = new DirectoryInfo(_goodDir1.ToString());
            var actual = new DirectoryInfo(_goodDir1.ToString());
            DirectoryAssert.AreEqual(expected, actual);
            DirectoryAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Failure Tests

        [Test]
        public void AreEqualFailsWithDirectoryInfos()
        {
            var expected = _goodDir1.Directory;
            var actual = _goodDir2.Directory;
            var expectedMessage =
                string.Format("  Expected: <{0}>{2}  But was:  <{1}>{2}",
                    expected.FullName, actual.FullName, Environment.NewLine);
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.AreEqual(expected, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        #endregion

        #endregion

        #region AreNotEqual

        #region Success Tests
        [Test]
        public void AreNotEqualPassesIfExpectedIsNull()
        {
            DirectoryAssert.AreNotEqual(_goodDir1.Directory, null);
        }

        [Test]
        public void AreNotEqualPassesIfActualIsNull()
        {
            DirectoryAssert.AreNotEqual(null, _goodDir1.Directory);
        }

        [Test]
        public void AreNotEqualPassesWithDirectoryInfos()
        {
            var expected = _goodDir1.Directory;
            var actual = _goodDir2.Directory;
            DirectoryAssert.AreNotEqual(expected, actual);
        }
        #endregion

        #region Failure Tests

        [Test]
        public void AreNotEqualFailsWithDirectoryInfos()
        {
            var expected = new DirectoryInfo(_goodDir1.ToString());
            var actual = new DirectoryInfo(_goodDir1.ToString());
            var expectedMessage = string.Format(
                "  Expected: not equal to <{0}>{2}  But was:  <{1}>{2}",
                expected.FullName,
                actual.FullName,
                Environment.NewLine);
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.AreNotEqual(expected, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        #endregion

        #endregion

        #region Exists

        [Test]
        public void ExistsPassesWhenDirectoryInfoExists()
        {
            DirectoryAssert.Exists(_goodDir1.Directory);
        }

        [Test]
        public void ExistsPassesWhenStringExists()
        {
            DirectoryAssert.Exists(_goodDir1.ToString());
        }

        [Test]
        public void ExistsFailsWhenDirectoryInfoDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.Exists(new DirectoryInfo(BAD_DIRECTORY)));
            Assert.That(ex.Message, Does.StartWith("  Expected: directory exists"));
        }

        [Test]
        public void ExistsFailsWhenStringDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.Exists(BAD_DIRECTORY));
            Assert.That(ex.Message, Does.StartWith("  Expected: directory exists"));
        }

        [Test]
        public void ExistsFailsWhenDirectoryInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.Exists(default(DirectoryInfo)!));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.Exists(default(string)!));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => DirectoryAssert.Exists(string.Empty));
            Assert.That(ex.Message, Does.StartWith("The actual value cannot be an empty string"));
        }

        #endregion

        #region DoesNotExist

        [Test]
        public void DoesNotExistFailsWhenDirectoryInfoExists()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.DoesNotExist(_goodDir1.Directory));
            Assert.That(ex.Message, Does.StartWith("  Expected: not directory exists"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringExists()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.DoesNotExist(_goodDir1.ToString()));
            Assert.That(ex.Message, Does.StartWith("  Expected: not directory exists"));
        }

        [Test]
        public void DoesNotExistPassesWhenDirectoryInfoDoesNotExist()
        {
            DirectoryAssert.DoesNotExist(new DirectoryInfo(BAD_DIRECTORY));
        }

        [Test]
        public void DoesNotExistPassesWhenStringDoesNotExist()
        {
            DirectoryAssert.DoesNotExist(BAD_DIRECTORY);
        }

        [Test]
        public void DoesNotExistFailsWhenDirectoryInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.DoesNotExist(default(DirectoryInfo)!));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.DoesNotExist(default(string)!));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => DirectoryAssert.DoesNotExist(string.Empty));
            Assert.That(ex.Message, Does.StartWith("The actual value cannot be an empty string"));
        }

        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => DirectoryAssert.Equals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("DirectoryAssert.Equals should not be used."));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => DirectoryAssert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("DirectoryAssert.ReferenceEquals should not be used."));
        }

        #endregion
    }
}
