// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_FileDirectoryTests
    {
        [Test]
        public void File_Exists_DoesNotExist_And_AreEqual_Via_Paths()
        {
            using var dir = new TestDirectory();
            using var tf1 = new TestFile(Path.Combine(dir.Directory.FullName, "a.txt"), "Hello", isContent: true);
            using var tf2 = new TestFile(Path.Combine(dir.Directory.FullName, "b.txt"), "Hello", isContent: true);

            Assert.FileExists(tf1.ToString());
            Assert.FileDoesNotExist(Path.Combine(dir.Directory.FullName, "missing.txt"));

            Assert.FileAreEqual(tf1.ToString(), tf2.ToString());
        }

        [Test]
        public void File_AreEqual_Via_Streams_And_FileInfo()
        {
            using var tf1 = new TestFile("TestImage1.jpg");
            using var tf2 = new TestFile("TestImage1.jpg");

            using (var s1 = File.OpenRead(tf1.ToString()))
            using (var s2 = File.OpenRead(tf2.ToString()))
            {
                Assert.AreEqual(s1, s2);
            }

            Assert.AreEqual(tf1.File, tf2.File);
        }

        [Test]
        public void Directory_Exists_DoesNotExist_And_AreNotEqual()
        {
            using var root = new TestDirectory();

            var d1 = Directory.CreateDirectory(Path.Combine(root.Directory.FullName, "d1"));
            var d2 = Directory.CreateDirectory(Path.Combine(root.Directory.FullName, "d2"));

            File.WriteAllText(Path.Combine(d1.FullName, "a.txt"), "x");
            File.WriteAllText(Path.Combine(d2.FullName, "a.txt"), "x");

            Assert.DirectoryExists(d1.FullName);
            Assert.DirectoryDoesNotExist(Path.Combine(root.Directory.FullName, "missing"));

            // Different directories (even with identical content) are not equal
            Assert.AreNotEqual(new DirectoryInfo(d1.FullName), new DirectoryInfo(d2.FullName));
        }
    }
}
