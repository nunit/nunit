// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class ApplyToTestTests
    {
        Test test;

        [SetUp]
        public void SetUp()
        {
            test = new TestDummy();
            test.RunState = RunState.Runnable;
        }

        #region CategoryAttribute
        
        [TestCase('!')]
        [TestCase('+')]
        [TestCase(',')]
        [TestCase('-')]
        public void CategoryAttributePassesOnSpecialCharacters(char specialCharacter)
        {
            var categoryName = new string(specialCharacter, 5);
            new CategoryAttribute(categoryName).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.Category), Is.EqualTo(categoryName));
        }

        [Test]
        public void CategoryAttributeSetsCategory()
        {
            new CategoryAttribute("database").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.Category), Is.EqualTo("database"));
        }

        [Test]
        public void CategoryAttributeSetsCategoryOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new CategoryAttribute("database").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.Category), Is.EqualTo("database"));
        }

        [Test]
        public void CategoryAttributeSetsMultipleCategories()
        {
            new CategoryAttribute("group1").ApplyToTest(test);
            new CategoryAttribute("group2").ApplyToTest(test);
            Assert.That(test.Properties[PropertyNames.Category],
                Is.EquivalentTo( new string[] { "group1", "group2" } ));
        }

        #endregion

        #region DescriptionAttribute

        [Test]
        public void DescriptionAttributeSetsDescription()
        {
            new DescriptionAttribute("Cool test!").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.Description), Is.EqualTo("Cool test!"));
        }

        [Test]
        public void DescriptionAttributeSetsDescriptionOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new DescriptionAttribute("Cool test!").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.Description), Is.EqualTo("Cool test!"));
        }

        #endregion

        #region IgnoreAttribute

        [Test]
        public void IgnoreAttributeIgnoresTest()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeSetsIgnoreReason()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("BECAUSE"));
        }

        [Test]
        public void IgnoreAttributeDoesNotAffectNonRunnableTest()
        {
            test.MakeInvalid("UNCHANGED");
            new IgnoreAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("UNCHANGED"));
        }

        [Test]
        public void IgnoreAttributeIgnoresTestUntilDateSpecified()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "4242-01-01";
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeIgnoresTestUntilDateTimeSpecified()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "4242-01-01 12:00:00Z";
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Test]
        public void IgnoreAttributeMarksTestAsRunnableAfterUntilDatePasses()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "1492-01-01";
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [TestCase("4242-01-01")]
        [TestCase("4242-01-01 00:00:00Z")]
        [TestCase("4242-01-01 00:00:00")]
        public void IgnoreAttributeUntilSetsTheReason(string date)
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = date;
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Ignoring until 4242-01-01 00:00:00Z. BECAUSE"));
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
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo("4242-01-01 00:00:00Z"));
        }

        [Test]
        public void IgnoreAttributeWithUntilAddsIgnoreUntilDatePropertyPastUntilDate()
        {
            var ignoreAttribute = new IgnoreAttribute("BECAUSE");
            ignoreAttribute.Until = "1242-01-01";
            ignoreAttribute.ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.IgnoreUntilDate), Is.EqualTo("1242-01-01 00:00:00Z"));
        }

        [Test]
        public void IgnoreAttributeWithExplicitIgnoresTest()
        {
            new IgnoreAttribute("BECAUSE").ApplyToTest(test);
            new ExplicitAttribute().ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
        }

        #endregion

        #region ExplicitAttribute

        [Test]
        public void ExplicitAttributeMakesTestExplicit()
        {
            new ExplicitAttribute().ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Explicit));
        }

        [Test]
        public void ExplicitAttributeSetsIgnoreReason()
        {
            new ExplicitAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("BECAUSE"));
        }

        [Test]
        public void ExplicitAttributeDoesNotAffectNonRunnableTest()
        {
            test.MakeInvalid("UNCHANGED");
            new ExplicitAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("UNCHANGED"));
        }

        [Test]
        public void ExplicitAttributeWithIgnoreIgnoresTest()
        {
            new ExplicitAttribute().ApplyToTest(test);
            new IgnoreAttribute("BECAUSE").ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
        }

        #endregion

        #region CombinatorialAttribute

        [Test]
        public void CombinatorialAttributeSetsJoinType()
        {
            new CombinatorialAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Combinatorial"));
        }

        [Test]
        public void CombinatorialAttributeSetsJoinTypeOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new CombinatorialAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Combinatorial"));
        }

        #endregion

        #region CultureAttribute

        [Test]
        public void CultureAttributeIncludingCurrentCultureRunsTest()
        {
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            new CultureAttribute(name).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void CultureAttributeDoesNotAffectNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            new CultureAttribute(name).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void CultureAttributeExcludingCurrentCultureSkipsTest()
        {
            string name = System.Globalization.CultureInfo.CurrentCulture.Name;
            CultureAttribute attr = new CultureAttribute(name);
            attr.Exclude = name;
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason),
                Is.EqualTo("Not supported under culture " + name));
        }

        [Test]
        public void CultureAttributeIncludingOtherCultureSkipsTest()
        {
            string name = "fr-FR";
            if (System.Globalization.CultureInfo.CurrentCulture.Name == name)
                name = "en-US";

            new CultureAttribute(name).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason),
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
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void CultureAttributeWithMultipleCulturesIncluded()
        {
            string current = System.Globalization.CultureInfo.CurrentCulture.Name;
            string other = current == "fr-FR" ? "en-US" : "fr-FR";
            string cultures = current + "," + other;

            new CultureAttribute(cultures).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region MaxTimeAttribute

        [Test]
        public void MaxTimeAttributeSetsMaxTime()
        {
            new MaxTimeAttribute(2000).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(2000));
        }

        [Test]
        public void MaxTimeAttributeSetsMaxTimeOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new MaxTimeAttribute(2000).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.MaxTime), Is.EqualTo(2000));
        }

        #endregion

        #region PairwiseAttribute

        [Test]
        public void PairwiseAttributeSetsJoinType()
        {
            new PairwiseAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Pairwise"));
        }

        [Test]
        public void PairwiseAttributeSetsJoinTypeOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new PairwiseAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Pairwise"));
        }

        #endregion

        #region PlatformAttribute

