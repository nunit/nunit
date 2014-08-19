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

using System.IO;
#if !NUNITLITE
#region Using Directives

using System;
using NUnit.Framework;

#endregion

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class DirectoryAssertTests
    {
        private string _goodDir;
        private string _appDataDir;
        private const string BAD_DIRECTORY = @"Z:\I\hope\this\is\garbage";

        [SetUp]
        public void SetUp()
        {
            _goodDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        #region AreEqual

        #region Success Tests

        [Test]
        public void AreEqualPassesWithDirectories()
        {
            DirectoryAssert.AreEqual(_goodDir, _goodDir, "Failed using directory names");
        }

        [Test]
        public void AreEqualPassesWithDirectoryInfos()
        {
            DirectoryInfo expected = new DirectoryInfo(_goodDir);
            DirectoryInfo actual = new DirectoryInfo(_goodDir);
            DirectoryAssert.AreEqual(expected, actual);
            DirectoryAssert.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqualPassesWithTextDirectories()
        {
            DirectoryAssert.AreEqual(_goodDir, _goodDir);
        }
        #endregion

        #region Failure Tests

        [Test]
        public void AreEqualFailsWithDirectoryInfos()
        {
            DirectoryInfo expected = new DirectoryInfo(_goodDir);
            DirectoryInfo actual = new DirectoryInfo(_appDataDir);
            var expectedMessage =
                string.Format("  Expected: <{0}>{2}  But was:  <{1}>{2}",
                    expected.FullName, actual.FullName, Environment.NewLine);
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.AreEqual(expected, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }


        [Test]
        public void AreEqualFailsWithDirectories()
        {
            string expected = _goodDir;
            string actual = _appDataDir;
            var expectedMessage =
                string.Format("  Expected: <{0}>{2}  But was:  <{1}>{2}",
                    new DirectoryInfo(expected).FullName, new DirectoryInfo(actual).FullName, Environment.NewLine);
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
            DirectoryAssert.AreNotEqual(new DirectoryInfo(_goodDir), null);
        }

        [Test]
        public void AreNotEqualPassesIfActualIsNull()
        {
            DirectoryAssert.AreNotEqual(null, new DirectoryInfo(_goodDir));
        }

        [Test]
        public void AreNotEqualPassesWithDirectories()
        {
            DirectoryAssert.AreNotEqual(_goodDir, _appDataDir);
        }

        [Test]
        public void AreNotEqualPassesWithDirectoryInfos()
        {
            DirectoryInfo expected = new DirectoryInfo(_goodDir);
            DirectoryInfo actual = new DirectoryInfo(_appDataDir);
            DirectoryAssert.AreNotEqual(expected, actual);
        }
        #endregion

        #region Failure Tests

        [Test]
        public void AreNotEqualFailsWithDirectoryInfos()
        {
            DirectoryInfo expected = new DirectoryInfo(_goodDir);
            DirectoryInfo actual = new DirectoryInfo(_goodDir);
            var expectedMessage = string.Format(
                "  Expected: not equal to <{0}>{2}  But was:  <{1}>{2}",
                expected.FullName,
                actual.FullName,
                Environment.NewLine );
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.AreNotEqual(expected, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreNotEqualFailsWithDirectories()
        {
            var expectedMessage = string.Format(
                "  Expected: not equal to <{0}>{1}  But was:  <{0}>{1}",
                _goodDir,
                Environment.NewLine);
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.AreNotEqual(_goodDir, _goodDir));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        #endregion

        #endregion

        #region Exists

        [Test]
        public void ExistsPassesWhenDirectoryInfoExists()
        {
            var actual = new DirectoryInfo(_goodDir);
            DirectoryAssert.Exists(actual);
        }

        [Test]
        public void ExistsPassesWhenStringExists()
        {
            DirectoryAssert.Exists(_goodDir);
        }

        [Test]
        public void ExistsFailsWhenDirectoryInfoDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.Exists(new DirectoryInfo(BAD_DIRECTORY)));
            Assert.That(ex.Message, Is.StringStarting("  Expected: directory exists"));
        }

        [Test]
        public void ExistsFailsWhenStringDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.Exists(BAD_DIRECTORY));
            Assert.That(ex.Message, Is.StringStarting("  Expected: directory exists"));
        }

        [Test]
        public void ExistsFailsWhenDirectoryInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.Exists((DirectoryInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.Exists((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => DirectoryAssert.Exists(string.Empty));
            Assert.That(ex.Message, Is.StringStarting("The actual value cannot be an empty string"));
        }

        #endregion

        #region DoesNotExist

        [Test]
        public void DoesNotExistFailsWhenDirectoryInfoExists()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.DoesNotExist(new DirectoryInfo(_goodDir)));
            Assert.That(ex.Message, Is.StringStarting("  Expected: not directory exists"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringExists()
        {
            var ex = Assert.Throws<AssertionException>(() => DirectoryAssert.DoesNotExist(_goodDir));
            Assert.That(ex.Message, Is.StringStarting("  Expected: not directory exists"));
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
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.DoesNotExist((DirectoryInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => DirectoryAssert.DoesNotExist((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => DirectoryAssert.DoesNotExist(string.Empty));
            Assert.That(ex.Message, Is.StringStarting("The actual value cannot be an empty string"));
        }

        #endregion
    }
}
#endif