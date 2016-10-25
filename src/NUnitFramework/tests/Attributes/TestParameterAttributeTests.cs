// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Linq;
using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.TestData.TestAttributeTests;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestParameterAttributeTests
    {
        private static readonly Type FixtureType = typeof(TestParameterAttributeFixture);

        [TestCase("TestParameterDiscovery", 1, new[] { "param1" }, new[] { typeof(string) }, new object[] { null }, new object[] { null }, new string[] { null },
            new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestMultipleParameterDiscovery2", 2, new[] { "param1", "param2" }, new[] { typeof(string), typeof(string) }, new object[] { null, null },
            new object[] { null, null }, new string[] { null, null }, new object[] { null, null }, RunState.Runnable, null)]
        [TestCase("TestMultipleParameterDiscovery3", 3, new[] { "param1", "param2", "param3" }, new[] { typeof(string), typeof(string), typeof(string) },
            new object[] { null, null, null }, new object[] { null, null, null }, new string[] { null, null, null }, new object[] { null, null, null }, RunState.Runnable, null)]
        [TestCase("TestTypedParameterDiscovery", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { null }, new object[] { null }, new string[] { null },
            new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestTypedParameterWithMethodDiscovery", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { null }, new object[] { null },
            new string[] { "param1" }, new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestTypedParameterDiscoveryWithMinAndMax", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { 0 }, new object[] { 32 }, new string[] { null },
            new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestTypedParameterDiscoveryWithValidValues", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { null }, new object[] { null },
            new string[] { null }, new object[] { new object[] { 1, 3, 5, 7 } }, RunState.Runnable, null)]
        [TestCase("TestUntypedParameterWithIntMethodDiscovery", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { null }, new object[] { null },
            new string[] { "param1" }, new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestUntypedParameterWithStringMethodDiscovery", 1, new[] { "param1" }, new[] { typeof(string) }, new object[] { null }, new object[] { null },
            new string[] { "param1" }, new object[] { null }, RunState.Runnable, null)]
        [TestCase("TestTypedParameterWithStringMethodAndIntParamDiscovery", 1, new[] { "param1" }, new[] { typeof(int) }, new object[] { null }, new object[] { null },
            new string[] { "param1" }, new object[] { null }, RunState.NotRunnable, "TestParameterAttribute Type must be the same a the type of \"param1\"")]
        [TestCase("TestUntypedParameterWithMissingMethodParamDiscovery", 1, new[] { "param1" }, new[] { typeof(string) }, new object[] { null }, new object[] { null },
            new string[] { "param1" }, new object[] { null }, RunState.NotRunnable, "TestParameterAttribute MethodParameterName must be one of the test method parameters")]
        [TestCase("TestUntypedParametersWithMethodParamDiscovery", 2, new[] { "param1", "param2" }, new[] { typeof(int), typeof(string) }, new object[] { null, null }, new object[] { null, null },
            new string[] { "param1", "param2" }, new object[] { null, null }, RunState.Runnable, null)]
        public void TestParameterCreate(string testName, int count, string[] names, Type[] types, object[] minVals, object[] maxVals, string[] parameterNames,
            object[] validValues, RunState expectedState, string expectedError)
        {
            var test = TestBuilder.MakeTestCase(FixtureType, testName);

            Assert.AreEqual(expectedState, test.RunState, "#0A");
            Assert.AreEqual(expectedError, test.Properties.Get(PropertyNames.SkipReason), "#0B");

            var tpas = test.TestParameterAttributes;
            Assert.AreEqual(count, tpas.Count, "#1A");
            Assert.AreEqual(count, tpas.Distinct().Count(), "#1B");

            tpas = tpas.OrderBy(tpa => tpa.Name).ToList();

            for (int ix = 0; ix < tpas.Count; ++ix)
            {
                var tpa = tpas[ix] as TestParameterAttribute;
                Assert.IsNotNull(tpa, "#2/" + ix);

                Assert.AreEqual(names[ix], tpa.Name, "#3/" + ix);
                Assert.AreEqual(types[ix], tpa.Type, "#4/" + ix);
                Assert.AreEqual(minVals[ix], tpa.MinimumValue, "#5/" + ix);
                Assert.AreEqual(maxVals[ix], tpa.MaximumValue, "#6/" + ix);
                Assert.AreEqual(parameterNames[ix], tpa.MethodParameterName, "#7/" + ix);
                var vv = validValues[ix];
                if (vv == null)
                    Assert.IsNull(tpa.ValidValues, "#8A/" + ix);
                else
                {
                    var vva = vv as object[];
                    Assert.IsNotNull(vva, "#8B/" + ix);
                    Assert.AreEqual(vva.Length, tpa.ValidValues.Length, "#9/" + ix);

                    for (int iy = 0; iy < vva.Length; ++iy)
                        Assert.AreEqual(vva[iy], tpa.ValidValues[iy], "#10/" + ix + "/" + iy);
                }
            }

            var xcase = test.ToXml(false);
            Assert.IsNotNull(xcase, "#11D");
            Assert.AreEqual("test-case", xcase.Name, "#11E");
            var xtestparam = xcase.SelectSingleNode("test-parameters");
            Assert.IsNotNull(xtestparam, "#11C");

            var xtpas = xtestparam.SelectNodes("test-parameter").ToArray();
            Assert.AreEqual(count, xtpas.Length, "#11A");
            Assert.AreEqual(count, xtpas.Select(xtpa => xtpa.Attributes["name"]).Distinct().Count(), "#11B");

            xtpas = xtpas.OrderBy(xtpa => xtpa.Attributes["name"]).ToArray();

            for (int ix = 0; ix < xtpas.Length; ++ix)
            {
                var xtpa = xtpas[ix];
                Assert.IsNotNull(xtpa, "#12/" + ix);

                Assert.AreEqual(names[ix], xtpa.Attributes["name"], "#13/" + ix);
                if (types[ix] == typeof(string))
                    Assert.IsNull(xtpa.Attributes["type"], "#14A");
                else
                    Assert.AreEqual(types[ix], Type.GetType(xtpa.Attributes["type"]), "#14B/" + ix);
                if (minVals[ix] == null)
                    Assert.IsNull(xtpa.Attributes["minvalue"], "#15A/" + ix);
                else
                    Assert.AreEqual(minVals[ix].ToString(), xtpa.Attributes["minvalue"], "#15B/" + ix);
                if (maxVals[ix] == null)
                    Assert.IsNull(xtpa.Attributes["maxvalue"], "#16A/" + ix);
                else
                    Assert.AreEqual(maxVals[ix].ToString(), xtpa.Attributes["maxvalue"], "#16B/" + ix);
                if (parameterNames[ix] == null)
                    Assert.IsNull(xtpa.Attributes["inject"], "#17A/" + ix);
                else
                    Assert.AreEqual(parameterNames[ix], xtpa.Attributes["inject"], "#17B/" + ix);
                var vv = validValues[ix];
                var xvv = xtpa.SelectNodes("valid-value").ToArray();
                if (vv == null)
                    Assert.AreEqual(0, xvv.Length, "#19A/" + ix);
                else
                {
                    var vva = vv as object[];
                    Assert.IsNotNull(vva, "#18/" + ix);
                    Assert.AreEqual(vva.Length, xvv.Length, "#19B/" + ix);

                    for (int iy = 0; iy < vva.Length; ++iy)
                        Assert.AreEqual(vva[iy].ToString(), xvv[iy].Attributes["value"], "#20/" + ix + "/" + iy);
                }
            }
        }

        [Test]
        public void TestTypedParameterWithMethod()
        {
            var parTemp = TestContext.Parameters;
            TestContext.Parameters = new RuntimeTestParameters();
            try
            {
                TestContext.Parameters.Add("param1", "1");
                var test = TestBuilder.MakeTestCase(FixtureType, "TestTypedParameterWithMethodDiscovery");

                Assert.AreEqual(1, test.parms.Arguments[0]);

                var result = TestBuilder.RunTest(test, new TestParameterAttributeFixture());

                Assert.AreEqual(ResultState.Success, result.ResultState, result.Output + " : " + result.Message);
            }
            finally
            {
                TestContext.Parameters = parTemp;
            }
        }

        [Test]
        public void TestUntypedParametersWithMethodParam()
        {
            var parTemp = TestContext.Parameters;
            TestContext.Parameters = new RuntimeTestParameters();
            try
            {
                TestContext.Parameters.Add("param1", "1");
                TestContext.Parameters.Add("param2", "2");
                var test = TestBuilder.MakeTestCase(FixtureType, "TestUntypedParametersWithMethodParamDiscovery");

                Assert.AreEqual(1, test.parms.Arguments[0]);
                Assert.AreEqual("2", test.parms.Arguments[1]);

                var result = TestBuilder.RunTest(test, new TestParameterAttributeFixture());

                Assert.AreEqual(ResultState.Success, result.ResultState, result.Output + " : " + result.Message);
            }
            finally
            {
                TestContext.Parameters = parTemp;
            }
        }

        [Test]
        public void TestTypedParameterWithMethodAndTestCase()
        {
            var parTemp = TestContext.Parameters;
            TestContext.Parameters = new RuntimeTestParameters();
            try
            {
                TestContext.Parameters.Add("param1", "1");

                var testSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestTypedParameterWithMethodAndTestCaseDiscovery");

                foreach (var test in testSuite.Tests)
                {
                    Assert.AreEqual(3, ((TestMethod)test).parms.Arguments.Length, "args.length not 3: " + test.FullName);
                    Assert.AreEqual(1, ((TestMethod)test).parms.Arguments[2], "arg[2] not 1: " + test.FullName);

                    var result = TestBuilder.RunTest((TestMethod)test, new TestParameterAttributeFixture());

                    Assert.AreEqual(ResultState.Success, result.ResultState, result.Output + " : " + result.Message);
                }
            }
            finally
            {
                TestContext.Parameters = parTemp;
            }
        }
    }
}
