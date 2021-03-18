// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.UI;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.Tests.Assemblies;

namespace NUnit.Framework.Api
{
    // Functional tests of the FrameworkController and all subordinate classes
    public class FrameworkControllerTests
    {
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.dll";
        private const string BAD_FILE = "mock-assembly.pdb";
        private const string MISSING_FILE = "junk.dll";
        private const string MISSING_NAME = "junk";
        private const string EMPTY_FILTER = "<filter/>";
        private const string FIXTURE_CAT_FILTER = "<filter><cat>FixtureCategory</cat></filter>";

        private static readonly string MOCK_ASSEMBLY_NAME = typeof(MockAssembly).GetTypeInfo().Assembly.FullName;
        private static readonly string EXPECTED_NAME = MOCK_ASSEMBLY_FILE;
        private static readonly string MOCK_ASSEMBLY_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY_FILE);

        private readonly IDictionary _settings = new Dictionary<string, object>();
        private FrameworkController _controller;
        private ICallbackEventHandler _handler;

        public static IEnumerable EmptyFilters
        {
            get
            {
                yield return new TestCaseData((object[])null);
                yield return new TestCaseData("");
                yield return new TestCaseData(EMPTY_FILTER);
            }
        }

        public class FixtureCategoryTester
        {
            [Category("FixtureCategory")]
            [Test]
            public void TestInFixtureCategory()
            {
            }

            [Test]
            public void TestOutOfFixtureCategory()
            {
            }
        }

        [SetUp]
        public void CreateController()
        {
            _controller = new FrameworkController(MOCK_ASSEMBLY_PATH, "ID", _settings);
            _handler = new CallbackEventHandler();
        }

        #region SettingsElement Tests

