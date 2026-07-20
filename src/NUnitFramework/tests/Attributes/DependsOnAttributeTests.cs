// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class DependsOnAttributeTests
    {
        [SetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Reset();
        }

        [Test]
        public void AttributeUsageIsClassLevelSingleUse()
        {
            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(DependsOnAttribute), typeof(AttributeUsageAttribute))!;

            Assert.Multiple(() =>
            {
                Assert.That(usage.ValidOn, Is.EqualTo(AttributeTargets.Class));
                Assert.That(usage.AllowMultiple, Is.False);
                Assert.That(usage.Inherited, Is.False);
            });
        }

        [Test]
        public void AppliesDependencyTypeToFixtureProperties()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureDependencyAfter));

            Assert.That(fixture.Properties.Get(PropertyNames.DependsOn), Is.EqualTo(typeof(FixtureDependencyBefore)));
        }

        [Test]
        public void DependencyFixturesAreOrderedAndMarkedNonParallel()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfter), typeof(FixtureDependencyBefore));

            var work = TestBuilder.CreateWorkItem(suite) as CompositeWorkItem;
            Assert.That(work, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(work!.Children, Has.Count.EqualTo(2));
                Assert.That(work.Children[0].Test.Name, Is.EqualTo(nameof(FixtureDependencyBefore)));
                Assert.That(work.Children[1].Test.Name, Is.EqualTo(nameof(FixtureDependencyAfter)));
                Assert.That(work.Children[0].ParallelScope, Is.EqualTo(ParallelScope.None));
                Assert.That(work.Children[1].ParallelScope, Is.EqualTo(ParallelScope.None));
            });
        }

        [Test]
        public void DependentFixtureRunsAfterDependencyCompletesEvenIfDependencyFails()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfterFailing), typeof(FixtureDependencyBeforeFailing));

            var work = TestBuilder.CreateWorkItem(suite) as CompositeWorkItem;
            Assert.That(work, Is.Not.Null);

            var result = TestBuilder.ExecuteWorkItem(work!);

            var beforeResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyBeforeFailing));
            var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyAfterFailing));

            var beforeTearDownIndex = FixtureDependencyEvents.Events.IndexOf(nameof(FixtureDependencyBeforeFailing) + ".OneTimeTearDown");
            var afterSetUpIndex = FixtureDependencyEvents.Events.IndexOf(nameof(FixtureDependencyAfterFailing) + ".OneTimeSetUp");

            Assert.Multiple(() =>
            {
                Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(beforeTearDownIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(afterSetUpIndex, Is.GreaterThan(beforeTearDownIndex));
            });
        }

        [Test]
        public void CycleMarksAllFixturesAsNotRunnable()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyCycleA), typeof(FixtureDependencyCycleB));

            var work = TestBuilder.CreateWorkItem(suite) as CompositeWorkItem;
            Assert.That(work, Is.Not.Null);

            var result = TestBuilder.ExecuteWorkItem(work!);

            var cycleAResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleA));
            var cycleBResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleB));

            Assert.Multiple(() =>
            {
                Assert.That(cycleAResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(cycleBResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(cycleAResult.ResultState.Label, Is.EqualTo("Invalid"));
                Assert.That(cycleBResult.ResultState.Label, Is.EqualTo("Invalid"));
                Assert.That(cycleAResult.Message, Does.Contain("Circular DependsOn dependency detected."));
                Assert.That(cycleBResult.Message, Does.Contain("Circular DependsOn dependency detected."));
                Assert.That(FixtureDependencyEvents.Events, Is.Empty);
            });
        }
    }
}
