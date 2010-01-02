// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Reflection;

namespace NUnit.Framework.Assertions
{
    public class TestDirectory : IDisposable
    {
        private bool _disposedValue = false;
        public string directoryName;
        public DirectoryInfo directoryInformation;
        public DirectoryInfo diSubSubDirectory;

        #region TestDirectory Utility Class

        public TestDirectory(string directoryName) : this(directoryName, true) { }

        public TestDirectory(string directoryName, bool CreateSubDirectory)
        {
            this.directoryName = Path.Combine(Environment.CurrentDirectory, directoryName);

            directoryInformation = Directory.CreateDirectory(directoryName);

            if (CreateSubDirectory)
            {
                DirectoryInfo diSubDirectory = directoryInformation.CreateSubdirectory("SubDirectory");
                diSubSubDirectory = diSubDirectory.CreateSubdirectory("SubSubDirectory");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    if (Directory.Exists(directoryName))
                    {
                        Directory.Delete(directoryName,true);
                    }
                }
            }
            this._disposedValue = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }

    /// <summary>
	/// Summary description for DirectoryAssertTests.
	/// </summary>
	[TestFixture]
    public class DirectoryAssertTests : MessageChecker
    {
        #region AreEqual

        #region Success Tests
        [Test]
        public void AreEqualPassesWhenBothAreNull()
        {
            DirectoryInfo expected = null;
            DirectoryInfo actual = null;
            DirectoryAssert.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqualPassesWithDirectoryInfos()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryAssert.AreEqual(td.directoryInformation, td.directoryInformation);
            }
        }

