// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
#pragma warning disable CS0618 // Type or member is obsolete
    [Order(3)]
#pragma warning restore CS0618 // Type or member is obsolete
    public class TestCaseOrderAttributeFixture
    {
        [Test]
#pragma warning disable CS0618 // Type or member is obsolete
        [Order(3)]
#pragma warning restore CS0618 // Type or member is obsolete
        public void Z_ThirdTest()
        {
            Assert.Pass("Z_ThirdTestWithSameOrderAsSecond");
        }

        [Test]
#pragma warning disable CS0618 // Type or member is obsolete
        [Order(1)]
#pragma warning restore CS0618 // Type or member is obsolete
        public void Y_FirstTest()
        {
            Assert.Pass("Y_FirstTest");
        }

        [Test]
#pragma warning disable CS0618 // Type or member is obsolete
        [Order(2)]
#pragma warning restore CS0618 // Type or member is obsolete
        public void Y_SecondTest()
        {
            Assert.Pass("Y_SecondTest");
        }

        [Test]
        public void D_NoOrderTest()
        {
            Assert.Pass("D_NoOrderTest");
        }

        [Test]
        public void A_NoOrderTestLowLetter()
        {
            Assert.Pass("A_NoOrderTestLowLetter");
        }
    }

    [TestFixture]
#pragma warning disable CS0618 // Type or member is obsolete
    [Order(1)]
#pragma warning restore CS0618 // Type or member is obsolete
    public class AnotherTestCaseOrderAttributeFixture
    {
        [Test]
        public void DummyTest()
        {
            Assert.Pass("DummyTest");
        }
    }

    [TestFixture]
#pragma warning disable CS0618 // Type or member is obsolete
    [Order(2)]
#pragma warning restore CS0618 // Type or member is obsolete
    public class ThirdTestCaseOrderAttributeFixture
    {
        [Test]
        public void DummyTest()
        {
            Assert.Pass("DummyTest");
        }
    }
}
