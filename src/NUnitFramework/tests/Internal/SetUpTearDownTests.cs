// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.TestUtilities;
using NUnit.TestData.SetUpData;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class SetUpTearDownTests
    {
        [Test]
        public void SetUpAndTearDownCounter()
        {
            SetUpAndTearDownCounterFixture fixture = new SetUpAndTearDownCounterFixture();
            TestBuilder.RunTestFixture( fixture );

            Assert.AreEqual(3, fixture.SetUpCounter);
            Assert.AreEqual(3, fixture.TearDownCounter);
        }


        [Test]
        public void MakeSureSetUpAndTearDownAreCalled()
        {
            SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
            TestBuilder.RunTestFixture( fixture );

            Assert.IsTrue(fixture.WasSetUpCalled);
            Assert.IsTrue(fixture.WasTearDownCalled);
        }

        [Test]
        public void CheckInheritedSetUpAndTearDownAreCalled()
        {
            InheritSetUpAndTearDown fixture = new InheritSetUpAndTearDown();
            TestBuilder.RunTestFixture( fixture );

            Assert.IsTrue(fixture.WasSetUpCalled);
            Assert.IsTrue(fixture.WasTearDownCalled);
        }

        [Test]
        public void CheckOverriddenSetUpAndTearDownAreNotCalled()
        {
            DefineInheritSetUpAndTearDown fixture = new DefineInheritSetUpAndTearDown();
            TestBuilder.RunTestFixture( fixture );

            Assert.IsFalse(fixture.WasSetUpCalled);
            Assert.IsFalse(fixture.WasTearDownCalled);
            Assert.IsTrue(fixture.DerivedSetUpCalled);
            Assert.IsTrue(fixture.DerivedTearDownCalled);
        }

        [Test]
        public void MultipleSetUpAndTearDownMethodsAreCalled()
        {
            MultipleSetUpTearDownFixture fixture = new MultipleSetUpTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.IsTrue(fixture.WasSetUp1Called, "SetUp1");
            Assert.IsTrue(fixture.WasSetUp2Called, "SetUp2");
            Assert.IsTrue(fixture.WasSetUp3Called, "SetUp3");
            Assert.IsTrue(fixture.WasTearDown1Called, "TearDown1");
            Assert.IsTrue(fixture.WasTearDown2Called, "TearDown2");
        }

        [Test]
        public void BaseSetUpIsCalledFirstTearDownLast()
        {
            DerivedClassWithSeparateSetUp fixture = new DerivedClassWithSeparateSetUp();
            TestBuilder.RunTestFixture(fixture);

            Assert.IsTrue(fixture.WasSetUpCalled, "Base SetUp Called");
            Assert.IsTrue(fixture.WasTearDownCalled, "Base TearDown Called");
            Assert.IsTrue(fixture.WasDerivedSetUpCalled, "Derived SetUp Called");
            Assert.IsTrue(fixture.WasDerivedTearDownCalled, "Derived TearDown Called");
            Assert.IsTrue(fixture.WasBaseSetUpCalledFirst, "SetUp Order");
            Assert.IsTrue(fixture.WasBaseTearDownCalledLast, "TearDown Order");
        }

        [Test]
        public void FailureInBaseSetUpCausesDerivedSetUpAndTearDownToBeSkipped()
        {
            DerivedClassWithSeparateSetUp fixture = new DerivedClassWithSeparateSetUp();
            fixture.ThrowInBaseSetUp = true;
            TestBuilder.RunTestFixture(fixture);

            Assert.IsTrue(fixture.WasSetUpCalled, "Base SetUp Called");
            Assert.IsTrue(fixture.WasTearDownCalled, "Base TearDown Called");
            Assert.IsFalse(fixture.WasDerivedSetUpCalled, "Derived SetUp Called");
            Assert.IsFalse(fixture.WasDerivedTearDownCalled, "Derived TearDown Called");
        }

        [Test]
        public void HandleExceptionInSetUp()
        {
            Exception e = new Exception("Test message for exception thrown from setup");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.SetupException = e;
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.IsTrue(suiteResult.HasChildren, "Fixture test should have child result.");
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.AreEqual(ResultState.Error, result.ResultState, "Test should be in error state");
            string expected = $"{e.GetType().FullName} : {e.Message}";
            Assert.AreEqual(expected, result.Message);

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName)); // Sanity check
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
            Assert.AreEqual(ResultState.Error, result.ResultState, "Test should be in error state");
            string expected = $"TearDown : {e.GetType().FullName} : {e.Message}";
            Assert.AreEqual(expected, result.Message);
            Assert.That(result.StackTrace, Does.StartWith("--TearDown"));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName)); // Sanity check
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
            Assert.AreEqual(ResultState.Error, result.ResultState, "Test should be in error state");
            string expected = $"{e1.GetType().FullName} : {e1.Message}" + Environment.NewLine
                                                                        + $"TearDown : {e2.GetType().FullName} : {e2.Message}";
            Assert.AreEqual(expected, result.Message);
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(result.StackTrace, Does.Contain(fixture.GetType().FullName)); // Sanity check
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
                Assert.AreEqual(2, SetupCount);
            }
        }
    }
}
