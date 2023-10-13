// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.CultureAttributeData
{
    [TestFixture, Culture("en,fr,de")]
    public class FixtureWithCultureAttribute
    {
        [Test, Culture("en,de")]
        public void EnglishAndGermanTest()
        {
        }

        [Test, Culture("fr")]
        public void FrenchTest()
        {
        }

        [Test, Culture("fr-CA")]
        public void FrenchCanadaTest()
        {
        }
    }
}
