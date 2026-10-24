// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [NonParallelizable]
    public class DependsOnFixtureAttributeTests
    {
        [SetUp]
        public void SetUp()
        {
            FixtureDependencyEvents.Reset();
        }

        [Test]
        public void AttributeUsageIsClassLevelSingleUse()
        {
            var usage = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(typeof(DependsOnFixtureAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage, Is.Not.Null, $"{nameof(DependsOnFixtureAttribute)} should have an AttributeUsage attribute.");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(usage.ValidOn, Is.EqualTo(AttributeTargets.Class));
                Assert.That(usage.AllowMultiple, Is.False);
                Assert.That(usage.Inherited, Is.False);
            }
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
                Assert.That(afterResult.Message, Does.Contain($"Dependency on {nameof(FixtureDependencyBeforeFailing)} did not pass"));
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
            }
        }

        [Test]
        public void FilteredOutDependencyFixtureIsStillRunWhenDependentFixtureIsSelected()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependsOnFixtureWithInternalMethodDependencies), typeof(MethodDependencyOrdered));
            var filter = new FullNameFilter(typeof(FixtureDependsOnFixtureWithInternalMethodDependencies).FullName!);

            var work = TestBuilder.CreateWorkItem(suite, filter);
            var result = TestBuilder.ExecuteWorkItem(work);

            var dependencyResult = result.Children.Single(x => x.Name == nameof(MethodDependencyOrdered));
            var dependencyChildren = dependencyResult.Children.ToArray();
            var dependentResult = result.Children.Single(x => x.Name == nameof(FixtureDependsOnFixtureWithInternalMethodDependencies));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dependencyResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(dependentResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(dependencyChildren, Has.Length.EqualTo(2));
                Assert.That(dependencyChildren[0].Name, Is.EqualTo(nameof(MethodDependencyOrdered.Before)));
                Assert.That(dependencyChildren[1].Name, Is.EqualTo(nameof(MethodDependencyOrdered.After)));
                Assert.That(FixtureDependencyEvents.Events, Is.EqualTo([nameof(MethodDependencyOrdered.Before), nameof(MethodDependencyOrdered.After), nameof(FixtureDependsOnFixtureWithInternalMethodDependencies.AfterTest)]));
            }
        }

        [Test]
        public void FilteredOutDependencyIsNotRunWhenDependentFixtureIsFilteredOut()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencyFilteredOutDependentAfter), typeof(FixtureDependencyFilteredOutDependentBefore), typeof(FixtureDependencyFilteredOutIndependent));
            var filter = new FullNameFilter(typeof(FixtureDependencyFilteredOutIndependent).FullName!);

            var work = TestBuilder.CreateWorkItem(suite, filter);
            var result = TestBuilder.ExecuteWorkItem(work);

            var resultChildren = result.Children.ToArray();
            var independentResult = resultChildren.Single(x => x.Name == nameof(FixtureDependencyFilteredOutIndependent));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultChildren, Has.Length.EqualTo(1));
                Assert.That(independentResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(FixtureDependencyEvents.Events, Is.EqualTo([nameof(FixtureDependencyFilteredOutIndependent.IndependentTest)]));
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
            }
        }

        [Test]
        public void SelfReferentialBaseMarksFixtureAsNotRunnable()
        {
            var suite = new TestSuite("dummy").Containing(typeof(FixtureDependencySelfReferentialBase));

            var work = TestBuilder.CreateWorkItem(suite);
            var result = TestBuilder.ExecuteWorkItem(work);

            var selfReferentialResult = result.Children.Single(x => x.Name == nameof(FixtureDependencySelfReferentialBase));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(selfReferentialResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(selfReferentialResult.ResultState.Label, Is.EqualTo("Invalid"));
                Assert.That(selfReferentialResult.Message, Does.Contain("Circular or self-referential test dependency detected via base class."));
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

        [Test]
        public void DependenciesAreOrderedWhenFirstChildIsNotAFixture()
        {
            // A child namespace suite or set-up fixture can come before the fixtures.
            var suite = new TestSuite("dummy")
                .Containing(new TestSuite("child"))
                .Containing(typeof(FixtureDependencyAfter), typeof(FixtureDependencyBefore));

            var work = (CompositeWorkItem)TestBuilder.CreateWorkItem(suite);
            var names = work.Children.Select(c => c.Test.Name).ToList();

            Assert.That(names.IndexOf(nameof(FixtureDependencyBefore)), Is.LessThan(names.IndexOf(nameof(FixtureDependencyAfter))));
        }

        [Test]
        public void ChainRunsInOrderWithQueuingDispatcher()
        {
            var suite = new TestSuite("dummy").Containing(
                typeof(QueuedDependencyAfterAfter),
                typeof(QueuedDependencyAfter),
                typeof(QueuedDependencyBeforeSlow),
                typeof(QueuedDependencyParallelIndependent));

            var result = RunWithParallelDispatcher(suite);

            using (Assert.EnterMultipleScope())
            {
                foreach (var child in result.Children)
                    Assert.That(child.ResultState.Status, Is.EqualTo(TestStatus.Passed), child.Name);

                var chain = FixtureDependencyEvents.Events.Where(e => e != nameof(QueuedDependencyParallelIndependent.QueuedParallelIndependentTest));
                Assert.That(chain, Is.EqualTo([
                    nameof(QueuedDependencyBeforeSlow.QueuedBeforeSlowTest),
                    nameof(QueuedDependencyAfter.QueuedAfterTest),
                    nameof(QueuedDependencyAfterAfter.QueuedAfterAfterTest)
                ]));
            }
        }

        [Test]
        public void FailingDependencySkipsChainWithQueuingDispatcher()
        {
            var suite = new TestSuite("dummy").Containing(
                typeof(QueuedDependencyAfterSkipped),
                typeof(QueuedDependencyAfterFailing),
                typeof(QueuedDependencyAfterFailingAllowed),
                typeof(QueuedDependencyBeforeSlowFailing));

            var result = RunWithParallelDispatcher(suite);

            ITestResult Child(string name) => result.Children.Single(x => x.Name == name);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Child(nameof(QueuedDependencyBeforeSlowFailing)).ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(Child(nameof(QueuedDependencyAfterFailing)).ResultState.Status, Is.EqualTo(TestStatus.Skipped));
                Assert.That(Child(nameof(QueuedDependencyAfterFailing)).Message, Does.Contain($"Dependency on {nameof(QueuedDependencyBeforeSlowFailing)} did not pass"));
                Assert.That(Child(nameof(QueuedDependencyAfterSkipped)).ResultState.Status, Is.EqualTo(TestStatus.Skipped));
                Assert.That(Child(nameof(QueuedDependencyAfterSkipped)).Message, Does.Contain($"Dependency on {nameof(QueuedDependencyAfterFailing)} did not pass"));
                Assert.That(Child(nameof(QueuedDependencyAfterFailingAllowed)).ResultState.Status, Is.EqualTo(TestStatus.Passed));

                // AllowFailure still waits for the dependency to finish.
                Assert.That(FixtureDependencyEvents.Events, Is.EqualTo([
                    nameof(QueuedDependencyBeforeSlowFailing.QueuedBeforeSlowFailingTest),
                    nameof(QueuedDependencyAfterFailingAllowed.QueuedAfterFailingAllowedTest)
                ]));
            }
        }

        [Test]
        public void CancelWhileDependentWaitsCompletesWithoutRunningDependent()
        {
            QueuedDependencyBeforeBlocking.Reset();
            var suite = new TestSuite("dummy").Containing(
                typeof(QueuedDependencyAfterBlocking),
                typeof(QueuedDependencyBeforeBlocking));

            var work = StartWithParallelDispatcher(suite);

            // The dependency is running, so its dependent is waiting.
            Assert.That(QueuedDependencyBeforeBlocking.Started.Wait(TimeSpan.FromSeconds(10)), Is.True, "Dependency did not start");
            work.Cancel();
            QueuedDependencyBeforeBlocking.Release.Set();

            Assert.That(Task.Run(work.WaitForCompletion).Wait(TimeSpan.FromSeconds(10)), Is.True, "Run did not complete after cancel");

            var after = work.Result.Children.Single(x => x.Name == nameof(QueuedDependencyAfterBlocking));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(after.ResultState.Status, Is.Not.EqualTo(TestStatus.Passed));
                Assert.That(FixtureDependencyEvents.Events, Is.EqualTo([nameof(QueuedDependencyBeforeBlocking.QueuedBeforeBlockingTest)]));
            }
        }

        [Test]
        public void LongWaitingChainDoesNotGrowStack([Values] bool firstLinkFails)
        {
            int shortChainDepth = RunChainAndGetStackDepth(1, firstLinkFails);
            int longChainDepth = RunChainAndGetStackDepth(200, firstLinkFails);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(shortChainDepth, Is.GreaterThan(0), "Last link did not run");
                // Small margin: the call path into the first completion can differ by a few frames.
                // Recursion would add hundreds of frames for 200 links.
                Assert.That(longChainDepth, Is.LessThanOrEqualTo(shortChainDepth + 10));
            }
        }

        // The first link is queued as non-parallel, so all later links wait for it and are started
        // from completion callbacks. They run directly, or are skipped when the first link fails.
        // The last link allows failure, so it always runs and records its stack depth.
        private static int RunChainAndGetStackDepth(int links, bool firstLinkFails)
        {
            QueuedDependencyChain.RecordedStackDepth = 0;
            var fixture = TestBuilder.MakeFixture(typeof(QueuedDependencyChain));
            fixture.Tests.Clear();

            var first = TestBuilder.MakeTestCase(typeof(QueuedDependencyChain),
                firstLinkFails ? nameof(QueuedDependencyChain.FailingLink) : nameof(QueuedDependencyChain.PassingLink));
            first.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);
            fixture.Add(first);

            string previous = first.Name;
            for (int i = 0; i < links; i++)
                previous = AddLink(nameof(QueuedDependencyChain.PassingLink), "Link" + i, previous).Name;

            AddLink(nameof(QueuedDependencyChain.RecordStackDepth), "Last", previous)
                .Properties.Set(PropertyNames.DependsOnAllowFailure, true);

            RunWithParallelDispatcher(fixture);
            return QueuedDependencyChain.RecordedStackDepth;

            TestMethod AddLink(string method, string name, string dependsOn)
            {
                var link = TestBuilder.MakeTestCase(typeof(QueuedDependencyChain), method);
                link.Name = name;
                link.Properties.Set(PropertyNames.DependsOnTest, dependsOn);
                fixture.Add(link);
                return link;
            }
        }

        [Test]
        [Platform(Includes = [PlatformNames.Win])]
        public void DependentStartedFromOtherWorkerReportsThatWorker()
        {
            QueuedDependencyOtherWorker.DependentThreadName = null;
            QueuedDependencyOtherWorker.DependentWorkerId = null;

            var result = RunWithParallelDispatcher(TestBuilder.MakeFixture(typeof(QueuedDependencyOtherWorker)));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(QueuedDependencyOtherWorker.DependentThreadName, Is.EqualTo("NUnit.Fw.NonParallelSTAWorker"));
                Assert.That(QueuedDependencyOtherWorker.DependentWorkerId, Is.EqualTo(QueuedDependencyOtherWorker.DependentThreadName));
            }
        }

        private static ITestResult RunWithParallelDispatcher(TestSuite suite)
        {
            var work = StartWithParallelDispatcher(suite);
            work.WaitForCompletion();

            return work.Result;
        }

        // ParallelWorkItemDispatcher only queues work items, unlike the synchronous TestBuilder dispatcher.
        private static WorkItem StartWithParallelDispatcher(TestSuite suite)
        {
            var dispatcher = new ParallelWorkItemDispatcher(4);
            var context = new TestExecutionContext
            {
                Dispatcher = dispatcher
            };

            var work = TestBuilder.CreateWorkItem(suite, context);
            dispatcher.Start(work);

            return work;
        }
    }
}
