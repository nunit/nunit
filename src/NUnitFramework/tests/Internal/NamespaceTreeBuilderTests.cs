// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Tests.Internal
{
    public class NamespaceTreeBuilderTests
    {
        private TestAssembly _testAssembly;
        private NamespaceTreeBuilder _builder;
        private const string NameofDll = "mytest.dll";

        [SetUp]
        public void CreateTreeBuilder()
        {
            _testAssembly = new TestAssembly(NameofDll);
            _builder = new NamespaceTreeBuilder(_testAssembly);
        }

        [Test]
        public void InitialTreeState()
        {
            Assert.That(_builder.RootSuite, Is.SameAs(_testAssembly));
            Assert.That(_builder.RootSuite.Tests, Is.Empty);
        }

        [Test]
        public void AddSingleFixture()
        {
            _builder.Add(new TestSuite(typeof(NUnit.TestData.TestFixtureTests.RegularFixtureWithOneTest)));

            CheckTree("NUnit", "TestData", "TestFixtureTests", "RegularFixtureWithOneTest");
        }

        [Test]
        public void AddMultipleFixtures_SameNamespace()
        {
            _builder.Add(new[]
            {
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.RegularFixtureWithOneTest)),
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.FixtureWithTestFixtureAttribute)),
                new TestSuite(
                    typeof(NUnit.TestData.TestFixtureTests.FixtureWithoutTestFixtureAttributeContainingTestCase))
            });

            CheckTree("NUnit", "TestData", "TestFixtureTests", "RegularFixtureWithOneTest");
            CheckTree("NUnit", "TestData", "TestFixtureTests", "FixtureWithTestFixtureAttribute");
            CheckTree("NUnit", "TestData", "TestFixtureTests", "FixtureWithoutTestFixtureAttributeContainingTestCase");
        }

        [Test]
        public void AddMultipleFixtures_DifferentNamespaces()
        {
            _builder.Add(new[]
            {
                new TestSuite(typeof(TestData.TestFixtureTests.RegularFixtureWithOneTest)),
                new TestSuite(typeof(TestData.SetUpData.SetUpAndTearDownFixture)),
                new TestSuite(typeof(TestData.TheoryFixture.TheoryFixture))
            });

            CheckTree("NUnit", "TestData", "TestFixtureTests", "RegularFixtureWithOneTest");
            CheckTree("NUnit", "TestData", "SetUpData", "SetUpAndTearDownFixture");
            CheckTree("NUnit", "TestData", "TheoryFixture", "TheoryFixture");
        }

        [Test]
        public void AddMultipleFixtures_DifferentTopLevelNamespaces()
        {
            _builder.Add(new[]
            {
                new TestSuite(typeof(One.Fixture1)),
                new TestSuite(typeof(Two.Fixture2)),
                new TestSuite(typeof(Three.Fixture3)),
            });

            CheckTree("One", "Fixture1");
            CheckTree("Two", "Fixture2");
            CheckTree("Three", "Fixture3");
        }

        [Test]
        public void AddSetUpFixture_BottomUp()
        {
            _builder.Add(new TestSuite(typeof(NUnit.TestData.SetupFixture.Namespace1.SomeFixture)));
            _builder.Add(new SetUpFixture(
                new TypeWrapper(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1))));

            CheckTree("NUnit", "TestData", "SetupFixture", "Namespace1", "SomeFixture");
            Assert.That(_builder.RootSuite.Tests[0].Tests[0].Tests[0].Tests[0], Is.TypeOf<SetUpFixture>());
        }

        [Test]
        public void AddSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(
                new TypeWrapper(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1))));
            _builder.Add(new TestSuite(typeof(NUnit.TestData.SetupFixture.Namespace1.SomeFixture)));

            CheckTree("NUnit", "TestData", "SetupFixture", "Namespace1", "SomeFixture");
            Assert.That(_builder.RootSuite.Tests[0].Tests[0].Tests[0].Tests[0], Is.TypeOf<SetUpFixture>());
        }

        [Test]
        public void AddGlobalSetUpFixture_BottomUp()
        {
            _builder.Add(new TestSuite(typeof(SomeFixture)));
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));

            CheckTree("[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddGlobalSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));
            _builder.Add(new TestSuite(typeof(SomeFixture)));

            CheckTree("[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddTwoGlobalSetupFixtures_BottomUp()
        {
            _builder.Add(new TestSuite(typeof(SomeFixture)));
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));

            CheckTree("[default namespace]", "[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddTwoGlobalSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));
            _builder.Add(new SetUpFixture(new TypeWrapper(typeof(NoNamespaceSetupFixture))));
            _builder.Add(new TestSuite(typeof(SomeFixture)));

            CheckTree("[default namespace]", "[default namespace]", "SomeFixture");
        }

        private void CheckTree(params string[] names)
        {
            ITest suite = _builder.RootSuite;
            Assert.That(suite.Name, Is.EqualTo(NameofDll));

            foreach (var name in names)
            {
                foreach (var child in suite.Tests)
                {
                    if (child.Name == name)
                    {
                        suite = child;
                        break;
                    }
                }

                if (suite.Name != name)
                {
                    var dump = DumpTree(_builder.RootSuite, "   ");
                    Assert.Fail($"Did not find {name} in tree under {suite.Name}\nTree Contains:\n{dump}");
                }
            }
        }

        private string DumpTree(ITest suite, string indent)
        {
            var sb = new StringBuilder(indent + suite.Name + Environment.NewLine);
            foreach (var child in suite.Tests)
                sb.Append(DumpTree(child, indent + "  "));

            return sb.ToString();
        }
    }
}

namespace One
{
    public class Fixture1 { }
}

namespace Two
{
    public class Fixture2 { }
}

namespace Three
{
    public class Fixture3 { }
}
