using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class TestCaseOrderAttributeFixture
    {
        
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
        [Order(2)]
        public void Z_ThirdTestWithSameOrderAsSecond()
        {
            Assert.Pass("Z_ThirdTestWithSameOrderAsSecond");
        }

        [Test]
        public void D_NoOrderTest()
        {
            Assert.Pass("D_NoOrderTest");
        }

        [Test]
        [Order(0)]
        public void A_NoOrderTestLowLetter()
        {
            Assert.Pass("A_NoOrderTestLowLetter");
        }
    }
}
