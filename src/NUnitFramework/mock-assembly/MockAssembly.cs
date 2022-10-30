// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Tests.Assemblies
{
    /// <summary>
    /// MockAssembly is intended for those few tests that can only
    /// be made to work by loading an entire assembly. Please don't
    /// add any other entries or use it for other purposes.
    ///
    /// Most tests used as data for NUnit's own tests should be
    /// in the testdata assembly.
    /// </summary>
    public class MockAssembly
    {
        /// <summary>
        /// Constant definitions used by tests that both reference the
        /// mock-assembly and load it in order to verify counts.
        /// </summary>
        public const string FileName = "mock.nunit.assembly.exe";

        public const int Classes = 9;
        public const int NamespaceSuites = 6; // assembly, NUnit, Tests, Assemblies, Singletons, TestAssembly

        public const int Tests = MockTestFixture.Tests
                    + Singletons.OneTestCase.Tests
                    + TestAssembly.MockTestFixture.Tests
                    + IgnoredFixture.Tests
                    + ExplicitFixture.Tests
                    + BadFixture.Tests
                    + FixtureWithTestCases.Tests
                    + ParameterizedFixture.Tests
                    + GenericFixtureConstants.Tests;

        public const int Suites = MockTestFixture.Suites
                    + Singletons.OneTestCase.Suites
                    + TestAssembly.MockTestFixture.Suites
                    + IgnoredFixture.Suites
                    + ExplicitFixture.Suites
                    + BadFixture.Suites
                    + FixtureWithTestCases.Suites
                    + ParameterizedFixture.Suites
                    + GenericFixtureConstants.Suites
                    + NamespaceSuites;

        public const int TestStartedEvents = Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests;
        public const int TestFinishedEvents = Tests;
        public const int TestOutputEvents = 1;

        public const int Nodes = Tests + Suites;

        public const int ExplicitFixtures = 1;
        public const int SuitesRun = Suites - ExplicitFixtures;

        public const int Passed = MockTestFixture.Passed
                    + Singletons.OneTestCase.Tests
                    + TestAssembly.MockTestFixture.Tests
                    + FixtureWithTestCases.Tests
                    + ParameterizedFixture.Tests
                    + GenericFixtureConstants.Tests;

        public const int Skipped_Ignored = MockTestFixture.Skipped_Ignored + IgnoredFixture.Tests;
        public const int Skipped_Explicit = MockTestFixture.Skipped_Explicit + ExplicitFixture.Tests;
        public const int Skipped = Skipped_Ignored + Skipped_Explicit;

        public const int Warnings = MockTestFixture.Warnings;

        public const int Failed_Error = MockTestFixture.Failed_Error;
        public const int Failed_Other = MockTestFixture.Failed_Other;
        public const int Failed_NotRunnable = MockTestFixture.Failed_NotRunnable + BadFixture.Tests;
        public const int Failed = Failed_Error + Failed_Other + Failed_NotRunnable;

        public const int Inconclusive = MockTestFixture.Inconclusive;

        public static readonly Assembly ThisAssembly = typeof(MockAssembly).Assembly;
        public static readonly string AssemblyPath = AssemblyHelper.GetAssemblyPath(ThisAssembly);

        public static void Main(string[] args)
        {
            new AutoRun(ThisAssembly).Execute(args);
        }
    }

    [TestFixture(Description="Fake Test Fixture")]
    [Category("FixtureCategory")]
    public class MockTestFixture
    {
        public const int Tests = 13;
        public const int Suites = 2;

        public const int Passed = 2;

        public const int Skipped_Ignored = 4;
        public const int Skipped_Explicit = 1;
        public const int Skipped = Skipped_Ignored + Skipped_Explicit;

        public const int Failed_Other = 1;
        public const int Failed_Error = 1;
        public const int Failed_NotRunnable = 2;
        public const int Failed = Failed_Error + Failed_Other + Failed_NotRunnable;

        public const int Warnings = 1;

        public const int Inconclusive = 1;

        [Test(Description="Mock Test #1")]
        [Category("MockCategory")]
        [Property("Severity", "Critical")]
        public void TestWithDescription() { }

        [Test]
        protected static void NonPublicTest() { }

        [Test]
        public void FailingTest()
        {
            Console.Error.WriteLine("Immediate Error Message");
            Assert.Fail("Intentional failure");
        }

        [Test]
        public void WarningTest()
        {
            Assert.Warn("Warning Message");
        }

        [Test, Ignore("Ignore Message")]
        public void IgnoreTest() { }

        [TestCaseSource(nameof(SkippedTestCaseData))]
        public void SkippedTest(int _)
        {
            Assert.Pass();
        }

        public static IEnumerable<TestCaseData> SkippedTestCaseData => new[]
        {
            new TestCaseData(1).Ignore("Ignore testcase"),
            new TestCaseData(2).Ignore("Ignore testcase"),
            new TestCaseData(3).Ignore("Ignore testcase")
        };

        [Test, Explicit]
        public void ExplicitTest() { }

        [Test]
        public void NotRunnableTest( int a, int b) { }

        [Test]
        public void InconclusiveTest()
        {
            Assert.Inconclusive("No valid data");
        }

        [Test]
        public void DisplayRunParameters()
        {
            foreach (string name in TestContext.Parameters.Names)
                Console.WriteLine("Parameter {0} = {1}", name, TestContext.Parameters[name]);
        }

        [Test]
        public void TestWithException()
        {
            MethodThrowsException();
        }

        private void MethodThrowsException()
        {
            throw new Exception("Intentional Exception");
        }
    }
}
