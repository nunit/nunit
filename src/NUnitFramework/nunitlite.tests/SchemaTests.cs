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
                    filter: TestFilter.FromXml("<filter><or><not><cat re='1'>c</cat></not><and><prop name='x'>v</prop></and></or></filter>"));
               
                SchemaTestUtils.AssertValidXml(xml.ToString(), "NUnitLite-Run.xsd");
            });
        }
    }
}
#endif