        [TestCaseSource(nameof(SettingsData))]
        public void InsertSettingsElement_MixedSettings_CreatesCorrectSubNodes(string value)
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = new Dictionary<string, object> { ["innerkey"] = value }
            };

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);
            // in parallel, an additional node is added with number of test workers
            Assert.That(inserted.ChildNodes.Count, Is.EqualTo(3));
            Assert.That(inserted.ChildNodes[0].Attributes["name"], Is.EqualTo("key1"));
            Assert.That(inserted.ChildNodes[0].Attributes["value"], Is.EqualTo("value1"));

            var innerNode = inserted.ChildNodes[1].FirstChild;
            Assert.That(innerNode.Attributes["key"], Is.EqualTo("innerkey"));
            Assert.That(innerNode.Attributes["value"], Is.EqualTo(value));
        }

        [Test]
        public void InsertSettingsElement_SettingIsValue_CreatesASettingElementPerKey()
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);

            // in parallel, an additional node is added with number of test workers
            Assert.That(inserted.ChildNodes.Count, Is.EqualTo(3));
        }

        [TestCaseSource(nameof(SettingsData))]
        public void InsertSettingsElement_SettingIsValue_SetsKeyAndValueAsAttributes(string value)
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = value
            };

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);

            Assert.That(inserted.ChildNodes[0].Attributes["name"], Is.EqualTo("key1"));
            Assert.That(inserted.ChildNodes[0].Attributes["value"], Is.EqualTo("value1"));

            Assert.That(inserted.ChildNodes[1].Attributes["name"], Is.EqualTo("key2"));
            Assert.That(inserted.ChildNodes[1].Attributes["value"], Is.EqualTo(value));
        }

        [TestCaseSource(nameof(SettingsData))]
        public void InsertSettingsElement_SettingIsDictionary_CreatesEntriesForDictionaryElements(string value)
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>
            {
                ["outerkey"] = new Dictionary<string, object> { { "key1", "value1" }, { "key2", value } }
            };

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);
            var settingNode = inserted.FirstChild;

            Assert.That(settingNode.ChildNodes.Count, Is.EqualTo(2));
        }

        [TestCaseSource(nameof(SettingsData))]
        public void InsertSettingsElement_SettingIsDictionary_CreatesValueAttributeForDictionaryElements(string value)
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>
            {
                ["outerkey"] = new Dictionary<string, object> { { "key1", "value1" }, { "key2", value } }
            };

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);
            var settingNode = inserted.FirstChild;

            Assert.That(settingNode.ChildNodes.Count, Is.EqualTo(2));
            Assert.That(settingNode.Attributes["value"], Is.EqualTo($"[key1, value1], [key2, {value}]"));
        }

        [TestCaseSource(nameof(SettingsData))]
        public void InsertSettingsElement_SettingIsDictionary_CreatesEntriesWithKeysAndValuesFromDictionary(string value)
        {
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>();
            testSettings.Add("outerkey", new Dictionary<string, object> { { "key1", "value1" }, { "key2", value } });

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);
            var settingNode = inserted.FirstChild;

            var key1Node = settingNode.ChildNodes[0];
            Assert.That(key1Node.Attributes["key"], Is.EqualTo("key1"));
            Assert.That(key1Node.Attributes["value"], Is.EqualTo("value1"));

            var key2Node = settingNode.ChildNodes[1];
            Assert.That(key2Node.Attributes["key"], Is.EqualTo("key2"));
            Assert.That(key2Node.Attributes["value"], Is.EqualTo(value));
        }

        [Test]
        public void InsertSettingsElement_SettingIsDictionaryHasNull_CreatesEntriesWithKeysAndValuesFromDictionary()
        {
            string value = "test";
            var outerNode = new TNode("test");
            var testSettings = new Dictionary<string, object>();
            testSettings.Add("outerkey", new Dictionary<string, object> { { "key1", null }, { "key2", value } });

            var inserted = FrameworkController.InsertSettingsElement(outerNode, testSettings);
            var settingNode = inserted.FirstChild;

            var key1Node = settingNode.ChildNodes[0];
            Assert.That(key1Node.Attributes["key"], Is.EqualTo("key1"));
            Assert.That(key1Node.Attributes["value"], Is.EqualTo(string.Empty));

            var key2Node = settingNode.ChildNodes[1];
            Assert.That(key2Node.Attributes["key"], Is.EqualTo("key2"));
            Assert.That(key2Node.Attributes["value"], Is.EqualTo(value));
        }

        public static IEnumerable SettingsData()
        {
            yield return new TestCaseData("value");
            yield return new TestCaseData("");
            yield return new TestCaseData("<value>");
            yield return new TestCaseData("\"value\"");
            yield return new TestCaseData("'value'");
            yield return new TestCaseData("value1;value2");
        }

        #endregion

        #region Construction Test
        [Test]
        public void ConstructController()
        {
            Assert.That(_controller.Builder, Is.TypeOf<DefaultTestAssemblyBuilder>());
            Assert.That(_controller.Runner, Is.TypeOf<NUnitTestAssemblyRunner>());
            Assert.That(_controller.AssemblyNameOrPath, Is.EqualTo(MOCK_ASSEMBLY_PATH));
            Assert.That(_controller.Settings, Is.SameAs(_settings));
        }
        #endregion

        #region LoadTestsAction
        [Test]
        public void LoadTestsAction_GoodFile_ReturnsRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void LoadTestsAction_Assembly_ReturnsRunnableSuite()
        {
            _controller = new FrameworkController(typeof(MockAssembly).GetTypeInfo().Assembly, "ID", _settings);
            new FrameworkController.LoadTestsAction(_controller, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME).IgnoreCase);
            Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void LoadTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(new FrameworkController(MISSING_FILE, "ID", _settings), _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_NAME));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void LoadTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(new FrameworkController(BAD_FILE, "ID", _settings), _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        #endregion

        #region ExploreTestsAction
        [Test]
        public void ExploreTestsAction_FilterCategory_ReturnsTests()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.ExploreTestsAction(_controller, FIXTURE_CAT_FILTER, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockTestFixture.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Explore result should have child tests");
        }

        [TestCaseSource(nameof(EmptyFilters))]
        public void ExploreTestsAction_AfterLoad_ReturnsRunnableSuite(string filter)
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.ExploreTestsAction(_controller, filter, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Explore result should have child tests");
        }

        [TestCase(FIXTURE_CAT_FILTER)]
        [TestCase(EMPTY_FILTER)]
        public void ExploreTestsAction_AfterLoad_ReturnsSameCount(string filter)
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.ExploreTestsAction(_controller, filter, _handler);
            var exploreResult = TNode.FromXml(_handler.GetCallbackResult());

            var exploreTestCount = exploreResult.Attributes["testcasecount"];

            new FrameworkController.CountTestsAction(_controller, filter, _handler);
            var countResult = _handler.GetCallbackResult();

            Assert.That(exploreTestCount, Is.EqualTo(countResult));
        }

        [Test]
        public void ExploreTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.ExploreTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("Tests must be loaded before exploring them."));
        }

        [Test]
        public void ExploreTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.ExploreTestsAction(controller, EMPTY_FILTER, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_NAME));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }

        [Test]
        public void ExploreTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.ExploreTestsAction(controller, EMPTY_FILTER, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }

