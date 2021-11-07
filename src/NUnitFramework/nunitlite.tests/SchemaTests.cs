// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Tests.Assemblies;
using NUnit.TestUtilities;

namespace NUnitLite.Tests
{
    public static class SchemaTests
    {
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
                var result = TestBuilder.RunTestFixture(typeof(MockTestFixture));

                var xml = new StringWriter();
                new NUnit3XmlOutputWriter().WriteResultFile(
                    result,
                    xml,
                    runSettings: new Dictionary<string, object>
                    {
                        ["TestDictionary"] = new Dictionary<string, string>
                        {
                            ["TestKey"] = "TestValue",
                            ["ValuelessKey"] = null
                        }
                    },
                    filter: TestFilter.FromXml("<filter><id>x</id><or><not><cat re='1'>c</cat></not><and><prop name='x'>v</prop></and></or></filter>"));

                SchemaTestUtils.AssertValidXml(xml.ToString(), "TestResult.xsd");
            });
        }
    }
}
