using NUnit.Framework.Api;
using NUnit.TestData.OneTimeSetUpTearDownData;
using NUnit.TestData.TestFixtureSourceData;
using NUnit.TestUtilities;

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
            Assert.AreEqual(copiedTestSuite.GetType(), typeof(TestSuite));
        }
        
        [Test]
        public void CopyParameterizedTestFixtureReturnsCorrectType()
        {
            TestSuite parameterizedTestFixture =
                new ParameterizedFixtureSuite(new TypeWrapper(typeof(NUnit.TestData.ParameterizedTestFixture)));
            var copiedParameterizedTestFixture = parameterizedTestFixture.Copy(TestFilter.Empty);
            Assert.AreEqual(copiedParameterizedTestFixture.GetType(), typeof(ParameterizedFixtureSuite));
        }

        [Test]
        public void CopyParameterizedMethodSuiteReturnsCorrectType()
        {
            TestSuite parameterizedMethodSuite = new ParameterizedMethodSuite(new MethodWrapper(
                typeof(NUnit.TestData.ParameterizedTestFixture),
                nameof(NUnit.TestData.ParameterizedTestFixture.MethodWithParams)));
            var copiedparameterizedMethodSuite = parameterizedMethodSuite.Copy(TestFilter.Empty);
            Assert.AreEqual(copiedparameterizedMethodSuite.GetType(), typeof(ParameterizedMethodSuite));
        }

        [Test]
        public void CopyTestFixtureReturnsCorrectType()
        {
            TestSuite testFixture = TestBuilder.MakeFixture(typeof(FixtureWithNoTests));
            var copiedTestFixture = testFixture.Copy(TestFilter.Empty);
            Assert.AreEqual(copiedTestFixture.GetType(), typeof(TestFixture));
        }

        [Test]
        public void CopySetUpFixtureReturnsCorrectType()
        {
            TestSuite setUpFixture =
                new SetUpFixture(
                    new TypeWrapper(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1)));
            var copiedSetupFixture = setUpFixture.Copy(TestFilter.Empty);
            Assert.AreEqual(copiedSetupFixture.GetType(), typeof(SetUpFixture));
        }

        [Test]
        public void CopyTestAssemblyReturnsCorrectType()
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            TestSuite assembly = new TestAssembly(AssemblyHelper.Load("mock-assembly.dll"), "mock-assembly");
            var copiedAssembly = assembly.Copy(TestFilter.Empty);
            Assert.AreEqual(copiedAssembly.GetType(), typeof(TestAssembly));
        }
    }
}
