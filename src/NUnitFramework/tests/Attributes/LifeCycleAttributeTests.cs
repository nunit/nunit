// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.LifeCycleTests;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [NonParallelizable]
    public class LifeCycleAttributeTests
    {
        private readonly string[] _referenceAssemblies =
        [
            typeof(Test).Assembly.Location,
            typeof(BaseLifeCycle).Assembly.Location,
            typeof(DirectoryAssert).Assembly.Location,
        ];

        private TestCompiler _compiler;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _compiler = new TestCompiler(_referenceAssemblies);
        }

        [SetUp]
        public void SetUp()
        {
            BaseLifeCycle.Reset();
        }

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
            BaseLifeCycle.Reset();
            var fixture = TestBuilder.MakeFixture(typeof(FullLifecycleTestCase));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.InstancePerTestCase);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed), result.Message);

            BaseLifeCycle.VerifyInstancePerTestCase(3);
        }

        [Test]
        public void SingleInstanceFullLifeCycleTest()
        {
            BaseLifeCycle.Reset();
            var fixture = TestBuilder.MakeFixture(typeof(FullLifecycleTestCase));
            var attr = new FixtureLifeCycleAttribute(LifeCycle.SingleInstance);
            attr.ApplyToTest(fixture);

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(
                result.Children.Select(t => t.ResultState),
                Is.EquivalentTo(new[] { ResultState.Success, ResultState.Failure, ResultState.Failure }));

            BaseLifeCycle.VerifySingleInstance(3);
        }
        #endregion

        #region Fixture Validation
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
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithTestFixtureSourceFixture));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            // TODO: OneTimeSetUpCount is called twice. Expected? Does seem consistent w/ reusing the class; then there are also two calls.
            BaseLifeCycle.VerifyInstancePerTestCase(4, 2);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestCaseTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTestCases));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(4);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestCaseSourceTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTestCaseSource));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(2);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTestsWithValuesParameters()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithValuesAttributeTest));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(3);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToTheories()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithTheoryTest));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(3);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToRepeat()
        {
            RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter = 0;
            var fixture = TestBuilder.MakeFixture(typeof(RepeatingLifeCycleFixtureInstancePerTestCase));
            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter, Is.EqualTo(3));
            BaseLifeCycle.VerifyInstancePerTestCase(3);
        }

        [Test]
        public void InstancePerTestCaseShouldApplyToParralelTests()
        {
            var fixture = TestBuilder.MakeFixture(typeof(ParallelLifeCycleFixtureInstancePerTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(3);
        }
        #endregion

        #region Nesting and inheritance
        [Test]
        public void NestedFeatureWithoutLifeCycleShouldInheritLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithNestedFixture.NestedFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(2);
        }

        [Test]
        public void NestedFeatureWithLifeCycleShouldOverrideLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleWithNestedOverridingFixture.NestedFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void ChildClassWithoutLifeCycleShouldInheritLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleInheritedFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(2);
        }

        [Test]
        public void ChildClassWithLifeCycleShouldOverrideLifeCycle()
        {
            var fixture = TestBuilder.MakeFixture(typeof(LifeCycleInheritanceOverriddenFixture));
            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }
        #endregion

        #region DisposeOnly
        [Test]
        public void InstancePerTestCaseWithDispose()
        {
            var fixture = TestBuilder.MakeFixture(typeof(InstancePerTestCaseWithDisposeTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(InstancePerTestCaseWithDisposeTestCase.DisposeCount, Is.EqualTo(fixture.TestCaseCount));
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void InstancePerTestCaseWithAsyncDispose()
        {
            var fixture = TestBuilder.MakeFixture(typeof(InstancePerTestCaseWithAsyncDisposeTestCase));

            ITestResult result = TestBuilder.RunTest(fixture);
            Assert.That(InstancePerTestCaseWithAsyncDisposeTestCase.DisposeCount, Is.EqualTo(fixture.TestCaseCount));
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        #endregion

        #region Assembly level InstancePerTestCase

        [Test]
        public void AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelFixtureLifeCycleTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(2);
        }

        [Test]
        public void FixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = _compiler.GenerateInMemoryAssembly(OverrideAssemblyLevelFixtureLifeCycleTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void OuterFixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycleInNestedFixture()
        {
            var asm = _compiler.GenerateInMemoryAssembly(NestedOverrideAssemblyLevelFixtureLifeCycleTest.OuterClass);
            var testType = asm.GetType("FixtureUnderTest+NestedFixture");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void InnerFixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycleInNestedFixture()
        {
            var asm = _compiler.GenerateInMemoryAssembly(NestedOverrideAssemblyLevelFixtureLifeCycleTest.InnerClass);
            var testType = asm.GetType("FixtureUnderTest+NestedFixture");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void BaseLifecycleShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = _compiler.GenerateInMemoryAssembly(InheritedOverrideTest.InheritClassWithOtherLifecycle);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void BaseLifecycleFromOtherAssemblyShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = _compiler.GenerateInMemoryAssembly(InheritedOverrideTest.InheritClassWithOtherLifecycleFromOtherAssembly);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void GivenFixtureWithTestFixtureSource_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelLifeCycleTestFixtureSourceTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTestCases_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelLifeCycleFixtureWithTestCasesTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTestCaseSource_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelLifeCycleFixtureWithTestCaseSourceTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithValuesAttribute_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelLifeCycleFixtureWithValuesTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void GivenFixtureWithTheory_AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = _compiler.GenerateInMemoryAssembly(AssemblyLevelLifeCycleFixtureWithTheoryTest.Code);
            var testType = asm.GetType("FixtureUnderTest");
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        #endregion
    }
}