#endregion

#region CountTestsAction
        [TestCaseSource(nameof(EmptyFilters))]
        public void CountTestsAction_AfterLoad_ReturnsCorrectCount(string filter)
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.CountTestsAction(_controller, filter, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo(MockAssembly.Tests.ToString()));
        }

        [Test]
        public void CountTestsAction_WithoutLoad_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.CountTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("Tests must be loaded before counting test cases."));
        }

        [Test]
        public void CountTestsAction_FileNotFound_ReturnsZero()
        {
            var controller = new FrameworkController(MISSING_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.CountTestsAction(controller, EMPTY_FILTER, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo("0"));
        }

        [Test]
        public void CountTestsAction_BadFile_ReturnsZero()
        {
            var controller = new FrameworkController(BAD_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.CountTestsAction(controller, EMPTY_FILTER, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo("0"));
        }

#endregion

#region RunTestsAction
        [TestCaseSource(nameof(EmptyFilters))]
        public void RunTestsAction_AfterLoad_ReturnsRunnableSuite(string filter)
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.RunTestsAction(_controller, filter, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            // TODO: Any failure here throws an exception because the call to RunTestsAction
            // has destroyed the test context. We need to figure out how to execute the run
            // in a cleaner way, perhaps on another thread or in a process.
            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.Attributes["result"], Is.EqualTo("Failed"));
            Assert.That(result.Attributes["passed"], Is.EqualTo(MockAssembly.Passed.ToString()));
            Assert.That(result.Attributes["failed"], Is.EqualTo(MockAssembly.Failed.ToString()));
            Assert.That(result.Attributes["warnings"], Is.EqualTo(MockAssembly.Warnings.ToString()));
            Assert.That(result.Attributes["skipped"], Is.EqualTo(MockAssembly.Skipped.ToString()));
            Assert.That(result.Attributes["inconclusive"], Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Run result should have child tests");
        }

        [Test]
        public void RunTestsAction_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.RunTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("Tests must be loaded before running them."));
        }

        [Test]
        public void RunTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunTestsAction(controller, EMPTY_FILTER, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_NAME));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void RunTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunTestsAction(controller, EMPTY_FILTER, _handler);
            var result = TNode.FromXml(_handler.GetCallbackResult());

            Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

#endregion

#region RunAsyncAction
        [Test]
        public void RunAsyncAction_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.RunAsyncAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("Tests must be loaded before running them."));
        }

#endregion

#region Helper Methods

        private static string GetSkipReason(TNode result)
        {
            var propNode = result.SelectSingleNode(string.Format("properties/property[@name='{0}']", PropertyNames.SkipReason));
            return propNode == null ? null : propNode.Attributes["value"];
        }

#endregion
    }
}
