// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

#if NET5_0_OR_GREATER
using System;
using System.Linq;
using System.Runtime.Versioning;
using NUnit.Framework.Interfaces;
#endif

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class OSPlatformTranslatorTests
    {
        [TestCase("Windows", ExpectedResult = "Win")]
        [TestCase("Windows7.0", ExpectedResult = "Windows7")]
        [TestCase("Windows10.0", ExpectedResult = "Windows10")]
        [TestCase("Windows11.0", ExpectedResult = "Windows11")]
        [TestCase("Linux", ExpectedResult = "Linux")]
        [TestCase("OSX", ExpectedResult = "MacOsX")]
        [TestCase("MacOS", ExpectedResult = "MacOsX")]
        [TestCase("Android", ExpectedResult = "Android")]
        public string TranslatePlatform(string platformName)
        {
            return OSPlatformTranslator.Translate(platformName);
        }

#if NET5_0_OR_GREATER
        [Test]
        public void TranslateSupportedOSPlatformAttribute()
        {
            var supported = new SupportedOSPlatformAttribute("Windows7.0");

            PlatformAttribute platform = TranslateIntoSinglePlatform(supported);
            Assert.That(platform.Include, Is.EqualTo("Windows7"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslateUnsupportedOSPlatformAttribute()
        {
            var unsupported = new UnsupportedOSPlatformAttribute("Linux");

            PlatformAttribute platform = TranslateIntoSinglePlatform(unsupported);
            Assert.That(platform.Include, Is.Null, nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo("Linux"), nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMultipleOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Linux");
            var osPlatforms = new OSPlatformAttribute[] { supported1, supported2 };

            PlatformAttribute platform = TranslateIntoSinglePlatform(osPlatforms);
            Assert.That(platform.Include, Is.EqualTo("Windows7,Linux"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMixedOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Linux");
            var unsupported = new UnsupportedOSPlatformAttribute("Android");

            PlatformAttribute platform = TranslateIntoSinglePlatform(supported1, unsupported, supported2);
            Assert.That(platform.Include, Is.EqualTo("Windows7,Linux"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo("Android"), nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMixedPlatformAndOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows");
            var supported2 = new SupportedOSPlatformAttribute("Windows10");
            var sourcePlatform = new PlatformAttribute("Win");

            PlatformAttribute platform = TranslateIntoSinglePlatform(sourcePlatform, supported1, supported2);
            Assert.That(platform.Include, Is.EqualTo("Win,Windows10"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslatePlatformAttribute()
        {
            var original = new PlatformAttribute("Win");

            PlatformAttribute platform = TranslateIntoSinglePlatform(original);
            Assert.That(platform.Include, Is.EqualTo(original.Include), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo(original.Exclude), nameof(platform.Exclude));
        }

        [Test]
        public void PassThroughAttribute()
        {
            IApplyToTest source = new TestAttribute();

            IApplyToTest result = OSPlatformTranslator.Translate(Array.Empty<Attribute>(), new[] { source }).Single();
            Assert.That(result, Is.SameAs(source));
        }

        private static PlatformAttribute TranslateIntoSinglePlatform(params OSPlatformAttribute[] osPlatformAttributes)
        {
            return (PlatformAttribute)OSPlatformTranslator.Translate(osPlatformAttributes, Array.Empty<IApplyToTest>()).Single();
        }

        private static PlatformAttribute TranslateIntoSinglePlatform(IApplyToTest applyToTestAttribute, params OSPlatformAttribute[] osPlatformAttributes)
        {
            return (PlatformAttribute)OSPlatformTranslator.Translate(osPlatformAttributes, new[] { applyToTestAttribute }).Single();
        }
#endif
    }
}
