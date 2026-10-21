// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.TestData.OneTimeSetUpTearDownData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
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
        public void CopyTestFixtureKeepsTypeInfo()
        {
            TestSuite testFixture = TestBuilder.MakeFixture(typeof(FixtureWithNoTests));
            var copiedTestFixture = testFixture.Copy(TestFilter.Empty);
            Assert.That(copiedTestFixture.TypeInfo, Is.Not.Null);
        }

        [Test]
        public void CopySetUpFixtureKeepsTypeInfo()
        {
            TestSuite setUpFixture =
                new SetUpFixture(
                    new TypeWrapper(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1)));
            var copiedSetupFixture = setUpFixture.Copy(TestFilter.Empty);
            Assert.That(copiedSetupFixture.TypeInfo, Is.Not.Null);
        }

        [Test]
        public void CopyTestSuiteKeepsTypeInfo()
        {
            TestSuite testSuite =
                new TestSuite(new TypeWrapper(typeof(NUnit.TestData.ParameterizedTestFixture)));
            var copiedTestSuite = testSuite.Copy(TestFilter.Empty);
            Assert.That(copiedTestSuite.TypeInfo, Is.Not.Null);
        }

        [Test]
        public void CopyParameterizedFixtureSuiteKeepsStateOfChildFixtures()
        {
            TestSuite parameterizedTestFixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.ParameterizedTestFixture));
            Assert.That(parameterizedTestFixture, Is.TypeOf<ParameterizedFixtureSuite>());

            var copiedParameterizedTestFixture = parameterizedTestFixture.Copy(TestFilter.Empty);

            Assert.That(copiedParameterizedTestFixture.Tests, Has.Count.EqualTo(parameterizedTestFixture.Tests.Count));
            using (Assert.EnterMultipleScope())
            {
                for (int i = 0; i < parameterizedTestFixture.Tests.Count; i++)
                {
                    var original = (TestFixture)parameterizedTestFixture.Tests[i];
                    var copy = (TestFixture)copiedParameterizedTestFixture.Tests[i];
                    Assert.That(copy.TypeInfo, Is.Not.Null);
                    Assert.That(copy.Arguments, Is.EqualTo(original.Arguments));
                }
            }
        }

        [Test]
        public void CopyTestFixtureKeepsFixtureState()
        {
            var testFixture = (TestFixture)TestBuilder.MakeFixture(typeof(SetUpAndTearDownFixture));
            testFixture.LifeCycle = LifeCycle.InstancePerTestCase;

            var copiedTestFixture = (TestFixture)testFixture.Copy(TestFilter.Empty);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(copiedTestFixture.OneTimeSetUpMethods, Is.EqualTo(testFixture.OneTimeSetUpMethods));
                Assert.That(copiedTestFixture.OneTimeTearDownMethods, Is.EqualTo(testFixture.OneTimeTearDownMethods));
                Assert.That(copiedTestFixture.SetUpMethods, Is.EqualTo(testFixture.SetUpMethods));
                Assert.That(copiedTestFixture.TearDownMethods, Is.EqualTo(testFixture.TearDownMethods));
                Assert.That(copiedTestFixture.LifeCycle, Is.EqualTo(LifeCycle.InstancePerTestCase));
            }
        }

        [Test]
        public void CopiedTestFixtureRunsOneTimeSetUpAndTearDown()
        {
            var fixture = new SetUpAndTearDownFixture();
            TestSuite testFixture = TestBuilder.MakeFixture(fixture);

            TestBuilder.RunTest(testFixture.Copy(TestFilter.Empty), fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.SetUpCount, Is.EqualTo(1), "OneTimeSetUp");
                Assert.That(fixture.TearDownCount, Is.EqualTo(1), "OneTimeTearDown");
            }
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
