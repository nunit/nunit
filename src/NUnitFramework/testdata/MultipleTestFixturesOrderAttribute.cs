// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.MultipleTestFixturesOrderAttribute
{
    [Order(2)]
    public class NoTestFixtureAttributeOrder2
    {
        [Test]
        public void A()
        {
            Assert.Pass("A");
        }
    }

    [TestFixture("1")]
    [TestFixture("2")]
    [Order(1)]
    public class MultipleTestFixtureAttributesOrder1
    {
        private string _s;
        public MultipleTestFixtureAttributesOrder1(string s)
        {
            this._s = s;
        }

        [Test]
        public void B()
        {
            Assert.Pass("B");
        }
    }

    [Order(0)]
    public class NoTestFixtureAttributeOrder0
    {
        [Test]
        public void C()
        {
            Assert.Pass("C");
        }
    }
}