        [Test]
        public void AreEqualPassesWithStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryAssert.AreEqual(td.directoryName, td.directoryName);
            }
        }
        #endregion

        #region Failure Tests
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFailsWhenOneIsNull()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {               
                DirectoryAssert.AreEqual(td.directoryInformation, null);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFailsWhenOneDoesNotExist()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryInfo actual = new DirectoryInfo("NotExistingDirectoryName");
                DirectoryAssert.AreEqual(td.directoryInformation, actual);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFailsWithDirectoryInfos()
        {
            using (TestDirectory td1 = new TestDirectory("ParentDirectory1"))
            {
                using (TestDirectory td2 = new TestDirectory("ParentDirectory2"))
                {
                    DirectoryAssert.AreEqual(td1.directoryInformation, td2.directoryInformation);
                }
            }
        }


        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFailsWithStringPath()
        {
            using (TestDirectory td1 = new TestDirectory("ParentDirectory1"))
            {
                using (TestDirectory td2 = new TestDirectory("ParentDirectory2"))
                {
                    DirectoryAssert.AreEqual(td1.directoryName, td2.directoryName);
                }
            }
        }
        #endregion

        #endregion

        #region AreNotEqual

        #region Success Tests
        [Test]
        public void AreNotEqualPassesIfOneIsNull()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryAssert.AreNotEqual(td.directoryInformation, null);
            }
        }

        [Test]
        public void AreNotEqualPassesWhenOneDoesNotExist()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryInfo actual = new DirectoryInfo("NotExistingDirectoryName");
                DirectoryAssert.AreNotEqual(td.directoryInformation, actual);
            }
        }

        public void AreNotEqualPassesWithDirectoryInfos()
        {
            using (TestDirectory td1 = new TestDirectory("ParentDirectory1"))
            {
                using (TestDirectory td2 = new TestDirectory("ParentDirectory2"))
                {
                    DirectoryAssert.AreNotEqual(td1.directoryInformation, td2.directoryInformation);
                }
            }
        }

        [Test]
        public void AreNotEqualPassesWithStringPath()
        {
            using (TestDirectory td1 = new TestDirectory("ParentDirectory1"))
            {
                using (TestDirectory td2 = new TestDirectory("ParentDirectory2"))
                {
                    DirectoryAssert.AreNotEqual(td1.directoryName, td2.directoryName);
                }
            }
        }
        #endregion

        #region Failure Tests
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFailsWhenBothAreNull()
        {
            DirectoryInfo expected = null;
            DirectoryInfo actual = null;
            expectedMessage =
    "  Expected: not null" + Environment.NewLine +
    "  But was:  null" + Environment.NewLine;
            DirectoryAssert.AreNotEqual(expected, actual);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFailsWithDirectoryInfos()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryAssert.AreNotEqual(td.directoryInformation, td.directoryInformation);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFailsWithStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory"))
            {
                DirectoryAssert.AreNotEqual(td.directoryName, td.directoryName);
            }
        }
        #endregion

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmptyPassesWithEmptyDirectoryUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsEmpty(td.directoryInformation);
            }
        }

        [Test]
        public void IsEmptyPassesWithEmptyDirectoryUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsEmpty(td.directoryName);
            }
        }

        [Test, ExpectedException(typeof(DirectoryNotFoundException))]
        public void IsEmptyFailsWithInvalidDirectory()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsEmpty(td.directoryName + "INVALID");
            }
        }

        [Test,ExpectedException(typeof(ArgumentException))]
        public void IsEmptyThrowsUsingNull()
        {
            DirectoryAssert.IsEmpty((DirectoryInfo)null);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsWithNonEmptyDirectoryUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsEmpty(td.directoryInformation);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsWithNonEmptyDirectoryUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsEmpty(td.directoryName);
            }
        }

        #endregion

        #region IsNotEmpty

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsWithEmptyDirectoryUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsNotEmpty(td.directoryInformation);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsWithEmptyDirectoryUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsNotEmpty(td.directoryName);
            }
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IsNotEmptyThrowsUsingNull()
        {
            DirectoryAssert.IsNotEmpty((DirectoryInfo) null);
        }

        [Test, ExpectedException(typeof(DirectoryNotFoundException))]
        public void IsNotEmptyFailsWithInvalidDirectory()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", false))
            {
                DirectoryAssert.IsNotEmpty(td.directoryName + "INVALID");
            }
        }

        [Test]
        public void IsNotEmptyPassesWithNonEmptyDirectoryUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsNotEmpty(td.directoryInformation);
            }
        }

        [Test]
        public void IsNotEmptyPassesWithNonEmptyDirectoryUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsNotEmpty(td.directoryName);
            }
        }

        #endregion

        #region IsWithin

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IsWithinThrowsWhenBothAreNull()
        {
            DirectoryInfo expected = null;
            DirectoryInfo actual = null;
            DirectoryAssert.IsWithin(expected, actual);
        }

        [Test]
        public void IsWithinPassesWithDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory",true))
            {
                DirectoryAssert.IsWithin(td.directoryInformation, td.diSubSubDirectory);
            }
        }

        [Test]
        public void IsWithinPassesWithStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsWithin(td.directoryName, td.diSubSubDirectory.FullName);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsWithinFailsWhenOutsidePathUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryInfo diSystemFolder = new DirectoryInfo(Environment.SpecialFolder.System.ToString());
                DirectoryAssert.IsWithin(td.directoryInformation, diSystemFolder);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsWithinFailsWhenOutsidePathUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsWithin(td.directoryName, Environment.SpecialFolder.System.ToString());
            }
        }

        #endregion

        #region IsNotWithin

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IsNotWithinThrowsWhenBothAreNull()
        {
            DirectoryInfo expected = null;
            DirectoryInfo actual = null;
            DirectoryAssert.IsNotWithin(expected, actual);
        }

        [Test]
        public void IsNotWithinPassesWhenOutsidePathUsingDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryInfo diSystemFolder = new DirectoryInfo(Environment.SpecialFolder.System.ToString());
                DirectoryAssert.IsNotWithin(td.directoryInformation, diSystemFolder);
            }
        }

        [Test]
        public void IsNotWithinPassesWhenOutsidePathUsingStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsNotWithin(td.directoryName, Environment.SpecialFolder.System.ToString());
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsNotWithinFailsWithDirectoryInfo()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsNotWithin(td.directoryInformation, td.diSubSubDirectory);
            }
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsNotWithinFailsWithStringPath()
        {
            using (TestDirectory td = new TestDirectory("ParentDirectory", true))
            {
                DirectoryAssert.IsNotWithin(td.directoryName, td.diSubSubDirectory.FullName);
            }
        }

        #endregion
    }
}
