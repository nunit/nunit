// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;
using NUnit.TestData.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    internal class PlatformAttributeTests
    {
        [Test]
        public void ChildTestWithoutAttribute_DoesntExplicitlyInheritPlatforms()
        {
            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var fixtureIncludes = fixture.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.NoTestLevelAttributeSpecified));
            var methodIncludes = testMethod.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            Assert.That(fixtureIncludes, Is.Not.Empty);
            Assert.That(methodIncludes, Is.Null);
        }

        [Test]
        public void ChildTestWithAttribute_WithoutDuplication_DoesntImplicitlyInheritOrMergePlatforms()
        {
            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var fixtureIncludes = fixture.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithoutDuplicateProperty));
            var methodIncludes = testMethod.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            Assert.That(fixtureIncludes, Is.Not.Empty);
            Assert.That(methodIncludes, Is.Not.Empty);
            Assert.That(fixtureIncludes.Intersect(methodIncludes), Is.Empty);
        }

        [Test]
        public void ChildTestWithAttribute_WithDuplication_DoesntImplicitlyInheritOrMergePlatforms()
        {
            var fixture = TestBuilder.MakeFixture<PlatformAttributeFixture>();
            var fixtureIncludes = fixture.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            var testMethod = fixture.Tests.First(x => x.Name == nameof(PlatformAttributeFixture.WithDuplicateProperty));
            var methodIncludes = testMethod.Properties.Get(ObservablePlatformAttribute.PlatformIncludesPropertyName) as string[];

            Assert.That(fixtureIncludes, Is.Not.Empty);
            Assert.That(methodIncludes, Is.Not.Empty);
            Assert.That(fixtureIncludes.Intersect(methodIncludes), Is.Not.Empty);
        }
    }
}
