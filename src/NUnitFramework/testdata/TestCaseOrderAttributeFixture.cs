using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class TestCaseOrderAttributeFixture
    {
        [Test]

        public void A_FirstTest()
        {
            Assert.Pass("A_FirstTest");
        }

        public void B_SecondTest()
        {
            Assert.Pass("B_SecondTest");
        }

        public void C_NoOrderTest()
        {
            Assert.Pass("C_NoOrderTest");
        }
    }
}
