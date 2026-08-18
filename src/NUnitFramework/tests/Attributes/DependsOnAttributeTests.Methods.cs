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
        public class Methods
        {
            [SetUp]
            public void SetUp()
            {
                FixtureDependencyEvents.Reset();
            }

            [Test]
            public void AppliesPropertiesForMethodDependencies()
            {
                var test = TestBuilder.MakeTestFromMethod(typeof(MethodDependencyOrdered), nameof(MethodDependencyOrdered.After));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(test.Properties.Get(PropertyNames.DependsOnMethod), Is.EqualTo(nameof(MethodDependencyOrdered.Before)));
                    Assert.That(test.Properties.Get(PropertyNames.DependsOnAllowFailure), Is.False);
                }
            }

            [Test]
            public void MethodDependenciesAreOrdered()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyOrdered)) as CompositeWorkItem;
                Assert.That(work, Is.Not.Null);

                TestBuilder.ExecuteWorkItem(work);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(work.Children, Has.Count.EqualTo(2));
                    Assert.That(work.Children[0].Test.Name, Is.EqualTo(nameof(MethodDependencyOrdered.Before)));
                    Assert.That(work.Children[1].Test.Name, Is.EqualTo(nameof(MethodDependencyOrdered.After)));
                    Assert.That(FixtureDependencyEvents.Events, Is.EqualTo(new[] { nameof(MethodDependencyOrdered.Before), nameof(MethodDependencyOrdered.After) }));
                }
            }

            [Test]
            public void DependentMethodIsSkippedIfDependencyFails()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyFailing));
                var result = TestBuilder.ExecuteWorkItem(work);

                var beforeResult = result.Children.Single(x => x.Name == nameof(MethodDependencyFailing.BeforeFailing));
                var afterResult = result.Children.Single(x => x.Name == nameof(MethodDependencyFailing.AfterFailing));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Skipped));
                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(nameof(MethodDependencyFailing.BeforeFailing)));
                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(nameof(MethodDependencyFailing.AfterFailing)));
                }
            }

            [Test]
            public void DependentMethodRunsWhenDependencyFailsAndRequiresSuccessIsFalse()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyFailingAllowed));
                var result = TestBuilder.ExecuteWorkItem(work);

                var beforeResult = result.Children.Single(x => x.Name == nameof(MethodDependencyFailingAllowed.BeforeFailing));
                var afterResult = result.Children.Single(x => x.Name == nameof(MethodDependencyFailingAllowed.AfterFailingAllowed));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(beforeResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(afterResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(nameof(MethodDependencyFailingAllowed.BeforeFailing)));
                    Assert.That(FixtureDependencyEvents.Events, Does.Contain(nameof(MethodDependencyFailingAllowed.AfterFailingAllowed)));
                }
            }

            [Test]
            public void DependentMethodIsNotRunnableIfDependencyAbsent()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyMissing));
                var result = TestBuilder.ExecuteWorkItem(work);

                var dependantResult = result.Children.Single(x => x.Name == nameof(MethodDependencyMissing.DependsOnMissingMethod));
                var expectedMsg = "Test dependency NotATestMethod can not be found. Please verify it was configured correctly and was not filtered out.";

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(dependantResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(dependantResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(dependantResult.Message, Does.Contain(expectedMsg));
                    Assert.That(FixtureDependencyEvents.Events, Does.Not.Contain(nameof(MethodDependencyMissing.DependsOnMissingMethod)));
                }
            }

            [Test]
            public void CycleMarksAllTestsAsNotRunnable()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyCycle));
                var result = TestBuilder.ExecuteWorkItem(work);

                var aResult = result.Children.Single(x => x.Name == nameof(MethodDependencyCycle.A));
                var bResult = result.Children.Single(x => x.Name == nameof(MethodDependencyCycle.B));
                var referrerResult = result.Children.Single(x => x.Name == nameof(MethodDependencyCycle.Referrer));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(aResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(bResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(referrerResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(aResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(bResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(referrerResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(aResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(bResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(referrerResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(FixtureDependencyEvents.Events, Is.Empty);
                }
            }

            [Test]
            public void SelfReferentialMethodMarksTestAsNotRunnable()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencySelfReferential));
                var result = TestBuilder.ExecuteWorkItem(work);

                var selfResult = result.Children.Single(x => x.Name == nameof(MethodDependencySelfReferential.Self));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(selfResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(selfResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(selfResult.Message, Does.Contain("Circular or self-referential test dependency detected."));
                    Assert.That(FixtureDependencyEvents.Events, Is.Empty);
                }
            }

            [Test]
            public void DependencyTargetWithOrderIsMarkedInvalid()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyOrderTarget));
                var result = TestBuilder.ExecuteWorkItem(work);

                var referrerResult = result.Children.Single(x => x.Name == nameof(MethodDependencyOrderTarget.Referrer));
                var targetResult = result.Children.Single(x => x.Name == nameof(MethodDependencyOrderTarget.Target));

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
            public void TypeConstructorOnMethodMarksMethodInvalid()
            {
                var work = TestBuilder.CreateWorkItem(typeof(MethodDependencyInvalidTypeOnMethod));
                var result = TestBuilder.ExecuteWorkItem(work);

                var dependencyTargetResult = result.Children.Single(x => x.Name == nameof(MethodDependencyInvalidTypeOnMethod.DependencyTarget));
                var dependantMethodResult = result.Children.Single(x => x.Name == nameof(MethodDependencyInvalidTypeOnMethod.DependantMethod));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(dependencyTargetResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(dependantMethodResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                    Assert.That(dependantMethodResult.ResultState.Label, Is.EqualTo("Invalid"));
                    Assert.That(dependantMethodResult.Message, Does.Contain("DependsOnAttribute Type constructor may only be used on fixtures."));
                }
            }
        }
    }
}
