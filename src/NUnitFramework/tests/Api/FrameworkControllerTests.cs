// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.UI;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Tests.Assemblies;

namespace NUnit.Framework.Api
{
    // Functional tests of the FrameworkController and all subordinate classes
    public class FrameworkControllerTests
    {
#if NETSTANDARD1_3 || NETSTANDARD1_6
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.dll";
#else
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.exe";
#endif
        private const string BAD_FILE = "mock-assembly.pdb";
        private const string MISSING_FILE = "junk.dll";
        private const string MISSING_NAME = "junk";
        private const string EMPTY_FILTER = "<filter/>";
        private const string FIXTURE_CAT_FILTER = "<filter><cat>FixtureCategory</cat></filter>";

        private static readonly string MOCK_ASSEMBLY_NAME = typeof(MockAssembly).GetTypeInfo().Assembly.FullName;
        private static readonly string EXPECTED_NAME = MOCK_ASSEMBLY_FILE;
        private static readonly string MOCK_ASSEMBLY_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY_FILE);

        private IDictionary _settings = new Dictionary<string, object>();
        private FrameworkController _controller;
        private ICallbackEventHandler _handler;

        public static IEnumerable EmptyFilters
        {
            get
            {
                yield return new TestCaseData(null);
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
#if NETSTANDARD1_3 || NETSTANDARD1_6
            _controller = new FrameworkController(typeof(MockAssembly).GetTypeInfo().Assembly, "ID", _settings);
#else
            _controller = new FrameworkController(MOCK_ASSEMBLY_PATH, "ID", _settings);
#endif
            _handler = new CallbackEventHandler();
        }

        #region Construction Test
        [Test]
        public void ConstructController()
        {
            Assert.That(_controller.Builder, Is.TypeOf<DefaultTestAssemblyBuilder>());
            Assert.That(_controller.Runner, Is.TypeOf<NUnitTestAssemblyRunner>());
#if NETSTANDARD1_3 || NETSTANDARD1_6
            Assert.That(_controller.AssemblyNameOrPath, Is.EqualTo(MOCK_ASSEMBLY_NAME));
#else
            Assert.That(_controller.AssemblyNameOrPath, Is.EqualTo(MOCK_ASSEMBLY_PATH));
#endif
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

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
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
#endif
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
            Assert.That(ex.Message, Is.EqualTo("The Explore method was called but no test has been loaded"));
        }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
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
#endif
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
            Assert.That(ex.Message, Is.EqualTo("The CountTestCases method was called but no test has been loaded"));
        }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
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
#endif
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
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
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
#endif
        #endregion

        #region RunAsyncAction
        [TestCaseSource(nameof(EmptyFilters))]
        public void RunAsyncAction_AfterLoad_ReturnsRunnableSuite(string filter)
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.RunAsyncAction(_controller, filter, _handler);
            //var result = TNode.FromXml(_handler.GetCallbackResult());

            //Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            //Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            //Assert.That(result.Attributes["id"], Is.Not.Null.And.StartWith("ID"));
            //Assert.That(result.Attributes["name"], Is.EqualTo(EXPECTED_NAME));
            //Assert.That(result.Attributes["runstate"], Is.EqualTo("Runnable"));
            //Assert.That(result.Attributes["testcasecount"], Is.EqualTo(MockAssembly.Tests.ToString()));
            //Assert.That(result.Attributes["result"], Is.EqualTo("Failed"));
            //Assert.That(result.Attributes["passed"], Is.EqualTo(MockAssembly.Success.ToString()));
            //Assert.That(result.Attributes["failed"], Is.EqualTo(MockAssembly.ErrorsAndFailures.ToString()));
            //Assert.That(result.Attributes["skipped"], Is.EqualTo((MockAssembly.NotRunnable + MockAssembly.Ignored).ToString()));
            //Assert.That(result.Attributes["inconclusive"], Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            //Assert.That(result.FindDescendants("test-suite").Count, Is.GreaterThan(0), "Run result should have child tests");
        }

        [Test]
        public void RunAsyncAction_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.RunAsyncAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
        [Test]
        public void RunAsyncAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunAsyncAction(controller, EMPTY_FILTER, _handler);
            //var result = TNode.FromXml(_handler.GetCallbackResult());

            //Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            //Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            //Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            //Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            //Assert.That(GetSkipReason(result), Contains.Substring(MISSING_FILE));
            //Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void RunAsyncAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, "ID", _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunAsyncAction(controller, EMPTY_FILTER, _handler);
            //var result = TNode.FromXml(_handler.GetCallbackResult());

            //Assert.That(result.Name.ToString(), Is.EqualTo("test-suite"));
            //Assert.That(result.Attributes["type"], Is.EqualTo("Assembly"));
            //Assert.That(result.Attributes["runstate"], Is.EqualTo("NotRunnable"));
            //Assert.That(result.Attributes["testcasecount"], Is.EqualTo("0"));
            // Minimal check here to allow for platform differences
            //Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE));
            //Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
#endif
        #endregion

        #region Helper Methods

        private static string GetSkipReason(TNode result)
        {
            var propNode = result.SelectSingleNode(string.Format("properties/property[@name='{0}']", PropertyNames.SkipReason));
            return propNode == null ? null : propNode.Attributes["value"];
        }

        #endregion

        #region Nested Callback Class

        private class CallbackEventHandler : System.Web.UI.ICallbackEventHandler
        {
            private string _result;

            public string GetCallbackResult()
            {
                return _result;
            }

            public void RaiseCallbackEvent(string eventArgument)
            {
                _result = eventArgument;
            }
        }

        #endregion
    }
}
