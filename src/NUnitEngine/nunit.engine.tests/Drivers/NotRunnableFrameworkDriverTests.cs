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
using NUnit.Engine.Internal;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Engine.Extensibility;
using NUnit.Tests.Assemblies;

namespace NUnit.Engine.Drivers.Tests
{
    // Functional tests of the NUnitFrameworkDriver calling into the framework.
    public class NotRunnableFrameworkDriverTests
    {
        private const string BAD_FILE = "junk.dll";
        private const string REASON = "Assembly could not be found";

        private IFrameworkDriver _driver;

        [SetUp]
        public void CreateDriver()
        {
            _driver = new NotRunnableFrameworkDriver( BAD_FILE, REASON);
        }

        [Test]
        public void Load_ReturnsNonRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(_driver.Load());

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.EqualTo(REASON));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
        }

        [Test]
        public void Explore_ReturnsNonRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(_driver.Explore(TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.EqualTo(REASON));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Result should not have child tests");
        }

        [Test]
        public void CountTestCases_ReturnsZero()
        {
            Assert.That(_driver.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        [Test]
        public void Run_ReturnsNonRunnableSuite()
        {
            var result = XmlHelper.CreateXmlNode(_driver.Run(new NullListener(), TestFilter.Empty));

            Assert.That(result.Name, Is.EqualTo("test-suite"));
            Assert.That(result.GetAttribute("type"), Is.EqualTo("Assembly"));
            Assert.That(result.GetAttribute("runstate"), Is.EqualTo("NotRunnable"));
            Assert.That(result.GetAttribute("testcasecount"), Is.EqualTo("0"));
            Assert.That(GetSkipReason(result), Is.EqualTo(REASON));
            Assert.That(result.SelectNodes("test-suite").Count, Is.EqualTo(0), "Load result should not have child tests");
            Assert.That(result.GetAttribute("result"), Is.EqualTo("Failed"));
            Assert.That(result.GetAttribute("label"), Is.EqualTo("Invalid"));
            Assert.That(result.SelectSingleNode("reason/message").InnerText, Is.EqualTo(REASON));
        }

        #region Helper Methods
        private static string GetSkipReason(XmlNode result)
        {
            var propNode = result.SelectSingleNode(string.Format("properties/property[@name='{0}']", PropertyNames.SkipReason));
            return propNode == null ? null : propNode.GetAttribute("value");
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
