// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

#if NET8_0_OR_GREATER
using System;
using System.Linq;
using System.Runtime.Versioning;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class OSPlatformConverterTests
    {
        [TestCase("Windows")]
        [TestCase("Linux")]
        [TestCase("MacOS")]
        public void ConvertSupportedOSPlatformAttributeWindows7(string platformName)
        {
            var supported = new SupportedOSPlatformAttribute(platformName);

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(supported);
            Assert.That(platform.Includes, Is.EqualTo([platformName]), nameof(platform.Includes));
            Assert.That(platform.Excludes, Is.Empty, nameof(platform.Excludes));
        }

        [TestCase("Linux")]
        public void ConvertUnsupportedOSPlatformAttribute(string platformName)
        {
            var unsupported = new UnsupportedOSPlatformAttribute(platformName);

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(unsupported);
            Assert.That(platform.Includes, Is.Empty, nameof(platform.Includes));
            Assert.That(platform.Excludes, Is.EqualTo([platformName]), nameof(platform.Excludes));
        }

        [Test]
        public void ConvertMultipleOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Windows10.0");
            var supported3 = new SupportedOSPlatformAttribute("Linux");

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(supported1, supported2, supported3);
            Assert.That(platform.Includes, Does.Contain("Windows7.0"), nameof(platform.Includes));
            Assert.That(platform.Includes, Does.Contain("Windows10.0"), nameof(platform.Includes));
            Assert.That(platform.Includes, Does.Contain("Linux"), nameof(platform.Includes));
            Assert.That(platform.Excludes, Is.Empty, nameof(platform.Excludes));
        }

        [Test]
        public void ConvertMixedOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Linux");
            var unsupported = new UnsupportedOSPlatformAttribute("Android");

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(supported1, unsupported, supported2);
            Assert.That(platform.Includes, Does.Contain("Windows7.0"), nameof(platform.Includes));
            Assert.That(platform.Includes, Does.Contain("Linux"), nameof(platform.Includes));
            Assert.That(platform.Excludes, Does.Contain("Android"), nameof(platform.Excludes));
        }

        [Test]
        public void ConvertMixedNetPlatformAndOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows");
            var supported2 = new SupportedOSPlatformAttribute("Windows10");
            var sourcePlatform = new NetPlatformAttribute("Linux");

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(sourcePlatform, supported1, supported2);
            Assert.That(platform.Includes, Does.Contain("Windows"), nameof(platform.Includes));
            Assert.That(platform.Includes, Does.Contain("Windows10"), nameof(platform.Includes));
            Assert.That(platform.Includes, Does.Contain("Linux"), nameof(platform.Includes));
            Assert.That(platform.Excludes, Is.Empty, nameof(platform.Excludes));
        }

        [Test]
        public void ConvertNetPlatformAttribute()
        {
            var original = new NetPlatformAttribute("Windows");

            NetPlatformAttribute platform = ConvertIntoSinglePlatform(original);
            Assert.That(platform.Includes, Is.EqualTo(original.Includes), nameof(platform.Includes));
            Assert.That(platform.Excludes, Is.EqualTo(original.Excludes), nameof(platform.Excludes));
        }

        [Test]
        public void PassThroughAttribute()
        {
            IApplyToTest source = new TestAttribute();

            IApplyToTest result = OSPlatformConverter.Convert(Array.Empty<OSPlatformAttribute>(), new[] { source }).Single();
            Assert.That(result, Is.SameAs(source));
        }

        private static NetPlatformAttribute ConvertIntoSinglePlatform(params OSPlatformAttribute[] osPlatformAttributes)
        {
            return (NetPlatformAttribute)OSPlatformConverter.Convert(osPlatformAttributes, Array.Empty<IApplyToTest>()).Single();
        }

        private static NetPlatformAttribute ConvertIntoSinglePlatform(IApplyToTest applyToTestAttribute, params OSPlatformAttribute[] osPlatformAttributes)
        {
            return (NetPlatformAttribute)OSPlatformConverter.Convert(osPlatformAttributes, new[] { applyToTestAttribute }).Single();
        }
    }
}

#endif
