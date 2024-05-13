// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class TestXmlTests
    {
        private TestSuite _testSuite;
        private TestFixture _testFixture;
        private TestMethod _testMethod;

        [SetUp]
        public void SetUp()
        {
            _testMethod = new TestMethod(new MethodWrapper(typeof(DummyFixture), "DummyMethod"));
            _testMethod.Properties.Set(PropertyNames.Description, "Test description");
            _testMethod.Properties.Add(PropertyNames.Category, "Dubious");
            _testMethod.Properties.Set("Priority", "low");

            _testFixture = new TestFixture(new TypeWrapper(typeof(DummyFixture)));
            _testFixture.Properties.Set(PropertyNames.Description, "Fixture description");
            _testFixture.Properties.Add(PropertyNames.Category, "Fast");
            _testFixture.Properties.Add("Value", 3);

            _testFixture.Tests.Add(_testMethod);

            _testSuite = new TestSuite(typeof(DummyFixture));
            _testSuite.Properties.Set(PropertyNames.Description, "Suite description");
        }

        [Test]
        public void TestTypeTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_testMethod.TestType, Is.EqualTo("TestMethod"));
                Assert.That(_testFixture.TestType, Is.EqualTo("TestFixture"));
                Assert.That(_testSuite.TestType, Is.EqualTo("TestSuite"));
                Assert.That(new TestAssembly("junk").TestType, Is.EqualTo("Assembly"));
                Assert.That(new ParameterizedMethodSuite(new MethodWrapper(typeof(DummyFixture), "GenericMethod")).TestType, Is.EqualTo("GenericMethod"));
                Assert.That(new ParameterizedMethodSuite(new MethodWrapper(typeof(DummyFixture), "ParameterizedMethod")).TestType, Is.EqualTo("ParameterizedMethod"));
                Assert.That(new ParameterizedFixtureSuite(new TypeWrapper(typeof(DummyFixture))).TestType, Is.EqualTo("ParameterizedFixture"));
            });
            Type genericType = typeof(DummyGenericFixture<int>).GetGenericTypeDefinition();
            Assert.That(new ParameterizedFixtureSuite(new TypeWrapper(genericType)).TestType, Is.EqualTo("GenericFixture"));
        }

        [Test]
        public void TestMethodToXml()
        {
            CheckXmlForTest(_testMethod, false);
        }

        [Test]
        public void TestFixtureToXml()
        {
            CheckXmlForTest(_testFixture, false);
        }

        [Test]
        public void TestFixtureToXml_Recursive()
        {
            CheckXmlForTest(_testFixture, true);
        }

        [Test]
        public void TestSuiteToXml()
        {
            CheckXmlForTest(_testSuite, false);
        }

        [Test]
        public void TestSuiteToXml_Recursive()
        {
            CheckXmlForTest(_testSuite, true);
        }

        [Test]
        public void TestNameWithInvalidCharacter()
        {
            _testMethod.Name = "\u0001HappyFace";
            // This throws if the name is not properly escaped
            Assert.That(_testMethod.ToXml(false).OuterXml, Contains.Substring("name=\"\\u0001HappyFace\""));
        }

        [Test]
        public void TestNameWithInvalidCharacter_NonFirstPosition()
        {
            _testMethod.Name = "Happy\u0001Face";
            // This throws if the name is not properly escaped
            Assert.That(_testMethod.ToXml(false).OuterXml, Contains.Substring("name=\"Happy\\u0001Face\""));
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
            if (test.TypeInfo is not null)
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
                {
                    foreach (object? value in test.Properties[key])
                    {
                        if (value is not null)
                            expectedProps.Add(key + "=" + value);
                    }
                }

                TNode? propsNode = topNode.SelectSingleNode("properties");
                Assert.That(propsNode, Is.Not.Null);

                var actualProps = new List<string>();
                foreach (TNode node in propsNode.ChildNodes)
                {
                    string? name = node.Attributes["name"];
                    string? value = node.Attributes["value"];
                    Assert.Multiple(() =>
                    {
                        Assert.That(name, Is.Not.Null);
                        Assert.That(value, Is.Not.Null);
                    });
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
                        Assert.That(childNode, Is.Not.Null, $"Expected node for test with ID={child.Id}, Name={child.Name}");

                        CheckXmlForTest(child, childNode, recursive);
                    }
                }
            }
        }

        #endregion

        public class DummyFixture
        {
            public void DummyMethod()
            {
            }
            public void ParameterizedMethod(int x)
            {
            }
            public void GenericMethod<T>(T x)
            {
            }
        }

        public class DummyGenericFixture<T>
        {
        }
    }
}
