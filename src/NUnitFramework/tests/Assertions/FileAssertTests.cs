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

#if !SILVERLIGHT && !PORTABLE
using System;
using System.IO;
using System.Reflection;
using System.Net.Sockets;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Summary description for FileAssertTests.
    /// </summary>
    [TestFixture]
    public class FileAssertTests
    {
        private readonly static string BAD_FILE = Path.Combine(Path.GetTempPath(), "garbage.txt");

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
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            using (FileStream actual = tf2.File.OpenRead())
            {
                FileAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void NonReadableStreamGivesException()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            using (FileStream actual = tf2.File.OpenWrite())
            {
                var ex = Assert.Throws<ArgumentException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Does.Contain("not readable"));
            }
        }

        [Test]
        public void NonSeekableStreamGivesException()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            using (var actual = new FakeStream())
            {
                var ex = Assert.Throws<ArgumentException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Does.Contain("not seekable"));
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
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                FileAssert.AreEqual(tf1.File.FullName, tf2.File.FullName, "Failed using file names");
            }
        }

        [Test]
        public void AreEqualPassesUsingSameFileTwice()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            {
                FileAssert.AreEqual(tf1.File.FullName, tf1.File.FullName);
            }
        }

        [Test]
        public void AreEqualPassesWithFileInfos()
        {
            using (var expectedTestFile = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var actualTestFile = new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                FileAssert.AreEqual(expectedTestFile.File, actualTestFile.File);
            }
        }

        [Test]
        public void AreEqualPassesWithTextFiles()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            using (var tf2 = new TestFile("Test2.txt", "TestText1.txt"))
            {
                FileAssert.AreEqual(tf1.File.FullName, tf2.File.FullName);
            }
        }
#endregion

