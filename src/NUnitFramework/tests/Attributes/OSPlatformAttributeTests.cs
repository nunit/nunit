// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NET5_0_OR_GREATER

using System.IO;
using System.Runtime.Versioning;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class OSPlatformAttributeTests : ThreadingTests
    {
        [Test]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("OSX")]
        public void SupportedForwardSlashDirectorySeparator()
        {
            Assert.That(Path.DirectorySeparatorChar, Is.EqualTo('/'));
        }

        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Windows10.0")]
        [SupportedOSPlatform("Windows11.0")]
        [Test]
        public void SupportedBackwardSlashDirectorySeparator()
        {
            Assert.That(Path.DirectorySeparatorChar, Is.EqualTo('\\'));
        }

        [Test]
        [UnsupportedOSPlatform("Windows")]
        public void UnsupportedForwardSlashDirectorySeparator()
        {
            Assert.That(Path.DirectorySeparatorChar, Is.EqualTo('/'));
        }

        [UnsupportedOSPlatform("Linux")]
        [UnsupportedOSPlatform("MacOS")]
        [Test]
        public void UnsupportedBackwardSlashDirectorySeparator()
        {
            Assert.That(Path.DirectorySeparatorChar, Is.EqualTo('\\'));
        }
    }
}

#endif
