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
            testMethod = new TestMethod(typeof(DummyFixture).GetMethod("DummyMethod"));
            testMethod.Properties.Set(PropertyNames.Description, "Test description");
            testMethod.Properties.Add(PropertyNames.Category, "Dubious");
            testMethod.Properties.Set("Priority", "low");

            testFixture = new TestFixture(typeof(DummyFixture));
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
            Assert.That(new ParameterizedMethodSuite(typeof(DummyFixture).GetMethod("GenericMethod")).TestType,
                Is.EqualTo("GenericMethod"));
            Assert.That(new ParameterizedMethodSuite(typeof(DummyFixture).GetMethod("ParameterizedMethod")).TestType,
                Is.EqualTo("ParameterizedMethod"));
#if !NUNITLITE
            Assert.That(new ParameterizedFixtureSuite(typeof(DummyFixture)).TestType,
                Is.EqualTo("ParameterizedFixture"));
            Type genericType = typeof(DummyGenericFixture<int>).GetGenericTypeDefinition();
            Assert.That(new ParameterizedFixtureSuite(genericType).TestType,
                Is.EqualTo("GenericFixture"));
#endif
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

        #region Helper Methods For Checking XML

        private void CheckXmlForTest(Test test, bool recursive)
        {
            XmlNode topNode = test.ToXml(true);
            CheckXmlForTest(test, topNode, recursive);
        }

        private void CheckXmlForTest(Test test, XmlNode topNode, bool recursive)
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
            Assert.That(topNode.Attributes["runstate"], Is.EqualTo(test.RunState.ToString()));

            if (test.Properties.Keys.Count > 0)
            {
                var expectedProps = new List<string>();
                foreach (string key in test.Properties.Keys)
                    foreach (object value in test.Properties[key])
                        expectedProps.Add(key + "=" + value.ToString());

                XmlNode propsNode = topNode.FindDescendant("properties");
                Assert.NotNull(propsNode);

                var actualProps = new List<string>();
                foreach (XmlNode node in propsNode.ChildNodes)
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
                        XmlNode childNode = topNode.FindDescendant(xpathQuery);
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

#if !NUNITLITE
        public class DummyGenericFixture<T>
        {
        }
#endif
    }
}
