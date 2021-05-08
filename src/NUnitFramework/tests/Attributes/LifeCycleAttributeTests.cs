// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.LifeCycleTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    [NonParallelizable]
    public class LifeCycleAttributeTests
    {
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
            Assert.AreEqual(3, RepeatingLifeCycleFixtureInstancePerTestCase.RepeatCounter);
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

        #region Assembly level InstancePerTestCase

#if NETFRAMEWORK
        [Test]
        public void AssemblyLevelInstancePerTestCaseShouldCreateInstanceForEachTestCase()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                AssemblyLevelFixtureLifeCycleTest.Code, new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifyInstancePerTestCase(2);
        }

        [Test]
        public void FixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                OverrideAssemblyLevelFixtureLifeCycleTest.Code,
                new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void OuterFixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycleInNestedFixture()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                NestedOverrideAssemblyLevelFixtureLifeCycleTest.OuterClass,
                new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest+NestedFixture");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void InnerFixtureLevelLifeCycleShouldOverrideAssemblyLevelLifeCycleInNestedFixture()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                NestedOverrideAssemblyLevelFixtureLifeCycleTest.InnerClass,
                new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest+NestedFixture");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void BaseLifecycleShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                InheritedOverrideTest.InheritClassWithOtherLifecycle,
                new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
        }

        [Test]
        public void BaseLifecycleFromOtherAssemblyShouldOverrideAssemblyLevelLifeCycle()
        {
            var asm = TestAssemblyHelper.GenerateInMemoryAssembly(
                InheritedOverrideTest.InheritClassWithOtherLifecycleFromOtherAssembly,
                new[] { typeof(Test).Assembly.Location, typeof(BaseLifeCycle).Assembly.Location });
            var testType = asm.GetType("FixtureUnderTest");
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            BaseLifeCycle.VerifySingleInstance(2);
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

    /// <summary>
    /// To prove that <see cref="NestedFeatureWithoutLifeCycleShouldInheritLifeCycle"/> is indeed broken.
    /// </summary>
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class Nested
    {
        [TestFixture]
        public class NestedAgain
        {
            int x;

            [Test, Order(1)]
            public void Test1()
            {
                Assert.That(x++, Is.EqualTo(0));
            }

            [Test, Order(2)]
            public void Test2()
            {
                Assert.That(x++, Is.EqualTo(0));
            }
        }
    }
}
