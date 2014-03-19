// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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

#if !NUNITLITE
using System;
using System.IO;
using System.Reflection;
using System.Net.Sockets;

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Summary description for FileAssertTests.
    /// </summary>
    [TestFixture]
    public class FileAssertTests
    {
        #region AreEqual

        #region Success Tests
        [Test]
        public void AreEqualPassesWhenBothAreNull()
        {
            FileStream expected = null;
            FileStream actual = null;
            FileAssert.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqualPassesWithSameStream()
        {
            Stream exampleStream = new MemoryStream(new byte[] { 1, 2, 3 });
            Assert.That(exampleStream, Is.EqualTo(exampleStream));
        }

        [Test]
        public void AreEqualPassesWithEqualStreams()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            using (FileStream actual = File.OpenRead("Test2.jpg"))
            {
                FileAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void NonReadableStreamGivesException()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            using (FileStream actual = File.OpenWrite("Test2.jpg"))
            {
                var ex = Assert.Throws<ArgumentException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Contains.Substring("not readable"));
            }
        }

        [Test]
        public void NonSeekableStreamGivesException()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            using (FakeStream actual = new FakeStream())
            {
                var ex = Assert.Throws<ArgumentException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Contains.Substring("not seekable"));
            }
        }

        private class FakeStream : MemoryStream
        {
            public override bool CanSeek
            {
                get { return false; }
            }
        }

        [Test]
        public void AreEqualPassesWithFiles()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                FileAssert.AreEqual("Test1.jpg", "Test2.jpg", "Failed using file names");
            }
        }

        [Test]
        public void AreEqualPassesUsingSameFileTwice()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            {
                FileAssert.AreEqual("Test1.jpg", "Test1.jpg");
            }
        }

        [Test]
        public void AreEqualPassesWithFileInfos()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                FileInfo expected = new FileInfo("Test1.jpg");
                FileInfo actual = new FileInfo("Test2.jpg");
                FileAssert.AreEqual(expected, actual);
                FileAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AreEqualPassesWithTextFiles()
        {
            using (new TestFile("Test1.txt", "TestText1.txt"))
            using (new TestFile("Test2.txt", "TestText1.txt"))
            {
                FileAssert.AreEqual("Test1.txt", "Test2.txt");
            }
        }
        #endregion

        #region Failure Tests
        [Test]
        public void AreEqualFailsWhenOneIsNull()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            {
                var expectedMessage =
                    "  Expected: <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  null" + Environment.NewLine;

                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expected, null));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithStreams()
        {
            string expectedFile = "Test1.jpg";
            string actualFile = "Test2.jpg";
            using (new TestFile(expectedFile, "TestImage1.jpg"))
            using (new TestFile(actualFile, "TestImage2.jpg"))
            using (FileStream expected = File.OpenRead(expectedFile))
            using (FileStream actual = File.OpenRead(actualFile))
            {
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        new FileInfo(expectedFile).Length, new FileInfo(actualFile).Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithFileInfos()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                FileInfo expected = new FileInfo("Test1.jpg");
                FileInfo actual = new FileInfo("Test2.jpg");
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        expected.Length, actual.Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }


        [Test]
        public void AreEqualFailsWithFiles()
        {
            string expected = "Test1.jpg";
            string actual = "Test2.jpg";
            using (new TestFile(expected, "TestImage1.jpg"))
            using (new TestFile(actual, "TestImage2.jpg"))
            {
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        new FileInfo(expected).Length, new FileInfo(actual).Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithTextFilesAfterReadingBothFiles()
        {
            using (TestFile tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            using (new TestFile("Test2.txt", "TestText2.txt"))
            {
                var expectedMessage = string.Format(
                    "  Stream lengths are both {0}. Streams differ at offset {1}." + Environment.NewLine,
                    tf1.FileLength,
                    tf1.OffsetOf('!'));
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual("Test1.txt", "Test2.txt"));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }
        #endregion

        #endregion

        #region AreNotEqual

        #region Success Tests
        [Test]
        public void AreNotEqualPassesIfOneIsNull()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            {
                FileAssert.AreNotEqual(expected, null);
            }
        }

        [Test]
        public void AreNotEqualPassesWithStreams()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage2.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            {
                using (FileStream actual = File.OpenRead("Test2.jpg"))
                {
                    FileAssert.AreNotEqual(expected, actual);
                }
            }
        }

        [Test]
        public void AreNotEqualPassesWithFiles()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                FileAssert.AreNotEqual("Test1.jpg", "Test2.jpg");
            }
        }

        [Test]
        public void AreNotEqualPassesWithFileInfos()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                FileInfo expected = new FileInfo("Test1.jpg");
                FileInfo actual = new FileInfo("Test2.jpg");
                FileAssert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void AreNotEqualIteratesOverTheEntireFile()
        {
            using (new TestFile("Test1.txt", "TestText1.txt"))
            using (new TestFile("Test2.txt", "TestText2.txt"))
            {
                FileAssert.AreNotEqual("Test1.txt", "Test2.txt");
            }
        }
        #endregion

        #region Failure Tests
        [Test]
        public void AreNotEqualFailsWhenBothAreNull()
        {
            FileStream expected = null;
            FileStream actual = null;
            var expectedMessage =
                "  Expected: not equal to null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(expected, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreNotEqualFailsWithStreams()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = File.OpenRead("Test1.jpg"))
            using (FileStream actual = File.OpenRead("Test2.jpg"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreNotEqualFailsWithFileInfos()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                FileInfo expected = new FileInfo("Test1.jpg");
                FileInfo actual = new FileInfo("Test2.jpg");
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreNotEqualFailsWithFiles()
        {
            using (new TestFile("Test1.jpg", "TestImage1.jpg"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual("Test1.jpg", "Test1.jpg"));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreNotEqualIteratesOverTheEntireFileAndFails()
        {
            using (new TestFile("Test1.txt", "TestText1.txt"))
            using (new TestFile("Test2.txt", "TestText1.txt"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual("Test1.txt", "Test2.txt"));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }
        #endregion

        #endregion

        #region Exists

        [Test]
        public void ExistsPassesWhenFileInfoExists()
        {
            using (new TestFile("Test1.txt", "TestText1.txt"))
            {
                var actual = new FileInfo("Test1.txt");
                FileAssert.Exists(actual);
            }
        }

        [Test]
        public void ExistsPassesWhenStringExists()
        {
            using (new TestFile("Test1.txt", "TestText1.txt"))
            {
                FileAssert.Exists("Test1.txt");
            }
        }

        [Test]
        public void ExistsFailsWhenFileInfoDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => FileAssert.Exists(new FileInfo("Garbage.txt")));
            Assert.That(ex.Message, Is.StringStarting("  Expected: file exists"));
        }

        [Test]
        public void ExistsFailsWhenStringDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => FileAssert.Exists("Garbage.txt"));
            Assert.That(ex.Message, Is.StringStarting("  Expected: file exists"));
        }

        [Test]
        public void ExistsFailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.Exists((FileInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.Exists((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => FileAssert.Exists(string.Empty));
            Assert.That(ex.Message, Is.StringStarting("The actual value cannot be an empty string"));
        }

        #endregion

        #region DoesNotExist

        [Test]
        public void DoesNotExistFailsWhenFileInfoExists()
        {
            using(new TestFile("Test1.txt", "TestText1.txt"))
            {
                var ex = Assert.Throws<AssertionException>(() => FileAssert.DoesNotExist(new FileInfo("Test1.txt")));
                Assert.That(ex.Message, Is.StringStarting("  Expected: not file exists"));
            }
        }

        [Test]
        public void DoesNotExistFailsWhenStringExists()
        {
            using(new TestFile("Test1.txt", "TestText1.txt"))
            {
                var ex = Assert.Throws<AssertionException>(() => FileAssert.DoesNotExist("Test1.txt"));
                Assert.That(ex.Message, Is.StringStarting("  Expected: not file exists"));
            }
        }

        [Test]
        public void DoesNotExistPassesWhenFileInfoDoesNotExist()
        {
            FileAssert.DoesNotExist(new FileInfo("Garbage.txt"));
        }

        [Test]
        public void DoesNotExistPassesWhenStringDoesNotExist()
        {
            FileAssert.DoesNotExist("Garbage.txt");
        }

        [Test]
        public void DoesNotExistFailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.DoesNotExist((FileInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.DoesNotExist((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => FileAssert.DoesNotExist(string.Empty));
            Assert.That(ex.Message, Is.StringStarting("The actual value cannot be an empty string"));
        }

        #endregion
    }

    #region TestFile Utility Class

    public class TestFile : IDisposable
    {
        private bool _disposedValue = false;
        private string _resourceName;
        private string _fileName;
        private long _fileLength;

        public TestFile(string fileName, string resourceName)
        {
            _resourceName = "NUnit.Framework.Tests." + resourceName;
            _fileName = fileName;
            _fileLength = 0L;

            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null) throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                byte[] buffer = new byte[1024];
                using (FileStream fs = File.Create(_fileName))
                {
                    while (true)
                    {
                        int count = s.Read(buffer, 0, buffer.Length);
                        if (count == 0) break;
                        fs.Write(buffer, 0, count);
                        _fileLength += count;
                    }
                }
            }
        }

        public long FileLength
        {
            get { return _fileLength; }
        }

        public long OffsetOf(char target)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null) throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                byte[] buffer = new byte[1024];
                long offset = 0L;

                while (true)
                {
                    int count = s.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    foreach (char c in buffer)
                        if (c == target)
                            return offset;
                        else
                            offset++;
                }

                return -1L;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (File.Exists(_fileName))
                    {
                        File.Delete(_fileName);
                    }
                }
            }
            _disposedValue = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #endregion
}
#endif
