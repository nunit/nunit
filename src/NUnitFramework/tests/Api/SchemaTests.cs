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

#if !NETCOREAPP1_1    // Schema validation doesn’t exist
#if !(NET20 || NET35) // Framework bug causes NRE: https://social.msdn.microsoft.com/Forums/en-US/53be44de-30b2-4d18-968d-d3414d0783b1
                      // We don’t really need these tests to run on more than one platform.

using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework.Internal;
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
                var controller = new FrameworkController("mock-assembly", Test.IdPrefix, new Dictionary<string, string>());
                var loadXml = controller.LoadTests();
                var exploreXml = controller.ExploreTests(null);

                SchemaTestUtils.AssertValidXml(loadXml, "Test.xsd");
                SchemaTestUtils.AssertValidXml(exploreXml, "Test.xsd");
            });
        }

        [Test]
        public static void TestSchemaDisallowsDuplicateIds()
        {
            SchemaTestUtils.AssertInvalidXml(@"
                <test-suite id='0' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0'>
                  <test-suite id='1' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0'>
                    <test-case id='0' name='0' fullname='0' runstate='Runnable' seed='0' />
                  </test-suite>
                </test-suite>",
                "Test.xsd");
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
                var controller = new FrameworkController("mock-assembly", Test.IdPrefix, new Dictionary<string, string>());
                controller.LoadTests();

                var frameworkXml = XElement.Parse(controller.RunTests(null));

                var fullXml = new XElement("test-run",
                    new XElement("command-line"),
                    new XElement("filter"),
                    frameworkXml,
                    new XAttribute("id", 0),
                    new XAttribute("name", 0),
                    new XAttribute("fullname", 0),
                    new XAttribute("testcasecount", 0),
                    new XAttribute("result", "Passed"),
                    new XAttribute("total", 0),
                    new XAttribute("passed", 0),
                    new XAttribute("failed", 0),
                    new XAttribute("inconclusive", 0),
                    new XAttribute("skipped", 0),
                    new XAttribute("asserts", 0),
                    new XAttribute("random-seed", 0));

                SchemaTestUtils.AssertValidXml(fullXml.ToString(), "TestResult.xsd");
            });
        }

        [Test]
        public static void MultipleTestsAreAllowedInsideTestRun()
        {
            Assert.Multiple(() =>
            {
                SchemaTestUtils.AssertValidXml(@"
                    <test-run id='0' name='0' fullname='0' testcasecount='0' result='Passed' total='0' passed='0' failed='0' inconclusive='0' skipped='0' asserts='0' random-seed='0'>
                      <command-line />
                      <filter />
                      <test-case result='Passed' asserts='0' id='1' name='0' fullname='0' runstate='Runnable' seed='0' />
                      <test-suite result='Passed' asserts='0' total='0' passed='0' failed='0' warnings='0' inconclusive='0' skipped='0' id='2' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0' />
                      <test-case result='Passed' asserts='0' id='3' name='0' fullname='0' runstate='Runnable' seed='0' />
                    </test-run>",
                    "TestResult.xsd");
            });
        }


        [Test]
        public static void TestResultSchemaDisallowsRootElement_Filter()
        {
            SchemaTestUtils.AssertInvalidXml("<filter />", "TestResult.xsd");
        }

        [Test]
        public static void TestResultSchemaDisallowsRootElement_TestSuite()
        {
            SchemaTestUtils.AssertInvalidXml(@"
                <test-suite
                  result='Passed'
                  asserts='0'
                  total='0'
                  passed='0'
                  failed='0'
                  warnings='0'
                  inconclusive='0'
                  skipped='0'
                  id='0'
                  name='0'
                  fullname='0'
                  runstate='Runnable'
                  type='TestMethod'
                  testcasecount='0' />",
                "TestResult.xsd");
        }

        [Test]
        public static void TestResultSchemaDisallowsRootElement_TestCase()
        {
            SchemaTestUtils.AssertInvalidXml(@"
                <test-case
                  result='Passed'
                  asserts='0'
                  id='0'
                  name='0'
                  fullname='0'
                  runstate='Runnable'
                  seed='0' />",
                "TestResult.xsd");
        }

        [Test]
        public static void TestResultSchemaDisallowsDuplicateIds()
        {
            SchemaTestUtils.AssertInvalidXml(@"
                <test-run id='0' name='0' fullname='0' testcasecount='0' result='Passed' total='0' passed='0' failed='0' inconclusive='0' skipped='0' asserts='0' random-seed='0'>
                  <command-line />
                  <filter />
                  <test-suite id='1' result='Passed' asserts='0' total='0' passed='0' failed='0' warnings='0' inconclusive='0' skipped='0' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0'>
                    <test-case id='0' result='Passed' asserts='0' name='0' fullname='0' runstate='Runnable' seed='0' />
                  </test-suite>
                </test-run>",
                "TestResult.xsd");

            SchemaTestUtils.AssertInvalidXml(@"
                <test-run id='0' name='0' fullname='0' testcasecount='0' result='Passed' total='0' passed='0' failed='0' inconclusive='0' skipped='0' asserts='0' random-seed='0'>
                  <command-line />
                  <filter />
                  <test-suite id='1' result='Passed' asserts='0' total='0' passed='0' failed='0' warnings='0' inconclusive='0' skipped='0' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0'>
                    <test-suite id='2' result='Passed' asserts='0' total='0' passed='0' failed='0' warnings='0' inconclusive='0' skipped='0' name='0' fullname='0' runstate='Runnable' type='TestMethod' testcasecount='0'>
                      <test-case id='1' result='Passed' asserts='0' name='0' fullname='0' runstate='Runnable' seed='0' />
                    </test-suite>
                  </test-suite>
                </test-run>",
                "TestResult.xsd");
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
                    <filter>
                      <class>c</class>
                      <or>
                        <not>
                          <class re='1'>c</class>
                        </not>
                        <and>
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
                      </or>
                    </filter>",
                    "TestFilter.xsd");
            });
        }

        [Test]
        public static void TestFilterSchemaDisallowsRootElement_And()
        {
            SchemaTestUtils.AssertInvalidXml("<and><class>x</class></and>", "TestFilter.xsd");
        }
    }
}
#endif
#endif