#if PLATFORM_DETECTION
        [Test]
        public void PlatformAttributeRunsTest()
        {
            string myPlatform = GetMyPlatform();
            new PlatformAttribute(myPlatform).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void PlatformAttributeSkipsTest()
        {
            string notMyPlatform = System.IO.Path.DirectorySeparatorChar == '/'
                ? "Win" : "Linux";
            new PlatformAttribute(notMyPlatform).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Skipped));
        }

        [Test]
        public void PlatformAttributeDoesNotAffectNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            string myPlatform = GetMyPlatform();
            new PlatformAttribute(myPlatform).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void InvalidPlatformAttributeIsNotRunnable()
        {
            var invalidPlatform = "FakePlatform";
            new PlatformAttribute(invalidPlatform).ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason),
                Does.StartWith("Invalid platform name"));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason),
                Does.Contain(invalidPlatform));
        }

        string GetMyPlatform()
        {
            if (System.IO.Path.DirectorySeparatorChar == '/')
            {
                return OSPlatform.CurrentPlatform.IsMacOSX ? "MacOSX" : "Linux";
            }
            return "Win";
        }
#endif

        #endregion

        #region RepeatAttribute

        [Test]
        public void RepeatAttributeSetsRepeatCount()
        {
            new RepeatAttribute(5).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.RepeatCount), Is.EqualTo(5));
        }

        [Test]
        public void RepeatAttributeSetsRepeatCountOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new RepeatAttribute(5).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.RepeatCount), Is.EqualTo(5));
        }

        #endregion

#if PARALLEL

#if APARTMENT_STATE
        #region RequiresMTAAttribute

        [Test]
        public void RequiresMTAAttributeSetsApartmentState()
        {
            new ApartmentAttribute(ApartmentState.MTA).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.MTA));
        }

        [Test]
        public void RequiresMTAAttributeSetsApartmentStateOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new ApartmentAttribute(ApartmentState.MTA).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.MTA));
        }

        #endregion

        #region RequiresSTAAttribute

        [Test]
        public void RequiresSTAAttributeSetsApartmentState()
        {
            new ApartmentAttribute(ApartmentState.STA).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.STA));
        }

        [Test]
        public void RequiresSTAAttributeSetsApartmentStateOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new ApartmentAttribute(ApartmentState.STA).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.STA));
        }

        #endregion
#endif

        #region RequiresThreadAttribute

        [Test]
        public void RequiresThreadAttributeSetsRequiresThread()
        {
            new RequiresThreadAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
        }

        [Test]
        public void RequiresThreadAttributeSetsRequiresThreadOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new RequiresThreadAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
        }

#if APARTMENT_STATE
        [Test]
        public void RequiresThreadAttributeMaySetApartmentState()
        {
            new RequiresThreadAttribute(ApartmentState.STA).ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.RequiresThread), Is.EqualTo(true));
            Assert.That(test.Properties.Get(PropertyNames.ApartmentState),
                Is.EqualTo(ApartmentState.STA));
        }
#endif
        #endregion

#endif

        #region SequentialAttribute

        [Test]
        public void SequentialAttributeSetsJoinType()
        {
            new SequentialAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Sequential"));
        }

        [Test]
        public void SequentialAttributeSetsJoinTypeOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new SequentialAttribute().ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.JoinType), Is.EqualTo("Sequential"));
        }

        #endregion

#if PARALLEL

        #region SetCultureAttribute

        public void SetCultureAttributeSetsSetCultureProperty()
        {
            new SetCultureAttribute("fr-FR").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.SetCulture), Is.EqualTo("fr-FR"));
        }

        public void SetCultureAttributeSetsSetCulturePropertyOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new SetCultureAttribute("fr-FR").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.SetCulture), Is.EqualTo("fr-FR"));
        }

        #endregion

        #region SetUICultureAttribute

        public void SetUICultureAttributeSetsSetUICultureProperty()
        {
            new SetUICultureAttribute("fr-FR").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.SetUICulture), Is.EqualTo("fr-FR"));
        }

        public void SetUICultureAttributeSetsSetUICulturePropertyOnNonRunnableTest()
        {
            test.RunState = RunState.NotRunnable;
            new SetUICultureAttribute("fr-FR").ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.SetUICulture), Is.EqualTo("fr-FR"));
        }

        #endregion

#endif
    }
}
