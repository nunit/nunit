// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.TestData.CultureAttributeData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Summary description for CultureDetectionTests.
    /// </summary>
    [TestFixture]
    public class CultureSettingAndDetectionTests
    {
        private readonly CultureDetector _detector = new CultureDetector("fr-FR");

        private void ExpectMatch( string culture )
        {
            if ( !_detector.IsCultureSupported( culture ) )
                Assert.Fail($"Failed to match \"{culture}\"");
        }

        private void ExpectMatch( CultureAttribute attr )
        {
            if ( !_detector.IsCultureSupported( attr ) )
                Assert.Fail($"Failed to match attribute with Include=\"{attr.Include}\",Exclude=\"{attr.Exclude}\"");
        }

        private void ExpectFailure( string culture )
        {
            if ( _detector.IsCultureSupported( culture ) )
                Assert.Fail($"Should not match \"{culture}\"");
            Assert.That( _detector.Reason, Is.EqualTo("Only supported under culture " + culture));
        }

        private void ExpectFailure( CultureAttribute attr, string msg )
        {
            if ( _detector.IsCultureSupported( attr ) )
                Assert.Fail($"Should not match attribute with Include=\"{attr.Include}\",Exclude=\"{attr.Exclude}\"");
            Assert.That( _detector.Reason, Is.EqualTo(msg));
        }

        [Test]
        public void CanMatchStrings()
        {
            ExpectMatch( "fr-FR" );
            ExpectMatch( "fr" );
            ExpectMatch( "fr-FR,fr-BE,fr-CA" );
            ExpectMatch( "en,de,fr,it" );
            ExpectFailure( "en-GB" );
            ExpectFailure( "en" );
            ExpectFailure( "fr-CA" );
            ExpectFailure( "fr-BE,fr-CA" );
            ExpectFailure( "en,de,it" );
        }

        [Test]
        public void CanMatchAttributeWithInclude()
        {
            ExpectMatch( new CultureAttribute( "fr-FR" ) );
            ExpectMatch( new CultureAttribute( "fr-FR,fr-BE,fr-CA" ) );
            ExpectFailure( new CultureAttribute( "en" ), "Only supported under culture en" );
            ExpectFailure( new CultureAttribute( "en,de,it" ), "Only supported under culture en,de,it" );
        }

        [Test]
        public void CanMatchAttributeWithExclude()
        {
            CultureAttribute attr = new CultureAttribute();
            attr.Exclude = "en";
            ExpectMatch( attr );
            attr.Exclude = "en,de,it";
            ExpectMatch( attr );
            attr.Exclude = "fr";
            ExpectFailure( attr, "Not supported under culture fr");
            attr.Exclude = "fr-FR,fr-BE,fr-CA";
            ExpectFailure( attr, "Not supported under culture fr-FR,fr-BE,fr-CA" );
        }

        [Test]
        public void CanMatchAttributeWithIncludeAndExclude()
        {
            CultureAttribute attr = new CultureAttribute( "en,fr,de,it" );
            attr.Exclude="fr-CA,fr-BE";
            ExpectMatch( attr );
            attr.Exclude = "fr-FR";
            ExpectFailure( attr, "Not supported under culture fr-FR" );
        }

        [Test,SetCulture("fr-FR")]
        public void LoadWithFrenchCulture()
        {
            Assert.That( CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-FR"), "Culture not set correctly" );
            TestSuite fixture = TestBuilder.MakeFixture( typeof( FixtureWithCultureAttribute ) );
            Assert.That( fixture.RunState, Is.EqualTo(RunState.Runnable), "Fixture" );
            foreach( Test test in fixture.Tests )
            {
                RunState expected = test.Name == "FrenchTest" ? RunState.Runnable : RunState.Skipped;
                Assert.That( test.RunState, Is.EqualTo(expected), test.Name );
            }
        }

        [Test,SetCulture("fr-CA")]
        public void LoadWithFrenchCanadianCulture()
        {
            Assert.That( CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-CA"), "Culture not set correctly" );
            TestSuite fixture = TestBuilder.MakeFixture( typeof( FixtureWithCultureAttribute ) );
            Assert.That( fixture.RunState, Is.EqualTo(RunState.Runnable), "Fixture" );
            foreach( Test test in fixture.Tests )
            {
                RunState expected = test.Name.StartsWith( "French" ) ? RunState.Runnable : RunState.Skipped;
                Assert.That( test.RunState, Is.EqualTo(expected), test.Name );
            }
        }

        [Test,SetCulture("ru-RU")]
        public void LoadWithRussianCulture()
        {
            Assert.That( CultureInfo.CurrentCulture.Name, Is.EqualTo("ru-RU"), "Culture not set correctly" );
            TestSuite fixture = TestBuilder.MakeFixture( typeof( FixtureWithCultureAttribute ) );
            Assert.That( fixture.RunState, Is.EqualTo(RunState.Skipped), "Fixture" );
            foreach( Test test in fixture.Tests )
                Assert.That( test.RunState, Is.EqualTo(RunState.Skipped), test.Name );
        }

        [TestFixture, SetCulture("en-GB")]
        public class NestedFixture
        {
            [Test]
            public void CanSetCultureOnFixture()
            {
                Assert.That( CultureInfo.CurrentCulture.Name, Is.EqualTo("en-GB"));
            }
        }
    }
}
