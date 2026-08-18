// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    public partial class DependsOnAttributeTests
    {
        [TestFixture]
        public class Fixtures
        {
            [SetUp]
            public void SetUp()
            {
                FixtureDependencyEvents.Reset();
            }

            [Test]
            public void AppliesPropertiesForFixtureDependencies()
            {
                var fixture = TestBuilder.MakeFixture<FixtureDependencyAfter>();

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(fixture.Properties.Get(PropertyNames.DependsOnFixture), Is.EqualTo(typeof(FixtureDependencyBefore)));
                    Assert.That(fixture.Properties.Get(PropertyNames.DependsOnAllowFailure), Is.False);
                }
            }

            [Test]
            public void TestDependenciesAreOrdered()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfter), typeof(FixtureDependencyBefore));

                var work = (CompositeWorkItem)TestBuilder.CreateWorkItem(suite);
                Assert.That(work, Is.Not.Null);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(work.Children, Has.Count.EqualTo(2));

                    Assert.That(suite.Tests[0].Name, Is.EqualTo(nameof(FixtureDependencyAfter)));
                    Assert.That(suite.Tests[1].Name, Is.EqualTo(nameof(FixtureDependencyBefore)));

                    Assert.That(work.Children[0].Test.Name, Is.EqualTo(nameof(FixtureDependencyBefore)));
                    Assert.That(work.Children[1].Test.Name, Is.EqualTo(nameof(FixtureDependencyAfter)));
                }
            }

            [Test]
            public void DependentTestIsSkippedIfDependencyFails()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfterFailing), typeof(FixtureDependencyBeforeFailing));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var beforeResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyBeforeFailing));
                var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyAfterFailing));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Skipped));

                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(beforeResult.Name + ".OneTimeTearDown"));
                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(afterResult.Name + ".OneTimeSetUp"));
                }
            }

            [Test]
            public void DependentTestIsNotRunnableIfDependencyAbsent()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfter));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyAfter));
                var expectedMsg = $"Test dependency {typeof(FixtureDependencyBefore)} can not be found. Please verify it was configured correctly and was not filtered out.";

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(afterResult.Message, Does.Contain(expectedMsg));

                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(afterResult.Name + ".OneTimeSetUp"));
                }
            }

            [Test]
            public void DependentTestRunsWhenDependencyFailsAndRequiresSuccessIsFalse()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfterFailingAllowed), typeof(FixtureDependencyBeforeFailing));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var beforeResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyBeforeFailing));
                var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyAfterFailingAllowed));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));

                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(beforeResult.Name + ".OneTimeTearDown"));
                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(afterResult.Name + ".OneTimeSetUp"));
                }
            }

            [Test]
            public void DependentTestsAreOrderedWhenDependenciesFork()
            {
                var suite = new TestSuite("dummy").Containing(typeof(ForkingDependencyRoot), typeof(ForkingDependencyNodeA), typeof(ForkingDependencyNodeB));

                var work = (CompositeWorkItem)TestBuilder.CreateWorkItem(suite);
                Assert.That(work, Is.Not.Null);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(work.Children, Has.Count.EqualTo(3));
                    Assert.That(work.Children[0].Test.Name, Is.EqualTo(nameof(ForkingDependencyRoot)));
                    Assert.That(work.Children[1].Test.Name, Is.EqualTo(nameof(ForkingDependencyNodeA)));
                    Assert.That(work.Children[2].Test.Name, Is.EqualTo(nameof(ForkingDependencyNodeB)));
                }
            }

            [Test]
            public void CycleMarksAllTestsAsNotRunnable()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyCycleA), typeof(FixtureDependencyCycleB));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var cycleAResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleA));
                var cycleBResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleB));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(cycleAResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(cycleBResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(cycleAResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(cycleBResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(cycleAResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(cycleBResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(FixtureDependencyEvents.Events, Is.Empty);
                }
            }

            [Test]
            public void DependentTestReferencingCycleIsMarkedAsNotRunnable()
            {
                var suite = new TestSuite("dummy")
                    .Containing(typeof(FixtureDependencyCycleReferrer), typeof(FixtureDependencyCycleA), typeof(FixtureDependencyCycleB));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var cycleReferrerResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleReferrer));
                var cycleAResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleA));
                var cycleBResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyCycleB));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(cycleReferrerResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(cycleAResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(cycleBResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(cycleReferrerResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(cycleReferrerResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(FixtureDependencyEvents.Events, Is.Empty);
                }
            }

            [Test]
            public void SelfReferentialMarksTestAsNotRunnable()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencySelfReferential));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var selfReferentialResult = result.Children.Single(x => x.Name == nameof(FixtureDependencySelfReferential));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(selfReferentialResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(selfReferentialResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(selfReferentialResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(FixtureDependencyEvents.Events, Is.Empty);
                }
            }

            [Test]
            public void DependentTestUsingDependsOnAndOrderIsMarkedInvalid()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyOrderBefore), typeof(FixtureDependencyOrderAfter));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var beforeResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyOrderBefore));
                var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyOrderAfter));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(afterResult.Message, Does.Contain("Test may not participate in both DependsOn and Order chains."));
                }
            }

            [Test]
            public void DependencyTargetWithOrderIsMarkedInvalid()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyOrderReferrer), typeof(FixtureDependencyOrderTarget));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var referrerResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyOrderReferrer));
                var targetResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyOrderTarget));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(referrerResult.ResultState.Status, Is.EqualTo(TestStatus.Skipped));
                    Assert.That(referrerResult.Message, Does.Contain("did not pass"));
                    Assert.That(targetResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(targetResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(targetResult.Message, Does.Contain("Test may not participate in both DependsOn and Order chains."));
                }
            }

            [Test]
            public void OrderAndDependsOnCoexistIndependently()
            {
                var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyAfter), typeof(FixtureDependencyBefore), typeof(FixtureDependencyOrderTarget));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var dependencyAfter = result.Children.Single(x => x.Name == nameof(FixtureDependencyAfter));
                var dependencyBefore = result.Children.Single(x => x.Name == nameof(FixtureDependencyBefore));
                var order = result.Children.Single(x => x.Name == nameof(FixtureDependencyOrderTarget));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(dependencyBefore.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(dependencyAfter.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(order.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                }
            }

            [Test]
            public void ParallelConfiguredTestInvalidatesDependencyChain()
            {
                var suite = new TestSuite("dummy")
                    .Containing(typeof(FixtureDependencyParallelAfter), typeof(FixtureDependencyParallelBeforeSlow), typeof(FixtureDependencyParallelIndependent));
                suite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.Fixtures);

                var dispatcher = new ParallelWorkItemDispatcher(4);
                var context = new TestExecutionContext
                {
                    Dispatcher = dispatcher
                };

                var work = TestBuilder.CreateWorkItem(suite, context) as CompositeWorkItem;
                Assert.That(work, Is.Not.Null);

                dispatcher.Start(work);
                work.WaitForCompletion();

                var result = work.Result;
                var beforeResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyParallelBeforeSlow));
                var afterResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyParallelAfter));
                var independentResult = result.Children.Single(x => x.Name == nameof(FixtureDependencyParallelIndependent));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(beforeResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(afterResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(beforeResult.Message, Does.Contain("Test dependency chains may not include tests configured for parallel execution."));
                    Assert.That(afterResult.Message, Does.Contain("Test dependency chains may not include tests configured for parallel execution."));
                    Assert.That(independentResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));

                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(beforeResult.Name + ".OneTimeSetUp"));
                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(afterResult.Name + ".OneTimeSetUp"));
                }
            }

            [Test]
            public void StringConstructorOnFixtureMarksFixtureInvalid()
            {
                var suite = new TestSuite("dummy").Containing(typeof(MethodDependencyInvalidStringOnFixture));

                var work = TestBuilder.CreateWorkItem(suite);
                var result = TestBuilder.ExecuteWorkItem(work);

                var fixtureResult = result.Children.Single(x => x.Name == nameof(MethodDependencyInvalidStringOnFixture));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(fixtureResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(fixtureResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(fixtureResult.Message, Does.Contain("DependsOnAttribute string constructor may only be used on methods."));
                }
            }
        }
    }
}
