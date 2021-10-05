// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

#if NET5_0_OR_GREATER
using System.Linq;
using System.Runtime.Versioning;
#endif

namespace NUnit.Framework.Attributes
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

            PlatformAttribute platform = (PlatformAttribute)OSPlatformTranslator.Translate(new[] { supported }).Single();
            Assert.That(platform.Include, Is.EqualTo("Windows7"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslateUnsupportedOSPlatformAttribute()
        {
            var unsupported = new UnsupportedOSPlatformAttribute("Linux");

            PlatformAttribute platform = (PlatformAttribute)OSPlatformTranslator.Translate(new[] { unsupported }).Single();
            Assert.That(platform.Include, Is.Null, nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo("Linux"), nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMultipleOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Linux");
            var osPlatforms = new OSPlatformAttribute[] { supported1, supported2 };

            object[] attributes = OSPlatformTranslator.Translate(osPlatforms).ToArray();
            Assert.That(attributes, Has.Length.EqualTo(1));
            Assert.That(attributes[0], Is.InstanceOf<PlatformAttribute>());
            PlatformAttribute platform = (PlatformAttribute)attributes[0];
            Assert.That(platform.Include, Is.EqualTo("Windows7,Linux"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMixedOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows7.0");
            var supported2 = new SupportedOSPlatformAttribute("Linux");
            var unsupported = new UnsupportedOSPlatformAttribute("Android");
            var osPlatforms = new OSPlatformAttribute[] { supported1, unsupported, supported2 };

            object[] attributes = OSPlatformTranslator.Translate(osPlatforms).ToArray();
            Assert.That(attributes, Has.Length.EqualTo(1));
            Assert.That(attributes[0], Is.InstanceOf<PlatformAttribute>());
            PlatformAttribute platform = (PlatformAttribute)attributes[0];
            Assert.That(platform.Include, Is.EqualTo("Windows7,Linux"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo("Android"), nameof(platform.Exclude));
        }

        [Test]
        public void TranslateMixedPlatformAndOSPlatformAttributes()
        {
            var supported1 = new SupportedOSPlatformAttribute("Windows");
            var supported2 = new SupportedOSPlatformAttribute("Windows10");
            var sourcePlatform = new PlatformAttribute("Win");
            var sourceAttributes = new object[] { supported1, supported2, sourcePlatform };

            object[] attributes = OSPlatformTranslator.Translate(sourceAttributes).ToArray();
            Assert.That(attributes, Has.Length.EqualTo(1));
            Assert.That(attributes[0], Is.InstanceOf<PlatformAttribute>());
            PlatformAttribute platform = (PlatformAttribute)attributes[0];
            Assert.That(platform.Include, Is.EqualTo("Win,Windows10"), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.Null, nameof(platform.Exclude));
        }

        [Test]
        public void TranslatePlatformAttribute()
        {
            var original = new PlatformAttribute("Win");

            PlatformAttribute platform = (PlatformAttribute)OSPlatformTranslator.Translate(new[] { original }).Single();
            Assert.That(platform.Include, Is.EqualTo(original.Include), nameof(platform.Include));
            Assert.That(platform.Exclude, Is.EqualTo(original.Exclude), nameof(platform.Exclude));
        }

        [Test]
        public void TranslateUnknownAttribute()
        {
            var unknown = new TestAttribute();

            object attribute = OSPlatformTranslator.Translate(new[] { unknown }).Single();
            Assert.That(attribute, Is.SameAs(unknown));
        }
#endif
    }
}
