// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    public class NamespaceTreeBuilderTests
    {
        private TestAssembly _testAssembly;
        private NamespaceTreeBuilder _builder;


        [SetUp]
        public void CreateTreeBuilder()
        {
            _testAssembly = new TestAssembly("mytest.dll");
            _builder = new NamespaceTreeBuilder(_testAssembly);
        }

        [Test]
        public void InitialTreeState()
        {
            Assert.That(_builder.RootSuite, Is.SameAs(_testAssembly));
            Assert.That(_builder.RootSuite.Tests.Count, Is.Zero);
        }

        [Test]
        public void AddSingleFixture()
        {
            _builder.Add( new TestSuite(typeof(NUnit.TestData.TestFixtureTests.RegularFixtureWithOneTest)));

            CheckTree("NUnit", "TestData", "TestFixtureTests", "RegularFixtureWithOneTest");
        }

        [Test]
        public void AddMultipleFixtures_SameNamespace()
        {
            _builder.Add(new[]
            {
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.RegularFixtureWithOneTest)),
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.FixtureWithTestFixtureAttribute)),
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.FixtureWithoutTestFixtureAttributeContainingTestCase))
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
                new TestSuite(typeof(NUnit.TestData.TestFixtureTests.RegularFixtureWithOneTest)),
                new TestSuite(typeof(NUnit.TestData.SetUpData.SetUpAndTearDownFixture)),
                new TestSuite(typeof(NUnit.TestData.TheoryFixture.TheoryFixture))
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
            _builder.Add(new SetUpFixture(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1)));

            CheckTree("NUnit", "TestData", "SetupFixture", "Namespace1", "SomeFixture");
            Assert.That(_builder.RootSuite.Tests[0].Tests[0].Tests[0].Tests[0], Is.TypeOf<SetUpFixture>());
        }

        [Test]
        public void AddSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(typeof(NUnit.TestData.SetupFixture.Namespace1.NUnitNamespaceSetUpFixture1)));
            _builder.Add(new TestSuite(typeof(NUnit.TestData.SetupFixture.Namespace1.SomeFixture)));

            CheckTree("NUnit", "TestData", "SetupFixture", "Namespace1", "SomeFixture");
            Assert.That(_builder.RootSuite.Tests[0].Tests[0].Tests[0].Tests[0], Is.TypeOf<SetUpFixture>());
        }

        [Test]
        public void AddGlobalSetUpFixture_BottomUp()
        {
            _builder.Add(new TestSuite(typeof(SomeFixture)));
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));

            CheckTree("[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddGlobalSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));
            _builder.Add(new TestSuite(typeof(SomeFixture)));

            CheckTree("[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddTwoGlobalSetupFixtures_BottomUp()
        {
            _builder.Add(new TestSuite(typeof(SomeFixture)));
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));

            CheckTree("[default namespace]", "[default namespace]", "SomeFixture");
        }

        [Test]
        public void AddTwoGlobalSetUpFixture_TopDown()
        {
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));
            _builder.Add(new SetUpFixture(typeof(NoNamespaceSetupFixture)));
            _builder.Add(new TestSuite(typeof(SomeFixture)));

            CheckTree("[default namespace]", "[default namespace]", "SomeFixture");
        }

        private void CheckTree(params string[] names)
        {
            ITest suite = _builder.RootSuite;
            Assert.That(suite.Name, Is.EqualTo("mytest.dll"));

            foreach (var name in names)
            {
                foreach (var child in suite.Tests)
                    if (child.Name == name)
                    {
                        suite = child;
                        break;
                    }

                if (suite.Name != name)
                {
                    var dump = DumpTree(_builder.RootSuite, "   ");
                    Assert.Fail("Did not find {0} in tree under {1}\nTree Contains:\n{2}", name, suite.Name, dump);
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
