using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestData.TestCaseSourceAttributeFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    class TestOrderAttributeTests
    {
        [Test]
        public void CheckOrderIsCorrect()
        {
            var testFixt = TestBuilder.MakeFixture(typeof (TestCaseOrderAttributeFixture));
            
            var res = TestBuilder.RunTestSuite(testFixt,null,ParallelScope.Fixtures);
            
            Assert.AreEqual(res.Children.Count, 5);
            Assert.AreEqual(res.Children[0].Name, "A_NoOrderTestLowLetter");
            Assert.AreEqual(res.Children[1].Name, "D_NoOrderTest");
            Assert.AreEqual(res.Children[2].Name, "Y_FirstTest");
            Assert.AreEqual(res.Children[3].Name, "Y_SecondTest");
            Assert.AreEqual(res.Children[4].Name, "Z_ThirdTestWithSameOrderAsSecond");
        }
        
    }
}
