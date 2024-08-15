// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class ApplyToTestTests
    {
        private NUnit.Framework.Internal.Test _test;

        [SetUp]
        public void SetUp()
        {
            _test = new TestDummy();
            _test.RunState = RunState.Runnable;
        }

        #region CategoryAttribute

        [TestCase('!')]
        [TestCase('+')]
        [TestCase(',')]
        [TestCase('-')]
        public void CategoryAttributePassesOnSpecialCharacters(char specialCharacter)
        {
            var categoryName = new string(specialCharacter, 5);
            new CategoryAttribute(categoryName).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.Category), Is.EqualTo(categoryName));
        }

        [Test]
        public void CategoryAttributeSetsCategory()
        {
            new CategoryAttribute("database").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.Category), Is.EqualTo("database"));
        }

        [Test]
        public void CategoryAttributeSetsCategoryOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new CategoryAttribute("database").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.Category), Is.EqualTo("database"));
        }

        [Test]
        public void CategoryAttributeSetsMultipleCategories()
        {
            new CategoryAttribute("group1").ApplyToTest(_test);
            new CategoryAttribute("group2").ApplyToTest(_test);
            Assert.That(_test.Properties[PropertyNames.Category],
                Is.EquivalentTo(new[] { "group1", "group2" }));
        }

        #endregion

        #region DescriptionAttribute

        [Test]
        public void DescriptionAttributeSetsDescription()
        {
            new DescriptionAttribute("Cool test!").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.Description), Is.EqualTo("Cool test!"));
        }

        [Test]
        public void DescriptionAttributeSetsDescriptionOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new DescriptionAttribute("Cool test!").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.Description), Is.EqualTo("Cool test!"));
        }

        #endregion

        #region IgnoreAttribute

        [Test]
        public void IgnoreAttributeReason()
        {
            var ignore = new IgnoreAttribute("BECAUSE");
            Assert.That(ignore.Reason, Is.EqualTo("BECAUSE"));
        }

        [Test]
        public void IgnoreAttributeNullReason()
        {
            Assert.That(() => new IgnoreAttribute(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void IgnoreAttributeIgnoresTest()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeSetsIgnoreReason()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("BECAUSE"));
        }

        [Test]
        public void IgnoreAttributeDoesNotAffectNonRunnableTest()
        {
            _test.MakeInvalid("UNCHANGED");
            new IgnoreAttribute("BECAUSE").ApplyToTest(_test);
            Assert.Multiple(() =>
            {
                Assert.That(_test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(_test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("UNCHANGED"));
            });
        }

        [Test]
        public void IgnoreAttributeIgnoresTestUntilDateSpecified()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "4242-01-01";
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeIgnoresTestUntilDateTimeSpecified()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "4242-01-01 12:00:00Z";
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeMarksTestAsRunnableAfterUntilDatePasses()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "1492-01-01";
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [TestCase("4242-01-01")]
        [TestCase("4242-01-01 00:00:00Z")]
        [TestCase("4242-01-01 00:00:00")]
        public void IgnoreAttributeUntilSetsTheReason(string date)
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = date;
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Ignoring until 4242-01-01 00:00:00Z. BECAUSE"));
        }

        [Test]
        public void IgnoreAttributeWithInvalidDateThrowsException()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            Assert.Throws<FormatException>(() => ignoreAttribute.Until = "Thursday the twenty fifth of December");
        }

        [Test]
        public void IgnoreAttributeWithUntilAddsIgnoreUntilDateProperty()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "4242-01-01";
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo("4242-01-01 00:00:00Z"));
        }

        [Test]
        public void IgnoreAttributeWithUntilAddsIgnoreUntilDatePropertyPastUntilDate()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "1242-01-01";
            ignoreAttribute.ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo("1242-01-01 00:00:00Z"));
        }

        [Test]
        public void IgnoreAttributeWithExplicitIgnoresTest()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(_test);
            new ExplicitAttribute().ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
        }

        #endregion

        #region ExplicitAttribute

        [Test]
        public void ExplicitAttributeMakesTestExplicit()
        {
            new ExplicitAttribute().ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Explicit));
        }

        [Test]
        public void ExplicitAttributeSetsIgnoreReason()
        {
            new ExplicitAttribute("BECAUSE").ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("BECAUSE"));
        }

        [Test]
        public void ExplicitAttributeDoesNotAffectNonRunnableTest()
        {
            _test.MakeInvalid("UNCHANGED");
            new ExplicitAttribute("BECAUSE").ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("UNCHANGED"));
        }

        [Test]
        public void ExplicitAttributeWithIgnoreIgnoresTest()
        {
            new ExplicitAttribute().ApplyToTest(_test);
            new IgnoreAttribute("BECAUSE").ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Ignored));
        }

        #endregion

        #region CombinatorialAttribute

        [Test]
        public void CombinatorialAttributeSetsJoinType()
        {
            new CombinatorialAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Combinatorial"));
        }

        [Test]
        public void CombinatorialAttributeSetsJoinTypeOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new CombinatorialAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Combinatorial"));
        }

        #endregion

        #region CultureAttribute

        [Test]
        public void CultureAttributeIncludingCurrentCultureRunsTest()
        {
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            new CultureAttribute(name).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void CultureAttributeDoesNotAffectNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            new CultureAttribute(name).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void CultureAttributeExcludingCurrentCultureSkipsTest()
        {
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            CultureAttribute attr = new CultureAttribute(name);
            attr.Exclude = name;
            attr.ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason),
                Is.EqualTo("Not supported under culture " + name));
        }

        [Test]
        public void CultureAttributeIncludingOtherCultureSkipsTest()
        {
            string name = "fr-FR";
            if (System.Globalization.CultureInfo.CurrentCulture.Name == name)
                name = "en-US";

            new CultureAttribute(name).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(_test.Properties.Get(PropertyNames.SkipReason),
                Is.EqualTo("Only supported under culture " + name));
        }

        [Test]
        public void CultureAttributeExcludingOtherCultureRunsTest()
        {
            string other = "fr-FR";
            if (System.Globalization.CultureInfo.CurrentCulture.Name == other)
                other = "en-US";

            CultureAttribute attr = new CultureAttribute();
            attr.Exclude = other;
            attr.ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void CultureAttributeWithMultipleCulturesIncluded()
        {
            string current = System.Globalization.CultureInfo.CurrentCulture.Name;
            string other = current == "fr-FR" ? "en-US" : "fr-FR";
            string cultures = current + "," + other;

            new CultureAttribute(cultures).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region MaxTimeAttribute

        [Test]
        public void MaxTimeAttributeSetsMaxTime()
        {
            new MaxTimeAttribute(2000).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(2000));
        }

        [Test]
        public void MaxTimeAttributeSetsMaxTimeOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new MaxTimeAttribute(2000).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(2000));
        }

        #endregion

        #region PairwiseAttribute

        [Test]
        public void PairwiseAttributeSetsJoinType()
        {
            new PairwiseAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Pairwise"));
        }

        [Test]
        public void PairwiseAttributeSetsJoinTypeOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new PairwiseAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Pairwise"));
        }

        #endregion

        #region PlatformAttribute

        [Test]
        public void PlatformAttributeRunsTest()
        {
            string myPlatform = GetMyPlatform();
            new PlatformAttribute(myPlatform).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void PlatformAttributeSkipsTest()
        {
            string notMyPlatform = System.IO.Path.DirectorySeparatorChar == '/'
                ? "Win" : "Linux";
            new PlatformAttribute(notMyPlatform).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.Skipped));
        }

        [Test]
        public void PlatformAttributeDoesNotAffectNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            string myPlatform = GetMyPlatform();
            new PlatformAttribute(myPlatform).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void InvalidPlatformAttributeIsNotRunnable()
        {
            var invalidPlatform = "FakePlatform";
            new PlatformAttribute(invalidPlatform).ApplyToTest(_test);
            Assert.That(_test.RunState, Is.EqualTo(RunState.NotRunnable));
            string? skipReason = (string?)_test.Properties.Get(PropertyNames.SkipReason);
            Assert.That(skipReason, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(skipReason, Does.StartWith("Invalid platform name"));
                Assert.That(skipReason, Does.Contain(invalidPlatform));
            });
        }

        private string GetMyPlatform()
        {
            if (System.IO.Path.DirectorySeparatorChar == '/')
            {
                return OSPlatform.CurrentPlatform.IsMacOSX ? "MacOSX" : "Linux";
            }
            return "Win";
        }

        #endregion

        #region RepeatAttribute

        [Test]
        public void RepeatAttributeSetsRepeatCount()
        {
            new RepeatAttribute(5).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.RepeatCount), Is.EqualTo(5));
        }

        [Test]
        public void RepeatAttributeSetsRepeatCountOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new RepeatAttribute(5).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.RepeatCount), Is.EqualTo(5));
        }

        #endregion

        #region RequiresMTAAttribute

        [Test]
        public void RequiresMTAAttributeSetsApartmentState()
        {
            new ApartmentAttribute(ApartmentState.MTA).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.MTA));
        }

        [Test]
        public void RequiresMTAAttributeSetsApartmentStateOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new ApartmentAttribute(ApartmentState.MTA).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.MTA));
        }

        #endregion

        #region RequiresSTAAttribute

        [Test]
        public void RequiresSTAAttributeSetsApartmentState()
        {
            new ApartmentAttribute(ApartmentState.STA).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.STA));
        }

        [Test]
        public void RequiresSTAAttributeSetsApartmentStateOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new ApartmentAttribute(ApartmentState.STA).ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.STA));
        }

        #endregion

        #region RequiresThreadAttribute

        [Test]
        public void RequiresThreadAttributeSetsRequiresThread()
        {
            new RequiresThreadAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
        }

        [Test]
        public void RequiresThreadAttributeSetsRequiresThreadOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new RequiresThreadAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
        }

        [Test]
        public void RequiresThreadAttributeMaySetApartmentState()
        {
            new RequiresThreadAttribute(ApartmentState.STA).ApplyToTest(_test);
            Assert.Multiple(() =>
            {
                Assert.That(_test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
                Assert.That(_test.Properties.Get(PropertyNames.ApartmentState),
                    Is.EqualTo(ApartmentState.STA));
            });
        }

        #endregion

        #region SequentialAttribute

        [Test]
        public void SequentialAttributeSetsJoinType()
        {
            new SequentialAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Sequential"));
        }

        [Test]
        public void SequentialAttributeSetsJoinTypeOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new SequentialAttribute().ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Sequential"));
        }

        #endregion

        #region SetCultureAttribute

        [Test]
        public void SetCultureAttributeSetsSetCultureProperty()
        {
            new SetCultureAttribute("fr-FR").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.SetCulture), Is.EqualTo("fr-FR"));
        }

        [Test]
        public void SetCultureAttributeSetsSetCulturePropertyOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new SetCultureAttribute("fr-FR").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.SetCulture), Is.EqualTo("fr-FR"));
        }

        #endregion

        #region SetUICultureAttribute

        [Test]
        public void SetUICultureAttributeSetsSetUICultureProperty()
        {
            new SetUICultureAttribute("fr-FR").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.SetUICulture), Is.EqualTo("fr-FR"));
        }

        [Test]
        public void SetUICultureAttributeSetsSetUICulturePropertyOnNonRunnableTest()
        {
            _test.RunState = RunState.NotRunnable;
            new SetUICultureAttribute("fr-FR").ApplyToTest(_test);
            Assert.That(_test.Properties.Get(PropertyNames.SetUICulture), Is.EqualTo("fr-FR"));
        }

        #endregion
    }
}
