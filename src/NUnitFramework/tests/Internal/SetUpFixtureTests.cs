// ****************************************************************
// Copyright 2007, Charlie Poole, Rob Prouse
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class SetUpFixtureTests
    {
        private static readonly string testAssembly = AssemblyHelper.GetAssemblyPath(typeof(NUnit.TestData.SetupFixture.Namespace1.SomeFixture).GetTypeInfo().Assembly);

        ITestAssemblyBuilder builder;
        ITestAssemblyRunner runner;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            TestUtilities.SimpleEventRecorder.Clear();

            builder = new DefaultTestAssemblyBuilder();
            runner = new NUnitTestAssemblyRunner(builder);
        }
        #endregion SetUp

        private ITestResult runTests(string nameSpace)
        {
            return runTests(nameSpace, TestFilter.Empty);
        }

        private ITestResult runTests(string nameSpace, TestFilter filter)
        {
            IDictionary<string, object> options = new Dictionary<string, object>();
            if (nameSpace != null)
                options["LOAD"] = new string[] { nameSpace };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            if (runner.Load(testAssembly, options) != null)
                return runner.Run(TestListener.NULL, filter);

            return null;
        }

        #region Builder Tests

        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interprets a SetupFixture class as a 'virtual namespace' into which
        /// all its sibling classes are inserted.
        /// </summary>
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureReplacesNamespaceNodeInTree()
        {
            string nameSpace = "NUnit.TestData.SetupFixture.Namespace1";
            IDictionary<string, object> options = new Dictionary<string, object>();
            options["LOAD"] = new string[] { nameSpace };
            ITest suite = builder.Build(testAssembly, options);

            Assert.IsNotNull(suite);

            Assert.AreEqual(testAssembly, suite.FullName);
            Assert.AreEqual(1, suite.Tests.Count, "Error in top level test count");

            string[] nameSpaceBits = nameSpace.Split('.');
            for (int i = 0; i < nameSpaceBits.Length; i++)
            {
                suite = suite.Tests[0] as TestSuite;
                Assert.AreEqual(nameSpaceBits[i], suite.Name);
                Assert.AreEqual(1, suite.Tests.Count);
                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            }

            Assert.That(suite, Is.InstanceOf<SetUpFixture>());

            suite = suite.Tests[0] as TestSuite;
            Assert.AreEqual("SomeFixture", suite.Name);
            Assert.AreEqual(1, suite.Tests.Count);
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
        }

        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interprets a SetupFixture class with no parent namespace
        /// as a 'virtual assembly' into which all its sibling fixtures are inserted.
        /// </summary>
        [Test]
        public void AssemblySetUpFixtureFollowsAssemblyNodeInTree()
        {
            IDictionary<string, object> options = new Dictionary<string, object>();
            var rootSuite = builder.Build(testAssembly, options);
            Assert.That(rootSuite, Is.TypeOf<TestAssembly>());
            var setupFixture = rootSuite.Tests[0];
            Assert.That(setupFixture, Is.TypeOf<SetUpFixture>());

            var testFixture = TestFinder.Find("SomeFixture", (SetUpFixture)setupFixture, false);
            Assert.NotNull(testFixture);
            Assert.AreEqual(1, testFixture.Tests.Count);
        }

        [Test]
        public void InvalidAssemblySetUpFixtureIsLoadedCorrectly()
        {
            string nameSpace = "NUnit.TestData.SetupFixture.Namespace6";
            IDictionary<string, object> options = new Dictionary<string, object>();
            options["LOAD"] = new string[] { nameSpace };
            ITest suite = builder.Build(testAssembly, options);

            Assert.IsNotNull(suite);

            Assert.AreEqual(testAssembly, suite.FullName);
            Assert.AreEqual(1, suite.Tests.Count, "Error in top level test count");
            Assert.AreEqual(RunState.Runnable, suite.RunState);

            string[] nameSpaceBits = nameSpace.Split('.');
            for (int i = 0; i < nameSpaceBits.Length; i++)
            {
                suite = suite.Tests[0] as TestSuite;
                Assert.AreEqual(nameSpaceBits[i], suite.Name);
                Assert.AreEqual(1, suite.Tests.Count);
                Assert.That(suite.RunState, Is.EqualTo(i < nameSpaceBits.Length - 1 ? RunState.Runnable : RunState.NotRunnable));
            }

            suite = suite.Tests[0] as TestSuite;
            Assert.AreEqual("SomeFixture", suite.Name);
            Assert.AreEqual(1, suite.Tests.Count);
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region Simple
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfSingleTest()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace1").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NS1.OneTimeSetup",
                                                     "NS1.Fixture.SetUp",
                                                     "NS1.Test.SetUp",
                                                     "NS1.Test",
                                                     "NS1.Test.TearDown",
                                                     "NS1.Fixture.TearDown",
                                                     "NS1.OneTimeTearDown");
        }
        #endregion Simple

        #region Static
        [Test]
        public void NamespaceSetUpMethodsMayBeStatic()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace5").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NS5.OneTimeSetUp",
                                                     "NS5.Fixture.SetUp",
                                                     "NS5.Test.SetUp",
                                                     "NS5.Test",
                                                     "NS5.Test.TearDown",
                                                     "NS5.Fixture.TearDown",
                                                     "NS5.OneTimeTearDown");
        }
        #endregion

        #region TwoTestFixtures
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfTwoTests()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace2").ResultState.Status, Is.EqualTo(TestStatus.Passed));

            // There are two fixtures but we can't be sure of the order of execution so they use the same events
            TestUtilities.SimpleEventRecorder.Verify("NS2.OneTimeSetUp",
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
                                                     "NS2.OneTimeTearDown");
        }
        #endregion TwoTestFixtures

        #region SubNamespace
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsNestedNamespaceSetUpFixture()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace3").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NS3.OneTimeSetUp",
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
                                                     "NS3.OneTimeTearDown");
        }
        #endregion SubNamespace

        #region TwoSetUpFixtures
        [NUnit.Framework.Test]
        public void WithTwoSetUpFixturesBothAreUsed()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace4").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.ExpectEvents("NS4.OneTimeSetUp1", "NS4.OneTimeSetUp2")
                                             .AndThen("NS4.Fixture.SetUp")
                                             .AndThen("NS4.Test.SetUp")
                                             .AndThen("NS4.Test")
                                             .AndThen("NS4.Test.TearDown")
                                             .AndThen("NS4.Fixture.TearDown")
                                             .AndThen("NS4.OneTimeTearDown1", "NS4.OneTimeTearDown2")
                                             .Verify();
        }
        #endregion TwoSetUpFixtures

        #region InvalidSetUpFixture

        [Test]
        public void InvalidSetUpFixtureTest()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace6").ResultState.Status, Is.EqualTo(TestStatus.Failed));
            TestUtilities.SimpleEventRecorder.Verify(new string[0]);
        }

        #endregion

        #region NoNamespaceSetupFixture
        [NUnit.Framework.Test]
        public void AssemblySetupFixtureWrapsExecutionOfTest()
        {
            ITestResult result = runTests(null, new Filters.FullNameFilter("SomeFixture"));
            Assert.AreEqual(1, result.PassCount);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("Assembly.OneTimeSetUp",
                                                     "NoNamespaceTest",
                                                     "Assembly.OneTimeTearDown");
        }
        #endregion NoNamespaceSetupFixture
    }
}
