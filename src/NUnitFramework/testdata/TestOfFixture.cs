// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture(TestOf = typeof(TestOfAttribute))]
    [TestOf(typeof(TestOfAttribute))]
    [TestOf(typeof(TestFixtureAttribute))]
    public class TestOfFixture
    {
        [Test(TestOf = typeof(TestOfAttribute))]
        public void Method()
        { }

        [Test]
        public void NoTestOfMethod()
        { }

        [Test]
        [TestOf(typeof(TestOfAttribute))]
        public void SeparateTestOfTypeMethod()
        { }

        [Test]
        [TestOf("NUnit.Framework.TestOfAttribute")]
        public void SeparateTestOfStringMethod()
        { }

        [Test, TestOf(typeof(TestAttribute))]
        [TestCase(5, TestOf = typeof(TestCaseAttribute))]
        public void TestCaseWithTestOf(int x)
        { }

        [Test]
        [TestOf(typeof(TestOfAttribute))][TestOf(typeof(TestAttribute))]
        public void TestOfMultipleAttributesMethod()
        { }
    }
}
