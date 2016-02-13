using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Attributes;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    class TestOrderAttributeTests
    {
        [TestCase]
        [TestCaseOrder(1)]
        public void BFirstTestCase()
        {
            Console.WriteLine("BFirstTestCase");
        }

        [TestCase]
        [TestCaseOrder(2)]
        public void ASecondTestCase()
        {
            Console.WriteLine("ASecondTestCase");
        }
    }
}
