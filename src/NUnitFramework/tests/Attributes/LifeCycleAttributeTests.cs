// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.LifeCycleTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class LifeCycleAttributeTests
    {
        #region Basic Lifecycle
        [Test]
        public void InstancePerTestCaseCreatesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(CountingLifeCycleTestFixture));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.InstancePerTestCase);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void SingleInstanceSharesAnInstanceForEachTestCase()
        {
            var fixture = TestBuilder.MakeFixture(typeof(CountingLifeCycleTestFixture));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.SingleInstance);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(
                result.Children.Select(t => t.ResultState),
                Is.EquivalentTo(new[] { ResultState.Success, ResultState.Failure }));
        }

        [Test]
        public void InstancePerTestCaseFullLifeCycleTest()
        {
            FullLifecycleTestCase.Reset();
            var fixture = TestBuilder.MakeFixture(typeof(FullLifecycleTestCase));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.InstancePerTestCase);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), result.Message);

            Assert.That(FullLifecycleTestCase.ConstructCount, Is.EqualTo(3));
            Assert.That(FullLifecycleTestCase.DisposeCount, Is.EqualTo(3));
            Assert.That(FullLifecycleTestCase.OneTimeSetUpCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.OneTimeTearDownCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.SetUpCountTotal, Is.EqualTo(3));
            Assert.That(FullLifecycleTestCase.TearDownCountTotal, Is.EqualTo(3));
        }

        [Test]
        public void SingleInstanceFullLifeCycleTest()
        {
            FullLifecycleTestCase.Reset();
            var fixture = TestBuilder.MakeFixture(typeof(FullLifecycleTestCase));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.SingleInstance);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(
                result.Children.Select(t => t.ResultState),
                Is.EquivalentTo(new[] { ResultState.Success, ResultState.Failure, ResultState.Failure }));

            Assert.That(FullLifecycleTestCase.ConstructCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.DisposeCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.OneTimeSetUpCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.OneTimeTearDownCount, Is.EqualTo(1));
            Assert.That(FullLifecycleTestCase.SetUpCountTotal, Is.EqualTo(3));
            Assert.That(FullLifecycleTestCase.TearDownCountTotal, Is.EqualTo(3));
        }
        #endregion

        #region Validation
        [Test]
        public void InstanceOneTimeSetupTearDownThrows()
        {
            var fixture = TestBuilder.MakeFixture(typeof(InstanceOneTimeSetupAndTearDownFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Label, Is.EqualTo("Error"));
        }
        #endregion

        #region Test Annotations
        [Test]
        public void InstancePerTestCaseShouldApplyToTestFixtureSourceTests()
        {
            LifeCycleWithTestFixtureSourceFixture.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithTestFixtureSourceFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(LifeCycleWithTestFixtureSourceFixture.DisposeCalls, Is.EqualTo(4));
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestCaseTests()
        {
            FixtureWithTestCases.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTestCases));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(FixtureWithTestCases.DisposeCalls, Is.EqualTo(4));
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestCaseSourceTests()
        {
            FixtureWithTestCaseSource.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTestCaseSource));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(FixtureWithTestCaseSource.DisposeCalls, Is.EqualTo(2));
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestsWithValuesParameters()
        {
            FixtureWithValuesAttributeTest.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithValuesAttributeTest));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(FixtureWithValuesAttributeTest.DisposeCalls, Is.EqualTo(3));
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTheories()
        {
            FixtureWithTheoryTest.DisposeCalls = 0;
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTheoryTest));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(FixtureWithTheoryTest.DisposeCalls, Is.EqualTo(3));
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToRepeat()
        {
            RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter = 0;
            var fixture = TestBuilder.MakeFixture(typeof(RepeatingLifeCycleFixtureInstancePerTestCase));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            Assert.AreEqual(3, RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToParralelTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(ParallelLifeCycleFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }
        #endregion

        #region Nesting and inheritance
        [Test]
        public void NestedFeatureWithoutLifeCycleShouldInheritLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithNestedFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void NestedFeatureWithLifeCycleShouldOverrideLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithNestedOverridingFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }
        #endregion

        #region Assembly level InstancePerTestCase

#if NETFRAMEWORK
        [Test]
        public void AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelFixtureLifeCycleTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTestFixtureSource_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelLifeCycleTestFixtureSourceTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTestCases_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelLifeCycleFixtureWithTestCasesTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTestCaseSource_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelLifeCycleFixtureWithTestCaseSourceTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithValuesAttribute_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelLifeCycleFixtureWithValuesTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTheory_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelLifeCycleFixtureWithTheoryTest.Code, new[] { typeof(Test).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }
#endif
        #endregion


    }
}
