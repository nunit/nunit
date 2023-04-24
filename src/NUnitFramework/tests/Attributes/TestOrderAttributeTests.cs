// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.TestData;
using NUnit.TestData.MultipleTestFixturesOrderAttribute;
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

            Assert.That(work.Children, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(work.Children[0].Test.Name, Is.EqualTo("Y_FirstTest"));
                Assert.That(work.Children[1].Test.Name, Is.EqualTo("Y_SecondTest"));
                Assert.That(work.Children[2].Test.Name, Is.EqualTo("Z_ThirdTest"));
            });
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void CheckClassOrderIsCorrect(Type[] candidateTypes)
        {
            var testSuite = new TestSuite("dummy").Containing(candidateTypes);

            var work = TestBuilder.CreateWorkItem(testSuite) as CompositeWorkItem;

            var fixtureWorkItems = work.Children;

            Assert.That(fixtureWorkItems, Has.Count.EqualTo(candidateTypes.Length));
            for (var i = 1; i < fixtureWorkItems.Count; i++)
            {
                var previousTestOrder = GetOrderAttributeValue(fixtureWorkItems[i - 1]);
                var currentTestOrder = GetOrderAttributeValue(fixtureWorkItems[i]);

                Assert.That(previousTestOrder, Is.LessThan(currentTestOrder));
            }
        }

        private static int GetOrderAttributeValue(WorkItem item)
        {
            var order = item.Test.Properties.Get(PropertyNames.Order);
            return int.Parse(order.ToString());
        }

        private static readonly object[] Cases =
        {
            new[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            },
            new[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture)
            },
            new[]
            {
                typeof(TestCaseOrderAttributeFixture),
                typeof(AnotherTestCaseOrderAttributeFixture),
                typeof(ThirdTestCaseOrderAttributeFixture)
            },
            new[]
            {
                typeof(NoTestFixtureAttributeOrder2),
                typeof(MultipleTestFixtureAttributesOrder1),
                typeof(NoTestFixtureAttributeOrder0)
            }
        };
    }
}
