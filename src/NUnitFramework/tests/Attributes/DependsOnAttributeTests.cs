// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [NonParallelizable]
    public partial class DependsOnAttributeTests
    {
        [Test]
        public void AttributeUsageIsClassOrMethodLevelSingleUse()
        {
            var usage = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(typeof(DependsOnAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage, Is.Not.Null, "DependsOnAttribute should have an AttributeUsage attribute.");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(usage.ValidOn, Is.EqualTo(AttributeTargets.Class | AttributeTargets.Method));
                Assert.That(usage.AllowMultiple, Is.False);
                Assert.That(usage.Inherited, Is.False);
            }
        }

        [Test]
        public void FixtureDependencyAndMethodDependencyCoexistWhenSeparate()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependsOnFixtureWithInternalMethodDependencies), typeof(MethodDependencyOrdered));

            var work = TestBuilder.CreateWorkItem(suite);
            var result = TestBuilder.ExecuteWorkItem(work);

            var firstFixture = result.Children.Single(x => x.Name == nameof(MethodDependencyOrdered));
            var firstFixtureChildA = firstFixture.Children.Single(x => x.Name == nameof(MethodDependencyOrdered.Before));
            var firstFixtureChildB = firstFixture.Children.Single(x => x.Name == nameof(MethodDependencyOrdered.After));

            var secondFixture = result.Children.Single(x => x.Name == nameof(FixtureDependsOnFixtureWithInternalMethodDependencies));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(firstFixture.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(firstFixtureChildA.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(firstFixtureChildB.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(secondFixture.ResultState.Status, Is.EqualTo(TestStatus.Passed));

                Assert.That(FixtureDependencyEvents.Events, Is.EqualTo([
                    nameof(MethodDependencyOrdered.Before),
                    nameof(MethodDependencyOrdered.After),
                    nameof(FixtureDependsOnFixtureWithInternalMethodDependencies.AfterTest)
                ]));
            }
        }
    }
}
