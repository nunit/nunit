// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Globalization;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class SetCultureAttributeTests
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        [SetUp]
        public void Setup()
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;
        }

        [Test, SetUICulture("fr-FR")]
        public void SetUICultureOnlyToFrench()
        {
            Assert.AreEqual(CultureInfo.CurrentCulture, originalCulture, "Culture should not change");
            Assert.AreEqual("fr-FR", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetUICulture("fr-CA")]
        public void SetUICultureOnlyToFrenchCanadian()
        {
            Assert.AreEqual(CultureInfo.CurrentCulture, originalCulture, "Culture should not change");
            Assert.AreEqual("fr-CA", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetUICulture("ru-RU")]
        public void SetUICultureOnlyToRussian()
        {
            Assert.AreEqual(CultureInfo.CurrentCulture, originalCulture, "Culture should not change");
            Assert.AreEqual("ru-RU", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetCulture("fr-FR"), SetUICulture("fr-FR")]
        public void SetBothCulturesToFrench()
        {
            Assert.AreEqual("fr-FR", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
            Assert.AreEqual("fr-FR", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetCulture("fr-CA"), SetUICulture("fr-CA")]
        public void SetBothCulturesToFrenchCanadian()
        {
            Assert.AreEqual("fr-CA", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
            Assert.AreEqual("fr-CA", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetCulture("ru-RU"), SetUICulture("ru-RU")]
        public void SetBothCulturesToRussian()
        {
            Assert.AreEqual("ru-RU", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
            Assert.AreEqual("ru-RU", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetCulture("fr-FR"), SetUICulture("fr-CA")]
        public void SetMixedCulturesToFrenchAndUIFrenchCanadian()
        {
            Assert.AreEqual("fr-FR", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
            Assert.AreEqual("fr-CA", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [Test, SetCulture("ru-RU"), SetUICulture("en-US")]
        public void SetMixedCulturesToRussianAndUIEnglishUS()
        {
            Assert.AreEqual("ru-RU", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
            Assert.AreEqual("en-US", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
        }

        [TestFixture, SetCulture("ru-RU"), SetUICulture("ru-RU")]
        public class NestedBehavior
        {
            [Test]
            public void InheritedRussian()
            {
                Assert.AreEqual("ru-RU", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
                Assert.AreEqual("ru-RU", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
            }

            [Test, SetUICulture("fr-FR")]
            public void InheritedRussianWithUIFrench()
            {
                Assert.AreEqual("ru-RU", CultureInfo.CurrentCulture.Name, "Culture not set correctly");
                Assert.AreEqual("fr-FR", CultureInfo.CurrentUICulture.Name, "UICulture not set correctly");
            }
        }

        [Test, SetCulture("de-DE")]
        public void UseWithParameterizedTest()
        {
            // Get platform-specific culture formatting behavior
            var expectedResult = new DateTime(2010, 6, 1).ToString(new CultureInfo("de-DE"));

            Assert.That(new DateTime(2010, 6, 1).ToString(), Is.EqualTo(expectedResult));
        }
    }
}
