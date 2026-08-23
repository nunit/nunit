// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.MultipleTestFixturesOrderAttribute
{
#pragma warning disable CS0618 // Type or member is obsolete
    [Order(2)]
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
    [Order(1)]
#pragma warning restore CS0618 // Type or member is obsolete
    public class MultipleTestFixtureAttributesOrder1
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly string _s;
#pragma warning restore IDE0052 // Remove unread private members
        public MultipleTestFixtureAttributesOrder1(string s)
        {
            _s = s;
        }

        [Test]
        public void B()
        {
            Assert.Pass("B");
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    [Order(0)]
#pragma warning restore CS0618 // Type or member is obsolete
    public class NoTestFixtureAttributeOrder0
    {
        [Test]
        public void C()
        {
            Assert.Pass("C");
        }
    }
}