#region Failure Tests
        [Test]
        public void AreEqualFailsWhenOneIsNull()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
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
            using (var tf1 = new TestFile(expectedFile, "TestImage1.jpg"))
            using (var tf2 = new TestFile(actualFile, "TestImage2.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            using (FileStream actual = tf2.File.OpenRead())
            {
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        tf1.File.Length, tf2.File.Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expected, actual));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithFileInfos()
        {
            using (var expectedTestFile = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var actualTestFile = new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        expectedTestFile.File.Length, actualTestFile.File.Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expectedTestFile.File, actualTestFile.File));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithFiles()
        {
            string expected = "Test1.jpg";
            string actual = "Test2.jpg";
            using (var expectedTestFile = new TestFile(expected, "TestImage1.jpg"))
            using (var actualTestFile = new TestFile(actual, "TestImage2.jpg"))
            {
                var expectedMessage =
                    string.Format("  Expected Stream length {0} but was {1}." + Environment.NewLine,
                        expectedTestFile.File.Length, actualTestFile.File.Length);
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(expectedTestFile.File.FullName, actualTestFile.File.FullName));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreEqualFailsWithTextFilesAfterReadingBothFiles()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            using (var tf2 = new TestFile("Test2.txt", "TestText2.txt"))
            {
                var expectedMessage = string.Format(
                    "  Stream lengths are both {0}. Streams differ at offset {1}." + Environment.NewLine,
                    tf1.FileLength,
                    tf1.OffsetOf('!'));
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreEqual(tf1.File.FullName, tf2.File.FullName));
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
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            {
                FileAssert.AreNotEqual(expected, null);
            }
        }

        [Test]
        public void AreNotEqualPassesWithStreams()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage2.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            {
                using (FileStream actual = tf2.File.OpenRead())
                {
                    FileAssert.AreNotEqual(expected, actual);
                }
            }
        }

        [Test]
        public void AreNotEqualPassesWithFiles()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                FileAssert.AreNotEqual(tf1.File.FullName, tf2.File.FullName);
            }
        }

        [Test]
        public void AreNotEqualPassesWithFileInfos()
        {
            using (var expectedTestFile = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var actualTestFile = new TestFile("Test2.jpg", "TestImage2.jpg"))
            {
                FileAssert.AreNotEqual(expectedTestFile.File, actualTestFile.File);
            }
        }

        [Test]
        public void AreNotEqualIteratesOverTheEntireFile()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            using (var tf2 = new TestFile("Test2.txt", "TestText2.txt"))
            {
                FileAssert.AreNotEqual(tf1.File.FullName, tf2.File.FullName);
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
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var tf2 = new TestFile("Test2.jpg", "TestImage1.jpg"))
            using (FileStream expected = tf1.File.OpenRead())
            using (FileStream actual = tf2.File.OpenRead())
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
            using (var expectedTestFile = new TestFile("Test1.jpg", "TestImage1.jpg"))
            using (var actualTestFile = new TestFile("Test2.jpg", "TestImage1.jpg"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(expectedTestFile.File, actualTestFile.File));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreNotEqualFailsWithFiles()
        {
            using (var tf1 = new TestFile("Test1.jpg", "TestImage1.jpg"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(tf1.File.FullName, tf1.File.FullName));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }

        [Test]
        public void AreNotEqualIteratesOverTheEntireFileAndFails()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            using (var tf2 = new TestFile("Test2.txt", "TestText1.txt"))
            {
                var expectedMessage =
                    "  Expected: not equal to <System.IO.FileStream>" + Environment.NewLine +
                    "  But was:  <System.IO.FileStream>" + Environment.NewLine;
                var ex = Assert.Throws<AssertionException>(() => FileAssert.AreNotEqual(tf1.File.FullName, tf2.File.FullName));
                Assert.That(ex.Message, Is.EqualTo(expectedMessage));
            }
        }
#endregion

#endregion

#region Exists

        [Test]
        public void ExistsPassesWhenFileInfoExists()
        {
            using (var actualTestFile = new TestFile("Test1.txt", "TestText1.txt"))
            {
                FileAssert.Exists(actualTestFile.File);
            }
        }

        [Test]
        public void ExistsPassesWhenStringExists()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            {
                FileAssert.Exists(tf1.File.FullName);
            }
        }

        [Test]
        public void ExistsFailsWhenFileInfoDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => FileAssert.Exists(new FileInfo(BAD_FILE)));
            Assert.That(ex.Message, Does.StartWith("  Expected: file exists"));
        }

        [Test]
        public void ExistsFailsWhenStringDoesNotExist()
        {
            var ex = Assert.Throws<AssertionException>(() => FileAssert.Exists(BAD_FILE));
            Assert.That(ex.Message, Does.StartWith("  Expected: file exists"));
        }

        [Test]
        public void ExistsFailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.Exists((FileInfo)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.Exists((string)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void ExistsFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => FileAssert.Exists(string.Empty));
            Assert.That(ex.Message, Does.StartWith("The actual value cannot be an empty string"));
        }

#endregion

#region DoesNotExist

        [Test]
        public void DoesNotExistFailsWhenFileInfoExists()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            {
                var ex = Assert.Throws<AssertionException>(() => FileAssert.DoesNotExist(tf1.File));
                Assert.That(ex.Message, Does.StartWith("  Expected: not file exists"));
            }
        }

        [Test]
        public void DoesNotExistFailsWhenStringExists()
        {
            using (var tf1 = new TestFile("Test1.txt", "TestText1.txt"))
            {
                var ex = Assert.Throws<AssertionException>(() => FileAssert.DoesNotExist(tf1.File.FullName));
                Assert.That(ex.Message, Does.StartWith("  Expected: not file exists"));
            }
        }

        [Test]
        public void DoesNotExistPassesWhenFileInfoDoesNotExist()
        {
            FileAssert.DoesNotExist(new FileInfo(BAD_FILE));
        }

        [Test]
        public void DoesNotExistPassesWhenStringDoesNotExist()
        {
            FileAssert.DoesNotExist(BAD_FILE);
        }

        [Test]
        public void DoesNotExistFailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.DoesNotExist((FileInfo)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => FileAssert.DoesNotExist((string)null));
            Assert.That(ex.Message, Does.StartWith("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void DoesNotExistFailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => FileAssert.DoesNotExist(string.Empty));
            Assert.That(ex.Message, Does.StartWith("The actual value cannot be an empty string"));
        }

        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => FileAssert.Equals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("FileAssert.Equals should not be used for Assertions"));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => FileAssert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("FileAssert.ReferenceEquals should not be used for Assertions"));
        }

        #endregion
    }
}
#endif
