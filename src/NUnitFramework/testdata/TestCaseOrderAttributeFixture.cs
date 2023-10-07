// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    [Order(3)]
    public class TestCaseOrderAttributeFixture
    {
        [Test]
        [Order(3)]
        public void Z_ThirdTest()
        {
            Assert.Pass("Z_ThirdTestWithSameOrderAsSecond");
        }

        [Test]
        [Order(1)]
        public void Y_FirstTest()
        {
            Assert.Pass("Y_FirstTest");
        }

        [Test]
        [Order(2)]
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
    [Order(1)]
    public class AnotherTestCaseOrderAttributeFixture
    {
        [Test]
        public void DummyTest()
        {
            Assert.Pass("DummyTest");
        }
    }

    [TestFixture]
    [Order(2)]
    public class ThirdTestCaseOrderAttributeFixture
    {
        [Test]
        public void DummyTest()
        {
            Assert.Pass("DummyTest");
        }
    }
}
