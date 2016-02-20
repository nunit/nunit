// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Text;
using NUnit.Framework;
using NUnit.Tests.Assemblies;

namespace NUnit.Engine.Services.Tests
{
    using Drivers;

    public class TestFilteringTests
    {
        private const string MOCK_ASSEMBLY = "mock-assembly.exe";

        private NUnit3FrameworkDriver _driver;

        [SetUp]
        public void LoadAssembly()
        {
            var mockAssemblyPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY);
            _driver = new NUnit3FrameworkDriver(AppDomain.CurrentDomain);
            _driver.Load(mockAssemblyPath, new Dictionary<string, object>());
        }

        [TestCase("<filter/>", MockAssembly.Tests)]
        [TestCase("<filter><test>NUnit.Tests.Assemblies.MockTestFixture</test></filter>", MockTestFixture.Tests)]
        [TestCase("<filter><test>NUnit.Tests.Assemblies.MockTestFixture.IgnoreTest</test></filter>", 1)]
        [TestCase("<filter><test>NUnit.Tests.TestNameEscaping.MustBeEscaped(&quot;&lt; left bracket&quot;)</test></filter>", 1)]
        [TestCase("<filter><test>NUnit.Tests.TestNameEscaping.MustBeEscaped(&quot;&gt; right bracket&quot;)</test></filter>", 1)]
        [TestCase("<filter><test>NUnit.Tests.TestNameEscaping.MustBeEscaped(&quot;'single quote'&quot;)</test></filter>", 1)]
        [TestCase("<filter><test>NUnit.Tests.TestNameEscaping.MustBeEscaped(&quot;\\&quot;double quote\\&quot;&quot;)</test></filter>", 1)]
        [TestCase("<filter><test>NUnit.Tests.TestNameEscaping.MustBeEscaped(&quot;&amp;amp&quot;)</test></filter>", 1)]
        [TestCase("<filter><class>NUnit.Tests.Assemblies.MockTestFixture</class></filter>", MockTestFixture.Tests)]
        [TestCase("<filter><name>IgnoreTest</name></filter>", 1)]
        [TestCase("<filter><name>MockTestFixture</name></filter>", MockTestFixture.Tests + NUnit.Tests.TestAssembly.MockTestFixture.Tests)]
        [TestCase("<filter><method>IgnoreTest</method></filter>", 1)]
        [TestCase("<filter><cat>FixtureCategory</cat></filter>", MockTestFixture.Tests)]
        public void UsingXml(string filter, int count)
        {
            Assert.That(_driver.CountTestCases(filter), Is.EqualTo(count));
        }

        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", MockTestFixture.Tests, TestName = "{m}_MockTestFixture")]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture.IgnoreTest", 1, TestName = "{m}_MockTest4")]
        [TestCase("NUnit.Tests.TestNameEscaping.MustBeEscaped(\"< left bracket\")", 1, TestName = "{m}_left_bracket")]
        [TestCase("NUnit.Tests.TestNameEscaping.MustBeEscaped(\"> right bracket\")", 1, TestName = "{m}_right_bracket")]
        [TestCase("NUnit.Tests.TestNameEscaping.MustBeEscaped(\"'single quote'\")", 1, TestName = "{m}_single_quote")]
        [TestCase("NUnit.Tests.TestNameEscaping.MustBeEscaped(\"\\\"double quote\\\"\")", 1, TestName = "{m}_double_quote")]
        [TestCase("NUnit.Tests.TestNameEscaping.MustBeEscaped(\"&amp\")", 1, TestName = "{m}_ampersand")]
        public void UsingTestFilterBuilderAddTest(string testName, int count)
        {
            var builder = new TestFilterBuilder();
            builder.AddTest(testName);

            Assert.That(_driver.CountTestCases(builder.GetFilter().Text), Is.EqualTo(count));

        }

        [TestCase("test==NUnit.Tests.Assemblies.MockTestFixture", MockTestFixture.Tests, TestName = "{m}_MockTestFixture")]
        [TestCase("test==NUnit.Tests.Assemblies.MockTestFixture.IgnoreTest", 1, TestName = "{m}_MockTest4")]
        [TestCase("test=='NUnit.Tests.TestNameEscaping.MustBeEscaped(\"< left bracket\")'", 1, TestName = "{m}_left_bracket")]
        [TestCase("test=='NUnit.Tests.TestNameEscaping.MustBeEscaped(\"> right bracket\")'", 1, TestName = "{m}_right_bracket")]
        [TestCase("test==/NUnit.Tests.TestNameEscaping.MustBeEscaped(\"'single quote'\")/", 1, TestName = "{m}_single_quote")]
        [TestCase("test==/NUnit.Tests.TestNameEscaping.MustBeEscaped(\"\\\\\"double quote\\\\\"\")/", 1, TestName = "{m}_double_quote")]
        [TestCase("test==/NUnit.Tests.TestNameEscaping.MustBeEscaped(\"&amp\")/", 1, TestName = "{m}_ampersand")]
        [TestCase("class==NUnit.Tests.Assemblies.MockTestFixture", MockTestFixture.Tests)]
        [TestCase("name==IgnoreTest", 1)]
        [TestCase("name==MockTestFixture", MockTestFixture.Tests + NUnit.Tests.TestAssembly.MockTestFixture.Tests)]
        [TestCase("method==IgnoreTest", 1)]
        [TestCase("cat==FixtureCategory", MockTestFixture.Tests)]
        public void UsingTestFilterBuilderSelectWhere(string expression, int count)
        {
            var builder = new TestFilterBuilder();
            builder.SelectWhere(expression);

            Assert.That(_driver.CountTestCases(builder.GetFilter().Text), Is.EqualTo(count));

        }
    }
}
