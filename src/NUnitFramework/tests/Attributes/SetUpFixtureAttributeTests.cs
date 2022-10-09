// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class SetUpFixtureAttributeTests
    {
        [Test]
        public void SetUpFixtureCanBeIgnored()
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(typeof(IgnoredSetUpFixture)));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Ignore("Just Because")]
        private class IgnoredSetUpFixture
        {
        }

        [Test]
        public void SetUpFixtureMayBeParallelizable()
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(typeof(ParallelizableSetUpFixture)));
            foreach (var fixture in fixtures)
                Assert.That(fixture.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.Self));
        }

        [Parallelizable]
        private class ParallelizableSetUpFixture
        {
        }

        [TestCase(typeof(TestSetupClass))]
        [TestCase(typeof(TestTearDownClass))]
        public void CertainAttributesAreNotAllowed(Type type)
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(type));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void AbstractClassNotAllowed()
        {
            var abstractType = typeof(AbstractSetupClass);
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(abstractType));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void StaticClassIsAllowed()
        {
            var abstractType = typeof(StaticSetupClass);
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(abstractType));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void AttributeUsage_NoInheritance()
        {
            var usageAttrib = Attribute.GetCustomAttribute(typeof(SetUpFixtureAttribute), typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

            Assert.NotNull(usageAttrib);
            Assert.False(usageAttrib.Inherited);
        }

        private static class StaticSetupClass
        {
            [OneTimeSetUp]
            public static void SomeSetUpMethod() { }

            [OneTimeTearDown]
            public static void SomeTearDownMethod() { }
        }

        private abstract class AbstractSetupClass
        {
            [OneTimeSetUp]
            public void SomeSetUpMethod() { }
        }

        private class TestSetupClass
        {
            [SetUp]
            public void SomeMethod() { }
        }

        private class TestTearDownClass
        {
            [TearDown]
            public void SomeMethod() { }
        }
    }
}
