// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if !NETCOREAPP1_1
using System.Collections.Generic;
using NUnit.TestUtilities;

namespace NUnit.Framework.Api
{
    public static class SchemaTests
    {
        [Test]
        public static void TestSchemaIsValid()
        {
            Assert.Multiple(() => SchemaTestUtils.AssertValidXsd("Test.xsd"));
        }

        [Test]
        public static void TestSchemaMatches()
        {
            Assert.Multiple(() =>
            {
                var controller = new FrameworkController("mock-assembly", null, new Dictionary<string, string>());
                var loadXml = controller.LoadTests();
                var exploreXml = controller.ExploreTests(null);

                SchemaTestUtils.AssertValidXml(loadXml, "Test.xsd");
                SchemaTestUtils.AssertValidXml(exploreXml, "Test.xsd");
            });
        }

        [Test]
        public static void TestResultSchemaIsValid()
        {
            Assert.Multiple(() => SchemaTestUtils.AssertValidXsd("TestResult.xsd"));
        }

        [Test]
        public static void TestResultSchemaMatches()
        {
            Assert.Multiple(() =>
            {
                var controller = new FrameworkController("mock-assembly", null, new Dictionary<string, string>());
                controller.LoadTests();
                var resultXml = controller.RunTests(null);

                SchemaTestUtils.AssertValidXml(resultXml, "TestResult.xsd");
            });
        }

        [Test]
        public static void TestFilterSchemaIsValid()
        {
            Assert.Multiple(() => SchemaTestUtils.AssertValidXsd("TestFilter.xsd"));
        }

        [Test]
        public static void TestFilterSchemaMatches()
        {
            Assert.Multiple(() =>
            {
                SchemaTestUtils.AssertValidXml(@"
                    <or>
                      <not>
                        <class>c</class>
                      </not>
                      <and>
                        <class re='1'>c</class>
                        <cat>c</cat>
                        <cat re='1'>c</cat>
                        <id>i</id>
                        <id re='1'>i</id>
                        <method>m</method>
                        <method re='1'>m</method>
                        <name>n</name>
                        <name re='1'>n</name>
                        <namespace>n</namespace>
                        <namespace re='1'>n</namespace>
                        <prop name='p'>v</prop>
                        <prop name='p' re='1'>v</prop>
                        <test>t</test>
                        <test re='1'>t</test>
                      </and>
                    </or>",
                    "TestFilter.xsd");
            });
        }
    }
}
#endif