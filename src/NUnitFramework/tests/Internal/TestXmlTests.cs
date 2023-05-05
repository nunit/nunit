// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

        [Test]
        public void TestNameWithInvalidCharacter_NonFirstPosition()
        {
            testMethod.Name = "Happy\u0001Face";
            // This throws if the name is not properly escaped
            Assert.That(testMethod.ToXml(false).OuterXml, Contains.Substring("name=\"Happy\\u0001Face\""));
        }

        #region Helper Methods For Checking XML

        private void CheckXmlForTest(Test test, bool recursive)
        {
            TNode topNode = test.ToXml(true);
            CheckXmlForTest(test, topNode, recursive);
        }

        private void CheckXmlForTest(Test test, TNode topNode, bool recursive)
        {
            Assert.That(topNode, Is.Not.Null);

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
            Assert.That(topNode.Attributes["id"], Is.EqualTo(test.Id));
            Assert.That(topNode.Attributes["name"], Is.EqualTo(test.Name));
            Assert.That(topNode.Attributes["fullname"], Is.EqualTo(test.FullName));
            if (test.TypeInfo != null)
            {
                Assert.That(test.ClassName, Is.Not.Null);
                Assert.That(topNode.Attributes["classname"], Is.EqualTo(test.ClassName));
            }
            if (test is TestMethod)
            {
                Assert.That(test.MethodName, Is.Not.Null);
                Assert.That(topNode.Attributes["methodname"], Is.EqualTo(test.MethodName));
            }
            Assert.That(topNode.Attributes["runstate"], Is.EqualTo(test.RunState.ToString()));

            if (test.Properties.Keys.Count > 0)
            {
                var expectedProps = new List<string>();
                foreach (string key in test.Properties.Keys)
                    foreach (object? value in test.Properties[key])
                        if (value is not null)
                            expectedProps.Add(key + "=" + value);

                TNode? propsNode = topNode.SelectSingleNode("properties");
                Assert.That(propsNode, Is.Not.Null);

                var actualProps = new List<string>();
                foreach (TNode node in propsNode.ChildNodes)
                {
                    string? name = node.Attributes["name"];
                    string? value = node.Attributes["value"];
                    Assert.That(name, Is.Not.Null);
                    Assert.That(value, Is.Not.Null);
                    actualProps.Add(name + "=" + value);
                }

                Assert.That(actualProps, Is.EquivalentTo(expectedProps));
            }

            if (recursive)
            {
                if (test is TestSuite suite)
                {
                    foreach (Test child in suite.Tests)
                    {
                        string xpathQuery = $"{child.XmlElementName}[@id={child.Id}]";
                        TNode? childNode = topNode.SelectSingleNode(xpathQuery);
                        Assert.That(childNode, Is.Not.Null, "Expected node for test with ID={0}, Name={1}", child.Id, child.Name);

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
