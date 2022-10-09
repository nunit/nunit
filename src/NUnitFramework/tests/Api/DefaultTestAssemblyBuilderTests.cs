// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Tests;
using NUnit.Tests.Singletons;

namespace NUnit.Framework.Api
{
    public class DefaultTestAssemblyBuilderTests
    {
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.dll";

        private string _mockAssemblyPath;
        private DefaultTestAssemblyBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _mockAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY_FILE);
            _builder = new DefaultTestAssemblyBuilder();
        }

        // No filter
        [TestCase(ExpectedResult = MockAssembly.Tests)]
        // Namespace filters
        [TestCase("NUnit", ExpectedResult = MockAssembly.Tests)]
        [TestCase("NUnit.Tests", ExpectedResult = MockAssembly.Tests)]
        [TestCase("NUnit.Tests.Assemblies", ExpectedResult = MockTestFixture.Tests)]
        [TestCase("NUnit.Tests.Singletons", ExpectedResult = OneTestCase.Tests)]
        // Fixture filters
        [TestCase("NUnit.Tests.FixtureWithTestCases", ExpectedResult = FixtureWithTestCases.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", ExpectedResult = MockTestFixture.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", "NUnit.Tests.FixtureWithTestCases", ExpectedResult = MockTestFixture.Tests + FixtureWithTestCases.Tests)]
        [TestCase("NUnit.Tests.Singletons.OneTestCase", ExpectedResult = OneTestCase.Tests)]
        // Method filters
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture.TestWithException", ExpectedResult = 1)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture.TestWithException", "NUnit.Tests.Singletons.OneTestCase.TestCase", ExpectedResult = 2)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture.TestWithException", "NUnit.Tests.Assemblies.MockTestFixture.WarningTest", "NUnit.Tests.Assemblies.MockTestFixture.FailingTest", ExpectedResult = 3)]
        // Mixed filters
        [TestCase("NUnit.Tests.Assemblies", "NUnit.Tests.Singletons", "NUnit.Tests.FixtureWithTestCases", ExpectedResult = MockTestFixture.Tests + FixtureWithTestCases.Tests + OneTestCase.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", "NUnit.Tests.Assemblies", ExpectedResult = MockTestFixture.Tests)]
        // Nested Filters (highest level one should be used)
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", "NUnit.Tests.Assemblies.MockTestFixture.TestWithException", ExpectedResult = MockTestFixture.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture.TestWithException", "NUnit.Tests.Assemblies.MockTestFixture", ExpectedResult = MockTestFixture.Tests)]
        public int LoadWithPreFilter(params string[] filters)
        {
            var settings = new Dictionary<string, object>();
            settings.Add("LOAD", filters);
            var result = _builder.Build(_mockAssemblyPath, settings);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(MOCK_ASSEMBLY_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.Runnable), (string)result.Properties.Get(PropertyNames.SkipReason));

            return result.TestCaseCount;
        }
    }
}
