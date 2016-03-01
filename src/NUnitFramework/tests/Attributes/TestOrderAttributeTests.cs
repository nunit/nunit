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
            
            var res = TestBuilder.GenerateWorkItem(testFixt,null);
            
            Assert.AreEqual(res.Children.Count, 5);
            Assert.AreEqual(res.Children[0].Test.Name, "Y_FirstTest");
            Assert.AreEqual(res.Children[1].Test.Name, "Y_SecondTest");
            Assert.AreEqual(res.Children[2].Test.Name, "Z_ThirdTest");
        }
        
    }
}
