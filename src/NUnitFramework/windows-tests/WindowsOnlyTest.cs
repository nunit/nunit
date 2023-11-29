using System.IO;
using NUnit.Framework;

namespace NUnit.Windows.Tests
{
    [TestFixture]
    public sealed class PathTest
    {
        [Test]
        public void DirectorySeparator()
        {
            Assert.That(Path.DirectorySeparatorChar, Is.EqualTo('\\'));
        }

        [Test]
        public void VolumeSeparator()
        {
            Assert.That(Path.VolumeSeparatorChar, Is.EqualTo(':'));
        }

        [Test]
        public void PathSeparator()
        {
            Assert.That(Path.PathSeparator, Is.EqualTo(';'));
        }
    }
}
