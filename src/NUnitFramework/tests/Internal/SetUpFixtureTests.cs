// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class SetUpFixtureTests
    {
        private static readonly string ASSEMBLY_PATH = AssemblyHelper.GetAssemblyPath(typeof(TestData.SetupFixture.Namespace1.SomeFixture).Assembly);
        private ITestAssemblyBuilder _builder;
        private ITestAssemblyRunner _runner;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            NUnit.TestUtilities.SimpleEventRecorder.Clear();

            _builder = new DefaultTestAssemblyBuilder();
            _runner = new NUnitTestAssemblyRunner(_builder);
        }
        #endregion SetUp

        private ITestResult? RunTests(string nameSpace)
        {
            return RunTests(nameSpace, TestFilter.Empty);
        }

        private ITestResult? RunTests(string? nameSpace, TestFilter filter)
        {
            IDictionary<string, object> options = new Dictionary<string, object>();
            if (nameSpace is not null)
                options["LOAD"] = new[] { nameSpace };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            if (_runner.Load(ASSEMBLY_PATH, options) is not null)
                return _runner.Run(TestListener.NULL, filter);

            return null;
        }

        #region Builder Tests

        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interprets a SetupFixture class as a 'virtual namespace' into which
        /// all its sibling classes are inserted.
        /// </summary>
        [Test]
        public void NamespaceSetUpFixtureReplacesNamespaceNodeInTree()
        {
            string nameSpace = "NUnit.TestData.SetupFixture.Namespace1";
            IDictionary<string, object> options = new Dictionary<string, object>
            {
                ["LOAD"] = new[] { nameSpace }
            };
            ITest? suite = _builder.Build(ASSEMBLY_PATH, options);

            Assert.That(suite, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(suite.FullName, Is.EqualTo(ASSEMBLY_PATH));
                Assert.That(suite.Tests, Has.Count.EqualTo(1), "Error in top level test count");
            });

            string[] nameSpaceBits = ("[default namespace]." + nameSpace).Split('.');
            for (int i = 0; i < nameSpaceBits.Length; i++)
            {
                suite = suite.Tests[0] as TestSuite;
                Assert.That(suite, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(suite.Name, Is.EqualTo(nameSpaceBits[i]));
                    Assert.That(suite.Tests, Has.Count.EqualTo(1));
                    Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
                });
            }

            Assert.That(suite, Is.InstanceOf<SetUpFixture>());

            suite = suite.Tests[0] as TestSuite;
            Assert.That(suite, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(suite.Name, Is.EqualTo("SomeFixture"));
                Assert.That(suite.Tests, Has.Count.EqualTo(1));
                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
            });
        }

        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interprets a SetupFixture class with no parent namespace
        /// as a 'virtual assembly' into which all its sibling fixtures are inserted.
        /// </summary>
        [Test]
        public void AssemblySetUpFixtureFollowsAssemblyNodeInTree()
        {
            IDictionary<string, object> options = new Dictionary<string, object>();
            var rootSuite = _builder.Build(ASSEMBLY_PATH, options);
            Assert.That(rootSuite, Is.TypeOf<TestAssembly>());
            var setupFixture = rootSuite.Tests[0];
            Assert.That(setupFixture, Is.TypeOf<SetUpFixture>());

            var testFixture = TestFinder.Find("SomeFixture", (SetUpFixture)setupFixture, false);
            Assert.That(testFixture, Is.Not.Null);
            Assert.That(testFixture.Tests, Has.Count.EqualTo(1));
        }

        [Test]
        public void InvalidAssemblySetUpFixtureIsLoadedCorrectly()
        {
            string nameSpace = "NUnit.TestData.SetupFixture.Namespace6";
            IDictionary<string, object> options = new Dictionary<string, object>
            {
                ["LOAD"] = new[] { nameSpace }
            };
            ITest? suite = _builder.Build(ASSEMBLY_PATH, options);

            Assert.That(suite, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(suite.FullName, Is.EqualTo(ASSEMBLY_PATH));
                Assert.That(suite.Tests, Has.Count.EqualTo(1), "Error in top level test count");
                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            });

            string[] nameSpaceBits = ("[default namespace]." + nameSpace).Split('.');
            for (int i = 0; i < nameSpaceBits.Length; i++)
            {
                suite = suite.Tests[0] as TestSuite;
                Assert.That(suite, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(suite.Name, Is.EqualTo(nameSpaceBits[i]));
                    Assert.That(suite.Tests, Has.Count.EqualTo(1));
                    Assert.That(suite.RunState, Is.EqualTo(i < nameSpaceBits.Length - 1 ? RunState.Runnable : RunState.NotRunnable));
                });
            }

            suite = suite.Tests[0] as TestSuite;
            Assert.That(suite, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(suite.Name, Is.EqualTo("SomeFixture"));
                Assert.That(suite.Tests, Has.Count.EqualTo(1));
                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
            });
        }

        #endregion

        #region Simple
        [Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfSingleTest()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace1");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NS1.OneTimeSetup",
                                                     "NS1.Fixture.SetUp",
                                                     "NS1.Test.SetUp",
                                                     "NS1.Test",
                                                     "NS1.Test.TearDown",
                                                     "NS1.Fixture.TearDown",
                                                     "NS1.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }

        [Test]
        public void MethodNameSetUpFixtureWrapsExecutionOfSingleTest()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace3.SubNamespace.SomeFixture.Test");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NS3.OneTimeSetUp",
                                                     "NS3.SubNamespace.OneTimeSetUp",
                                                     "NS3.SubNamespace.Fixture.SetUp",
                                                     "NS3.SubNamespace.Test.SetUp",
                                                     "NS3.SubNamespace.Test",
                                                     "NS3.SubNamespace.Test.TearDown",
                                                     "NS3.SubNamespace.Fixture.TearDown",
                                                     "NS3.SubNamespace.OneTimeTearDown",
                                                     "NS3.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }
        #endregion Simple

        #region Static
        [Test]
        public void NamespaceSetUpMethodsMayBeStatic()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace5");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NS5.OneTimeSetUp",
                                                     "NS5.Fixture.SetUp",
                                                     "NS5.Test.SetUp",
                                                     "NS5.Test",
                                                     "NS5.Test.TearDown",
                                                     "NS5.Fixture.TearDown",
                                                     "NS5.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }

        [Test]
        public void NamespaceSetUpFixtureMayBeStatic()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.StaticFixture");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "StaticFixture.OneTimeSetUp",
                                                     "StaticFixture.Fixture.SetUp",
                                                     "StaticFixture.Test.SetUp",
                                                     "StaticFixture.Test",
                                                     "StaticFixture.Test.TearDown",
                                                     "StaticFixture.Fixture.TearDown",
                                                     "StaticFixture.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }
        #endregion

        #region TwoTestFixtures
        [Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfTwoTests()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace2");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));

            // There are two fixtures but we can't be sure of the order of execution so they use the same events
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NS2.OneTimeSetUp",
                                                     "NS2.Fixture.SetUp",
                                                     "NS2.Test.SetUp",
                                                     "NS2.Test",
                                                     "NS2.Test.TearDown",
                                                     "NS2.Fixture.TearDown",
                                                     "NS2.Fixture.SetUp",
                                                     "NS2.Test.SetUp",
                                                     "NS2.Test",
                                                     "NS2.Test.TearDown",
                                                     "NS2.Fixture.TearDown",
                                                     "NS2.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }
        #endregion TwoTestFixtures

        #region SubNamespace
        [Test]
        public void NamespaceSetUpFixtureWrapsNestedNamespaceSetUpFixture()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace3");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NS3.OneTimeSetUp",
                                                     "NS3.Fixture.SetUp",
                                                     "NS3.Test.SetUp",
                                                     "NS3.Test",
                                                     "NS3.Test.TearDown",
                                                     "NS3.Fixture.TearDown",
                                                     "NS3.SubNamespace.OneTimeSetUp",
                                                     "NS3.SubNamespace.Fixture.SetUp",
                                                     "NS3.SubNamespace.Test.SetUp",
                                                     "NS3.SubNamespace.Test",
                                                     "NS3.SubNamespace.Test.TearDown",
                                                     "NS3.SubNamespace.Fixture.TearDown",
                                                     "NS3.SubNamespace.OneTimeTearDown",
                                                     "NS3.OneTimeTearDown",
                                                     "Assembly.OneTimeTearDown");
        }
        #endregion SubNamespace

        #region TwoSetUpFixtures
        [Test]
        public void WithTwoSetUpFixturesBothAreUsed()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace4");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            NUnit.TestUtilities.SimpleEventRecorder.ExpectEvents("Assembly.OneTimeSetUp")
                 .AndThen("NS4.OneTimeSetUp1", "NS4.OneTimeSetUp2")
                 .AndThen("NS4.Fixture.SetUp")
                 .AndThen("NS4.Test.SetUp")
                 .AndThen("NS4.Test")
                 .AndThen("NS4.Test.TearDown")
                 .AndThen("NS4.Fixture.TearDown")
                 .AndThen("NS4.OneTimeTearDown1", "NS4.OneTimeTearDown2")
                 .AndThen("Assembly.OneTimeTearDown")
                 .Verify();
        }
        #endregion TwoSetUpFixtures

        #region InvalidSetUpFixture

        [Test]
        public void InvalidSetUpFixtureTest()
        {
            ITestResult? testResult = RunTests("NUnit.TestData.SetupFixture.Namespace6");
            Assert.That(testResult, Is.Not.Null);
            Assert.That(testResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            NUnit.TestUtilities.SimpleEventRecorder.Verify(Array.Empty<string>());
        }

        #endregion

        #region NoNamespaceSetupFixture
        [Test]
        public void AssemblySetupFixtureWrapsExecutionOfTest()
        {
            ITestResult? result = RunTests(null, new Framework.Internal.Filters.FullNameFilter("SomeFixture"));
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.PassCount, Is.EqualTo(1));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            });
            SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                       "NoNamespaceTest",
                                       "Assembly.OneTimeTearDown");
        }
        #endregion NoNamespaceSetupFixture
    }
}
