// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if !SILVERLIGHT && !NETCF
using System.Collections;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class SetUpFixtureTests
    {
        private static readonly string testAssembly = AssemblyHelper.GetAssemblyPath(typeof(NUnit.TestData.SetupFixture.Namespace1.SomeTestFixture));

        ITestAssemblyBuilder builder;
        ITestAssemblyRunner runner;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            TestUtilities.SimpleEventRecorder.Clear();
            
            builder = new DefaultTestAssemblyBuilder();
#if NUNITLITE
            runner = new NUnitLiteTestAssemblyRunner(builder);
#else
            runner = new NUnitTestAssemblyRunner(builder);
#endif
        }
        #endregion SetUp

        private ITestResult runTests(string nameSpace)
        {
            return runTests(nameSpace, TestFilter.Empty);
        }

        private ITestResult runTests(string nameSpace, TestFilter filter)
        {
            IDictionary options = new Hashtable();
            if (nameSpace != null)
                options["LOAD"] = new string[] { nameSpace };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            if (runner.Load(testAssembly, options) != null)
                return runner.Run(TestListener.NULL, filter);

            return null;
        }

        #region Builder
        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interperets a SetupFixture class as a 'virtual namespace' into which 
        /// all it's sibling classes are inserted.
        /// </summary>
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureReplacesNamespaceNodeInTree()
        {
            string nameSpace = "NUnit.TestData.SetupFixture.Namespace1";
            IDictionary options = new Hashtable();
            options["LOAD"] = new string[] { nameSpace };
            ITest suite = builder.Build( testAssembly, options );

            Assert.IsNotNull(suite);

            Assert.AreEqual(testAssembly, suite.FullName);
            Assert.AreEqual(1, suite.Tests.Count, "Error in top level test count");

            string[] nameSpaceBits = nameSpace.Split('.');
            for (int i = 0; i < nameSpaceBits.Length; i++)
            {
                suite = suite.Tests[0] as TestSuite;
                Assert.AreEqual(nameSpaceBits[i], suite.Name);
                Assert.AreEqual(1, suite.Tests.Count);
            }

            Assert.That(suite, Is.InstanceOf<SetUpFixture>());

            suite = suite.Tests[0] as TestSuite;
            Assert.AreEqual("SomeTestFixture", suite.Name);
            Assert.AreEqual(1, suite.Tests.Count);
        }
        #endregion Builder

        #region NoNamespaceBuilder
        /// <summary>
        /// Tests that the TestSuiteBuilder correctly interperets a SetupFixture class with no parent namespace 
        /// as a 'virtual assembly' into which all it's sibling fixtures are inserted.
        /// </summary>
        [NUnit.Framework.Test]
        public void AssemblySetUpFixtureReplacesAssemblyNodeInTree()
        {
            IDictionary options = new Hashtable();
            ITest suite = builder.Build(testAssembly, options);

            Assert.IsNotNull(suite);
            Assert.That(suite, Is.InstanceOf<SetUpFixture>());

            suite = suite.Tests[1] as TestSuite;
            Assert.AreEqual("SomeTestFixture", suite.Name);
            Assert.AreEqual(1, suite.Tests.Count);
        }
        #endregion NoNamespaceBuilder

        #region Simple
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfSingleTest()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace1").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NamespaceSetup",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                  "NamespaceTearDown");
        }
        #endregion Simple

        #region Static
        [Test]
        public void NamespaceSetUpMethodsMayBeStatic()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace5").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NamespaceSetup",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                  "NamespaceTearDown");
        }
        #endregion

        #region TwoTestFixtures
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsExecutionOfTwoTests()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace2").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NamespaceSetup",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                  "NamespaceTearDown");
        }
        #endregion TwoTestFixtures

        #region SubNamespace
        [NUnit.Framework.Test]
        public void NamespaceSetUpFixtureWrapsNestedNamespaceSetUpFixture()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace3").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NamespaceSetup",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                    "SubNamespaceSetup",
                                        "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                    "SubNamespaceTearDown",
                                  "NamespaceTearDown");
        }
        #endregion SubNamespace

        #region TwoSetUpFixtures
        [NUnit.Framework.Test]
        public void WithTwoSetUpFixturesOnlyOneIsUsed()
        {
            Assert.That(runTests("NUnit.TestData.SetupFixture.Namespace4").ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("NamespaceSetup2",
                                    "FixtureSetup", "Setup", "Test", "TearDown", "FixtureTearDown",
                                  "NamespaceTearDown2");
        }
        #endregion TwoSetUpFixtures

        #region NoNamespaceSetupFixture
        [NUnit.Framework.Test]
        public void AssemblySetupFixtureWrapsExecutionOfTest()
        {
            ITestResult result = runTests(null, new Filters.SimpleNameFilter("SomeTestFixture"));
            Assert.AreEqual(1, result.PassCount);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            TestUtilities.SimpleEventRecorder.Verify("RootNamespaceSetup",
                                    "Test",
                                  "RootNamespaceTearDown");
        }
        #endregion NoNamespaceSetupFixture
    }
}
#endif
