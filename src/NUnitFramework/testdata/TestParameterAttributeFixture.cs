// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.TestData.TestAttributeTests
{
    [TestFixture]
    public class TestParameterAttributeFixture
    {
        [Test]
        [TestParameter("param1")]
        public void TestParameterDiscovery()
        {

        }

        [Test]
        [TestParameter("param1")]
        [TestParameter("param2")]
        public void TestMultipleParameterDiscovery2()
        {

        }

        [Test]
        [TestParameter("param1")]
        [TestParameter("param2")]
        [TestParameter("param3")]
        public void TestMultipleParameterDiscovery3()
        {

        }

        [Test]
        [TestParameter("param1", typeof(int))]
        public void TestTypedParameterDiscovery()
        {

        }

        [Test]
        [TestParameter("param1", typeof(int), MethodParameterName = "param1")]
        public void TestTypedParameterWithMethodDiscovery(int param1)
        {
            Assert.AreEqual(TestContext.Parameters.Get<int>("param1"), param1);
        }

        [Test]
        [TestParameter("param1", typeof(int), MinimumValue = 0, MaximumValue = 32)]
        public void TestTypedParameterDiscoveryWithMinAndMax()
        {

        }

        [Test]
        [TestParameter("param1", typeof(int), ValidValues = new object[] { 1, 3, 5, 7 })]
        public void TestTypedParameterDiscoveryWithValidValues()
        {

        }

        [TestCase(123, "123")]
        [TestCase(456, "456")]
        [TestParameter("param1", typeof(int), MethodParameterName = "param1")]
        public void TestTypedParameterWithMethodAndTestCaseDiscovery(int testarg1, string testarg2, int param1)
        {
            Assert.AreEqual(TestContext.Parameters.Get<int>("param1"), param1);
        }

        [Test]
        [TestParameter("param1", MethodParameterName = "param1")]
        public void TestUntypedParameterWithIntMethodDiscovery(int param1)
        {
            Assert.AreEqual(TestContext.Parameters.Get<int>("param1"), param1);
        }

        [Test]
        [TestParameter("param1", MethodParameterName = "param1")]
        public void TestUntypedParameterWithStringMethodDiscovery(string param1)
        {
            Assert.AreEqual(TestContext.Parameters.Get<string>("param1"), param1);
        }

        [Test]
        [TestParameter("param1", typeof(int), MethodParameterName = "param1")]
        public void TestTypedParameterWithStringMethodAndIntParamDiscovery(string param1)
        {
        }

        [Test]
        [TestParameter("param1", MethodParameterName = "param1")]
        public void TestUntypedParameterWithMissingMethodParamDiscovery(int param2)
        {
        }

        [Test]
        [TestParameter("param1", MethodParameterName = "param1")]
        [TestParameter("param2", MethodParameterName = "param2")]
        public void TestUntypedParametersWithMethodParamDiscovery(int param1, string param2)
        {
            Assert.AreEqual(TestContext.Parameters.Get<int>("param1"), param1);
            Assert.AreEqual(TestContext.Parameters.Get<string>("param2"), param2);
        }
    }
}