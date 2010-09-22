using System;
using System.Xml;
using NUnit.Framework.Api;

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
        public void ElementNameTests()
        {
            Assert.That(testMethod.ElementName, 
                Is.EqualTo("test-case"));
            Assert.That(testFixture.ElementName, 
                Is.EqualTo("test-fixture"));
            Assert.That(testSuite.ElementName, 
                Is.EqualTo("test-suite"));
            Assert.That(new TestAssembly("junk").ElementName, 
                Is.EqualTo("test-assembly"));
#if CLR_2_0 || CLR_4_0
            Assert.That(new ParameterizedMethodSuite(typeof(DummyFixture).GetMethod("GenericMethod")).ElementName,
                Is.EqualTo("generic-method"));
            Assert.That(new ParameterizedMethodSuite(typeof(DummyFixture).GetMethod("ParameterizedMethod")).ElementName,
                Is.EqualTo("method"));
#if !NUNITLITE
            Assert.That(new ParameterizedFixtureSuite(typeof(DummyFixture)).ElementName,
                Is.EqualTo("parameterized-fixture"));
            Type genericType = typeof(DummyGenericFixture<int>).GetGenericTypeDefinition();
            Assert.That(new ParameterizedFixtureSuite(genericType).ElementName,
                Is.EqualTo("generic-fixture"));
#endif
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

            Assert.That(topNode.Name, Is.EqualTo(test.ElementName));

            Assert.That(topNode.Attributes["id"].Value, Is.EqualTo(test.Id.ToString()));
            Assert.That(topNode.Attributes["name"].Value, Is.EqualTo(test.Name));
            Assert.That(topNode.Attributes["fullname"].Value, Is.EqualTo(test.FullName));

            // TODO: Replace SelectSingleNode to allow testing under CF 1.0
#if !NETCF_1_0
            int expectedCount = test.Properties.Count;
            if (expectedCount > 0)
            {
                string[] expectedProps = new string[expectedCount];
                int count = 0;
                foreach (PropertyEntry entry in test.Properties)
                    expectedProps[count++] = entry.ToString();

                XmlNode propsNode = topNode.SelectSingleNode("properties");
                Assert.NotNull(propsNode);

                int actualCount = propsNode.ChildNodes.Count;
                string[] actualProps = new string[actualCount];
                for (int i = 0; i < actualCount; i++)
                {
                    XmlNode node = propsNode.ChildNodes[i];
                    string name = node.Attributes["name"].Value;
                    string value = node.Attributes["value"].Value;
                    actualProps[i] = name + "=" + value.ToString();
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
                        string xpathQuery = string.Format("{0}[@id={1}]", child.ElementName, child.Id);
                        XmlNode childNode = topNode.SelectSingleNode(xpathQuery);
                        Assert.NotNull(childNode, "Expected node for test with ID={0}, Name={1}", child.Id, child.Name);

                        CheckXmlForTest(child, childNode, recursive);
                    }
                }
            }
#endif
        }

        #endregion

        public class DummyFixture
        {
            public void DummyMethod() { }
            public void ParameterizedMethod(int x) { }
#if CLR_2_0 || CLR_4_0
            public void GenericMethod<T>(T x) { }
#endif
        }

#if (CLR_2_0 || CLR_4_0) && !NUNITLITE
        public class DummyGenericFixture<T>
        {
        }
#endif
    }
}
