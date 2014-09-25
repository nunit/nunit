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
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using NUnit.Tests.Assemblies;
using NUnit.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Engine.Internal.Tests
{
    // Functional tests of the NUnitFrameworkDriver calling into the framework.
    public class NUnitFrameworkDriverTests
    {
        private string MOCK_ASSEMBLY = "mock-nunit-assembly.dll";
        private const string MISSING_FILE = "junk.dll";
        private const string BAD_FILE = "mock-nunit-assembly.pdb";

        private IDictionary<string, object> _settings = new Dictionary<string, object>();
        private NUnitFrameworkDriver _driver;
        private ICallbackEventHandler _handler;
        private string _mockAssemblyPath;

        [SetUp]
        public void CreateDriver()
        {
            _mockAssemblyPath = PathUtils.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY);
            _driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, _mockAssemblyPath, _settings);
            _handler = new CallbackEventHandler();
        }

        #region Construction Test
        //[Test]
        //public void ConstructContoller()
        //{
        //    Assert.That(_controller..Builder, Is.TypeOf<DefaultTestAssemblyBuilder>());
        //    Assert.That(_controller.Runner, Is.TypeOf<DefaultTestAssemblyRunner>());
        //    Assert.That(_controller.AssemblyPath, Is.EqualTo(MOCK_ASSEMBLY));
        //    Assert.That(_controller.Settings, Is.SameAs(_settings));
        //}
        #endregion

        #region Load
        [Test]
        public void Load_GoodFile_ReturnsRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(_driver.Load());

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void Load_FileNotFound_ReturnsNonRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(
                new NUnitFrameworkDriver(AppDomain.CurrentDomain, MISSING_FILE, _settings).Load());

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void Load_BadFile_ReturnsNonRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(
                new NUnitFrameworkDriver(AppDomain.CurrentDomain, BAD_FILE, _settings).Load());

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region Explore
        [Test]
        public void Explore_AfterLoad_ReturnsRunnableSuite()
        {
            _driver.Load();
            var result = XmlHelper.CreateXmlNode(_driver.Explore(TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Explore result should have child tests");
        }

        [Test]
        public void ExploreTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.Explore(TestFilter.Empty));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo("The Explore method was called but no test has been loaded"));
        }

        [Test]
        public void ExploreTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, MISSING_FILE, _settings);
            driver.Load();
            var result = XmlHelper.CreateXmlNode(driver.Explore(TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }

        [Test]
        public void ExploreTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, BAD_FILE, _settings);
            driver.Load();
            var result = XmlHelper.CreateXmlNode(driver.Explore(TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }
        #endregion

        #region CountTests
        [Test]
        public void CountTestsAction_AfterLoad_ReturnsCorrectCount()
        {
            _driver.Load();
            Assert.That(_driver.CountTestCases(TestFilter.Empty), Is.EqualTo(MockAssembly.Tests - MockAssembly.Explicit));
        }

        [Test]
        public void CountTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.CountTestCases(TestFilter.Empty));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo("The CountTestCases method was called but no test has been loaded"));
        }

        [Test]
        public void CountTestsAction_FileNotFound_ReturnsZero()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, MISSING_FILE, _settings);
            driver.Load();
            Assert.That(driver.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        [Test]
        public void CountTestsAction_BadFile_ReturnsZero()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, BAD_FILE, _settings);
            driver.Load();
            Assert.That(driver.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }
        #endregion

        #region RunTests
        [Test]
        public void RunTestsAction_AfterLoad_ReturnsRunnableSuite()
        {
            _driver.Load();
            var result = XmlHelper.CreateXmlNode(_driver.Run(new NullListener(), TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.GetAttribute("result"), Is.EqualTo("Failed"));
            Assert.That(result.GetAttribute("passed"), Is.EqualTo(MockAssembly.Success.ToString()));
            Assert.That(result.GetAttribute("failed"), Is.EqualTo(MockAssembly.ErrorsAndFailures.ToString()));
            Assert.That(result.GetAttribute("skipped"), Is.EqualTo((MockAssembly.NotRunnable + MockAssembly.Ignored).ToString()));
            Assert.That(result.GetAttribute("inconclusive"), Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            Assert.That(result.SelectNodes("test-suite").Count, Is.GreaterThan(0), "Explore result should have child tests");
        }

        [Test]
        public void RunTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.Run(new NullListener(), TestFilter.Empty));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

        [Test]
        public void RunTestsAction_FileNotFound_ReturnsNonRunnableSuite()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, MISSING_FILE, _settings);
            driver.Load();
            var result = XmlHelper.CreateXmlNode(driver.Run(new NullListener(), TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Contains.Substring(MISSING_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void RunTestsAction_BadFile_ReturnsNonRunnableSuite()
        {
            var driver = new NUnitFrameworkDriver(AppDomain.CurrentDomain, BAD_FILE, _settings);
            driver.Load();
            var result = XmlHelper.CreateXmlNode(driver.Run(new NullListener(), TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Contains.Substring(BAD_FILE).And.StartsWith("Could not load"));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region Helper Methods
        private static string GetSkipReason(XmlNode result)
        {
            var propNode = result.SelectSingleNode(string.Format("properties/property[@name='{0}']", PropertyNames.SkipReason));
            return propNode == null ? null : propNode.GetAttribute("value");
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

        #region Nested NullListener Class
        public class NullListener : ITestEventListener
        {
            public void OnTestEvent(string testEvent)
            {
                // No action
            }
        }
        #endregion
    }
}
