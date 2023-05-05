// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class TestNameParserTests
    {
        [TestCase("Test.Namespace.Fixture.Method")]
        [TestCase("Test.Namespace.Fixture.Method,")]
        [TestCase("  Test.Namespace.Fixture.Method  ")]
        [TestCase("  Test.Namespace.Fixture.Method  ,")]
        [TestCase("Test.Namespace.Fixture.Method()")]
        [TestCase("Test.Namespace.Fixture.Method(\"string,argument\")")]
        [TestCase("Test.Namespace.Fixture.Method(1,2,3)")]
        [TestCase("Test.Namespace.Fixture.Method<int,int>()")]
        [TestCase("Test.Namespace.Fixture.Method(\")\")")]
        public void SingleName(string name)
        {
            string[] names = TestNameParser.Parse(name);
            Assert.That(names, Has.Length.EqualTo(1));
            Assert.That(names[0], Is.EqualTo(name.Trim(new char[] { ' ', ',' })));
        }

        [TestCase("Test.Namespace.Fixture.Method1", "Test.Namespace.Fixture.Method2")]
        [TestCase("Test.Namespace.Fixture.Method1", "Test.Namespace.Fixture.Method2,")] // <= trailing comma
        [TestCase("Test.Namespace.Fixture.Method1(1,2)", "Test.Namespace.Fixture.Method2(3,4)")]
        [TestCase("Test.Namespace.Fixture.Method1(\"(\")", "Test.Namespace.Fixture.Method2(\"<\")")]
        public void TwoNames(string name1, string name2)
        {
            char[] delims = new char[] { ' ', ',' };
            string[] names = TestNameParser.Parse(name1 + "," + name2);
            Assert.That(names, Has.Length.EqualTo(2));
            Assert.That(names[0], Is.EqualTo(name1.Trim(delims)));
            Assert.That(names[1], Is.EqualTo(name2.Trim(delims)));
        }
    }
}
