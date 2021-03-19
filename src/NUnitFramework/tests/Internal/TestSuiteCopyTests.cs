// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Api;
using NUnit.TestData.OneTimeSetUpTearDownData;
using NUnit.TestUtilities;
using System.IO;

namespace NUnit.Framework.Internal
{
    // Tests that Copy is properly overridden for all types extending TestSuite - #3171
    public class TestSuiteCopyTests
    {
        [Test]
        public void CopyTestSuiteReturnsCorrectType()
        {
            TestSuite testSuite =
                new TestSuite(new TypeWrapper(typeof(NUnit.TestData.ParameterizedTestFixture)));
            var copiedTestSuite = testSuite.Copy(TestFilter.Empty);
            Assert.That(copiedTestSuite, Is.TypeOf<TestSuite>());
        }
        
        [Test]
        public void CopyParameterizedTestFixtureReturnsCorrectType()
        {
            TestSuite parameterizedTestFixture =
                new ParameterizedFixtureSuite(new TypeWrapper(typeof(NUnit.TestData.ParameterizedTestFixture)));
            var copiedParameterizedTestFixture = parameterizedTestFixture.Copy(TestFilter.Empty);
            Assert.That(copiedParameterizedTestFixture, Is.TypeOf<ParameterizedFixtureSuite>());
        }

        [Test]
        public void CopyParameterizedMethodSuiteReturnsCorrectType()
        {
            TestSuite parameterizedMethodSuite = new ParameterizedMethodSuite(new MethodWrapper(
                typeof(NUnit.TestData.ParameterizedTestFixture),
                nameof(NUnit.TestData.ParameterizedTestFixture.MethodWithParams)));
            var copiedparameterizedMethodSuite = parameterizedMethodSuite.Copy(TestFilter.Empty);
            Assert.That(copiedparameterizedMethodSuite, Is.TypeOf<ParameterizedMethodSuite>());

        }

        [Test]
        public void CopyTestFixtureReturnsCorrectType()
        {
            TestSuite testFixture = TestBuilder.MakeFixture(typeof(FixtureWithNoTests));
            var copiedTestFixture = testFixture.Copy(TestFilter.Empty);
            Assert.That(copiedTestFixture, Is.TypeOf<TestFixture>());
        }

        [Test]
        public void CopySetUpFixtureReturnsCorrectType()
        {
            TestSuite setUpFixture =
                new SetUpFixture(
                    new TypeWrapper(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1)));
            var copiedSetupFixture = setUpFixture.Copy(TestFilter.Empty);
            Assert.That(copiedSetupFixture, Is.TypeOf<SetUpFixture>());
        }

        [Test]
        public void CopyTestAssemblyReturnsCorrectType()
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            TestSuite assembly =
                new TestAssembly(
                    AssemblyHelper.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "mock-assembly.dll")),
                    "mock-assembly");
            var copiedAssembly = assembly.Copy(TestFilter.Empty);
            Assert.That(copiedAssembly, Is.TypeOf<TestAssembly>());
        }
    }
}
