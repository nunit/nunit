// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.IO;
using System.Web.UI;
using System.Xml;
using NUnit.Framework.Internal;
using NUnit.Tests.Assemblies;

namespace NUnit.Framework.Api
{
    // Functional tests of the FrameworkController and all subordinate classes
    public class FrameworkControllerTests
    {
#if NUNITLITE
        private const string MOCK_ASSEMBLY = "mock-nunitlite-assembly.dll";
#else
        private const string MOCK_ASSEMBLY = "mock-nunit-assembly.dll";
#endif
        private const string MISSING_FILE = "junk.dll";
        private const string BAD_FILE = "mock-assembly.pdb";
        private const string EMPTY_FILTER = "<filter/>";

        private IDictionary _settings = new Hashtable();
        private FrameworkController _controller;
        private ICallbackEventHandler _handler;
        private string _mockAssemblyPath;

        [SetUp]
        public void CreateController()
        {
            _mockAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY);
            _controller = new FrameworkController(_mockAssemblyPath, _settings);
            _handler = new CallbackEventHandler();
        }

        #region Construction Test
        [Test]
        public void ConstructContoller()
        {
            Assert.That(_controller.Builder, Is.TypeOf<DefaultTestAssemblyBuilder>());
#if NUNITLITE
            Assert.That(_controller.Runner, Is.TypeOf<NUnitLiteTestAssemblyRunner>());
#else
            Assert.That(_controller.Runner, Is.TypeOf<NUnitTestAssemblyRunner>());
#endif
            Assert.That(_controller.AssemblyPath, Is.EqualTo(_mockAssemblyPath));
            Assert.That(_controller.Settings, Is.SameAs(_settings));
        }
        #endregion

        #region LoadTestsAction
        [Test]
        public void LoadTestsAction_GoodFile_ReturnsRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("Runnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void LoadTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(new FrameworkController(MISSING_FILE, _settings), _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(MISSING_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void LoadTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(new FrameworkController(BAD_FILE, _settings), _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region ExploreTestsAction
        [Test]
        public void ExploreTestsAction_AfterLoad_ReturnsRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.ExploreTestsAction(_controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("Runnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Explore result should have child tests");
        }

        [Test]
        public void ExploreTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.ExploreTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("The Explore method was called but no test has been loaded"));
        }

        [Test]
        public void ExploreTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.ExploreTestsAction(controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(MISSING_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }

        [Test]
        public void ExploreTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.ExploreTestsAction(controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }
        #endregion

        #region CountTestsAction
        [Test]
        public void CountTestsAction_AfterLoad_ReturnsCorrectCount()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.CountTestsAction(_controller, EMPTY_FILTER, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo((MockAssembly.Tests-MockAssembly.Explicit).ToString()));
        }

        [Test]
        public void CountTestsAction_WithoutLoad_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.CountTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("The CountTestCases method was called but no test has been loaded"));
        }

        [Test]
        public void CountTestsAction_FileNotFound_ReturnsZero()
        {
            var controller = new FrameworkController(MISSING_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.CountTestsAction(controller, EMPTY_FILTER, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo("0"));
        }

        [Test]
        public void CountTestsAction_BadFile_ReturnsZero()
        {
            var controller = new FrameworkController(BAD_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.CountTestsAction(controller, EMPTY_FILTER, _handler);
            Assert.That(_handler.GetCallbackResult(), Is.EqualTo("0"));
        }
        #endregion

        #region RunTestsAction
        [Test]
        public void RunTestsAction_AfterLoad_ReturnsRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.RunTestsAction(_controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("Runnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(GetAttribute(result, "result"), Is.EqualTo("Failed"));
            Assert.That(GetAttribute(result, "passed"), Is.EqualTo(MockAssembly.Success.ToString()));
            Assert.That(GetAttribute(result, "failed"), Is.EqualTo(MockAssembly.ErrorsAndFailures.ToString()));
            Assert.That(GetAttribute(result, "skipped"), Is.EqualTo((MockAssembly.NotRunnable + MockAssembly.Ignored).ToString()));
            Assert.That(GetAttribute(result, "inconclusive"), Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Run result should have child tests");
        }

        [Test]
        public void RunTestsAction_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.RunTestsAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

        [Test]
        public void RunTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunTestsAction(controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(MISSING_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void RunTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunTestsAction(controller, EMPTY_FILTER, _handler);
            var result = GetXmlResult();

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(BAD_FILE));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region RunAsyncAction
        [Test]
        public void RunAsyncAction_AfterLoad_ReturnsRunnableSuite()
        {
            new FrameworkController.LoadTestsAction(_controller, _handler);
            new FrameworkController.RunAsyncAction(_controller, EMPTY_FILTER, _handler);
            //var result = GetXmlResult();

            //Assert.That(result.Name, Is.EqualTo("test-suite"));
            //Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            //Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("Runnable"));
            //Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            //Assert.That(GetAttribute(result, "result"), Is.EqualTo("Failed"));
            //Assert.That(GetAttribute(result, "passed"), Is.EqualTo(MockAssembly.Success.ToString()));
            //Assert.That(GetAttribute(result, "failed"), Is.EqualTo(MockAssembly.ErrorsAndFailures.ToString()));
            //Assert.That(GetAttribute(result, "skipped"), Is.EqualTo((MockAssembly.NotRunnable + MockAssembly.Ignored).ToString()));
            //Assert.That(GetAttribute(result, "inconclusive"), Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            //Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Run result should have child tests");
        }

        [Test]
        public void RunAsyncAction_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => new FrameworkController.RunAsyncAction(_controller, EMPTY_FILTER, _handler));
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

        [Test]
        public void RunAsyncAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(MISSING_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunAsyncAction(controller, EMPTY_FILTER, _handler);
            //var result = GetXmlResult();

            //Assert.That(result.Name, Is.EqualTo("test-suite"));
            //Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            //Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            //Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            //Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(MISSING_FILE));
            //Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void RunAsyncAction_BadFile_ReturnsNonRunnableSuite()
        {
            var controller = new FrameworkController(BAD_FILE, _settings);
            new FrameworkController.LoadTestsAction(controller, _handler);
            new FrameworkController.RunAsyncAction(controller, EMPTY_FILTER, _handler);
            //var result = GetXmlResult();

            //Assert.That(result.Name, Is.EqualTo("test-suite"));
            //Assert.That(GetAttribute(result, "type"), Is.EqualTo("Assembly"));
            //Assert.That(GetAttribute(result, "runstate"), Is.EqualTo("NotRunnable"));
            //Assert.That(GetAttribute(result, "testcasecount"), Is.EqualTo("0"));
            //Assert.That(GetSkipReason(result), Is.StringStarting("Could not load").And.Contains(BAD_FILE));
            //Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region Helper Methods
        private XmlNode GetXmlResult()
        {
            var doc = new XmlDocument();
            doc.LoadXml(_handler.GetCallbackResult());
            return doc.FirstChild;
        }

        private static string GetAttribute(XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            return attr == null ? null : attr.Value;
        }

        private static string GetSkipReason(XmlNode result)
        {
            var propNode = result.SelectSingleNode(string.Format("properties/property[@name='{0}']", PropertyNames.SkipReason));
            return propNode == null ? null : GetAttribute(propNode, "value");
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
