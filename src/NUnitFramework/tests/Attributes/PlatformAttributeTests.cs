// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [NonParallelizable]
    internal class PlatformAttributeTests
    {
        private Dictionary<string, Func<OSPlatform, bool>> _originalPlatformChecks;
        private FieldInfo _platformField;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _platformField = typeof(PlatformHelper).GetField("PlatformChecks", BindingFlags.Static | BindingFlags.NonPublic)!;
            _originalPlatformChecks = (Dictionary<string, Func<OSPlatform, bool>>)_platformField.GetValue(null)!;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _platformField.SetValue(null, _originalPlatformChecks);
        }

        private void OverridePlatformCheck(string key, bool value = false)
        {
            var currentChecks = (Dictionary<string, Func<OSPlatform, bool>>)_platformField.GetValue(null)!;
            var newChecks = new Dictionary<string, Func<OSPlatform, bool>>(currentChecks)
            {
                [key] = _ => value
            };
            _platformField.SetValue(null, newChecks);
        }

        [Test]
        public void ChildTestWithoutAttribute_DoesntExplicitlyInheritFixturePlatforms([Values]bool allowOS)
        {
            OverridePlatformCheck(PlatformNames.Win, allowOS);
            OverridePlatformCheck(PlatformNames.X64BitOS, true);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.NoTestLevelAttributeSpecified));

            Assert.That(testMethod.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(fixture.RunState == RunState.Runnable, Is.EqualTo(allowOS));
        }

        [Test]
        public void ChildTestWithAttribute_DoesntImplicitlyInheritFixturePlatforms([Values] bool allowBitness)
        {
            OverridePlatformCheck(PlatformNames.Win, false);
            OverridePlatformCheck(PlatformNames.X64BitOS, allowBitness);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithoutDuplicateProperty));

            Assert.That(fixture.RunState, Is.EqualTo(RunState.Skipped));
            Assert.That(testMethod.RunState == RunState.Runnable, Is.EqualTo(allowBitness));
        }

        [TestCase(true, false, ExpectedResult = RunState.Runnable)]
        [TestCase(false, true, ExpectedResult = RunState.Runnable)]
        [TestCase(true, true, ExpectedResult = RunState.Runnable)]
        [TestCase(false, false, ExpectedResult = RunState.Skipped)]
        public RunState ChildTestWithAttribute_WithMultiplePlatforms_WillRunForAnySpecified(bool allowOS, bool allowBitness)
        {
            OverridePlatformCheck(PlatformNames.Win, allowOS);
            OverridePlatformCheck(PlatformNames.X64BitOS, allowBitness);

            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithDuplicateProperty));

            return testMethod.RunState;
        }
    }
}
