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
            var fixture = TestBuilder.MakeFixture(typeof (TestCaseOrderAttributeFixture));
            var work = TestBuilder.PrepareWorkItem(fixture, null) as CompositeWorkItem;

            // This triggers sorting
            TestBuilder.ExecuteWorkItem(work);
            
            Assert.AreEqual(work.Children.Count, 5);
            Assert.AreEqual(work.Children[0].Test.Name, "Y_FirstTest");
            Assert.AreEqual(work.Children[1].Test.Name, "Y_SecondTest");
            Assert.AreEqual(work.Children[2].Test.Name, "Z_ThirdTest");
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void CheckClassOrderIsCorrect(List<Type> candidateTypes)
        {
            var testSuite = TestBuilder.MakeFixture(candidateTypes);

            var work = TestBuilder.PrepareWorkItem(testSuite, null) 
                as CompositeWorkItem;

            var fixtureWorkItems = 
                ((work.Children[0] as CompositeWorkItem)
                .Children[0] as CompositeWorkItem)
                .Children;

            Assert.AreEqual(candidateTypes.Count, fixtureWorkItems.Count);
            for (var i = 1; i < fixtureWorkItems.Count; i++)
            {
                var previousTestOrder = int.Parse(fixtureWorkItems[i - 1]
                    .Test.Properties.Get(PropertyNames.Order).ToString());
                var currentTestOrder = int.Parse(fixtureWorkItems[i]
                    .Test.Properties.Get(PropertyNames.Order).ToString());

                Assert.IsTrue(previousTestOrder < currentTestOrder);
            }
        }

        private static readonly object[] Cases =
        {
            new List<Type>
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            },
            new List<Type>
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture)
            },
            new List<Type>
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            }
        };
    }
}
