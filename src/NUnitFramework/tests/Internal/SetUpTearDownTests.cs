// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.SetUpData;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class SetUpTearDownTests
    {
        [Test]
        public void SetUpAndTearDownCounter()
        {
            SetUpAndTearDownCounterFixture fixture = new SetUpAndTearDownCounterFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCounter, Is.EqualTo(3));
            Assert.That(fixture.TearDownCounter, Is.EqualTo(3));
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreCalled()
        {
            SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUpCalled, Is.True);
            Assert.That(fixture.WasTearDownCalled, Is.True);
        }

        [Test]
        public void CheckInheritedSetUpAndTearDownAreCalled()
        {
            InheritSetUpAndTearDown fixture = new InheritSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUpCalled, Is.True);
            Assert.That(fixture.WasTearDownCalled, Is.True);
        }

        [Test]
        public void CheckOverriddenSetUpAndTearDownAreNotCalled()
        {
            DefineInheritSetUpAndTearDown fixture = new DefineInheritSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUpCalled, Is.False);
            Assert.That(fixture.WasTearDownCalled, Is.False);
            Assert.That(fixture.DerivedSetUpCalled, Is.True);
            Assert.That(fixture.DerivedTearDownCalled, Is.True);
        }

        [Test]
        public void MultipleSetUpAndTearDownMethodsAreCalled()
        {
            MultipleSetUpTearDownFixture fixture = new MultipleSetUpTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUp1Called, Is.True, "SetUp1");
            Assert.That(fixture.WasSetUp2Called, Is.True, "SetUp2");
            Assert.That(fixture.WasSetUp3Called, Is.True, "SetUp3");
            Assert.That(fixture.WasTearDown1Called, Is.True, "TearDown1");
            Assert.That(fixture.WasTearDown2Called, Is.True, "TearDown2");
        }

        [Test]
        public void BaseSetUpIsCalledFirstTearDownLast()
        {
            DerivedClassWithSeparateSetUp fixture = new DerivedClassWithSeparateSetUp();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUpCalled, Is.True, "Base SetUp Called");
            Assert.That(fixture.WasTearDownCalled, Is.True, "Base TearDown Called");
            Assert.That(fixture.WasDerivedSetUpCalled, Is.True, "Derived SetUp Called");
            Assert.That(fixture.WasDerivedTearDownCalled, Is.True, "Derived TearDown Called");
            Assert.That(fixture.WasBaseSetUpCalledFirst, Is.True, "SetUp Order");
            Assert.That(fixture.WasBaseTearDownCalledLast, Is.True, "TearDown Order");
        }

        [Test]
        public void FailureInBaseSetUpCausesDerivedSetUpAndTearDownToBeSkipped()
        {
            DerivedClassWithSeparateSetUp fixture = new DerivedClassWithSeparateSetUp();
            fixture.ThrowInBaseSetUp = true;
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.WasSetUpCalled, Is.True, "Base SetUp Called");
            Assert.That(fixture.WasTearDownCalled, Is.True, "Base TearDown Called");
            Assert.That(fixture.WasDerivedSetUpCalled, Is.False, "Derived SetUp Called");
            Assert.That(fixture.WasDerivedTearDownCalled, Is.False, "Derived TearDown Called");
        }

        [Test]
        public void HandleExceptionInSetUp()
        {
            Exception e = new Exception("Test message for exception thrown from setup");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.SetupException = e;
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.That(suiteResult.HasChildren, Is.True, "Fixture test should have child result.");
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Error), "Test should be in error state");
            string expected = $"{e.GetType().FullName} : {e.Message}";
            Assert.That(result.Message, Is.EqualTo(expected));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName())); // Sanity check
            });
        }

        [Test]
        public void HandleExceptionInTearDown()
        {
            Exception e = new Exception("Test message for exception thrown from tear down");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.TearDownException = e;
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.That(suiteResult.HasChildren, "Fixture test should have child result.");
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Error), "Test should be in error state");
            string expected = $"TearDown : {e.GetType().FullName} : {e.Message}";
            Assert.That(result.Message, Is.EqualTo(expected));
            Assert.That(result.StackTrace, Does.StartWith("--TearDown"));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName())); // Sanity check
            });
        }

        [Test]
        public void HandleExceptionInBothSetUpAndTearDown()
        {
            Exception e1 = new Exception("Test message for exception thrown from setup");
            Exception e2 = new Exception("Test message for exception thrown from tear down");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.SetupException = e1;
            fixture.TearDownException = e2;
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.That(suiteResult.HasChildren, "Fixture test should have child result.");
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Error), "Test should be in error state");
            string expected = $"{e1.GetType().FullName} : {e1.Message}" + Environment.NewLine
                                                                        + $"TearDown : {e2.GetType().FullName} : {e2.Message}";
            Assert.That(result.Message, Is.EqualTo(expected));
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName())); // Sanity check
            });
        }

        public class SetupCallBase
        {
            protected int SetupCount = 0;
            public virtual void Init()
            {
                SetupCount++;
            }
            public virtual void AssertCount()
            {
            }
        }

        [TestFixture]
        // Test for bug 441022
        public class SetupCallDerived : SetupCallBase
        {
            [SetUp]
            public override void Init()
            {
                SetupCount++;
                base.Init();
            }
            [Test]
            public override void AssertCount()
            {
                Assert.That(SetupCount, Is.EqualTo(2));
            }
        }
    }
}
