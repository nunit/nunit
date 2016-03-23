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
using System.Linq;
using System.Xml.Linq;
using NUnit.Tests.Assemblies;
using NUnit.Engine.Internal;
using NUnit.Framework;
using System.Reflection;

namespace NUnit.Engine.Tests
{
    public class NUnitPortableDriverTests
    {
        const string EMPTY_FILTER = "<filter />";
        const string MOCK_ASSEMBLY = "mock-assembly.exe";
        const string MISSING_FILE = "junk.dll";
        const string NUNIT_FRAMEWORK = "nunit.framework.dll";
        const string LOAD_MESSAGE = "Method called without calling Load first";

        IDictionary<string, object> _settings = new Dictionary<string, object>();

        NUnitPortableDriver _driver;
        Assembly _mockAssembly;
        Assembly _frameworkAssembly;

        [SetUp]
        public void CreateDriver()
        {
            var mockAssemblyPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY);
            _mockAssembly = Assembly.LoadFrom(mockAssemblyPath);
            var frameworkAssemblyPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, NUNIT_FRAMEWORK);
            _frameworkAssembly = Assembly.LoadFrom(frameworkAssemblyPath);
            _driver = new NUnitPortableDriver();
        }

        #region Construction Test

        public void ConstructController_MissingFile_ThrowsArgumentInvalid()
        {
            Assert.That(new NUnitPortableDriver(), Throws.ArgumentException);
        }
        #endregion

        #region Load
        [Test]
        public void Load_GoodFile_ReturnsRunnableSuite()
        {
            var result = XmlHelper.CreateXElement(_driver.Load(_frameworkAssembly, _mockAssembly, _settings));

            Assert.That(result.Name.LocalName, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.Elements("test-suite").Count(), Is.EqualTo(0), "Load result should not have child tests");
        }
        #endregion

        #region Explore
        [Test]
        public void Explore_AfterLoad_ReturnsRunnableSuite()
        {
            _driver.Load(_frameworkAssembly, _mockAssembly, _settings);
            var result = XmlHelper.CreateXElement(_driver.Explore(EMPTY_FILTER));

            Assert.That(result.Name.LocalName, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.Elements("test-suite").Count(), Is.GreaterThan(0), "Explore result should have child tests");
        }

        [Test]
        public void ExploreTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.Explore(EMPTY_FILTER));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo(LOAD_MESSAGE));
        }
        #endregion

        #region CountTests
        [Test]
        public void CountTestsAction_AfterLoad_ReturnsCorrectCount()
        {
            _driver.Load(_frameworkAssembly, _mockAssembly, _settings);
            Assert.That(_driver.CountTestCases(EMPTY_FILTER), Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void CountTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.CountTestCases(EMPTY_FILTER));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo(LOAD_MESSAGE));
        }
        #endregion

        #region RunTests
        [Test]
        public void RunTestsAction_AfterLoad_ReturnsRunnableSuite()
        {
            _driver.Load(_frameworkAssembly, _mockAssembly, _settings);
            var result = XmlHelper.CreateXElement(_driver.Run(null, EMPTY_FILTER));

            Assert.That(result.Name.LocalName, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("Runnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo(MockAssembly.Tests.ToString()));
            Assert.That(result.GetAttribute("result"), Is.EqualTo("Failed"));
            Assert.That(result.GetAttribute("passed"), Is.EqualTo(MockAssembly.Success.ToString()));
            Assert.That(result.GetAttribute("failed"), Is.EqualTo(MockAssembly.ErrorsAndFailures.ToString()));
            Assert.That(result.GetAttribute("skipped"), Is.EqualTo(MockAssembly.Skipped.ToString()));
            Assert.That(result.GetAttribute("inconclusive"), Is.EqualTo(MockAssembly.Inconclusive.ToString()));
            Assert.That(result.Elements("test-suite").Count(), Is.GreaterThan(0), "Explore result should have child tests");
        }

        [Test]
        public void RunTestsAction_WithoutLoad_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch(() => _driver.Run(null, EMPTY_FILTER));
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.Message, Is.EqualTo(LOAD_MESSAGE));
        }
        #endregion

        #region Nested Callback Class
        private class CallbackEventHandler
        {
            public Action<string> Action { get; private set; }

            public CallbackEventHandler()
            {
                Action = s => Result = s;
            }

            public string Result { get; private set; }
        }
        #endregion
    }
}
