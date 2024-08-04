// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestCaseSourceAttributeFixture;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public class Issue4584
    {
        [Test]
        public void DiscoverAndExecution()
        {
            var fixtureType = typeof(TestCaseSourceAttributeFixture.SelectionFail);
            var methodName = nameof(TestCaseSourceAttributeFixture.SelectionFail.Test);
            var fixture = TestBuilder.MakeTestFromMethod(fixtureType, methodName);

            foreach (var test in fixture.Tests)
            {
                var filter = TestFilter.FromXml(
                    $"<filter><name>{test.Name}</name></filter>");
                WorkItem? workItem = WorkItemBuilder.CreateWorkItem(fixture, filter, recursive: true);

                Assert.That(workItem, Is.Not.Null);
                Assert.That(workItem, Is.InstanceOf<CompositeWorkItem>());

                var compositeWorkItem = (CompositeWorkItem)workItem;
                Assert.That(compositeWorkItem.Children, Has.Count.EqualTo(1));
            }
        }
    }
}
