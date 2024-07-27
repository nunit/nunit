// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET6_0_OR_GREATER
using System;
using System.Runtime.Versioning;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class NetPlatformDetectionTests
    {
        [Test]
        public void ShouldMatchOSPlatform()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NetPlatformHelper.IsPlatformSupported("Windows"), Is.EqualTo(OperatingSystem.IsWindows()), "Windows");
                Assert.That(NetPlatformHelper.IsPlatformSupported("Linux"), Is.EqualTo(OperatingSystem.IsLinux()), "Linux");
                Assert.That(NetPlatformHelper.IsPlatformSupported("MacOS"), Is.EqualTo(OperatingSystem.IsMacOS()), "MacOS");
            });
        }

        [Test]
        [SupportedOSPlatform("Windows")]
        public void ShouldRunOnWindows()
        {
            Assert.That(OperatingSystem.IsWindows(), Is.True, "Windows");
        }

        [Test]
        [UnsupportedOSPlatform("Windows")]
        public void ShouldNotRunOnWindows()
        {
            Assert.That(OperatingSystem.IsWindows(), Is.False, "Windows");
        }

        [Test]
        [SupportedOSPlatform("Linux")]
        public void ShouldRunOnLinux()
        {
            Assert.That(OperatingSystem.IsLinux(), Is.True, "Linux");
        }

        [Test]
        [UnsupportedOSPlatform("Linux")]
        public void ShouldNotRunOnLinux()
        {
            Assert.That(OperatingSystem.IsLinux(), Is.False, "Linux");
        }

        [Test]
        [SupportedOSPlatform("MacOS")]
        public void ShouldRunOnMacOS()
        {
            Assert.That(OperatingSystem.IsMacOS(), Is.True, "MacOS");
        }

        [Test]
        [UnsupportedOSPlatform("MacOS")]
        public void ShouldNotRunOnMacOs()
        {
            Assert.That(OperatingSystem.IsMacOS(), Is.False, "MacOS");
        }
    }
}

#endif
