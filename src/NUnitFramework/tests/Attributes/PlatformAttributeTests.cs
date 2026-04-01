// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    internal class PlatformAttributeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PlatformHelper.PlatformChecks.Remove(PlatformAttributeFixture.NUnitOS);
            PlatformHelper.PlatformChecks.Remove(PlatformAttributeFixture.NUnitArchitecture);
        }

        private void OverridePlatformCheck(string key, bool value = false)
        {
            PlatformHelper.PlatformChecks[key] = _ => value;
        }

        [Test]
        public void ChildTestWithoutAttribute_DoesntExplicitlyInheritFixturePlatforms([Values]bool allowOS)
        {
            OverridePlatformCheck(PlatformAttributeFixture.NUnitOS, allowOS);
            OverridePlatformCheck(PlatformAttributeFixture.NUnitArchitecture, true);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.NoTestLevelAttributeSpecified));

            Assert.That(testMethod.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(fixture.RunState, Is.EqualTo(allowOS ? RunState.Runnable : RunState.Skipped));
        }

        [Test]
        public void ChildTestWithAttribute_DoesntImplicitlyInheritFixturePlatforms([Values] bool allowBitness)
        {
            OverridePlatformCheck(PlatformAttributeFixture.NUnitOS, false);
            OverridePlatformCheck(PlatformAttributeFixture.NUnitArchitecture, allowBitness);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithoutDuplicateProperty));

            Assert.That(fixture.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(testMethod.RunState, Is.EqualTo(allowBitness ? RunState.Runnable : RunState.Skipped));
        }

        [TestCase(true, false, ExpectedResult = RunState.Runnable)]
        [TestCase(false, true, ExpectedResult = RunState.Runnable)]
        [TestCase(true, true, ExpectedResult = RunState.Runnable)]
        [TestCase(false, false, ExpectedResult = RunState.Skipped)]
        public RunState ChildTestWithAttribute_WithMultiplePlatforms_WillRunForAnySpecified(bool allowOS, bool allowBitness)
        {
            OverridePlatformCheck(PlatformAttributeFixture.NUnitOS, allowOS);
            OverridePlatformCheck(PlatformAttributeFixture.NUnitArchitecture, allowBitness);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithDuplicateProperty));

            return testMethod.RunState;
        }
    }
}
