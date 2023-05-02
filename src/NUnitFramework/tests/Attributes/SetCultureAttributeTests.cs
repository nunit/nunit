// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
            Assert.That(originalCulture, Is.EqualTo(CultureInfo.CurrentCulture), "Culture should not change");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-FR"), "UICulture not set correctly");
        }

        [Test, SetUICulture("fr-CA")]
        public void SetUICultureOnlyToFrenchCanadian()
        {
            Assert.That(originalCulture, Is.EqualTo(CultureInfo.CurrentCulture), "Culture should not change");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-CA"), "UICulture not set correctly");
        }

        [Test, SetUICulture("ru-RU")]
        public void SetUICultureOnlyToRussian()
        {
            Assert.That(originalCulture, Is.EqualTo(CultureInfo.CurrentCulture), "Culture should not change");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("ru-RU"), "UICulture not set correctly");
        }

        [Test, SetCulture("fr-FR")]
        public void SetCultureOnlyToFrench()
        {
            Assert.That(originalUICulture, Is.EqualTo(CultureInfo.CurrentUICulture), "UICulture should not change");
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-FR"), "Culture not set correctly");
        }

        [Test, SetCulture("fr-FR"), SetUICulture("fr-FR")]
        public void SetBothCulturesToFrench()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-FR"), "Culture not set correctly");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-FR"), "UICulture not set correctly");
        }

        [Test, SetCulture("fr-CA"), SetUICulture("fr-CA")]
        public void SetBothCulturesToFrenchCanadian()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-CA"), "Culture not set correctly");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-CA"), "UICulture not set correctly");
        }

        [Test, SetCulture("ru-RU"), SetUICulture("ru-RU")]
        public void SetBothCulturesToRussian()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("ru-RU"), "Culture not set correctly");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("ru-RU"), "UICulture not set correctly");
        }

        [Test, SetCulture("fr-FR"), SetUICulture("fr-CA")]
        public void SetMixedCulturesToFrenchAndUIFrenchCanadian()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-FR"), "Culture not set correctly");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-CA"), "UICulture not set correctly");
        }

        [Test, SetCulture("ru-RU"), SetUICulture("en-US")]
        public void SetMixedCulturesToRussianAndUIEnglishUS()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("ru-RU"), "Culture not set correctly");
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("en-US"), "UICulture not set correctly");
        }

        [TestFixture, SetCulture("ru-RU"), SetUICulture("ru-RU")]
        public class NestedBehavior
        {
            [Test]
            public void InheritedRussian()
            {
                Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("ru-RU"), "Culture not set correctly");
                Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("ru-RU"), "UICulture not set correctly");
            }

            [Test, SetUICulture("fr-FR")]
            public void InheritedRussianWithUIFrench()
            {
                Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("ru-RU"), "Culture not set correctly");
                Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("fr-FR"), "UICulture not set correctly");
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
