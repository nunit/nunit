using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestOrderAttributeTests
    {
        [Test]
        public void CheckOrderIsCorrect()
        {
            var work = (CompositeWorkItem)TestBuilder.CreateWorkItem(typeof(TestCaseOrderAttributeFixture));

            // This triggers sorting
            TestBuilder.ExecuteWorkItem(work);
            
            Assert.AreEqual(work.Children.Count, 5);
            Assert.AreEqual(work.Children[0].Test.Name, "Y_FirstTest");
            Assert.AreEqual(work.Children[1].Test.Name, "Y_SecondTest");
            Assert.AreEqual(work.Children[2].Test.Name, "Z_ThirdTest");
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void CheckClassOrderIsCorrect(Type[] candidateTypes)
        {
            var testSuite = new TestSuite("dummy").Containing(candidateTypes);

            var work = TestBuilder.CreateWorkItem(testSuite) as CompositeWorkItem;

            var fixtureWorkItems = work.Children;

            Assert.AreEqual(candidateTypes.Length, fixtureWorkItems.Count);
            for (var i = 1; i < fixtureWorkItems.Count; i++)
            {
                var previousTestOrder = GetOrderAttributeValue(fixtureWorkItems[i - 1]);
                var currentTestOrder = GetOrderAttributeValue(fixtureWorkItems[i]);

                Assert.IsTrue(previousTestOrder < currentTestOrder);
            }
        }

        private static int GetOrderAttributeValue(WorkItem item)
        {
            var order = item.Test.Properties.Get(PropertyNames.Order);
            return int.Parse(order.ToString());
        }

        private static readonly object[] Cases =
        {
            new Type[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            },
            new Type[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture)
            },
            new Type[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            }
        };
    }
}
