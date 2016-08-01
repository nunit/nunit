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
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class TestXmlTests
    {
        private TestSuite testSuite;
        private TestFixture testFixture;
        private TestMethod testMethod;

        [SetUp]
        public void SetUp()
        {
            testMethod = new TestMethod(new MethodWrapper(typeof(DummyFixture), "DummyMethod"));
            testMethod.Properties.Set(PropertyNames.Description, "Test description");
            testMethod.Properties.Add(PropertyNames.Category, "Dubious");
            testMethod.Properties.Set("Priority", "low");

            testFixture = new TestFixture(new TypeWrapper(typeof(DummyFixture)));
            testFixture.Properties.Set(PropertyNames.Description, "Fixture description");
            testFixture.Properties.Add(PropertyNames.Category, "Fast");
            testFixture.Properties.Add("Value", 3);

            testFixture.Tests.Add(testMethod);

            testSuite = new TestSuite(typeof(DummyFixture));
            testSuite.Properties.Set(PropertyNames.Description, "Suite description");
        }

        [Test]
        public void TestTypeTests()
        {
            Assert.That(testMethod.TestType, 
                Is.EqualTo("TestMethod"));
            Assert.That(testFixture.TestType, 
                Is.EqualTo("TestFixture"));
            Assert.That(testSuite.TestType, 
                Is.EqualTo("TestSuite"));
            Assert.That(new TestAssembly("junk").TestType, 
                Is.EqualTo("Assembly"));
            Assert.That(new ParameterizedMethodSuite(new MethodWrapper(typeof(DummyFixture), "GenericMethod")).TestType,
                Is.EqualTo("GenericMethod"));
            Assert.That(new ParameterizedMethodSuite(new MethodWrapper(typeof(DummyFixture), "ParameterizedMethod")).TestType,
                Is.EqualTo("ParameterizedMethod"));
            Assert.That(new ParameterizedFixtureSuite(new TypeWrapper(typeof(DummyFixture))).TestType,
                Is.EqualTo("ParameterizedFixture"));
            Type genericType = typeof(DummyGenericFixture<int>).GetGenericTypeDefinition();
            Assert.That(new ParameterizedFixtureSuite(new TypeWrapper(genericType)).TestType,
                Is.EqualTo("GenericFixture"));
        }

        [Test]
        public void TestMethodToXml()
        {
            CheckXmlForTest(testMethod, false);
        }

        [Test]
        public void TestFixtureToXml()
        {
            CheckXmlForTest(testFixture, false);
        }

        [Test]
        public void TestFixtureToXml_Recursive()
        {
            CheckXmlForTest(testFixture, true);
        }

        [Test]
        public void TestSuiteToXml()
        {
            CheckXmlForTest(testSuite, false);
        }

        [Test]
        public void TestSuiteToXml_Recursive()
        {
            CheckXmlForTest(testSuite, true);
        }

        [Test]
        public void TestNameWithInvalidCharacter()
        {
            testMethod.Name = "\u0001HappyFace";
            // This throws if the name is not properly escaped
            Assert.That(testMethod.ToXml(false).OuterXml, Contains.Substring("name=\"\\u0001HappyFace\""));
        }

        #region Helper Methods For Checking XML

        private void CheckXmlForTest(Test test, bool recursive)
        {
            TNode topNode = test.ToXml(true);
            CheckXmlForTest(test, topNode, recursive);
        }

        private void CheckXmlForTest(Test test, TNode topNode, bool recursive)
        {
            Assert.NotNull(topNode);

            //if (test is TestSuite)
            //{
            //    Assert.That(xmlNode.Name, Is.EqualTo("test-suite"));
            //    Assert.That(xmlNode.Attributes["type"].Value, Is.EqualTo(test.XmlElementName));
            //}
            //else
            //{
            //    Assert.That(xmlNode.Name, Is.EqualTo("test-case"));
            //}

            Assert.That(topNode.Name, Is.EqualTo(test.XmlElementName));
            Assert.That(topNode.Attributes["id"], Is.EqualTo(test.Id.ToString()));
            Assert.That(topNode.Attributes["name"], Is.EqualTo(test.Name));
            Assert.That(topNode.Attributes["fullname"], Is.EqualTo(test.FullName));
            if (test.TypeInfo != null)
            {
                Assert.NotNull(test.ClassName);
                Assert.That(topNode.Attributes["classname"], Is.EqualTo(test.ClassName));
            }
            if (test is TestMethod)
            {
                Assert.NotNull(test.MethodName);
                Assert.That(topNode.Attributes["methodname"], Is.EqualTo(test.MethodName));
            }
            Assert.That(topNode.Attributes["runstate"], Is.EqualTo(test.RunState.ToString()));

            if (test.Properties.Keys.Count > 0)
            {
                var expectedProps = new List<string>();
                foreach (string key in test.Properties.Keys)
                    foreach (object value in test.Properties[key])
                        expectedProps.Add(key + "=" + value.ToString());

                TNode propsNode = topNode.SelectSingleNode("properties");
                Assert.NotNull(propsNode);

                var actualProps = new List<string>();
                foreach (TNode node in propsNode.ChildNodes)
                {
                    string name = node.Attributes["name"];
                    string value = node.Attributes["value"];
                    actualProps.Add(name + "=" + value.ToString());
                }

                Assert.That(actualProps, Is.EquivalentTo(expectedProps));
            }

            if (recursive)
            {
                TestSuite suite = test as TestSuite;
                if (suite != null)
                {
                    foreach (Test child in suite.Tests)
                    {
                        string xpathQuery = string.Format("{0}[@id={1}]", child.XmlElementName, child.Id);
                        TNode childNode = topNode.SelectSingleNode(xpathQuery);
                        Assert.NotNull(childNode, "Expected node for test with ID={0}, Name={1}", child.Id, child.Name);

                        CheckXmlForTest(child, childNode, recursive);
                    }
                }
            }
        }

        #endregion

        public class DummyFixture
        {
            public void DummyMethod() { }
            public void ParameterizedMethod(int x) { }
            public void GenericMethod<T>(T x) { }
        }

        public class DummyGenericFixture<T>
        {
        }
    }
}
